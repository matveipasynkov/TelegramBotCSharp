using System;
using Telegram.Bot.Types.ReplyMarkups;

namespace ConsoleApp;

/// <summary>
/// Класс клавиатур и кнопок, которые нужны данным клавиатурам.
/// </summary>
public static class Keyboard
{
    public static InlineKeyboardButton buttonGetCSV = new InlineKeyboardButton("Получение данных через CSV.")
    {
        Text = "Получение данных через CSV.",
        CallbackData = "getCSV"
    };

    public static InlineKeyboardButton buttonGetJson = new InlineKeyboardButton("Получение данных через JSON.")
    {
        Text = "Получение данных через JSON.",
        CallbackData = "getJSON"
    };

    public static InlineKeyboardButton buttonSorting = new InlineKeyboardButton("Сортировка")
    {
        Text = "Сортировка",
        CallbackData = "Sorting"
    };

    public static InlineKeyboardButton buttonFiltration = new InlineKeyboardButton("Фильтрация")
    {
        Text = "Фильтрация",
        CallbackData = "Filtration"
    };

    public static InlineKeyboardButton buttonChangeTheFile = new InlineKeyboardButton("Поменять файл")
    {
        Text = "Поменять файл",
        CallbackData = "changeTheFile"
    };

    public static InlineKeyboardButton buttonSortingByYear = new InlineKeyboardButton("Сортировка по Year")
    {
        Text = "Сортировка по Year",
        CallbackData = "yearSorting"
    };

    public static InlineKeyboardButton buttonSortingByNameOfStation = new InlineKeyboardButton("Сортировка по NameOfStation")
    {
        Text = "Сортировка по NameOfStation",
        CallbackData = "nameOfStationSorting"
    };

    public static InlineKeyboardButton buttonFiltrationByNameOfStation = new InlineKeyboardButton("Фильтрация по NameOfStation")
    {
        Text = "Фильтрация по NameOfStation",
        CallbackData = "nameOfStationFiltration"
    };

    public static InlineKeyboardButton buttonFiltrationByLine = new InlineKeyboardButton("Фильтрация по Line")
    {
        Text = "Фильтрация по Line",
        CallbackData = "lineFiltration"
    };

    public static InlineKeyboardButton buttonFiltrationByNameOfStationAndMonth = new InlineKeyboardButton("Фильтрация по NameOfStation и Month")
    {
        Text = "Фильтрация по NameOfStation и Month",
        CallbackData = "nameOfStation&MonthFiltration"
    };

    public static InlineKeyboardButton buttonSavingByJson = new InlineKeyboardButton("Сохранить в json-формате.")
    {
        Text = "Сохранение в json-формате",
        CallbackData = "jsonSaving"
    };

    public static InlineKeyboardButton buttonSavingByCsv = new InlineKeyboardButton("Сохранить в csv-формате.")
    {
        Text = "Сохранение в csv-формате",
        CallbackData = "csvSaving"
    };

    public static InlineKeyboardMarkup gettingDataKeyboard = new InlineKeyboardMarkup(new[]
    {
            new[] {buttonGetCSV},
            new[] {buttonGetJson}
        });

    public static InlineKeyboardMarkup menuKeyboard = new InlineKeyboardMarkup(new[]
    {
            new[] {buttonSorting},
            new[] {buttonFiltration},
            new[] {buttonChangeTheFile}
        });

    public static InlineKeyboardMarkup sortingKeyboard = new InlineKeyboardMarkup(new[]
    {
            new[] {buttonSortingByYear},
            new[] {buttonSortingByNameOfStation}
        });

    public static InlineKeyboardMarkup filtrationKeyboard = new InlineKeyboardMarkup(new[]
    {
            new[] {buttonFiltrationByNameOfStation},
            new[] {buttonFiltrationByLine},
            new[] {buttonFiltrationByNameOfStationAndMonth}
        });

    public static InlineKeyboardMarkup savingKeyboard = new InlineKeyboardMarkup(new[]
    {
            new[] {buttonSavingByCsv},
            new[] {buttonSavingByJson}
        });
}