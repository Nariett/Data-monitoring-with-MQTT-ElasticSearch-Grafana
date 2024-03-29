﻿using Nest;
namespace Publisher
{
    internal class GeneralClass
    {
        public int positionEngine { get; set; }
        public int stateSensor { get; set; }
        public int stateDetector { get; set; }
        public DateTime date { get; set; }
        public GeneralClass() { }
        public GeneralClass(int positionEngine, int stateSensor, int stateDetector)
        {
            this.positionEngine = positionEngine;
            this.stateSensor = stateSensor;
            this.stateDetector = stateDetector;
            this.date = DateTime.UtcNow;
        }
        public void sendData()
        {
            var setting = new ConnectionSettings(new Uri("http://localhost:9200/"))
                                                    .DefaultIndex("test");
            var client = new ElasticClient(setting); ;
            var response = client.IndexDocument(this);
            if (response.IsValid)
            {
                //Console.WriteLine("Пользователь подключен")
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Ошибка подключения к ElasticSearch");
            }
        }
        public string Show()
        {
            return $"Положение двигателя: {positionEngine}\nПоложение датчика: {stateSensor}\nПоложение детектора: {stateDetector} \nВремя последнего обновления: {date}";
        }
    }
}
