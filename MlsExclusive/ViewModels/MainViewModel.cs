using CoreWPF.MVVM;
using CoreWPF.Utilites;
using MlsExclusive.Models;
using MlsExclusive.Utilites;
using MlsExclusive.Utilites.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using CoreWPF.Utilites.Navigation;

namespace MlsExclusive.ViewModels
{
    public class MainViewModel : ViewModel
    {
        private bool filterBlock = false;
        private bool modeBlock = false;
        private NavigationManager Navigator;

        public string Status_string { get; set; }

        public Dictionary<string, Func<MlsOffer, bool>> Filters { get; private set; }

        private KeyValuePair<string, Func<MlsOffer, bool>> select_filter;
        public KeyValuePair<string, Func<MlsOffer, bool>> Select_filter
        {
            get { return this.select_filter; }
            set
            {
                this.select_filter = value;
                this.OnPropertyChanged("Select_filter");
                //чтобы не запрашивать несколько раз Current_offers при первом запуске
                if (this.filterBlock) this.OnPropertyChanged("Current_offers");
                else this.filterBlock = true;
                //---
            }
        }

        public ListExt<MlsMode> Modes { get; private set; }

        private MlsMode select_mode;
        public MlsMode Select_mode
        {
            get { return this.select_mode; }
            set
            {
                this.select_mode = value;
                this.Navigator.Navigate(this.select_mode.ToString());
                this.OnPropertyChanged("Select_mode");
                //чтобы не запрашивать несколько раз Current_offers при первом запуске
                if (this.modeBlock) this.OnPropertyChanged("Current_offers");
                else this.modeBlock = true;
                //---
            }
        }

        public ListExt<Agency> Agencys { get; private set; }

        private Agency select_agency;
        public Agency Select_agency
        {
            get { return this.select_agency; }
            set
            {
                this.select_agency = value;
                this.OnPropertyChanged("Select_agency");
                this.OnPropertyChanged("Current_offers");
            }
        }

        public DateTimeOffset CurrentUpdateTime
        {
            get
            {
                return UpdateTime.Get();
            }
        }

        public ListExt<MlsOffer> Current_offers
        {
            get
            {
                if (this.Select_agency != null)
                {
                    ListExt<MlsOffer> tmp_send = this.Select_agency.Offers.FindRange(new Func<MlsOffer, bool>(offer =>
                    {
                         if (offer.Mode == this.Select_mode) return true;
                         else return false;
                    }));

                    return tmp_send.FindRange(this.Select_filter.Value);
                }
                else return null;
            }
        }

        public MainViewModel(NavigationManager navigator)
        {
            this.Title = "МЛС Эксклюзивы";
            this.Status_string = "Проверка";
            this.Navigator = navigator;

            this.Filters = new Dictionary<string, Func<MlsOffer, bool>>();
            this.Filters.Add("Все", new Func<MlsOffer, bool>(offer =>
            {
                return true;
            }));
            this.select_filter = this.Filters.ElementAt(0);

            this.Modes = new ListExt<MlsMode>(Enum.GetValues(typeof(MlsMode)).Cast<MlsMode>());
            this.select_mode = MlsMode.Flat;

            this.Agencys = new ListExt<Agency>();
        }
    }
}
