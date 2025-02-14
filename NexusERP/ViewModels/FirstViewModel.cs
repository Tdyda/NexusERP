using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NexusERP.ViewModels
{
    public class FirstViewModel : ViewModelBase, IRoutableViewModel
    {
        public IScreen HostScreen { get; }
        public string? UrlPathSegment { get; } = Guid.NewGuid().ToString().Substring(0, 5);
        public FirstViewModel(IScreen screen) => HostScreen = screen;
    }
}
