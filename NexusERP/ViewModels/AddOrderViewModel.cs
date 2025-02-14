using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NexusERP.ViewModels
{
    public class AddOrderViewModel : ViewModelBase, IRoutableViewModel
    {
        public string UrlPathSegment => "addOrder";

        public IScreen HostScreen { get; }

        public AddOrderViewModel(IScreen screen)
        {
            HostScreen = screen;
        }
    }
}
