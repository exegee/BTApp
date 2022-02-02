using BTApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BTApp.Events
{
    public class OrderProcessingEventArgs : EventArgs
    {
        public List<Order> orders { get; set; }
    }
}
