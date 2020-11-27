using System;
using System.IO;
using System.Text;
using Domain;
using Domain.Converters;

namespace pg_pg {
    class Program {
        static void Main(string[] args) {

            var m1 = new Winner3Way(
                new Outcome.Priced(1.85m),
                new Outcome.PricedWithProb(3.3m, 0.33f),
                new Outcome.Resulted(OutcomeResult.Canceled));

            Console.WriteLine($"Orig: {m1}");

            using var outputStream = new MemoryStream();
            using var output = new Google.Protobuf.CodedOutputStream(outputStream);

            m1.FromDomain().WriteTo(output);
            output.Flush();

            var bytes = outputStream.ToArray();
            Console.WriteLine($"Protobuf Bytes Length={bytes.Length}, {BitConverter.ToString(bytes)}");

            using var inputStream = new MemoryStream(bytes);
            using var input = new Google.Protobuf.CodedInputStream(inputStream);
            var protoCopy = new Domain.Protobuf.Winner3Way();
            protoCopy.MergeFrom(input);

            var copy = protoCopy.ToDomain();

            Console.WriteLine($"Copy: {copy}");

            using var jsonStream = new MemoryStream();
            using var jsonWriter = new System.Text.Json.Utf8JsonWriter(jsonStream);
            m1.WriteJson(jsonWriter);
            jsonWriter.Flush();
            var jsonString = Encoding.UTF8.GetString(jsonStream.ToArray());
            Console.WriteLine($"Json {jsonString}");

            var jsonDoc = System.Text.Json.JsonDocument.Parse(jsonString);
            var jsonCopy = Json.ReadWinner3Way(jsonDoc.RootElement);
            Console.WriteLine($"Json Copy: {jsonCopy}");

            ComplexDomainDemo.Run();
        }
    }
}
