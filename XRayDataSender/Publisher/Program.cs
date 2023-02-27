using MQTTnet;//3.1.0
using MQTTnet.Client;
using MQTTnet.Client.Options;
using System.IO.Ports;
using System.Text.Json;

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
            Console.SetCursorPosition(0, 1);
            //Console.WriteLine($"Принял сообщение: {data}");
            if (arr.Length == 8 && arr[2] == 11)
            {
                Console.WriteLine($"Принял сообщение: {BitConverter.ToString(arr)}");
                if (arr[3] == 64)//movement to the right
                {
                    lampOn();
                    engineRight();
                }
                if (arr[3] == 32)//movement to the left
                {
                    lampOff();
                    engineLeft();
                }
                if (arr[3] == 23)//stop
                {
                    stopPosition = false;
                    stopCommand();
                }
                if (arr[4] == 1)//if the object drove up to the edge of the platform
                {
                    lampOff();
                }
                if (arr[4] == 2 && stopPosition == false)
                {
                    lampOff();
                    stopPosition = true;
                }
                if (arr[3] == 16 || arr[3] == 23 || arr[3] == 19)//detector start
                {
                    detectorOne.Start();
                }
                if (arr[3] == 0 || arr[3] == 3 || arr[3] == 7)//detector stop
                {
                    detectorOne.Stop();
                }
                if (arr[4] == 0)//set null data
                {
                    sensorOne.value = 0;
                }
                else if (arr[4] == 1)//set data from left sensor
                {
                    sensorOne.value = 1;
                }
                else if (arr[4] == 2)//set data from right sensor
                {
                    sensorOne.value = 2;
                }
                if (arr[4] == 1 || arr[4] == 2)//engine stop
                {
                    engineOne.Stop();
                }
                updateTime();
            }
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
            Thread.Sleep(10);
        }
        static void lampOff()
        {
            byte[] off = new byte[] { 0xDC, 0x03, 0x04, 0x01 };
            byte[] message = Combine(off, GetCRC(off));
            _serialPort.Write(message, 0, message.Length);
            Thread.Sleep(10);
        }
        static void engineRight()
        {
            byte[] right = new byte[] { 0xDC, 0x03, 0x02, 0x01 };
            byte[] message = Combine(right, GetCRC(right));
            _serialPort.Write(message, 0, message.Length);
            engineOne.Right();
        }
        static void engineLeft()
        {
            byte[] left = new byte[] { 0xDC, 0x03, 0x01, 0x01 };//AA max
            byte[] message = Combine(left, GetCRC(left));
            _serialPort.Write(message, 0, message.Length);
            engineOne.Left();
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
        static async public void SendData()//send data function to mosquitto
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
                engineOne.date = DateTime.UtcNow;//update date
                sensorOne.date = DateTime.UtcNow;
                detectorOne.date = DateTime.UtcNow;
                await SendObject(client, engineOne, "VEng");//send data
                await SendObject(client, sensorOne, "VScan");
                await SendObject(client, detectorOne, "VDet");
                await client.DisconnectAsync();//must be disconnected from the client
                Thread.Sleep(100);
            }
        }
        private static async Task SendObject(IMqttClient client, object obj, string topic)//function to send object to mosquitto
        {
            string json = JsonSerializer.Serialize(obj);
            var message = new MqttApplicationMessageBuilder()
                 .WithTopic(topic)//name topic
                 .WithPayload(json)//file that we send to mosqutto
                 .WithAtLeastOnceQoS()
                 .Build();
            await client.PublishAsync(message, CancellationToken.None);
            //Thread.Sleep(100);
        }
    }
}