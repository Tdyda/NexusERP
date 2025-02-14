using Avalonia.Media;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NexusERP.ViewModels
{
    public class OrderListViewModel : ViewModelBase, IRoutableViewModel
    {
        public string UrlPathSegment => "orderList";

        public IScreen HostScreen { get; }

        public OrderListViewModel(IScreen screen)
        {
            HostScreen = screen;
        }
    }
}
