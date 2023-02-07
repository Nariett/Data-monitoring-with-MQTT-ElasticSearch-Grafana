namespace Publisher
{
    internal class Sensor
    {
        public int state { get; set; }
        public DateTime date { get; set; }
        public Sensor() { }
        public Sensor(int st)
        {
            this.state = st;
            this.date = DateTime.UtcNow;
        }
        public void Start()
        {
            Console.WriteLine("Датчик включен");
            state = 1;
            this.date = DateTime.UtcNow;
        }
        public void Stop()
        {
            Console.WriteLine("Датчик отключен");
            state = 0;
            this.date = DateTime.UtcNow;
        }
        public string State()
        {
            return state == 1 ? "Вкл" : "Выкл";
        }
        public override string ToString()
        {
            return $"Положение {this.state} Дата {this.date}";
        }
    }

}

