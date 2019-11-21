using Offer;
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace MlsExclusive.Utilites.Converters
{
    public class FirstCharUpConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if(value is string title)
            {
                try
                {
                    return OfferBase.FirstUpper(title);
                }
                catch(ArgumentNullException)
                {
                    return "";
                }
            }
            else return DependencyProperty.UnsetValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return DependencyProperty.UnsetValue;
        }
    }
}
