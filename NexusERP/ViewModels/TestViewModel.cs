using Avalonia.ReactiveUI;
using NexusERP.Interfaces;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NexusERP.ViewModels
{
    public class TestViewModel : ViewModelBase, IAuthorizedViewModel
    {
        public string[] RequiredRoles => [];

        public string? UrlPathSegment => "test";

        public IScreen HostScreen { get; }

        public TestViewModel(IScreen screen)
        {
            HostScreen = screen;
        }
    }
}
