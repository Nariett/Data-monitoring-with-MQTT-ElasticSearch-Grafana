namespace CRUD
{
    internal class Engine
    {
        public int position { get; set; }
        public DateTime date { get; set; }
        public Engine(int state)
        {
            this.position = state;
            this.date = DateTime.UtcNow;
        }
        public Engine() { }

        public void Right()
        {
            Console.WriteLine("Двигатель вращается вправо");
            this.position = 1;
        }
        public void Left()
        {
            Console.WriteLine("Двигатель вращается влево");
            this.position = 2;
        }
        public void Stop()
        {
            Console.WriteLine("Двигатель остановлен");
            this.position = 0;
        }
        public string Position()
        {
            if (position == 1) return "Вращение вправо";
            else if (position == 2) return "Вращение влево";
            else return "Выключен";
        }
        public override string ToString()
        {
            return $"Положение {this.position} Дата {this.date}";
        }
    }
}
