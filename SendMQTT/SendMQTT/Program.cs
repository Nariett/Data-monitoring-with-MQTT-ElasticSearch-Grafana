using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Client.Options;
using System.Text.Json;

namespace SendMQTT
{
    class Program//send data in mqtt
    {
        static async Task Main(string[] args)
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
                Console.WriteLine("Сlient connected");
            }
            else
            {
                Console.WriteLine("Client disabled");
            }
            Random rand = new Random();
            int wave = 0;//create beautiful graph
            bool size = true;
            for (int i = 0; i < 1000; i++)
            {
                if (size)
                {
                    if (wave >= 200) size = false;
                    else wave += 2;
                }
                else
                {
                    if(wave == 0)
                    {
                        size = true;
                    } else wave -= 2;
                }
                Data test = new Data(i, wave, i.ToString(), DateTime.UtcNow);
                string json = JsonSerializer.Serialize(test);
                var message = new MqttApplicationMessageBuilder()
                     .WithTopic("TestTopicTest")
                     .WithPayload(json)
                     .WithAtLeastOnceQoS()
                     .Build();
                await client.PublishAsync(message, CancellationToken.None);
                Thread.Sleep(500);
                Console.WriteLine($"Class {i} sent");
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