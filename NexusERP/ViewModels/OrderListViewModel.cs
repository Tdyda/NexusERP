using Avalonia.Media;
using NexusERP.Interfaces;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NexusERP.ViewModels
{
    public class OrderListViewModel : ViewModelBase, IAuthorizedViewModel
    {
        public string UrlPathSegment => "orderList";

        public IScreen HostScreen { get; }

        public string[] RequiredRoles => [];

        public OrderListViewModel(IScreen screen)
        {
            HostScreen = screen;
        }
    }
}
