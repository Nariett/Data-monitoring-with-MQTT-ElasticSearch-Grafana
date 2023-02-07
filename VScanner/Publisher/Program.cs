using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Client.Options;
using System.Diagnostics;
using System.Text.Json;
namespace Publisher
{
    class Program
    {
        static public Engine engineOne = new Engine(0);
        static public Sensor sensorOne = new Sensor(0);
        static public Detector detectorOne = new Detector(0);
        static public GeneralClass generalClass = new GeneralClass();
        static public Timer TimeWhile = new Timer(false);
        static public void Main(string[] args)
        {
            int select = 0;
            //int sensorState, item, engneState, scanerState = 0;
            SendData();/////
            KeyEvent();
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();
            int iterationCounter = 0;
            while (select == 0)
            {
                TimeWhile.tick = !TimeWhile.tick;
                //Console.WriteLine(TimeWhile.tick);
                /*iterationCounter++;
                if (stopWatch.ElapsedMilliseconds >= 1000)
                {
                    Console.WriteLine("iteration " + iterationCounter);
                    break;
                }*/
                /*Console.WriteLine($"Основное состояние устройств:\nДвигатель = {engineOne.Position()}\nДатчик = {sensorOne.State()}\nДетектор = {detectorOne.State()}");
                Console.WriteLine("Выберите устройство:\n1 - Двигатель\n2 - Сенсор\n3 - Детектор\n4 - Сценарии");
                item = Convert.ToInt32(Console.ReadLine());

                switch (item)
                {
                    case 1:
                        {
                            Console.WriteLine("Выберите положение:\n1 - вправо\n2 - влево\n3 - остановить\n4 - вернуться назад");
                            engneState = Convert.ToInt32(Console.ReadLine());
                            switch (engneState)
                            {
                                case 1:
                                    {
                                        Console.WriteLine("Выбрано положение вправо");
                                        engineOne.Right();
                                        break;
                                    }
                                case 2:
                                    {
                                        Console.WriteLine("Выбрано положение влево");
                                        engineOne.Left();
                                        break;
                                    }
                                case 3:
                                    {
                                        Console.WriteLine("Выбрано положение остановить");
                                        engineOne.Stop();
                                        break;
                                    }
                                case 4: break;
                                default:
                                    {
                                        Console.WriteLine("Ошибка ввода данных. Вы возращены в главное меню!");
                                        break;
                                    }
                            }
                            break;
                        }
                    case 2:
                        {
                            Console.WriteLine("Выберите положение:\n1 - ВКЛ\n2 - ВЫКЛ\n3 - Остановить программу");
                            sensorState = Convert.ToInt32(Console.ReadLine());
                            switch (sensorState)
                            {
                                case 1:
                                    {
                                        Console.WriteLine("Выбрано положение ВКЛ");
                                        sensorOne.Start();
                                        break;
                                    }
                                case 2:
                                    {
                                        Console.WriteLine("Выбрано положение Выкл");
                                        sensorOne.Start();
                                        break;
                                    }
                                case 3: break;
                                default:
                                    {
                                        Console.WriteLine("Ошибка ввода данных. Вы возращены в главное меню! ");
                                        break;
                                    }
                            }
                            break;
                        }
                    case 3:
                        {
                            Console.WriteLine("Выберите положение:\n1 - ВКЛ\n2 - ВЫКЛ\n3 - Остановить программу");
                            scanerState = Convert.ToInt32(Console.ReadLine());
                            switch (scanerState)
                            {
                                case 1:
                                    {
                                        Console.WriteLine("Выбрано положение ВКЛ");
                                        detectorOne.Start();
                                        break;
                                    }
                                case 2:
                                    {
                                        Console.WriteLine("Выбрано положение Выкл");
                                        detectorOne.Stop();
                                        break;
                                    }
                                case 3: break;
                                default:
                                    {
                                        Console.WriteLine("Ошибка ввода данных. Вы возращены в главное меню! ");
                                        break;
                                    }
                            }
                            break;
                        }
                    case 4:
                        {
                            Console.WriteLine("Проводится инициализация устройств");
                            allInit();
                            Console.WriteLine("Устройства проинициализированы ");
                            engineOne.Stop();
                            EngineStart();
                            DetectorStart();
                            for (int i = 0; ; i++)
                            {
                                if (i == 4)
                                {
                                    Console.WriteLine("Сенсор обнаружил обьект");
                                    sensorOne.Start();
                                }
                                else Thread.Sleep(1000);
                                if (engineOne.position == 1 && sensorOne.state == 1 && detectorOne.result == 1)
                                {
                                    Console.WriteLine("Проводится сканирование");
                                    Thread.Sleep(20000);
                                    break;
                                }
                            }
                            engineOne.position = 1;
                            Console.WriteLine("Объект выехал из сканера");
                            devicesOff();
                            break;

                        }
                    default:
                        {
                            Console.WriteLine("Ошибка ввода данных");
                            break;
                        }
                }*/
            }
            //Console.ReadKey();
        }

        static async public void SendData()
        {
            while(true)
            {
                var mqttFactory = new MqttFactory();
                IMqttClient client = mqttFactory.CreateMqttClient();
                var options = new MqttClientOptionsBuilder()
                                  .WithWebSocketServer("test.mosquitto.org:8080")
                                  .WithCleanSession()
                                  .Build();
                await client.ConnectAsync(options, CancellationToken.None);
                if (client.IsConnected)
                {
                    //Console.WriteLine("Пользователь подключен");
                }
                else
                {
                    Console.WriteLine("Пользователь не подключен");
                }
                engineOne.date = DateTime.UtcNow;
                sensorOne.date = DateTime.UtcNow;
                detectorOne.date = DateTime.UtcNow;
                TimeWhile.date = DateTime.UtcNow;
                await SendObject(client, engineOne, "VEng");
                await SendObject(client, sensorOne, "VScan");
                await SendObject(client, detectorOne, "VDet");
                await SendObject(client, TimeWhile, "VTim");
                /*string jsonEngine = JsonSerializer.Serialize(engineOne);
                var message = new MqttApplicationMessageBuilder()
                     .WithTopic("VEng")
                     .WithPayload(jsonEngine)
                     .WithAtLeastOnceQoS()
                     .Build();
                await client.PublishAsync(message, CancellationToken.None);
                //Console.WriteLine($"Класс  двигатель отправлен ");

                string jsonSensor = JsonSerializer.Serialize(sensorOne);
                var message2 = new MqttApplicationMessageBuilder()
                     .WithTopic("VScan")
                     .WithPayload(jsonSensor)
                     .WithAtLeastOnceQoS()
                     .Build();
                await client.PublishAsync(message2, CancellationToken.None);
                //Console.WriteLine($"Класс  датчик отправлен ");

                string jsonDetector = JsonSerializer.Serialize(detectorOne);
                var message3 = new MqttApplicationMessageBuilder()
                     .WithTopic("VDet")
                     .WithPayload(jsonDetector)
                     .WithAtLeastOnceQoS()
                     .Build();
                await client.PublishAsync(message3, CancellationToken.None);*/
                await client.DisconnectAsync();
                Thread.Sleep(1000);

            }
        }
        private static async Task SendObject(IMqttClient client, object obj, string topic)//send object in mqtt
        {
            string json = JsonSerializer.Serialize(obj);
            var message = new MqttApplicationMessageBuilder()
                 .WithTopic(topic)
                 .WithPayload(json)
                 .WithAtLeastOnceQoS()
                 .Build();
            await client.PublishAsync(message, CancellationToken.None);
            Thread.Sleep(1000);
        }
        static public async void KeyEvent()
        {
            await Task.Run(() =>
            {

                ConsoleKeyInfo key = Console.ReadKey(true);
                while (true)
                {
                    if (key.Key == ConsoleKey.F1) engineOne.Right();
                    else if (key.Key == ConsoleKey.F2) engineOne.Left();
                    else if (key.Key == ConsoleKey.F3) engineOne.Stop();
                    else if (key.Key == ConsoleKey.F4) sensorOne.Start();
                    else if (key.Key == ConsoleKey.F5) sensorOne.Stop();
                    else if (key.Key == ConsoleKey.F6) detectorOne.Start();
                    else if (key.Key == ConsoleKey.F7) detectorOne.Stop();
                    else if (key.Key == ConsoleKey.F8) Scanning();
                    else if (key.Key == ConsoleKey.F12) ShowState();
                    key = Console.ReadKey(false);
                }
            });
        }
        static public void DevicesOff()//off all
        {
            engineOne.Stop();
            sensorOne.Stop();
            detectorOne.Stop();
            Console.WriteLine("Сканер выключен");
        }
        static public void ShowState()
        {
            Console.WriteLine($"Положение устрйоств\nПоложение двигателя: {engineOne.position}\nСостояние датчика: {sensorOne.state}\nСостояние детектора: {detectorOne.result}");
        }
        static async public void EngineStart()
        {
            await Task.Run(() =>
            {
                while(true)
                {
                    if (sensorOne.state == 1)
                    {
                        Console.WriteLine("Объект обнаружен");
                        engineOne.position = 1;
                        Console.WriteLine("Запущен двигатель");
                        break;
                    }

                }
            });
        }
        static async public void DetectorStart()
        {
            await Task.Run(() =>
            {
                int i = 0;
                while(true)
                {
                    i++;
                    if (i == 4)
                    {
                        Console.WriteLine("Сенсор обнаружил обьект");
                        sensorOne.Start();
                    }
                    else Thread.Sleep(1000);
                    if (engineOne.position == 1 && sensorOne.state == 1 && detectorOne.result == 1)
                    {
                        //Console.WriteLine("Проводится сканирование");
                        Thread.Sleep(2000);
                        break;
                    }

                }
            });
        }

        static public GeneralClass СreateClass()
        {
            return new GeneralClass(engineOne.position, sensorOne.state, detectorOne.result);
        }
        static public void AllInit()//device initialization
        {
            var readyEngine = EngineInit();
            var readySensor = SensorInit();
            var readyDetector = DetectorInit();
            Task.WaitAll(readyDetector, readyEngine, readySensor);
        }
        static public async Task<bool> EngineInit()
        {
            await Task.Delay(1000);
            return true;
        }
        static public async Task<bool> SensorInit()
        {
            await Task.Delay(2000);
            return true;
        }
        static public async Task<bool> DetectorInit()
        {
            await Task.Delay(4000);
            return true;
        }
        static public void Scanning()
        {
            Console.WriteLine("Проводится инициализация устройств");
            AllInit();
            Console.WriteLine("Устройства проинициализированы ");
            EngineStart();
            DetectorStart();
            int i = 0;
            while (true)
            {
                i++;
                if (i == 4)
                {
                    Console.WriteLine("Сенсор обнаружил обьект");
                    sensorOne.Start();
                    detectorOne.Start();
                }
                else Thread.Sleep(1000);
                if (engineOne.position == 1 && sensorOne.state == 1 && detectorOne.result == 1)
                {
                    Console.WriteLine("Проводится сканирование");
                    Thread.Sleep(20000);
                    break;
                }
            }
            engineOne.position = 1;
            Console.WriteLine("Объект выехал из сканера");
            DevicesOff();
        }
    }
}