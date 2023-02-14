using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Client.Options;
using Nest;
using System;
using System.Text;
using System.Text.Json;

namespace Subscriber
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
            string[] topic = { "VEng", "VScan", "VDet"};
            foreach (var topics in topic)
            {
                mqttClient.SubscribeAsync(topics, MQTTnet.Protocol.MqttQualityOfServiceLevel.AtMostOnce).Wait();
            }
            mqttClient.UseApplicationMessageReceivedHandler(e =>
            {
                string info = Encoding.UTF8.GetString(e.ApplicationMessage.Payload);
                if (e.ApplicationMessage.Topic == "VEng")
                {
                    Engine engineData = JsonSerializer.Deserialize<Engine>(info);
                    Console.WriteLine("Двигатель " + engineData.ToString());
                    sendObject(engineData);
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
        static void sendObject(object classObject)
        {
            /*ConnectionSettings setting;
            Type type = classObject.GetType();
            if (type == typeof(Engine))
            {
                setting = new ConnectionSettings(new Uri("http://localhost:9200/"))
                                                    .DefaultIndex("engine");
                Engine x = classObject as Engine;
                //x.date = DateTime.UtcNow;
                Console.WriteLine("Двигатель" + x.GetType()) ;
                var client = new ElasticClient(setting);
                var response = client.IndexDocument(x);
                if (response.IsValid)
                {
                    Console.WriteLine("Отправлено");
                }
                else
                {
                    Console.WriteLine("Error");
                }

            }
            else if (type == typeof(Sensor))
            {
                setting = new ConnectionSettings(new Uri("http://localhost:9200/"))
                                                    .DefaultIndex("sensor");
                Sensor x = classObject as Sensor;
                //x.date = DateTime.UtcNow;
                Console.WriteLine("Датчик");
                var client = new ElasticClient(setting);
                var response = client.IndexDocument(x);
                if (response.IsValid)
                {
                    Console.WriteLine("Отправлено");
                }
                else
                {
                    Console.WriteLine("Error");
                }
            }
            else if (type == typeof(Detector))
            {
                setting = new ConnectionSettings(new Uri("http://localhost:9200/"))
                                                    .DefaultIndex("detector");
                Console.WriteLine("Детектор");
                Detector x = classObject as Detector;
                //x.date = DateTime.UtcNow;
                var client = new ElasticClient(setting);
                var response = client.IndexDocument(x);
                if (response.IsValid)
                {
                    Console.WriteLine("Отправлено");
                }
                else
                {
                    Console.WriteLine("Error");
                }
            }*/
            var setting = new ConnectionSettings(new Uri("http://localhost:9200/"))
                                                    .DefaultIndex("alldata");///select on test
            var client = new ElasticClient(setting);
            var response = client.IndexDocument(classObject);
            if (response.IsValid)
            {
                Console.ForegroundColor = ConsoleColor.Green; // set color
                Console.WriteLine($"Данные отправлены");
                Console.ResetColor();
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Ошибка отправки данных");
                Console.ResetColor();
            }
        }
        static void sendEngine(Engine classEngine) 
        {
            var setting = new ConnectionSettings(new Uri("http://localhost:9200/"))
                                                    .DefaultIndex("test");
            var client = new ElasticClient(setting); ;
            var response = client.IndexDocument(classEngine);
            if (response.IsValid)
            {
                Console.WriteLine("Данные о двигателе отправлены");
            }
            else
            {
                Console.WriteLine("Ошибка отправки данных о двигателе");
            }
        }
        static void sendSensor(Sensor classSensor)
        {
            var setting = new ConnectionSettings(new Uri("http://localhost:9200/"))
                                                    .DefaultIndex("test");
            var client = new ElasticClient(setting);
            var response = client.IndexDocument(classSensor);
            if (response.IsValid)
            {
                Console.WriteLine("Данные о датчике отправлены");
            }
            else
            {
                Console.WriteLine("Ошибка отправки данных о датчике");
            }
        }
        static void sendDetector(Detector classDetector)
        {
            var setting = new ConnectionSettings(new Uri("http://localhost:9200/"))
                                                    .DefaultIndex("test");
            var client = new ElasticClient(setting); ;
            var response = client.IndexDocument(classDetector);
            if (response.IsValid)
            {
                Console.WriteLine("Данные о детекторе отправлены");
            }
            else
            {
                Console.WriteLine("Ошибка отправки данных о детекторе");
            }
        }
    }
}