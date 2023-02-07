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
            string[] topic = { "VEng", "VScan", "VDet","VTim" };
            foreach (var topics in topic)
            {
                mqttClient.SubscribeAsync(topics, MQTTnet.Protocol.MqttQualityOfServiceLevel.AtMostOnce).Wait();
            }
            mqttClient.UseApplicationMessageReceivedHandler(e =>
            {
                string info = Encoding.UTF8.GetString(e.ApplicationMessage.Payload);
                //Console.WriteLine(info);
                if (e.ApplicationMessage.Topic == "VEng")
                {
                    Engine engineData = JsonSerializer.Deserialize<Engine>(info);
                    //Console.SetCursorPosition(0, 0);
                    Console.WriteLine("Двигатель " + engineData.ToString());
                    sendObject(engineData);
                    //Thread.Sleep(1000);
                }
                else if (e.ApplicationMessage.Topic == "VScan")
                {
                    Sensor sensorData = JsonSerializer.Deserialize<Sensor>(info);
                    //Console.SetCursorPosition(0, 1);
                    Console.WriteLine("Датчик " + sensorData.ToString());
                    //sendSensor(sensorData);
                    sendObject(sensorData);
                }
                else if (e.ApplicationMessage.Topic == "VDet")
                {
                    Detector detectorData = JsonSerializer.Deserialize<Detector>(info);
                    //Console.SetCursorPosition(0, 2);
                    Console.WriteLine("Детектор " + detectorData.ToString());
                    sendObject(detectorData);
                    //sendDetector(detectorData);
                }
                else if (e.ApplicationMessage.Topic == "VTim")
                {
                    Timer timerData = JsonSerializer.Deserialize<Timer>(info);
                    //Console.SetCursorPosition(0, 2);
                    //Console.WriteLine("Детектор " + detectorData.ToString());
                    Console.WriteLine("Таймер " + timerData.ToString());
                    //sendObject(timerData);
                    //sendDetector(detectorData);
                }


                Thread.Sleep(350);
                //Console.WriteLine(q);
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
                Console.WriteLine("Отправлено");
                Console.ResetColor();
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Ошибка");
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
                Console.WriteLine("ДВ");
            }
            else
            {
                Console.WriteLine("ErrorДВ");
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
                Console.WriteLine("СЕН");
            }
            else
            {
                Console.WriteLine("ErrorСЕН");
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
                Console.WriteLine("ДЕТ");
            }
            else
            {
                Console.WriteLine("ErrorДЕТ");
            }
        }
    }
}