using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Publisher
{
    internal class Detector
    {
        public int result { get; set; }
        public DateTime date { get; set; }
        public Detector() { }
        public Detector(int result)
        {
            this.result = result;
            this.date = DateTime.UtcNow;
        }
        public void Start()
        {
            this.result = 1;
        }
        public void Stop()
        {
            this.result = 0;
        }
        public string State()
        {
            return result == 1 ? "Вкл" : "Выкл";
        }
        public override string ToString()
        {
            return $"Положение {this.result} Date {this.date}";
        }
    }
}
