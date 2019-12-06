using MlsExclusive.Models;
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace MlsExclusive.Utilites.Converters
{
    /// <summary>
    /// Для отображения необходимого контектстого меню для флагов <see cref="Agency"/>.
    /// </summary>
    public class ContextMenuAgencyConverter : IValueConverter
    {
        /// <summary>
        /// Конвертирет значение флага в необходимый <see cref="Visibility"/>.
        /// </summary>
        /// <param name="value">Флаг с типом данных <see cref="bool"/></param>
        /// <param name="targetType">Не используется в текущем методе.</param>
        /// <param name="parameter">Строковый параметр для конвертации: принимает "lock" или "unlock".</param>
        /// <param name="culture">Не используется в текущем методе.</param>
        /// <returns>Возвращает <see cref="Visibility"/> в зависимости от <paramref name="value"/> и <paramref name="parameter"/>; если не был передан валидный флаг, вернет <see cref="DependencyProperty.UnsetValue"/>.</returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool choice)
            {
                if (parameter is string param)
                {
                    switch (param)
                    {
                        case "lock":
                            if (choice) return Visibility.Visible;
                            else return Visibility.Collapsed;
                        case "unlock":
                            if (choice) return Visibility.Collapsed;
                            else return Visibility.Visible;
                    }
                }
            }
            return DependencyProperty.UnsetValue;
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
