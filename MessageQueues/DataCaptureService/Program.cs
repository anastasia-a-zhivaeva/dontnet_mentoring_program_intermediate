using Confluent.Kafka;

class Program
{

    static void Main(string[] args)
    {
        Watch();

        Console.WriteLine("Press any key to exit...");
        Console.ReadKey();
    }

    private static void Watch()
    {
        /*
         * Path and extensions can be stored in local or remote config in real application
         */
        Console.WriteLine("Type path for monitoring (default C:\\Images):");
        var path = Console.ReadLine();
        path = string.IsNullOrWhiteSpace(path) ? @"C:\Images" : path;

        try
        {
            using (var watcher = new FileSystemWatcher(path))
            {
                Console.WriteLine("Type extensions for monitoring, comma-separated (default '*.jpeg,*.jpg,*.png'):");
                var extensionString = Console.ReadLine();
                extensionString = string.IsNullOrWhiteSpace(extensionString) ? "*.jpeg,*.jpg,*.png" : extensionString;
                var extensions = extensionString.Split(',').Select(s => s.Trim());

                foreach (var extension in extensions)
                {
                    watcher.Filters.Add(extension);
                }

                /*
                 *  Topic name and bootstrap servers can be stored in local or remote config in real application
                 */
                var config = new ProducerConfig { BootstrapServers = "localhost:9092" };

                using (var producer = new ProducerBuilder<string, byte[]>(config).SetKeySerializer(Serializers.Utf8).Build())
                {
                    watcher.Created += async (sender, e) =>
                    {
                        await Publish(producer, e);
                    };
                }

                watcher.Error += (sender, e) =>
                {
                    Console.WriteLine(e.ToString());
                };

                watcher.EnableRaisingEvents = true;

                Console.WriteLine($"Monitoring folder: {path}");
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
        }
    }

    private static async Task Publish(IProducer<string, byte[]> producer, FileSystemEventArgs e)
    {
        Console.WriteLine($"New file created: {e.FullPath}");
        var fileLength = new FileInfo(e.FullPath).Length;
        var key = $"{e.Name}#{Guid.NewGuid()}";
        var chunkSize = 1000000;
        var chunkIndex = 1;
        var totalChunks = Math.Ceiling((decimal)(fileLength / chunkSize));
        using (FileStream fileStream = new FileStream(e.FullPath, FileMode.Open, FileAccess.Read))
        {
            for (int offset = 0; offset < fileLength; offset += chunkSize)
            {
                byte[] chunk = new byte[chunkSize];
                fileStream.Seek(offset, SeekOrigin.Begin);
                fileStream.Read(chunk, 0, chunkSize);
                /*
                 * In real application, info about chunks should be send in Value instead of Key
                 * I'm keeping it in Key for simplicity
                 * Key will be the same which means that all messages will be delivered to the same partition/consumer
                 * Message can be serialized/deserialized using JSONSchema, Protobuf, etc
                 */
                var message = new Message<string, byte[]>
                {
                    Key = $"{key}#{chunkIndex}#{totalChunks}",
                    Value = chunk
                };
                /*
                 *  Topic name and bootstrap servers can be stored in local or remote config in real application
                 */
                var deliveryReport = await producer.ProduceAsync("message-queues", message);
                Console.WriteLine($"Chunk {chunkIndex} of {totalChunks} delivered to: {deliveryReport.TopicPartitionOffset}");
                chunkIndex++;
            }
        }
    }
}