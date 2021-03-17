using CoreWPF.Windows;
using MlsExclusive.Utilites.Enums;
using MlsExclusive.ViewModels;
using System;
using System.Collections.Generic;

namespace MlsExclusive.Views
{
    /// <summary>
    /// Логика взаимодействия для WriteSelectView.xaml
    /// </summary>
    public partial class WriteSelectView : DialogWindowExt
    {
        /// <summary>
        /// Событие, возвращающее режим записи из <see cref="WriteSelectViewModel"/>.
        /// </summary>
        private event Func<KeyValuePair<string, WriteStatus>> event_getWriteStatus;

        /// <summary>
        /// Выбранный режим записи.
        /// </summary>
        public WriteStatus Result
        {
            get
            {
                return ((KeyValuePair<string, WriteStatus>)this.event_getWriteStatus?.Invoke()).Value;
            }
        }

        /// <summary>
        /// Инициализация окна.
        /// </summary>
        /// <param name="select_agency_name">Название выбранного агенства.</param>
        public WriteSelectView(string select_agency_name)
        {
            InitializeComponent();
            WriteSelectViewModel vm = new WriteSelectViewModel(select_agency_name);
            this.event_getWriteStatus += new Func<KeyValuePair<string, WriteStatus>>(vm.GetSelectStatus);
            this.DataContext = vm;
        }

    }
}
