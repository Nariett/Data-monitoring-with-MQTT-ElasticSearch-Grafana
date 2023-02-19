using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Subsriber
{
    internal class Sensor
    {
        public int value { get; set; }
        public DateTime date { get; set; }
        public Sensor() { }
        public Sensor(int st)
        {
            this.value = st;
            this.date = DateTime.UtcNow;
        }
        public void Start()
        {
            value = 1;
        }
        public void Stop()
        {
            value = 0;
        }
        public string State()
        {
            return value == 1 ? "Вкл" : "Выкл";
        }
        public override string ToString()
        {
            return $"Значение {this.value} Дата {this.date}";
        }
    }
}
