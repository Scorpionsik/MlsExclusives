using MlsExclusive.Utilites.Enums;
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace MlsExclusive.Utilites.Converters
{
    public class PriceSpaceConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if(value is double price)
            {
                string price_str = price.ToString();
                int length = price_str.Length;
                while (length >= 3)
                {
                    length -= 3;
                    if(length > 0) price_str = price_str.Insert(length, " ");
                }

                return price_str;
            }
            else return "";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return DependencyProperty.UnsetValue;
        }
    }
}
