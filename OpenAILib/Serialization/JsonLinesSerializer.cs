// Copyright (c) 2023 Owen Sigurdson
// MIT License

using System.Text.Json;

namespace OpenAILib.Serialization
{
    internal class JsonLinesSerializer
    {
        public static void Serialize<T>(Stream stream, IEnumerable<T> items)
        {
            var writer = new StreamWriter(stream);
            foreach (var item in items)
            {
                writer.WriteLine(JsonSerializer.Serialize(item));
            }
            writer.Flush();
        }

        public static IEnumerable<T> Deserialize<T>(Stream stream)
        {
            var reader = new StreamReader(stream);
            while (!reader.EndOfStream)
            {
                var line = reader.ReadLine();
                if (string.IsNullOrEmpty(line))
                {
                    continue;
                }
                var item = JsonSerializer.Deserialize<T>(line);
                if (item == null)
                {
                    continue;
                }
                yield return item;
            }
        }

        public static IEnumerable<T> Deserialize<T>(byte[] bytes)
        {
            var memoryStream = new MemoryStream(bytes);
            return Deserialize<T>(memoryStream);
        }    
    }
}
