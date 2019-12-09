using CoreWPF.MVVM;
using CoreWPF.Utilites;
using MlsExclusive.Utilites.Enums;
using MlsExclusive.Views;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MlsExclusive.ViewModels
{
    /// <summary>
    /// Логика для <see cref="WriteSelectView"/>.
    /// </summary>
    public class WriteSelectViewModel : ViewModel
    {
        /// <summary>
        /// Коллекция статусов записи; Key является строковым описанием, а Value - элементом <see cref="WriteStatus"/>.
        /// </summary>
        public Dictionary<string, WriteStatus> Statuses { get; private set; }

        private KeyValuePair<string, WriteStatus> select_status;
        /// <summary>
        /// Выбранный статус из коллекции <see cref="Statuses"/>.
        /// </summary>
        public KeyValuePair<string, WriteStatus> Select_status
        {
            get { return this.select_status; }
            set
            {
                this.select_status = value;
                this.OnPropertyChanged("Select_status");
            }
        }

        /// <summary>
        /// Инициализация логики окна <see cref="WriteSelectView"/>.
        /// </summary>
        /// <param name="select_agency_name">Название выбранного агенства.</param>
        public WriteSelectViewModel(string select_agency_name)
        {
            this.Title = "";
            this.Statuses = new Dictionary<string, WriteStatus>();

            foreach(WriteStatus status in new ListExt<WriteStatus>(Enum.GetValues(typeof(WriteStatus)).Cast<WriteStatus>()))
            {
                switch (status)
                {
                    case WriteStatus.All:
                        this.Statuses.Add("Записать объявления со всех агенств.", status);
                        break;
                    case WriteStatus.Select:
                        this.Statuses.Add("Записать объявления из выбранного агенства (" + select_agency_name + ").", status);
                        break;
                }
            }
            
            this.Select_status = this.Statuses.First();
        }

        /// <summary>
        /// Метод, который привязывается к <see cref="WriteSelectView.event_getWriteStatus"/>.
        /// </summary>
        /// <returns>Возвращает выбранный статус записи</returns>
        public KeyValuePair<string, WriteStatus> GetSelectStatus()
        {
            return this.Select_status;
        }
    }
}
