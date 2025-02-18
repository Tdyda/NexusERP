using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NexusERP.Models
{
    internal class UserSession : ReactiveObject
    {
        public bool IsAuthenticated { get; set; }
        public string UserId { get; set; }
        public List<string> Roles { get; set; } = new List<string>();

    }
}
