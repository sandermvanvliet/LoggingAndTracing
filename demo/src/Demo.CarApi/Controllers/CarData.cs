using System;

namespace Demo.CarApi.Controllers
{
    public class CarData
    {
        public DateTime Timestamp { get; set; }
        public int BatteryPercentage { get; set; }
        public string ChageState { get; set; }
    }
}