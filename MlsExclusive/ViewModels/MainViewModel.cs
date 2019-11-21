using CoreWPF.MVVM;
using CoreWPF.Utilites;
using MlsExclusive.Models;
using MlsExclusive.Utilites;
using MlsExclusive.Utilites.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Threading.Tasks;
using System.Diagnostics;
using CoreWPF.Windows.Enums;
using Offer;
using System.Xml;
using Microsoft.Win32;
using MlsExclusive.Views;

namespace MlsExclusive.ViewModels
{
    public class MainViewModel : ViewModel
    {
        //private bool filterBlock = false;
        //private bool modeBlock = true;

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
                //if (this.filterBlock)
                    this.OnPropertyChanged("Current_offers");
                //else this.filterBlock = true;
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
                //if (this.modeBlock)
                    this.OnPropertyChanged("Current_offers");
                //else this.modeBlock = true;
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

        public string CurrentUpdateTime
        {
            get
            {
                return UpdateTime.Get().ToString();
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

        private void LoadAllAgency()
        {
                if (this.Agencys.Count == 0)
                {
                    this.Unblock = false;
                    if (!Directory.Exists("Data/agencys/")) Directory.CreateDirectory("Data/agencys/");

                    string[] files = Directory.GetFiles("Data/agencys/", "*.agency");

                    foreach (string file_path in files)
                    {
                        Agency agency = Agency.Deserialize(file_path);
                        //agency.SetOldStatus();
                        this.AddAgency(agency);
                    }
                    this.Unblock = true;
                }
        }

        private void SaveChangesMethod()
        {
            foreach (Agency agency in this.Agencys)
            {
                if (agency.IsChanges)
                {
                    Agency.Serialize(agency, "Data/agencys/");
                }
            }   
        }

        private async void SaveInFileMethod()
        {
            await Task.Run(() =>
            {
                this.StatusBar.SetAsync("Идёт создание документа, пожалуйста подождите...", StatusString.Infinite);

                List<OfferBase> tmp_offer = new List<OfferBase>();

                foreach(Agency agency in this.Agencys)
                {
                    if (agency.IsLoad)
                    {
                        foreach (MlsOffer offer in agency.Offers)
                        {
                            if (offer.Status == OfferStatus.New)
                            {
                                DateTimeOffset tmp_date = new DateTimeOffset(int.Parse(offer.Date.Split('-')[0]), int.Parse(offer.Date.Split('-')[1]), int.Parse(offer.Date.Split('-')[2]), 0, 0, 0, new TimeSpan());

                                OfferBase tmp_send = new OfferBase(offer.Id.ToString(),
                                    OfferType.Sell,
                                    new OfferCategory(offer.Type),
                                    tmp_date,
                                    tmp_date,
                                    "Украина",
                                    "Харьков",
                                    offer.District,
                                    offer.Street,
                                    Convert.ToInt32(offer.Price),
                                    PriceCurrency.USD,
                                    offer.Description
                                    )
                                {
                                    SqAll = offer.SqAll,
                                    SqLive = offer.SqLive,
                                    SqKitchen = offer.SqKitchen,
                                    SqArea = offer.SqArea,
                                    Rooms = offer.RoomCount,
                                    Floor = offer.Floor,
                                    Floors_total = offer.Floors,
                                };

                                tmp_send.Phones.AddRange(offer.Phones);
                                if (agency.IsPicLoad) tmp_send.Photos.AddRange(offer.Photos);

                                tmp_offer.Add(tmp_send);
                            }
                        }
                    }
                }

                XmlDocument yandexDoc = OfferBase.GetYandexDoc(tmp_offer);
                this.StatusBar.SetAsync("Документ создан, выбираем папку...", StatusString.Infinite);
                SaveFileDialog window = new SaveFileDialog();
                window.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
                window.Title = "Сохранение МЛС фида...";
                window.FileName = "mls_feed.xml";
                if ((bool)window.ShowDialog())
                {
                    this.StatusBar.SetAsync("Идёт сохранение, пожалуйста подождите...", StatusString.Infinite);
                    yandexDoc.Save(window.FileName);
                    this.StatusBar.SetAsync("Сохранение успешно завершено! Записано объектов: " + tmp_offer.Count(), StatusString.LongTime);
                }
                else this.StatusBar.SetAsync("Отмена операции, документ не сохранён...", StatusString.LongTime);
            });
        }

        private void LoadMlsMethod()
        {
            /*await Task.Run(() =>
            {*/
                this.Unblock = false;
                this.Select_agency = null;
                this.StatusBar.SetAsync("Загружаем из МЛС...", StatusString.Infinite);
                try
                {
                    MlsServer.GetFeeds();
                    if (MlsServer.Flats.Length == 0 || MlsServer.Houses.Length == 0) throw new ArgumentException("Ошибка загрузки фидов!");
                }
                catch(Exception ex)
                {
                    this.StatusBar.SetAsync(ex.Message, StatusString.LongTime);
                    this.Unblock = true;
                    return;
                }

                this.StatusBar.SetAsync("Обработка объектов...", StatusString.Infinite);
           // });

            /*await Task.Run(() =>
            {
                this.InvokeInMainThread(() =>
                {*/
                    //search and delete
                    for (int agency_i = 0; agency_i < this.Agencys.Count; agency_i++)
                    {
                        this.Agencys[agency_i].SetOldStatus();
                        for (int offer_i = 0; offer_i < this.Agencys[agency_i].Offers.Count; offer_i++)
                        {
                            if (this.Agencys[agency_i].Offers[offer_i].Status == OfferStatus.Delete)
                            {
                                this.Agencys[agency_i].Offers.Remove(this.Agencys[agency_i].Offers[offer_i]);
                                offer_i--;
                                continue;
                            }
                            this.Agencys[agency_i].Offers[offer_i].SetDeleteStatus();
                        }
                    }
                    bool new_agencys = false;
                    for (int i = 0; i < 2; i++)
                    {

                        //add new
                        MlsMode select_mode;
                        string select_offers;

                        if (i == 0)
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
                                            if (agency.Name.ToLower().Contains(newOffer.Agency.ToLower())) return true;
                                            else return false;
                                        }));
                                        current_agency.AddOffer(newOffer);
                                    }
                                    catch(Exception ex)
                                    {
                                        current_agency = new Agency(newOffer.Agency);
                                        if (!new_agencys) new_agencys = true;
                                        this.AddAgency(current_agency);
                                        current_agency.AddOffer(newOffer);
                                    }
                                }
                            }
                            catch (Exception e)
                            {
                                //write in error-log code here
                                string id_error;
                                if (select_mode == MlsMode.Flat) id_error = offer.Split('\t')[20];
                                else id_error = offer.Split('\t')[22];
                                Debug.Print(UnixTime.CurrentDateTimeOffset(UnixTime.Local).ToString() + " - Mode: " + select_mode + ", Id: " + id_error + ", Message:" + e.Message);
                            }
                        }

                        this.OnPropertyChanged("Agencys");
                    }
            //});

            //  });

            /*  await Task.Run(() =>
              {*/
                string status = "Готово!";
                if (new_agencys) status += " Добавлены новые агенства!";
                this.StatusBar.SetAsync(status, StatusString.LongTime);
                this.OnPropertyChanged("CurrentUpdateTime");
                this.Unblock = true;
           // });
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

        public RelayCommand Command_SaveInFile
        {
            get
            {
                return new RelayCommand(obj =>
                {
                    this.SaveInFileMethod();
                });
            }
        }
        
        public RelayCommand Command_SaveChanges
        {
            get
            {
                return new RelayCommand(obj =>
                {

                        this.StatusBar.SetAsync("Идёт сохранение результатов работы...", StatusString.LongTime);
                        this.SaveChangesMethod();
                        this.StatusBar.SetAsync("Сохранение успешно завершено!", StatusString.LongTime);

                });
            }
        }

        public RelayCommand Command_ShowSettings
        {
            get
            {
                return new RelayCommand(obj =>
                {
                    SettingsView window = new SettingsView();
                    window.Show();
                });
            }
        }
    }
}
