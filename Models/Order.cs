using System;

namespace BTApp.Models
{
    public class Order
    {
        public string Id { get; set; }
        public int Length { get; set; }
        public int Quantity { get; set; }
        public int Heater { get; set; }
        public int SumLength { get; set; }
        public DateTime ImportDate { get; set; }

        public Order()
        {

        }
    }
}
