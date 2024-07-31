using System;
namespace Library
{
	/// <summary>
	/// Данный класс содержит методы сортировок данных.
	/// </summary>
	public static class Sortings
	{
		/// <summary>
		/// Данный метод реализует сортировку с помощью LINQ-запросов.
		/// </summary>
		/// <param name="stations"></param>
		/// <param name="key"></param>
		/// <returns></returns>
		public static List<Station> SortingByLINQ(List<Station> stations, string key)
		{
			List<Station> result = new();

			if (key == "Year")
			{
				result = (from station in stations
							 orderby station.Year
							 select station).ToList();
			}
			else if (key == "NameOfStation")
			{
				result = (from station in stations
                             orderby station.NameOfStation
                             select station).ToList();
            }

			return result;
		}
	}
}

