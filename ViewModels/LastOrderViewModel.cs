using BTApp.Models;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace BTApp.ViewModels
{
    public class LastOrderViewModel
    {
        public ObservableCollection<Order> lastOrder { get; set; }

        public LastOrderViewModel(List<Order> order)
        {
            lastOrder = new ObservableCollection<Order>(order);
        }

      
    }
}
