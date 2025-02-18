using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NexusERP.Models
{
    public interface IAuthorizedViewModel : IRoutableViewModel
    {
        string[] RequiredRoles { get; }
    }

}
