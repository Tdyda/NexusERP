using System;
using System.Collections.Generic;
using Avalonia.Data.Converters;
using NexusERP.ViewModels;

namespace NexusERP.Converters
{
    public class ClientSelectedConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            // value = client (string), parameter = SelectedClients (HashSet<string>)
            if (value is string client && parameter is HashSet<string> selectedClients)
            {
                return selectedClients.Contains(client);
            }
            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            // value = IsChecked (bool), parameter = client (string)
            if (value is bool isChecked && parameter is string client)
            {
                var selectedClients = parameter as HashSet<string>;
                if (selectedClients == null) return null;

                // Dodajemy lub usuwamy klienta na podstawie stanu checkboxa
                if (isChecked)
                {
                    selectedClients.Add(client);
                }
                else
                {
                    selectedClients.Remove(client);
                }

                return selectedClients;
            }
            return null;
        }
    }
}
