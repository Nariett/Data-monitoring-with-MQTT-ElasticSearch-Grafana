namespace Publisher
{
    internal class Engine
    {
        public int position { get; set; }
        public DateTime date { get; set; }
        public Engine() { }
        public Engine(int position)
        {
            this.position = position;
        }
        public void Right()
        {
            this.position = 1;
        }
        public void Left()
        {
            this.position = 2;
        }
        public void Stop()
        {
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
            return $"Положение {this.position}\n Дата {this.date}";
        }
    }
}
