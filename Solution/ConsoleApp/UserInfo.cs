using System;
using Library;

namespace ConsoleApp;

/// <summary>
/// Класс UserInfo создан для сохранения информации о каждом пользователе.
/// </summary>
public class UserInfo
{
    /// <summary>
    /// Данный enum содержит типы данных, которые пользователь может подать на вход.
    /// </summary>
    public enum TypeOfGettingData
    {
        JSON,
        CSV,
        Nothing
    }

    /// <summary>
    /// Данный enum содержит варианты того имеет ли пользователь файл или нет.
    /// </summary>
    public enum GotAFile
    {
        Yes,
        No
    }

    // chatId содержит Id чата с пользователем.
    public long chatId { get; set; }

    // typeOfGettingData содержит тип получаемых данных: json, csv или никакой.
    public TypeOfGettingData typeOfGettingData { get; set; }

    // gotAFile содержит информацию о том, получен ли был корректно хоть какой-то файл.
    public GotAFile gotAFile { get; set; }

    // waitingAFirstValueOfFiltration содержит информацию о том, ожидаем ли мы от пользователя ключ для фильтрации.
    public bool waitingAFirstValueOfFiltration { get; set; }

    // waitingASecondValueOfFiltration содержит информацию о том, ожидаем ли мы от пользователя второй ключ для фильтрации.
    public bool waitingASecondValueOfFiltration { get; set; }

    // filtrationValues содержит значения фильтрации.
    public List<string> filtrationValues { get; set; }

    // keyOfFiltration содержит название поля, по которому пользователь хочет вести фильтрацию.
    public string keyOfFiltration { get; set; }

    // stations содержит список объектов MyType, считанных из файла, который дал пользователь.
    public List<Station> stations { get; set; }

    // result содержит результат работы программы с stations по запросу пользователя.
    public List<Station> result { get; set; }

    public UserInfo(long chatId = 0)
	{
        this.chatId = chatId;
        this.typeOfGettingData = TypeOfGettingData.Nothing;
        this.gotAFile = GotAFile.No;
        this.waitingAFirstValueOfFiltration = false;
        this.waitingASecondValueOfFiltration = false;
        this.filtrationValues = new() { "", "" };
        this.keyOfFiltration = "";
    }
}

