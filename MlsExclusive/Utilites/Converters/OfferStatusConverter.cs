using MlsExclusive.Utilites.Enums;
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace MlsExclusive.Utilites.Converters
{
    /// <summary>
    /// Конвертирует <see cref="OfferStatus"/> в соответствующую строку; реализует <see cref="IValueConverter"/>.
    /// </summary>
    public class OfferStatusConverter : IValueConverter
    {
        /// <summary>
        /// Конвертирует <see cref="OfferStatus"/> в соответствующую строку.
        /// </summary>
        /// <param name="value"><see cref="OfferStatus"/> для работы.</param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns>Вернет соответствующую строку; если в <paramref name="value"/> был передан не <see cref="OfferStatus"/>, вернет <see cref="DependencyProperty.UnsetValue"/>.</returns>
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

        /// <summary>
        /// Не используется в текущем классе.
        /// </summary>
        /// <param name="value">Не используется в текущем методе.</param>
        /// <param name="targetType">Не используется в текущем методе.</param>
        /// <param name="parameter">Не используется в текущем методе.</param>
        /// <param name="culture">Не используется в текущем методе.</param>
        /// <returns>Вернет <see cref="DependencyProperty.UnsetValue"/>.</returns>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return DependencyProperty.UnsetValue;
        }
    }
}
