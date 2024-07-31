using System.Threading;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Library;
using Telegram.Bot.Types.ReplyMarkups;
using Serilog;
using Serilog.Events;
using Serilog.Formatting.Json;
using System;

/// <summary>
/// Примечание: данная программа использует следующие Nuget-пакеты: Serilog, Serilog.Sinks.Console, Serilog.Sinks.File и Telegram.Bot.
/// Так же логи, которые записываются в папке var, дублируются в консоль, чтобы проще было следить за ходом работы программы.
/// Не могу не упомянуть о принципе работы бота: вначале пользователь отправляет csv или json файл.
/// Затем при успешном чтении файла пользователю предоставляется меню из 3-ех функций: сортировка, фильтрация или поменять файл.
/// Если пользователь выберет сортировку или фильтрацию и результат данных команд будет ненулевым, то бот предложит сохранить файл в json или csv формате.
/// </summary>

class Program
{
    // Путь сохранения логов.
    static string outputPath = Directory.GetParent(Directory.GetParent(Directory.GetParent(Directory.GetParent(Directory.GetCurrentDirectory()).ToString()).ToString()).ToString()).ToString() + Path.DirectorySeparatorChar + "var" + Path.DirectorySeparatorChar + "output.logs";

    static async Task Main()
    {
        try
        {
            Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Debug()
            .WriteTo.File(outputPath, rollingInterval: RollingInterval.Day)
            .WriteTo.Console()
            .CreateLogger();

            using CancellationTokenSource cts = new();

            var bot = new WorkingBot();
            await bot.StartAsync(cts.Token);

        }
        catch
        {
            Log.Error($"Error accrued at {DateTime.Now}");
        }
    }
}