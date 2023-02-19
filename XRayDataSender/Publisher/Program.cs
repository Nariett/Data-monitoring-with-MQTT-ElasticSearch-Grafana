using System.IO.Ports;
using System.Text.Json;
using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Client.Options;

namespace Publisher
{
    class Program
    {
        static SerialPort _serialPort;
        static bool stopPosition = false;
        public static Engine engineOne = new Engine(0);
        public static Sensor sensorOne = new Sensor(0);
        public static Detector detectorOne = new Detector(0);
        static void Main(string[] args)
        {
            _serialPort = new SerialPort("COM11",
                                               19200,
                                               Parity.None,
                                               8,
                                               StopBits.One);
            _serialPort.Handshake = Handshake.None;
            _serialPort.DtrEnable = true;
            _serialPort.Open();
            int num = 0;
            SendData();
            TimerCallback tm = new TimerCallback(sendStatus);
            Timer timer = new Timer(tm, num, 0, 50);
            _serialPort.DataReceived += new SerialDataReceivedEventHandler(DataReceivedHandler);

            Console.ReadLine();
        }
        public static void sendStatus(object obj)
        {
            byte[] status = new byte[] { 0xDC, 0x02, 0x0B };
            byte[] message = Combine(status, GetCRC(status));
            _serialPort.Write(message, 0, message.Length);
            //Console.SetCursorPosition(0, 0);
            //WriteLine("Отправил сообщение " + DateTime.Now);
        }
        private static void DataReceivedHandler(
                           object sender,
                           SerialDataReceivedEventArgs e)
        {
            SerialPort sp = (SerialPort)sender;
            string indata = sp.ReadExisting();
            byte[] arr = System.Text.Encoding.UTF8.GetBytes(indata); //3 кнопки и сенсор 4 датчик 5 
            string data = BitConverter.ToString(arr);
            //Console.SetCursorPosition(0, 1);
            //Console.WriteLine($"Принял сообщение: {data} {DateTime.Now}");
            if (arr.Length == 8)
            {
                //Console.WriteLine(stopPosition);
                if (arr[4] == 2 && stopPosition == true)
                {
                    lampOff();
                    stopPosition = false;
                }
                if (arr[4] == 1 || arr[4] == 2)
                {
                    sensorOne.Start();
                }
                else { sensorOne.Stop(); }
                if (arr[3] == 32) engineLeft();
                if (arr[3] == 3) engineOne.Left();
                else if (arr[3] == 7) engineOne.Right();
                else if (arr[3] == 0) engineOne.Stop();
                else if (arr[3] == 16 || arr[3] == 23 || arr[3] == 19) detectorOne.Start();

                if (arr[3] == 0 || arr[3] == 3 || arr[3] == 7) detectorOne.Stop();
                else if (arr[3] == 64)///////не останвливается 
                {
                    lampOn();
                    engineRight();
                }
                if (arr[3] == 23 && stopPosition == false) stopCommand();
            }
            updateTime();
        }


        static byte[] Combine(byte[] first, byte[] second)
        {
            byte[] bytes = new byte[first.Length + second.Length];
            Buffer.BlockCopy(first, 0, bytes, 0, first.Length);
            Buffer.BlockCopy(second, 0, bytes, first.Length, second.Length);
            return bytes;
        }
        static byte[] GetCRC(byte[] arr)
        {
            Int16 sum = 0;
            for (var i = 0; i < arr.Length; i++)
                sum = (Int16)((sum + arr[i]) & WLB_CUT);
            return new byte[] {(byte)(((sum^WLB_CUT)+1) & WLB_CUT)
            };
        }
        private const Int16 WLB_CUT = 0x00FF;





        static void updateTime()
        {
            engineOne.date = DateTime.UtcNow;
            sensorOne.date = DateTime.UtcNow;
            detectorOne.date = DateTime.UtcNow;
        }
        static void lampOn()
        {
            byte[] on = new byte[] { 0xDC, 0x03, 0x03, 0x01 };
            byte[] message = Combine(on, GetCRC(on));
            _serialPort.Write(message, 0, message.Length);
        }
        static void lampOff()
        {
            byte[] off = new byte[] { 0xDC, 0x03, 0x04, 0x01 };
            byte[] message = Combine(off, GetCRC(off));
            _serialPort.Write(message, 0, message.Length);
        }
        static void engineRight()
        {
            byte[] right = new byte[] { 0xDC, 0x03, 0x02, 0x01 };
            byte[] message = Combine(right, GetCRC(right));
            _serialPort.Write(message, 0, message.Length);
        }
        static void engineLeft()
        {
            byte[] left = new byte[] { 0xDC, 0x03, 0x01, 0x01 };//AA max
            byte[] message = Combine(left, GetCRC(left));
            _serialPort.Write(message, 0, message.Length);
        }
        static void stopCommand()
        {
            byte[] stop = new byte[] { 0xDC, 0x02, 0x21 };
            byte[] message = Combine(stop, GetCRC(stop));
            _serialPort.Write(message, 0, message.Length);
            stopPosition = true;
            Thread.Sleep(1000);
            engineLeft();
        }
        static async public void SendData()
        {
            while (true)
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
                await SendObject(client, engineOne, "VEng");
                await SendObject(client, sensorOne, "VScan");
                await SendObject(client, detectorOne, "VDet");
                await client.DisconnectAsync();
                Thread.Sleep(200);
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
    }
}