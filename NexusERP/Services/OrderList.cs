using NexusERP.Enums;
using NexusERP.Models;
using Splat;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NexusERP.Services
{
    public class OrderList
    {
        private List<Order> _orderList;
        private UserSession _userSession;

        public OrderList(List<Order> orderList)
        {
            _orderList = orderList;
            _userSession = Locator.Current.GetService<UserSession>();
        }

        public List<Order> Calculate()
        {
            var finalList = _orderList
                .GroupBy(x => x.Index)
                .Select(g => new Order
                {
                    Index = g.Key,
                    Name = g.First().Name,
                    Quantity = g.Sum(x => x.Quantity),
                    OrderDate = DateTime.Now,
                    Status = OrderStatus.Pending,
                    ProdLine = _userSession.LocationName
                }).ToList();

            return finalList;
        }
    }
}
