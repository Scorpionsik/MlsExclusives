using CoreWPF.MVVM;
using CoreWPF.Utilites;
using MlsExclusive.Models;
using MlsExclusive.Utilites;
using MlsExclusive.Utilites.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using CoreWPF.Utilites.Navigation;
using CoreWPF.MVVM.Interfaces;
using System.IO;
using System.Threading.Tasks;
using System.Diagnostics;

namespace MlsExclusive.ViewModels
{
    public class MainViewModel : ViewModel
    {
        private bool filterBlock = false;
        private bool modeBlock = false;

        public bool Unblock { get; private set; }

        public StatusString StatusBar { get; private set; }

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

        public MainViewModel()
        {
            this.Title = "МЛС Эксклюзивы";
            this.StatusBar = new StatusString();
            this.Unblock = true;

            this.Filters = new Dictionary<string, Func<MlsOffer, bool>>();
            this.Filters.Add("Все", new Func<MlsOffer, bool>(offer =>
            {
                return true;
            }));
            this.select_filter = this.Filters.ElementAt(0);

            this.Modes = new ListExt<MlsMode>(Enum.GetValues(typeof(MlsMode)).Cast<MlsMode>());
            this.select_mode = MlsMode.Flat;

            this.Agencys = new ListExt<Agency>();

            this.LoadAllAgency();
        }

        private void SelectFromCheckBox(Model model)
        {
            if(model is Agency agency && this.Select_agency != agency)
            {
                this.Select_agency = agency;
            }
        }

        public void AddAgency(Agency agency)
        {
            if (agency != null)
            {
                agency.UpdateBindings(this.SelectFromCheckBox);
                this.Agencys.Add(agency);
            }
        }

        private async void LoadAllAgency()
        {
            await Task.Run(() =>
            {
                if (this.Agencys.Count == 0)
                {
                    this.Unblock = false;
                    string[] files = Directory.GetFiles("Data/", "*.agency");

                    foreach (string file_path in files)
                    {
                        this.AddAgency(Agency.Deserialize(file_path));
                    }
                    this.Unblock = true;
                }
            });
        }

        private async void LoadMlsMethod()
        {
            this.Unblock = false;
            await Task.Run(() =>
            {
                this.StatusBar.SetAsync("Загружаем из МЛС...", StatusString.Infinite);
                MlsServer.GetFeeds();
            });

            await Task.Run(() =>
            {
                this.StatusBar.SetAsync("Обработка объектов...", StatusString.Infinite);

                foreach(string offer in MlsServer.Flats.Split('\n'))
                {
                    try
                    {
                        if (offer != "")
                        {
                            MlsOffer newOffer = new MlsOffer(offer, MlsMode.Flat);
                            Agency current_agency;
                            try
                            {
                                current_agency = this.Agencys.FindFirst(new Func<Agency, bool>(agency =>
                                {
                                    if (agency.Name == newOffer.Agency) return true;
                                    else return false;
                                }));
                                current_agency.AddOffer(newOffer);
                            }
                            catch
                            {
                                Agency newAgency = new Agency(newOffer.Agency);
                                this.AddAgency(newAgency);
                                newAgency.AddOffer(newOffer);
                            }
                        }
                    }
                    catch(Exception e)
                    {
                        //write in error-log code here
                        Debug.Print(e.Message);
                    }
                }
            });

            await Task.Run(() =>
            {
                this.StatusBar.SetAsync("Готово!", StatusString.LongTime);
                this.Unblock = true;
            });
        }

        public RelayCommand Command_LoadMls
        {
            get
            {
                return new RelayCommand(obj =>
                {
                    this.LoadMlsMethod();
                },
                (obj) => this.Unblock
                );
            }
        }
    }
}
