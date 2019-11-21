using MlsExclusive.Utilites.Enums;
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace MlsExclusive.Utilites.Converters
{
    public class MlsModeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if(value is MlsMode mode)
            {
                switch (mode)
                {
                    case MlsMode.Flat:
                        return "Квартиры";
                    case MlsMode.House:
                        return "Дома/Участки";
                }
                return "";
            }
            else return DependencyProperty.UnsetValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return DependencyProperty.UnsetValue;
        }
    }
}
