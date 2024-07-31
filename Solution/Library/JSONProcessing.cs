using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Data;

namespace Library;

/// <summary>
/// Класс JSONProcessing из условия.
/// </summary>
public class JSONProcessing
{
	public List<Station> Read(Stream stream)
	{
        stream.Position = 0;
        var reader = new StreamReader(stream);
        string allLines = "";
        List<Station> stations = new List<Station> { };

        string input = reader.ReadLine();

        while (!reader.EndOfStream)
        {
            allLines += input + '\n';
            input = reader.ReadLine();
        }

        stations = JsonSerializer.Deserialize<List<Station>>(allLines + ']');

        return stations;
    }

    public Stream Write(List<Station> stations)
    {
        Stream stream = new MemoryStream();
        JsonSerializerOptions options = new(JsonSerializerDefaults.Web)
        {
            Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
            WriteIndented = true
        };

        JsonSerializer.Serialize(stream, stations, options);
        stream.Position = 0;

        return stream;
    }

	public JSONProcessing()
	{
	}
}