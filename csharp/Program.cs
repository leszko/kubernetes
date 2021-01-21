using System;
using System.Threading;
using System.Threading.Tasks;
using Hazelcast;

namespace Client
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var options = HazelcastOptions.Build(args);
            options.Networking.Addresses.Add("hz-hazelcast");
            await using var client = await HazelcastClientFactory.StartNewClientAsync(options);
            await using var map = await client.GetMapAsync<string, string>("map");
            await map.PutAsync("key", "value");
            var value = await map.GetAsync("key");
            if (value == "value")
            {
                Console.WriteLine("Successful connection!");
            }
            else
            {
                throw new Exception("Connection failed, check your configuration.");
            }
            Console.WriteLine("Starting to fill the map with random entries.");
            var random = new Random();
            while (true)
            {
                var randomKey = random.Next(100_000);
                try {
                    await map.PutAsync("key" + randomKey, "value" + randomKey);
                    if (randomKey % 100 == 0)
                    {
                        Console.WriteLine("Current map size: {0}", await map.GetSizeAsync());
                        Thread.Sleep(1000);
                    }
                } 
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                }
            }
        }
    }
}