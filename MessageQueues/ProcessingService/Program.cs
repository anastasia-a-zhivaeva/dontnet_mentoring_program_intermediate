using Confluent.Kafka;
using Confluent.Kafka.Admin;

public class Program
{
    /*
     * Path to processed file can be stored in local or remote config in real application
     */
    private static string imageFolderPath = @"C:\ProcessedFiles";

    /*
     * List of chunked files can be stored in the database to prevent loss in case of service shut down
     */
    private static Dictionary<string, ChunkedFile> Files = new Dictionary<string, ChunkedFile>();

    public static async Task Main(string[] args)
    {
        await CreateTopic();
        await Consume();
    }

    private static async Task CreateTopic()
    {
        /*
         *  Topic name and bootstrap servers can be stored in local or remote config in real application
         */
        using (var adminClient = new AdminClientBuilder(new AdminClientConfig { BootstrapServers = "localhost:9092" }).Build())
        {
            try
            {
                await adminClient.CreateTopicsAsync([
                    new TopicSpecification { Name = "message-queues", ReplicationFactor = 1, NumPartitions = 1 } ]);
            }
            catch (CreateTopicsException e)
            {
                Console.WriteLine($"An error occured creating topic {e.Results[0].Topic}: {e.Results[0].Error.Reason}");
            }
        }
    }

    private static async Task Consume()
    {
        /*
         *  Topic name and bootstrap servers can be stored in local or remote config in real application
         */
        var config = new ConsumerConfig
        {
            BootstrapServers = "localhost:9092",
            GroupId = "message-queues-consumer",
            EnableAutoOffsetStore = false,
            EnableAutoCommit = true,
            StatisticsIntervalMs = 5000,
            SessionTimeoutMs = 6000,
            AutoOffsetReset = AutoOffsetReset.Earliest,
            EnablePartitionEof = true,
            PartitionAssignmentStrategy = PartitionAssignmentStrategy.CooperativeSticky
        };

        using (var consumer = new ConsumerBuilder<string, byte[]>(config)
                .SetErrorHandler((_, e) => Console.WriteLine($"Error: {e.Reason}"))
                .SetPartitionsAssignedHandler((c, partitions) =>
                {
                    Console.WriteLine(
                        "Partitions incrementally assigned: [" +
                        string.Join(',', partitions.Select(p => p.Partition.Value)) +
                        "], all: [" +
                        string.Join(',', c.Assignment.Concat(partitions).Select(p => p.Partition.Value)) +
                        "]");
                })
                .SetPartitionsRevokedHandler((c, partitions) =>
                {
                    var remaining = c.Assignment.Where(atp => partitions.Where(rtp => rtp.TopicPartition == atp).Count() == 0);
                    Console.WriteLine(
                        "Partitions incrementally revoked: [" +
                        string.Join(',', partitions.Select(p => p.Partition.Value)) +
                        "], remaining: [" +
                        string.Join(',', remaining.Select(p => p.Partition.Value)) +
                        "]");
                })
                .SetPartitionsLostHandler((c, partitions) =>
                {
                    Console.WriteLine($"Partitions were lost: [{string.Join(", ", partitions)}]");
                })
                .Build())
        {
            consumer.Subscribe("message-queues");

            try
            {
                while (true)
                {
                    try
                    {
                        var consumeResult = consumer.Consume();

                        if (consumeResult.IsPartitionEOF)
                        {
                            Console.WriteLine(
                                $"Reached end of topic {consumeResult.Topic}, partition {consumeResult.Partition}, offset {consumeResult.Offset}.");

                            continue;
                        }

                        Console.WriteLine($"Received message at {consumeResult.TopicPartitionOffset}: {consumeResult.Message.Key}");

                        await Process(consumeResult.Message.Value, consumeResult.Message.Key);

                        try
                        {
                            consumer.StoreOffset(consumeResult);
                        }
                        catch (KafkaException e)
                        {
                            Console.WriteLine($"Store Offset error: {e.Error.Reason}");
                        }
                    }
                    catch (ConsumeException e)
                    {
                        Console.WriteLine($"Consume error: {e.Error.Reason}");
                    }
                }
            }
            catch (OperationCanceledException)
            {
                Console.WriteLine("Closing consumer.");
                consumer.Close();
            }
        }
    }

    private static async Task Process(byte[] chunk, string key)
    {
        var (_, _, uid, _) = ChunkedFile.ParseChunk(key);

        if (Files.TryGetValue(uid, out var file))
        {
            file.SetChunk(key, chunk);
            if (file.IsReady())
            {
                await file.WriteFile();
                Files.Remove(uid);
            }
        }
        else
        {
            var newFile = new ChunkedFile(key, chunk);
            Files.Add(uid, newFile);
        }
    }

    private class ChunkedFile
    {
        public int TotalChunks { private set; get; }
        public string FileName { private set; get; }
        public string Uid { private set; get; }
        private Dictionary<int, byte[]> Chunks = new Dictionary<int, byte[]>();

        public ChunkedFile(string key, byte[] value)
        {
            var (totalChunks, chunkIndex, uid, fileName) = ParseChunk(key);
            TotalChunks = totalChunks;
            FileName = fileName;
            Uid = uid;
            Chunks[chunkIndex] = value;
        }

        public void SetChunk(string key, byte[] value)
        {
            var (totalChunks, chunkIndex, uid, fileName) = ParseChunk(key);

            if (TotalChunks != totalChunks || Uid != uid || FileName != fileName)
                throw new ArgumentException("The chunk provided does not belong to the file");

            Chunks[chunkIndex] = value;
        }
        public bool IsReady()
        {
            return Chunks.Count == TotalChunks;
        }

        public async Task WriteFile()
        {
            if (!IsReady())
                throw new FileLoadException("The file is not ready to be written. Not all chunks are received");

            var file = Chunks.OrderBy(c => c.Key).Select(c => c.Value).SelectMany(c => c).ToArray();

            await File.WriteAllBytesAsync(Path.Combine(imageFolderPath, FileName), file);
        }

        public static (int totalChunk, int chunkIndex, string uid, string fileName) ParseChunk(string key)
        {
            var chunkData = key.Split('#');
            var totalChunks = int.Parse(chunkData[chunkData.Length - 1]);
            var chunkIndex = int.Parse(chunkData[chunkData.Length - 2]);
            var uid = chunkData[chunkData.Length - 3];
            var fileName = chunkData[chunkData.Length - 4];

            return (totalChunks, chunkIndex, uid, fileName);
        }
    }
}