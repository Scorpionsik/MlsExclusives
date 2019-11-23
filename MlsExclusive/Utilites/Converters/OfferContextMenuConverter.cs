using MlsExclusive.Utilites.Enums;
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace MlsExclusive.Utilites.Converters
{
    /// <summary>
    /// Получает значение <see cref="Visibility"/> в зависимости от переданного <see cref="OfferStatus"/>.
    /// </summary>
    public class OfferContextMenuConverter : IValueConverter
    {
        /// <summary>
        /// Получает значение <see cref="Visibility"/> в зависимости от переданного <see cref="OfferStatus"/>.
        /// </summary>
        /// <param name="value"><see cref="OfferStatus"/> для работы.</param>
        /// <param name="targetType">Не используется в текущем методе.</param>
        /// <param name="parameter">Строковый параметр для работы.<para>Для переключения в <see cref="OfferStatus.Incorrect"/> передайте следующую строку - "incorrect", иначе будет назначен статус <see cref="OfferStatus.NoChanges"/>.</para></param>
        /// <param name="culture">Не используется в текущем методе.</param>
        /// <returns>Возвращает соответствующий параметрам <see cref="Visibility"/>; если <paramref name="value"/> не является <see cref="OfferStatus"/>, вернет <see cref="DependencyProperty.UnsetValue"/>.</returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if(value is OfferStatus status)
            {
                switch (parameter)
                {
                    case "incorrect":
                        if (status == OfferStatus.Incorrect) return Visibility.Collapsed;
                        else return Visibility.Visible;
                    default:
                        if (status == OfferStatus.Incorrect) return Visibility.Visible;
                        else return Visibility.Collapsed;
                }
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
