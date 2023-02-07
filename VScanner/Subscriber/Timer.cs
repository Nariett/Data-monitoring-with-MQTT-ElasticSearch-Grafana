using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Subscriber
{
    internal class Timer
    {
        public bool tick { get; set; }
        public DateTime date { get; set; }
        public Timer() { }
        public Timer(bool state)
        {
            this.tick = state;
            this.date = DateTime.UtcNow;
        }
        public override string ToString()
        {
            return $"Состояние {this.tick} date {this.date}";
        }
    }
}
