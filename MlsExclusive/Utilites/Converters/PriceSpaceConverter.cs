using MlsExclusive.Utilites.Enums;
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace MlsExclusive.Utilites.Converters
{
    /// <summary>
    /// Преобразует <see cref="double"/>-цену объекта, разделяя каждые 3 символа с конца пробелами.
    /// </summary>
    public class PriceSpaceConverter : IValueConverter
    {
        /// <summary>
        /// Преобразует <see cref="double"/>-цену объекта, разделяя каждые 3 символа с конца пробелами.
        /// </summary>
        /// <param name="value"><see cref="double"/>-цена для работы.</param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns>Вернет соответствующую строку; если в <paramref name="value"/> был передан не <see cref="OfferStatus"/>, вернет <see cref="DependencyProperty.UnsetValue"/>.</returns>
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
