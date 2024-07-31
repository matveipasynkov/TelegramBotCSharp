using System;
using Library;
using Serilog;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using ConsoleApp;
using static ConsoleApp.UserInfo;

/// <summary>
/// Данный класс реализует работу самого бота.
/// </summary>
public class WorkingBot
{
    // Список chatId всех тех, кто начал взаимодействовать с ботом.
    static List<long> ids = new();

    static Dictionary<long, UserInfo> users = new();
    static CSVProcessing csvProcessing = new CSVProcessing();
    static JSONProcessing jsonProcessing = new JSONProcessing();

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        try
        {
            var botClient = new TelegramBotClient("YOUR-TOKEN");

            ReceiverOptions receiverOptions = new()
            {
                AllowedUpdates = new[] { UpdateType.Message, UpdateType.CallbackQuery },
                ThrowPendingUpdates = true
            };

            botClient.StartReceiving(
                updateHandler: HandleUpdateAsync,
                pollingErrorHandler: HandlePollingErrorAsync,
                receiverOptions: receiverOptions,
                cancellationToken: cancellationToken
            );

            var me = await botClient.GetMeAsync();

            // Оповещаем в себе в консоль, что можно начинать работу с ботом.
            Console.WriteLine($"Start listening for @{me.Username}");
            Console.ReadLine();

            await Task.Delay(-1, cancellationToken);
        }
        catch
        {
            Log.Error($"Error accrued at {DateTime.Now}");
        }
    }

    public async static Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
    {
        try
        {
            if (update.Type == UpdateType.Message)
            {
                var message = update.Message;
                var chatId = message.Chat.Id;

                if (update.Message.Document is null)
                {
                    var messageText = message.Text;

                    if (messageText == "/start")
                    {
                        if (!ids.Contains(chatId))
                        {
                            users.Add(chatId, new UserInfo(chatId));
                            ids.Add(chatId);
                        }

                        Message sentMessage = await botClient.SendTextMessageAsync(
                            chatId: chatId,
                            text: "Добро пожаловать! Выберите один из двух вариантов отправки файла:",
                            cancellationToken: cancellationToken,
                            replyMarkup: Keyboard.gettingDataKeyboard);

                        Log.Information($"User: {chatId}; userMessage: \"{messageText}\"; sentMessage: \"{sentMessage.Text}\"; date: {DateTime.Now}.");
                    }
                    else if (ids.Contains(chatId) && users[chatId].waitingAFirstValueOfFiltration)
                    {
                        users[chatId].filtrationValues[0] = messageText;

                        if (users[chatId].waitingASecondValueOfFiltration)
                        {
                            Message sentMessage = await botClient.SendTextMessageAsync(
                                chatId: chatId,
                                text: "Введите значение Month:",
                                cancellationToken: cancellationToken);

                            Log.Information($"User: {chatId}; userMessage: \"{messageText}\"; sentMessage: \"{sentMessage.Text}\"; date: {DateTime.Now}.");
                        }
                        else
                        {
                            users[chatId].result = Filtration.FiltrationByLINQ(users[chatId].stations, users[chatId].keyOfFiltration, users[chatId].filtrationValues[0]);

                            if (users[chatId].result.Count > 0)
                            {
                                Message sentMessage = await botClient.SendTextMessageAsync(
                                    chatId: chatId,
                                    text: $"Количество найденных станций: {users[chatId].result.Count}. Как сохранить результат?",
                                    cancellationToken: cancellationToken,
                                    replyMarkup: Keyboard.savingKeyboard);

                                Log.Information($"User: {chatId}; userMessage: \"{messageText}\"; sentMessage: \"{sentMessage.Text}\"; date: {DateTime.Now}.");
                            }
                            else
                            {
                                Message sentMessage = await botClient.SendTextMessageAsync(
                                    chatId: chatId,
                                    text: $"Таких станций не найдено. Меню:",
                                    cancellationToken: cancellationToken,
                                    replyMarkup: Keyboard.menuKeyboard);

                                Log.Information($"User: {chatId}; userMessage: \"{messageText}\"; sentMessage: \"{sentMessage.Text}\"; date: {DateTime.Now}.");
                            }
                        }

                        users[chatId].waitingAFirstValueOfFiltration = false;
                    }
                    else if (ids.Contains(chatId) && users[chatId].waitingASecondValueOfFiltration)
                    {
                        users[chatId].filtrationValues[1] = messageText;

                        users[chatId].result = Filtration.FiltrationByLINQ(users[chatId].stations, users[chatId].keyOfFiltration, users[chatId].filtrationValues[0], users[chatId].filtrationValues[1]);

                        if (users[chatId].result.Count > 0)
                        {
                            Message sentMessage = await botClient.SendTextMessageAsync(
                               chatId: chatId,
                               text: $"Количество найденных станций: {users[chatId].result.Count}. Как сохранить результат?",
                               cancellationToken: cancellationToken,
                               replyMarkup: Keyboard.savingKeyboard);

                            Log.Information($"User: {chatId}; userMessage: \"{messageText}\"; sentMessage: \"{sentMessage.Text}\"; date: {DateTime.Now}.");
                        }
                        else
                        {
                            Message sentMessage = await botClient.SendTextMessageAsync(
                                chatId: chatId,
                                text: $"Таких станций не найдено. Меню:",
                                cancellationToken: cancellationToken,
                                replyMarkup: Keyboard.menuKeyboard);

                            Log.Information($"User: {chatId}; userMessage: \"{messageText}\"; sentMessage: \"{sentMessage.Text}\"; date: {DateTime.Now}.");
                        }

                        users[chatId].waitingASecondValueOfFiltration = false;
                    }
                    else
                    {
                        Message sentMessage = await botClient.SendTextMessageAsync(
                            chatId: chatId,
                            text: "Вы ввели неверное сообщение или не запустили работу с ботом с помощью /start. Попробуйте ещё раз.",
                            cancellationToken: cancellationToken);

                        Log.Information($"User: {chatId}; userMessage: \"{messageText}\"; sentMessage: \"{sentMessage.Text}\"; date: {DateTime.Now}.");
                    }

                    return;
                }
                else if (ids.Contains(chatId) && users[chatId].typeOfGettingData != TypeOfGettingData.Nothing)
                {
                    var document = update.Message.Document;
                    var fileId = document.FileId;

                    var file = await botClient.GetFileAsync(fileId);

                    var pathToDownload = file.FilePath;
                    var stream = new MemoryStream();

                    await botClient.DownloadFileAsync(pathToDownload, stream);
                    stream.Position = 0;

                    if (users[chatId].typeOfGettingData == TypeOfGettingData.CSV)
                    {
                        if (Path.GetExtension(pathToDownload) == ".csv")
                        {
                            try
                            {
                                users[chatId].stations = csvProcessing.Read(stream);

                                Message sentMessage = await botClient.SendTextMessageAsync(
                                    chatId: chatId,
                                    text: "Файл считан корректно. Меню:",
                                    cancellationToken: cancellationToken,
                                    replyMarkup: Keyboard.menuKeyboard);

                                users[chatId].gotAFile = GotAFile.Yes;

                                Log.Information($"User: {chatId}; userDocument: {pathToDownload}; sentMessage: \"{sentMessage.Text}\"; date: {DateTime.Now}.");
                            }
                            catch
                            {
                                Message sentMessage = await botClient.SendTextMessageAsync(
                                    chatId: chatId,
                                    text: "Файл неверного формата. Отправьте другой файл. Какой файл вы хотите отправить?",
                                    cancellationToken: cancellationToken,
                                    replyMarkup: Keyboard.gettingDataKeyboard);

                                users[chatId].gotAFile = GotAFile.No;

                                Log.Information($"User: {chatId}; userDocument: {pathToDownload}; sentMessage: \"{sentMessage.Text}\"; date: {DateTime.Now}.");
                            }
                        }
                        else
                        {
                            Message sentMessage = await botClient.SendTextMessageAsync(
                                chatId: chatId,
                                text: "Файл не CSV-формата. Отправьте другой файл. Какой файл вы хотите отправить?",
                                cancellationToken: cancellationToken,
                                replyMarkup: Keyboard.gettingDataKeyboard);

                            users[chatId].gotAFile = GotAFile.No;

                            Log.Information($"User: {chatId}; userDocument: {pathToDownload}; sentMessage: \"{sentMessage.Text}\"; date: {DateTime.Now}.");
                        }
                    }
                    else if (users[chatId].typeOfGettingData == TypeOfGettingData.JSON)
                    {
                        if (Path.GetExtension(pathToDownload) == ".json")
                        {
                            try
                            {
                                users[chatId].stations = jsonProcessing.Read(stream);

                                Message sentMessage = await botClient.SendTextMessageAsync(
                                    chatId: chatId,
                                    text: "Файл считан корректно. Меню:",
                                    cancellationToken: cancellationToken,
                                    replyMarkup: Keyboard.menuKeyboard);

                                users[chatId].gotAFile = GotAFile.Yes;

                                Log.Information($"User: {chatId}; userDocument: {pathToDownload}; sentMessage: \"{sentMessage.Text}\"; date: {DateTime.Now}.");
                            }
                            catch
                            {
                                Message sentMessage = await botClient.SendTextMessageAsync(
                                    chatId: chatId,
                                    text: "Файл неверного формата. Отправьте другой файл. Какой файл вы хотите отправить?",
                                    cancellationToken: cancellationToken,
                                    replyMarkup: Keyboard.gettingDataKeyboard);

                                users[chatId].gotAFile = GotAFile.No;

                                Log.Information($"User: {chatId}; userDocument: {pathToDownload}; sentMessage: \"{sentMessage.Text}\"; date: {DateTime.Now}.");
                            }
                        }
                        else
                        {
                            Message sentMessage = await botClient.SendTextMessageAsync(
                                chatId: chatId,
                                text: "Файл не JSON-формата. Отправьте другой файл. Какой файл вы хотите отправить?",
                                cancellationToken: cancellationToken,
                                replyMarkup: Keyboard.gettingDataKeyboard);

                            users[chatId].gotAFile = GotAFile.No;

                            Log.Information($"User: {chatId}; userDocument: {pathToDownload}; sentMessage: \"{sentMessage.Text}\"; date: {DateTime.Now}.");
                        }
                    }

                    users[chatId].typeOfGettingData = TypeOfGettingData.Nothing;
                }
                else if (ids.Contains(chatId) && users[chatId].typeOfGettingData == TypeOfGettingData.Nothing)
                {
                    Message sentMessage = await botClient.SendTextMessageAsync(
                        chatId: chatId,
                        text: "Файл уже загружен или вы не выбрали способ загрузки файла.",
                        cancellationToken: cancellationToken);

                    Log.Information($"User: {chatId}; userDocument: unknown; sentMessage: \"{sentMessage.Text}\"; date: {DateTime.Now}.");
                }
                else
                {
                    Message sentMessage = await botClient.SendTextMessageAsync(
                        chatId: chatId,
                        text: "Скорее всего вы не написали команду /start или ввели ошибочную команду.",
                        cancellationToken: cancellationToken);

                    Log.Information($"User: {chatId}; userMessage: unknown; sentMessage: \"{sentMessage.Text}\"; date: {DateTime.Now}.");
                }

                return;
            }
            else if (update.Type == UpdateType.CallbackQuery)
            {
                var chatId = update.CallbackQuery.Message.Chat.Id;
                var data = update.CallbackQuery.Data;

                if (ids.Contains(chatId) && data == "getCSV")
                {
                    users[chatId].gotAFile = GotAFile.No;
                    users[chatId].typeOfGettingData = TypeOfGettingData.CSV;

                    Message sentMessage = await botClient.SendTextMessageAsync(
                        chatId: chatId,
                        text: "Отправьте CSV-файл.",
                        cancellationToken: cancellationToken);

                    Log.Information($"User: {chatId}; callbackQueryData: {data}; sentMessage: \"{sentMessage.Text}\"; date: {DateTime.Now}.");

                    return;
                }
                else if (ids.Contains(chatId) && data == "getJSON")
                {
                    users[chatId].gotAFile = GotAFile.No;
                    users[chatId].typeOfGettingData = TypeOfGettingData.JSON;

                    Message sentMessage = await botClient.SendTextMessageAsync(
                        chatId: chatId,
                        text: "Отправьте JSON-файл.",
                        cancellationToken: cancellationToken);

                    Log.Information($"User: {chatId}; callbackQueryData: {data}; sentMessage: \"{sentMessage.Text}\"; date: {DateTime.Now}.");

                    return;
                }
                else if (ids.Contains(chatId) && data == "Sorting")
                {
                    if (users[chatId].gotAFile == GotAFile.Yes)
                    {
                        Message sentMessage = await botClient.SendTextMessageAsync(
                            chatId: chatId,
                            text: "Выберите поле для сортировки:",
                            cancellationToken: cancellationToken,
                            replyMarkup: Keyboard.sortingKeyboard);

                        Log.Information($"User: {chatId}; callbackQueryData: {data}; sentMessage: \"{sentMessage.Text}\"; date: {DateTime.Now}.");

                        return;
                    }
                    else
                    {
                        Message sentMessage = await botClient.SendTextMessageAsync(
                            chatId: chatId,
                            text: "Нет файла с данными. Выберите способ отправки файла.",
                            cancellationToken: cancellationToken,
                            replyMarkup: Keyboard.gettingDataKeyboard);

                        Log.Information($"User: {chatId}; callbackQueryData: {data}; sentMessage: \"{sentMessage.Text}\"; date: {DateTime.Now}.");

                        return;
                    }
                }
                else if (ids.Contains(chatId) && data == "Filtration")
                {
                    if (users[chatId].gotAFile == GotAFile.Yes)
                    {
                        Message sentMessage = await botClient.SendTextMessageAsync(
                            chatId: chatId,
                            text: "Выберите поле для фильтрации",
                            cancellationToken: cancellationToken,
                            replyMarkup: Keyboard.filtrationKeyboard);

                        Log.Information($"User: {chatId}; callbackQueryData: {data}; sentMessage: \"{sentMessage.Text}\"; date: {DateTime.Now}.");

                        return;
                    }
                    else
                    {
                        Message sentMessage = await botClient.SendTextMessageAsync(
                            chatId: chatId,
                            text: "Нет файла с данными. Выберите способ отправки файла.",
                            cancellationToken: cancellationToken,
                            replyMarkup: Keyboard.gettingDataKeyboard);

                        Log.Information($"User: {chatId}; callbackQueryData: {data}; sentMessage: \"{sentMessage.Text}\"; date: {DateTime.Now}.");

                        return;
                    }
                }
                else if (ids.Contains(chatId) && data == "changeTheFile")
                {
                    users[chatId].gotAFile = GotAFile.No;

                    Message sentMessage = await botClient.SendTextMessageAsync(
                        chatId: chatId,
                        text: "Готово. Выберите способ получения файла.",
                        cancellationToken: cancellationToken,
                        replyMarkup: Keyboard.gettingDataKeyboard);

                    Log.Information($"User: {chatId}; callbackQueryData: {data}; sentMessage: \"{sentMessage.Text}\"; date: {DateTime.Now}.");

                    return;
                }
                else if (ids.Contains(chatId) && data == "yearSorting")
                {
                    if (users[chatId].gotAFile == GotAFile.Yes)
                    {
                        users[chatId].result = Sortings.SortingByLINQ(users[chatId].stations, "Year");

                        Message sentMessage = await botClient.SendTextMessageAsync(
                            chatId: chatId,
                            text: $"Сортировка по году выполнена. Как сохранить файл?",
                            cancellationToken: cancellationToken,
                            replyMarkup: Keyboard.savingKeyboard);

                        Log.Information($"User: {chatId}; callbackQueryData: {data}; sentMessage: \"{sentMessage.Text}\"; date: {DateTime.Now}.");
                    }
                    else
                    {
                        Message sentMessage = await botClient.SendTextMessageAsync(
                            chatId: chatId,
                            text: "Нет файла с данными. Выберите способ отправки файла.",
                            cancellationToken: cancellationToken,
                            replyMarkup: Keyboard.gettingDataKeyboard);

                        Log.Information($"User: {chatId}; callbackQueryData: {data}; sentMessage: \"{sentMessage.Text}\"; date: {DateTime.Now}.");
                    }

                    return;
                }
                else if (ids.Contains(chatId) && data == "nameOfStationSorting")
                {
                    if (users[chatId].gotAFile == GotAFile.Yes)
                    {
                        users[chatId].result = Sortings.SortingByLINQ(users[chatId].stations, "NameOfStation");

                        Message sentMessage = await botClient.SendTextMessageAsync(
                            chatId: chatId,
                            text: $"Сортировка по названию станции выполнена. Как сохранить файл?",
                            cancellationToken: cancellationToken,
                            replyMarkup: Keyboard.savingKeyboard);

                        Log.Information($"User: {chatId}; callbackQueryData: {data}; sentMessage: \"{sentMessage.Text}\"; date: {DateTime.Now}.");
                    }
                    else
                    {
                        Message sentMessage = await botClient.SendTextMessageAsync(
                            chatId: chatId,
                            text: "Нет файла с данными. Выберите способ отправки файла.",
                            cancellationToken: cancellationToken,
                            replyMarkup: Keyboard.gettingDataKeyboard);

                        Log.Information($"User: {chatId}; callbackQueryData: {data}; sentMessage: \"{sentMessage.Text}\"; date: {DateTime.Now}.");
                    }

                    return;
                }
                else if (ids.Contains(chatId) && data == "lineFiltration")
                {
                    if (users[chatId].gotAFile == GotAFile.Yes)
                    {
                        Message sentMessage = await botClient.SendTextMessageAsync(
                            chatId: chatId,
                            text: "Введите значение Line, по которому выполнить фильтрацию:",
                            cancellationToken: cancellationToken);

                        users[chatId].keyOfFiltration = "Line";
                        users[chatId].waitingAFirstValueOfFiltration = true;

                        Log.Information($"User: {chatId}; callbackQueryData: {data}; sentMessage: \"{sentMessage.Text}\"; date: {DateTime.Now}.");
                    }
                    else
                    {
                        Message sentMessage = await botClient.SendTextMessageAsync(
                            chatId: chatId,
                            text: "Нет файла с данными. Выберите способ отправки файла.",
                            cancellationToken: cancellationToken,
                            replyMarkup: Keyboard.gettingDataKeyboard);

                        Log.Information($"User: {chatId}; callbackQueryData: {data}; sentMessage: \"{sentMessage.Text}\"; date: {DateTime.Now}.");
                    }

                    return;
                }
                else if (ids.Contains(chatId) && data == "nameOfStationFiltration")
                {
                    if (users[chatId].gotAFile == GotAFile.Yes)
                    {
                        Message sentMessage = await botClient.SendTextMessageAsync(
                            chatId: chatId,
                            text: "Введите значение NameOfStation, по которому выполнить фильтрацию:",
                            cancellationToken: cancellationToken);

                        users[chatId].keyOfFiltration = "NameOfStation";
                        users[chatId].waitingAFirstValueOfFiltration = true;

                        Log.Information($"User: {chatId}; callbackQueryData: {data}; sentMessage: \"{sentMessage.Text}\"; date: {DateTime.Now}.");
                    }
                    else
                    {
                        Message sentMessage = await botClient.SendTextMessageAsync(
                            chatId: chatId,
                            text: "Нет файла с данными. Выберите способ отправки файла.",
                            cancellationToken: cancellationToken,
                            replyMarkup: Keyboard.gettingDataKeyboard);

                        Log.Information($"User: {chatId}; callbackQueryData: {data}; sentMessage: \"{sentMessage.Text}\"; date: {DateTime.Now}.");
                    }

                    return;
                }
                else if (ids.Contains(chatId) && data == "nameOfStation&MonthFiltration")
                {
                    if (users[chatId].gotAFile == GotAFile.Yes)
                    {
                        Message sentMessage = await botClient.SendTextMessageAsync(
                            chatId: update.CallbackQuery.Message.Chat.Id,
                            text: "Введите значение NameOfStation, по которому выполнить фильтрацию:",
                            cancellationToken: cancellationToken);

                        users[chatId].keyOfFiltration = "NameOfStation&Month";
                        users[chatId].waitingAFirstValueOfFiltration = true;
                        users[chatId].waitingASecondValueOfFiltration = true;

                        Log.Information($"User: {chatId}; callbackQueryData: {data}; sentMessage: \"{sentMessage.Text}\"; date: {DateTime.Now}.");
                    }
                    else
                    {
                        Message sentMessage = await botClient.SendTextMessageAsync(
                            chatId: chatId,
                            text: "Нет файла с данными. Выберите способ отправки файла.",
                            cancellationToken: cancellationToken,
                            replyMarkup: Keyboard.gettingDataKeyboard);

                        Log.Information($"User: {chatId}; callbackQueryData: {data}; sentMessage: \"{sentMessage.Text}\"; date: {DateTime.Now}.");
                    }

                    return;
                }
                else if (ids.Contains(chatId) && data == "jsonSaving")
                {
                    if (users[chatId].gotAFile == GotAFile.Yes)
                    {
                        Stream stream = jsonProcessing.Write(users[chatId].result);

                        Message message = await botClient.SendDocumentAsync(
                            chatId: chatId,
                            document: InputFile.FromStream(stream: stream, fileName: "new-station-rate.json"));

                        Message sentMessage = await botClient.SendTextMessageAsync(
                            chatId: chatId,
                            text: "Меню:",
                            cancellationToken: cancellationToken,
                            replyMarkup: Keyboard.menuKeyboard);

                        Log.Information($"User: {chatId}; callbackQueryData: {data}; sentMessage: \"{sentMessage.Text}\"; sentDocument: new-station-rate.json; date: {DateTime.Now}.");
                    }
                    else
                    {
                        Message sentMessage = await botClient.SendTextMessageAsync(
                            chatId: chatId,
                            text: "Нет файла с данными. Выберите способ отправки файла.",
                            cancellationToken: cancellationToken,
                            replyMarkup: Keyboard.gettingDataKeyboard);

                        Log.Information($"User: {chatId}; callbackQueryData: {data}; sentMessage: \"{sentMessage.Text}\"; date: {DateTime.Now}.");
                    }

                    return;
                }
                else if (ids.Contains(chatId) && data == "csvSaving")
                {
                    if (users[chatId].gotAFile == GotAFile.Yes)
                    {
                        Stream stream = csvProcessing.Write(users[chatId].result);

                        Message message = await botClient.SendDocumentAsync(
                            chatId: chatId,
                            document: InputFile.FromStream(stream: stream, fileName: "new-station-rate.csv"));

                        Message sentMessage = await botClient.SendTextMessageAsync(
                            chatId: chatId,
                            text: "Меню:",
                            cancellationToken: cancellationToken,
                            replyMarkup: Keyboard.menuKeyboard);

                        Log.Information($"User: {chatId}; callbackQueryData: {data}; sentMessage: \"{sentMessage.Text}\"; sentDocument: new-station-rate.csv; date: {DateTime.Now}.");
                    }
                    else
                    {
                        Message sentMessage = await botClient.SendTextMessageAsync(
                            chatId: chatId,
                            text: "Нет файла с данными. Выберите способ отправки файла.",
                            cancellationToken: cancellationToken,
                            replyMarkup: Keyboard.gettingDataKeyboard);

                        Log.Information($"User: {chatId}; callbackQueryData: {data}; sentMessage: \"{sentMessage.Text}\"; date: {DateTime.Now}.");
                    }
                }
                else
                {
                    Message sentMessage = await botClient.SendTextMessageAsync(
                        chatId: chatId,
                        text: "Скорее всего вы не написали команду /start или ввели ошибочную команду.",
                        cancellationToken: cancellationToken);

                    Log.Information($"User: {chatId}; callbackQueryData: {data}; sentMessage: \"{sentMessage.Text}\"; date: {DateTime.Now}.");
                }
                return;
            }
            return;
        }
        catch
        {
            Log.Error($"Error accrued at {DateTime.Now}");
            return;
        }
	}

    public async static Task HandlePollingErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
    {
        try
        {
            var ErrorMessage = exception switch
            {
                ApiRequestException apiRequestException
                    => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
                _ => exception.ToString()
            };

            Console.WriteLine(ErrorMessage);
            return;
        }
        catch
        {
            Log.Error($"Error accrued at {DateTime.Now}");
            return;
        }
    }

    public WorkingBot()
    { }
}