using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Client.Options;
using System.Text.Json;

namespace SendMQTT
{
    class Program
    {
        static async Task Main(string[] args)//send mqtt
        {

            var mqttFactory = new MqttFactory();
            IMqttClient client = mqttFactory.CreateMqttClient();
            var options = new MqttClientOptionsBuilder().WithClientId(Guid.NewGuid().ToString())
                                                        .WithWebSocketServer("test.mosquitto.org:8080")
                                                        .WithCleanSession()
                                                        .Build();
            await client.ConnectAsync(options, CancellationToken.None);
            if (client.IsConnected)
            {
                Console.WriteLine("Пользователь подключен");
            }
            else
            {
                Console.WriteLine("Пользователь не подключен");
            }
            Random rand = new Random();
            for (int i = 0; i < 1000; i++)
            {
                var x = i;
                Data test = new Data(i, rand.Next(0,100), i.ToString(), DateTime.UtcNow);
                string json = JsonSerializer.Serialize(test);
                var message = new MqttApplicationMessageBuilder()
                     .WithTopic("TestTopicTest")
                     .WithPayload(json)
                     .WithAtLeastOnceQoS()
                     .Build();
                await client.PublishAsync(message, CancellationToken.None);
                Thread.Sleep(1000);
                Console.WriteLine($"Класс {i} отправлен");
            }
        }
        class Data
        {
            public int Id { get; set; }
            public double Count { get; set; }
            public string Name { get; set; }
            public DateTime date { get; set; }
            public Data(int id, double count, string name, DateTime time)
            {
                Id = id;
                Count = count;
                Name = name;
                date = time;
            }
            public string Show()
            {
                return $"id = {this.Id}, Count = {this.Count}, Name = {this.Name}";
            }
        }

    }
}