using Offer;
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace MlsExclusive.Utilites.Converters
{
    /// <summary>
    /// Устанавливает у строки первую букву, как заглавную; реализует <see cref="IValueConverter"/>.
    /// </summary>
    public class FirstCharUpConverter : IValueConverter
    {
        /// <summary>
        /// Устанавливает заглавную букву.
        /// </summary>
        /// <param name="value">Строка для работы.</param>
        /// <param name="targetType">Не используется в текущем методе.</param>
        /// <param name="parameter">Не используется в текущем методе.</param>
        /// <param name="culture">Не используется в текущем методе.</param>
        /// <returns>Возвращает строку с заглавной первой буквой; если в <paramref name="value"/> передана не <see cref="string"/>, вернет <see cref="DependencyProperty.UnsetValue"/>.</returns>
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
