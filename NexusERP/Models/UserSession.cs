using ReactiveUI;
using DynamicData;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NexusERP.Models
{
    internal class UserSession : ReactiveObject
    {
        private bool _isAuthenticated;
        private string _userId;
        private string _locationName;
        private readonly SourceList<string> _rolesSourceList = new SourceList<string>();
        public ReadOnlyObservableCollection<string> Roles { get; }

        public bool IsAuthenticated
        {
            get => _isAuthenticated;
            set => this.RaiseAndSetIfChanged(ref _isAuthenticated, value);
        }

        public string UserId
        {
            get => _userId;
            set => this.RaiseAndSetIfChanged(ref _userId, value);
        }

        public string LocationName
        { 
            get => _locationName;
            set => this.RaiseAndSetIfChanged(ref _locationName, value);
        }

        public UserSession()
        {
            var roles = _rolesSourceList.Connect().Bind(out var readOnlyRoles).Subscribe();
            Roles = readOnlyRoles;
        }

        public void AddRole(string role)
        {
            _rolesSourceList.Add(role);
        }

        public void Logout()
        {
            _rolesSourceList.Clear();
            IsAuthenticated = false;
            UserId = null;
        }
    }
}
