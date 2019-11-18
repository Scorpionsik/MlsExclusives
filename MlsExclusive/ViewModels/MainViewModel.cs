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
using CoreWPF.Windows.Enums;
using System.Runtime.Serialization.Formatters.Binary;

namespace MlsExclusive.ViewModels
{
    public class MainViewModel : ViewModel
    {
        private bool filterBlock = false;
        private bool modeBlock = true;

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
            this.Filters.Add("Новые", new Func<MlsOffer, bool>(offer =>
            {
                if (offer.Status == OfferStatus.New) return true;
                else return false;
            }));
            this.Filters.Add("Измененные", new Func<MlsOffer, bool>(offer =>
            {
                if (offer.Status == OfferStatus.Modify) return true;
                else return false;
            }));
            this.Filters.Add("Удалённые", new Func<MlsOffer, bool>(offer =>
            {
                if (offer.Status == OfferStatus.Delete) return true;
                else return false;
            }));
            this.Filters.Add("Без ссылок", new Func<MlsOffer, bool>(offer =>
            {
                if (offer.Link == null || offer.Link.Length == 0 || !offer.Link.Contains("newcab.bee.th1.vps-private.net")) return true;
                else return false;
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

        private  void LoadAllAgency()
        {
                if (this.Agencys.Count == 0)
                {
                    this.Unblock = false;
                    string[] files = Directory.GetFiles("Data/agencys/", "*.agency");

                    foreach (string file_path in files)
                    {
                        this.AddAgency(Agency.Deserialize(file_path));
                    }
                    this.Unblock = true;
                }
        }

        private async void SaveChangesMethod()
        {
            await Task.Run(() =>
            {
                this.StatusBar.SetAsync("Идёт сохранение, пожалуйста подождите...", StatusString.Infinite);
                foreach (Agency agency in this.Agencys)
                {
                    if (agency.IsChanges)
                    {
                        Agency.Serialize(agency, "Data/agencys/");
                    }
                }
                this.StatusBar.SetAsync("Сохранение успешно завершено!", StatusString.LongTime);
            });
        }

        private void LoadMlsMethod()
        {
            this.Unblock = false;
            this.Select_agency = null;
            this.StatusBar.SetAsync("Загружаем из МЛС...", StatusString.Infinite);
            MlsServer.GetFeeds();



            this.StatusBar.SetAsync("Обработка объектов...", StatusString.Infinite);

            for (int i = 0; i < 2; i++)
            {
                MlsMode select_mode;
                string select_offers;

                if(i == 0)
                {
                    select_mode = MlsMode.Flat;
                    select_offers = MlsServer.Flats;
                }
                else
                {
                    select_mode = MlsMode.House;
                    select_offers = MlsServer.Houses;
                }

                foreach (string offer in select_offers.Split('\n'))
                {
                    try
                    {
                        if (offer != "")
                        {
                            MlsOffer newOffer = new MlsOffer(offer, select_mode);
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
                    catch (Exception e)
                    {
                        //write in error-log code here
                        string id_error;
                        if (select_mode == MlsMode.Flat) id_error = offer.Split('\t')[20];
                        else id_error = offer.Split('\t')[22];
                        Debug.Print(UnixTime.CurrentString() + " - Mode: " + select_mode + ", Id: " + id_error + ", Message:" + e.Message);
                    }
                }
            }

            this.StatusBar.SetAsync("Готово!", StatusString.LongTime);
            this.OnPropertyChanged("CurrentUpdateTime");
            this.Unblock = true;
        }

        public override WindowClose CloseMethod()
        {
            this.SaveChangesMethod();
            return base.CloseMethod();
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

        public RelayCommand One
        {
            get
            {
                return new RelayCommand(obj =>
                {
                    this.StatusBar.SetAsync("One", StatusString.LongTime);
                });
            }
        }

        public RelayCommand Two
        {
            get
            {
                return new RelayCommand(obj =>
                {
                    this.StatusBar.SetAsync("Two", StatusString.LongTime);
                });
            }
        }

        public RelayCommand Clear
        {
            get
            {
                return new RelayCommand(obj =>
                {
                    this.StatusBar.ClearAsync();
                });
            }
        }

        public RelayCommand Infinite
        {
            get
            {
                return new RelayCommand(obj =>
                {
                    this.StatusBar.SetAsync("Infinite", StatusString.Infinite);
                });
            }
        }

        public RelayCommand Command_SaveChanges
        {
            get
            {
                return new RelayCommand(obj =>
                {
                    this.SaveChangesMethod();
                });
            }
        }
    }
}
