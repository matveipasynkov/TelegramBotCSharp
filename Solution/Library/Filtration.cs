using System;
namespace Library;

/// <summary>
/// Данный класс реализует фильтрацию данных.
/// </summary>
public static class Filtration
{
    /// <summary>
    /// Данный метод реализует фильтрацию данных с помощью LINQ запросов.
    /// </summary>
    /// <param name="stations"></param>
    /// <param name="key"></param>
    /// <param name="firstValue"></param>
    /// <param name="secondValue"></param>
    /// <returns></returns>
	public static List<Station> FiltrationByLINQ(List<Station> stations, string key, string firstValue, string secondValue = "")
	{
        List<Station> result = new();

        if (key == "NameOfStation")
        {
            result = (from station in stations
                      where station.NameOfStation == firstValue
                      select station).ToList();
        }
        else if (key == "Line")
        {
            result = (from station in stations
                      where station.Line == firstValue
                      select station).ToList();
        }
        else if (key == "NameOfStation&Month")
        {
            result = (from station in stations
                      where station.NameOfStation == firstValue && station.Month == secondValue
                      select station).ToList();
        }

        return result;
    }
}