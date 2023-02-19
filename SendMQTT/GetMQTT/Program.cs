using MQTTnet;//3.10
using MQTTnet.Client;
using MQTTnet.Client.Options;
using Nest;
using System.Text;
using System.Text.Json;
namespace GetMQTT//receiving data from SendData and sending it to ElasticSearch
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var mqttFactory = new MqttFactory();
            using (var mqttClient = mqttFactory.CreateMqttClient())
            {
                var mqttClientOptions = new MqttClientOptionsBuilder()
                                                        .WithWebSocketServer("test.mosquitto.org:8080")//or WithTcpServer
                                                        .WithCleanSession()
                                                        .Build();
                mqttClient.UseApplicationMessageReceivedHandler(e =>
                {
                    Console.WriteLine("Received JSON files.");
                    e.DumpToConsole();
                    return Task.CompletedTask;
                });

                await mqttClient.ConnectAsync(mqttClientOptions, CancellationToken.None);

                var mqttSubscribeOptions = mqttFactory.CreateSubscribeOptionsBuilder()
                    .WithTopicFilter(f => f.WithTopic("TestTopicTest"))
                    .Build();

                await mqttClient.SubscribeAsync(mqttSubscribeOptions, CancellationToken.None);

                Console.WriteLine("MQTT client subscribed to topic.");
                Console.ReadLine();
            }
        }
    }

    static class ObjectExtensions
    {
        public static TObject DumpToConsole<TObject>(this TObject @object)
        {
            var output = "NULL";
            if (@object != null)
            {
                output = JsonSerializer.Serialize(@object, new JsonSerializerOptions { WriteIndented = true });
            }
            Console.WriteLine($"{output}");
            SendElastic(output);
            return @object;
        }

        public static void SendElastic(string json)
        {
            var setting = new ConnectionSettings(new Uri("http://localhost:9200/"))
                                                .DefaultIndex("test");
            var client = new ElasticClient(setting);
            string[] base64Text = json.Split('"');
            string fix = Decode64(base64Text[13]);
            Data message = JsonSerializer.Deserialize<Data>(fix);
            var response = client.IndexDocument(message);
            if (response.IsValid)
            {
                Console.WriteLine($"Data sent with ID {response.Id}");
                Console.WriteLine(message.Show());
            }
            else
            {
                Console.WriteLine("Sending data error");
            }
        }
        public static string Decode64(string json)
        {
            byte[] textBytes = Convert.FromBase64String(json);
            return Encoding.ASCII.GetString(textBytes);
        }
        class Data
        {
            public int Id { get; set; }
            public double Count { get; set; }
            public string Name { get; set; }
            public DateTime date { get; set; }
            public Data (int id, double count, string name, DateTime time)
            {
                Id = id;
                Count = count;
                Name = name;
                date = time;
            }
            public Data()
            { }
            public string Show()
            {
                return $"id = {this.Id}, Count = {this.Count}, Name = {this.Name}";
            }
        }
    }

}
