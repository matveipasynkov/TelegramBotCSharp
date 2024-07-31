using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
namespace Library;

/// <summary>
/// Класс CSVProcessing из условия.
/// </summary>
public class CSVProcessing
{
    public Stream Write(List<Station> stations)
    {
        var stream = new MemoryStream();
        var writer = new StreamWriter(stream);

        writer.WriteLine("\"ID\";\"NameOfStation\";\"Line\";\"Longitude_WGS84\";\"Latitude_WGS84\";\"AdmArea\";\"District\";\"Year\";\"Month\";\"global_id\";\"geodata_center\";\"geoarea\";");
        writer.WriteLine("\"№ п/п\";\"Станция метрополитена\";\"Линия\";\"Долгота в WGS-84\";\"Широта в WGS-84\";\"Административный округ\";\"Район\";\"Год\";\"Месяц\";\"global_id\";\"geodata_center\";\"geoarea\";");

        foreach (Station station in stations)
        {
            writer.WriteLine($"\"{station.ID}\";\"{station.NameOfStation}\";\"{station.Line}\";\"{station.Longitude}\";\"{station.Latitude}\";\"{station.AdmArea}\";\"{station.District}\";\"{station.Year}\";\"{station.Month}\";\"{station.GlobalId}\";\"{station.GeodataCenter}\";\"{station.Geoarea}\";");
        }

        writer.Flush();
        stream.Position = 0;
        return stream;
    }

    public List<Station> Read(Stream stream)
    {
        stream.Position = 0; 
        var reader = new StreamReader(stream);
        List<string> allLines = new List<string> { };
        List<Station> stations = new List<Station> { };

        string input = reader.ReadLine();

        while (!reader.EndOfStream)
        {
            allLines.Add(input);
            input = reader.ReadLine();
        }

        try
        {
            string firstRow = "\"ID\";\"NameOfStation\";\"Line\";\"Longitude_WGS84\";\"Latitude_WGS84\";\"AdmArea\";\"District\";\"Year\";\"Month\";\"global_id\";\"geodata_center\";\"geoarea\";";
            string secondRow = "\"№ п/п\";\"Станция метрополитена\";\"Линия\";\"Долгота в WGS-84\";\"Широта в WGS-84\";\"Административный округ\";\"Район\";\"Год\";\"Месяц\";\"global_id\";\"geodata_center\";\"geoarea\";";

            if (firstRow != allLines[0] || secondRow != allLines[1])
            {
                throw new ArgumentException();
            }

            for (int i = 2; i < allLines.Count; ++i)
            {
                string[] info = allLines[i][..^1].Split(";");

                for (int j = 0; j < info.Length; ++j)
                {
                    info[j] = info[j][1..^1];
                }

                Station station = new Station(
                    int.Parse(info[0]), info[1], info[2],
                    double.Parse(info[3]), double.Parse(info[4]), info[5],
                    info[6], int.Parse(info[7]), info[8],
                    long.Parse(info[9]), info[10], info[11]
                    );

                stations.Add(station);
            }
        }
        catch
        {
            throw new ArgumentException();
        }

        if (allLines.Count <= 2)
        {
            throw new ArgumentException();
        }

        return stations;
    }

    public CSVProcessing()
    { }
}
