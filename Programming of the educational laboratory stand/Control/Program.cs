using System.IO.Ports;
namespace Control
{
    class Program
    {
        static SerialPort _serialPort;
        static bool stopPosition = false;
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
            TimerCallback tm = new TimerCallback(sendStatus);
            Timer timer = new Timer(tm, num, 0, 25);
            _serialPort.DataReceived += new SerialDataReceivedEventHandler(DataReceivedHandler);
            Console.ReadLine();
        }
        public static void sendStatus(object obj)
        {
            byte[] status = new byte[] { 0xDC, 0x02, 0x0B };
            byte[] message = Combine(status, GetCRC(status));
            _serialPort.Write(message, 0, message.Length);
            Console.SetCursorPosition(0, 0);
            Console.WriteLine("Отправил сообщение " + DateTime.Now);
        }
        private static void DataReceivedHandler(object sender, SerialDataReceivedEventArgs e)
        {
            SerialPort sp = (SerialPort)sender;
            string indata = sp.ReadExisting();
            byte[] arr = System.Text.Encoding.UTF8.GetBytes(indata); //3 кнопки и сенсор 4 датчик 5 
            string data = BitConverter.ToString(arr);
            Console.SetCursorPosition(0, 1);
            Console.WriteLine($"Принял сообщение: {data} {DateTime.Now}");




            if (arr.Length == 8)
            {
                /*if (arr[4] == 2)//двигатели
                {
                    lampOn();
                    engineRight();
                }
                else if (arr[4] == 1)
                {
                    lampOff();
                    engineLeft();
                }*/
                if (arr[3] == 32)
                {
                    /*lampOn();*/
                    engineLeft();

                }
                else if (arr[3] == 64)
                {
                    lampOn();
                    /*lampOff();*/
                    engineRight();
                }
                if (arr[3] == 23 && stopPosition == false)
                {
                    Console.SetCursorPosition(0, 10);
                    Console.WriteLine("Отановлен");
                    stopCommand();
                }
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
            lampOn();
            Thread.Sleep(1000);
            engineLeft();
            stopPosition = false;
        }
    }
}