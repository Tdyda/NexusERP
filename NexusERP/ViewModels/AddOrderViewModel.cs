using NexusERP.Models;
using NexusERP.ViewModels;
using ReactiveUI;

public class AddOrderViewModel : ViewModelBase, IAuthorizedViewModel
{
    public string UrlPathSegment => "addOrder";
    public IScreen HostScreen { get; }
    public string[] RequiredRoles => ["admin"];

    public AddOrderViewModel(IScreen screen)
    {
        HostScreen = screen;
    }
}
