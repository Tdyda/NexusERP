using System;
using System.Globalization;
using Avalonia.Data.Converters;
using NexusERP.Models;

namespace NexusERP.Converters
{
    public class OrderStatusConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Order order && parameter is string newStatus)
            {
                return new Tuple<Order, string>(order, newStatus);
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
