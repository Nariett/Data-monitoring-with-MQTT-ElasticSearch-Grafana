using MQTTnet;//3.1.0
using MQTTnet.Client;
using MQTTnet.Client.Options;
using Nest;
using System.Text;
using System.Text.Json;
namespace Subsriber
{
    class Program
    {
        static void Main(string[] args)
        {
            var factory = new MqttFactory();
            var mqttClient = factory.CreateMqttClient();
            var options = new MqttClientOptionsBuilder()
                              .WithWebSocketServer("test.mosquitto.org:8080")
                              .WithCleanSession()
                              .Build();
            mqttClient.ConnectAsync(options).Wait();
            string[] topic = { "VEng", "VScan", "VDet" };//topics that accept data
            foreach (var topics in topic)
            {
                mqttClient.SubscribeAsync(topics, MQTTnet.Protocol.MqttQualityOfServiceLevel.AtMostOnce).Wait();
            }
            mqttClient.UseApplicationMessageReceivedHandler(e =>
            {
                string info = Encoding.UTF8.GetString(e.ApplicationMessage.Payload);
                if (e.ApplicationMessage.Topic == "VEng")//get topic name
                {
                    Engine engineData = JsonSerializer.Deserialize<Engine>(info);
                    Console.WriteLine("Двигатель " + engineData.ToString());
                    sendObject(engineData);//Sending data to elasticSearch
                }
                else if (e.ApplicationMessage.Topic == "VScan")
                {
                    Sensor sensorData = JsonSerializer.Deserialize<Sensor>(info);
                    Console.WriteLine("Датчик " + sensorData.ToString());
                    sendObject(sensorData);
                }
                else if (e.ApplicationMessage.Topic == "VDet")
                {
                    Detector detectorData = JsonSerializer.Deserialize<Detector>(info);
                    Console.WriteLine("Детектор " + detectorData.ToString());
                    sendObject(detectorData);
                }
                Thread.Sleep(350);
            });
            Console.ReadLine();
        }
        static void sendObject(object classObject)//Sending data to elasticSearch
        {
            var setting = new ConnectionSettings(new Uri("http://localhost:9200/"))
                                                    .DefaultIndex("all");///send in index "all" (index must be created manually. Creating an Index on a CRUD ElasticSearc Repository)
            var client = new ElasticClient(setting);
            var response = client.IndexDocument(classObject);
            if (response.IsValid)
            {
                Console.ForegroundColor = ConsoleColor.Green; // set color
                Console.WriteLine("Отправлено");
                Console.ResetColor();
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Ошибка отправки");
                Console.ResetColor();
            }
        }
    }
}
