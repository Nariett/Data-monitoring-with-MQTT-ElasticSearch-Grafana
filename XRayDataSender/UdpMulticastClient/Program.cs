using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Color = System.Drawing.Color;
using UdpClient = NetCoreServer.UdpClient;

namespace UdpMulticastClient
{
    class MulticastClient : UdpClient
    {
        public string Multicast;
        public List<byte[]> reciveBuffer = new List<byte[]>();
        public List<byte[]> ByteList
        {
            get { return reciveBuffer; }
            set { reciveBuffer = value; }
        }
        public MulticastClient(string address, int port) : base(address, port) { }

        public void DisconnectAndStop()
        {
            _stop = true;
            Disconnect();
            while (IsConnected)
                Thread.Yield();
        }

        protected override void OnConnected()
        {
            Console.WriteLine($"Multicast UDP client connected a new session with Id {Id}");

            // Join UDP multicast group
            JoinMulticastGroup(Multicast);

            // Start receive datagrams
            ReceiveAsync();
        }

        protected override void OnDisconnected()
        {
            Console.WriteLine($"Multicast UDP client disconnected a session with Id {Id}");

            // Wait for a while...
            Thread.Sleep(1000);

            // Try to connect again
            if (!_stop)
                Connect();
        }

        protected override void OnReceived(EndPoint endpoint, byte[] buffer, long offset, long size)
        {
            Console.WriteLine("Incoming: " + Encoding.UTF8.GetString(buffer, (int)offset, (int)size));
            Console.WriteLine("Incoming: " + Encoding.UTF8.GetString(buffer, (int)offset, (int)size).Length);
            byte[] newBuffer = new byte[Encoding.UTF8.GetString(buffer, (int)offset, (int)size).Length - 16];
            for (int i = 0; i < newBuffer.Length - 16; i++)
            {
                newBuffer[i] = buffer[i + 16];
            }
            reciveBuffer.Add(newBuffer);//add data in list
            ReceiveAsync();
        }

        protected override void OnError(SocketError error)
        {
            Console.WriteLine($"Multicast UDP client caught an error with code {error}");
        }
        private bool _stop;
    }

    class Program
    {
        static async Task Main(string[] args)
        {
            string listenAddress = "0.0.0.0";
            if (args.Length > 0)
                listenAddress = args[0];

            // UDP multicast address
            string multicastAddress = "230.168.1.0";
            if (args.Length > 1)
                multicastAddress = args[1];

            // UDP multicast port
            int multicastPort = 5000;
            if (args.Length > 2)
                multicastPort = int.Parse(args[2]);

            Console.WriteLine($"UDP listen address: {listenAddress}");
            Console.WriteLine($"UDP multicast address: {multicastAddress}");
            Console.WriteLine($"UDP multicast port: {multicastPort}");

            Console.WriteLine();
            // Create a new TCP chat client
            var client = new MulticastClient(listenAddress, multicastPort);
            client.SetupMulticast(true);
            client.Multicast = multicastAddress;

            // Connect the client
            Console.Write("Client connecting...");
            client.Connect();

            Console.WriteLine("Done!");
            await Disconnect(client, 5000);//setting a timer for 5 seconds
            Console.WriteLine("Press Enter to stop the client or '!' to reconnect the client...");

            // Perform text input
            for (; ; )
            {
                string line = Console.ReadLine();
                if (string.IsNullOrEmpty(line))
                    break;

                // Disconnect the client
                if (line == "!")
                {
                    Console.Write("Client disconnecting...");
                    client.Disconnect();
                    Console.WriteLine("Done!");
                    continue;
                }
            }
            // Disconnect the client
            Console.Write("Client disconnecting...");
            client.DisconnectAndStop();
            Console.WriteLine("Done!");
        }
        public static async Task Disconnect(MulticastClient client, int delay)
        {
            await Task.Delay(delay);
            Console.WriteLine("Disconnect");
            client.DisconnectAndStop();
            Console.WriteLine("Done!");
            CreatePNG(client.reciveBuffer);
        }
        public static void CreatePNG(List<byte[]> reciveBuffer)
        {
            int minWidth = reciveBuffer[0].Length;
            for (int i = 0; i < reciveBuffer.Count; i++)//find minLength
            {
                if (minWidth > reciveBuffer[i].Length) minWidth = reciveBuffer[i].Length;
            }
            int width = minWidth;
            int height = reciveBuffer.Count;//set height 
            using var bitmap = new Bitmap(width, height, PixelFormat.Format8bppIndexed);
            BitmapData bmpData = bitmap.LockBits(new System.Drawing.Rectangle(System.Drawing.Point.Empty, bitmap.Size), ImageLockMode.WriteOnly, bitmap.PixelFormat);

            // copy our date in bmpData 

            for (int row = 0; row < height; row++)
            {
                // bmpData.Stride - длина строки битмапа в байтах
                // bmpData.Scan0 - указатель на начало данных битмапа в неуправляемой области памяти GDI+
                Marshal.Copy(reciveBuffer[row], 0, bmpData.Scan0 + (row * width), width);
            }

            // save png with defined palette
            bitmap.UnlockBits(bmpData);
            ColorPalette pal = bitmap.Palette;
            for (int c = 0; c < 256; c++)//set rgb color
                pal.Entries[c] = Color.FromArgb(c, c, c);
            bitmap.Palette = pal;
            bitmap.Save("C:\\Users\\samoylov\\Desktop\\Scann.png", System.Drawing.Imaging.ImageFormat.Png);
        }
        private static byte[][] CreateData(int size, int tileSize)
        {
            int count = size * size;
            byte[][] data = new byte[count][];
            for (int i = 0; i < count; i++)
            {
                data[i] = CreateArray(tileSize * tileSize, (byte)(i * 4)); // 0 - 252 с шагом 4, для палитры с градациями серого
            }
            return data;
        }

        private static byte[] CreateArray(int size, byte value)
        {
            var array = new byte[size];
            Array.Fill(array, value);
            return array;
        }

    }
}