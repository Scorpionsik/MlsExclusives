using MlsExclusive.Utilites.Enums;
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace MlsExclusive.Utilites.Converters
{
    public class OfferStatusConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if(value is OfferStatus status)
            {
                switch (status)
                {
                    case OfferStatus.New:
                        return "Новый";
                    case OfferStatus.Modify:
                        return "Изменён";
                    case OfferStatus.NoChanges:
                        return "Без изменений";
                    case OfferStatus.Delete:
                        return "Удалён";
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
