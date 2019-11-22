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
using CoreWPF.Windows.Enums;
using Offer;
using System.Xml;
using Microsoft.Win32;
using MlsExclusive.Views;

namespace MlsExclusive.ViewModels
{
    /// <summary>
    /// Логика для <see cref="MainView"/>.
    /// </summary>
    public class MainViewModel : ViewModel
    {
        private bool unblock;
        /// <summary>
        /// Флаг, отмечающий, заблокирован интерфейс окна или нет.
        /// </summary>
        /// <remarks>
        /// true, если не заблокирован, иначе false.
        /// </remarks>
        public bool Unblock
        {
            get { return this.unblock; }
            private set
            {
                this.unblock = value;
                this.OnPropertyChanged("Unblock");
            }
        }

        /// <summary>
        /// Обновляемая строка для статус-бара окна.
        /// </summary>
        public StatusString StatusBar { get; private set; }

        /// <summary>
        /// Коллекция фильтров для отображения объявлений.
        /// </summary>
        public Dictionary<string, Func<MlsOffer, bool>> Filters { get; private set; }

        private KeyValuePair<string, Func<MlsOffer, bool>> select_filter;
        /// <summary>
        /// Выбранный фильтр из коллекции <see cref="Filters"/>.
        /// </summary>
        public KeyValuePair<string, Func<MlsOffer, bool>> Select_filter
        {
            get { return this.select_filter; }
            set
            {
                this.select_filter = value;
                this.OnPropertyChanged("Select_filter");
                    this.OnPropertyChanged("Current_offers");
            }
        }

        /// <summary>
        /// Коллекция фильтров для отображения, из какого фида взято объявления.
        /// </summary>
        public ListExt<MlsMode> Modes { get; private set; }

        private MlsMode select_mode;
        /// <summary>
        /// Выбранный фильтр из коллекции <see cref="Modes"/>.
        /// </summary>
        public MlsMode Select_mode
        {
            get { return this.select_mode; }
            set
            {
                this.select_mode = value;
                this.OnPropertyChanged("Select_mode");
                    this.OnPropertyChanged("Current_offers");
            }
        }

        /// <summary>
        /// Коллекция агенств.
        /// </summary>
        public ListExt<Agency> Agencys { get; private set; }

        private Agency select_agency;
        /// <summary>
        /// Выьранное агенство из коллекции <see cref="Agencys"/>.
        /// </summary>
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

        /// <summary>
        /// Возвращает время последней выгрузки из МЛС. 
        /// </summary>
        public string CurrentUpdateTime
        {
            get
            {
                return UpdateTime.Get().ToString();
            }
        }

        /// <summary>
        /// Отображаемая коллекция объявлений; при отображении учитываются <see cref="Select_agency"/>, <see cref="Select_mode"/> и <see cref="Select_filter"/>.
        /// </summary>
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

        /// <summary>
        /// Инициализация логики окна <see cref="MainView"/>.
        /// </summary>
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
        } //---конструктор MainViewModel

        /// <summary>
        /// Добавляет новое агенство в коллекцию <see cref="Agencys"/>, предварительно обновив привязки для этого агенства.
        /// </summary>
        /// <param name="agency"><see cref="Agency"/> для добавления.</param>
        public void AddAgency(Agency agency)
        {
            if (agency != null)
            {
                agency.UpdateBindings(this.SelectChangesForAgency);
                this.Agencys.Add(agency);
            }
        }

        /// <summary>
        /// Вызывается в конструкторе, загружает <see cref="Agency"/> из сохранений в коллекцию <see cref="Agencys"/>.
        /// </summary>
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

        /// <summary>
        /// Метод для сохранения <see cref="Agency"/> из коллекции <see cref="Agencys"/> в файлы.
        /// </summary>
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

        /// <summary>
        /// Метод формирует файл .yrl из <see cref="Agencys"/>, из тех объявлений, в чьих <see cref="MlsOffer.Link"/> пустые значения.
        /// </summary>
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
                            if (offer.Link == null || offer.Link.Length == 0 || !offer.Link.Contains("newcab.bee.th1.vps-private.net"))
                            {
                                DateTimeOffset tmp_date = new DateTimeOffset(int.Parse(offer.Date.Split('-')[0]), int.Parse(offer.Date.Split('-')[1]), int.Parse(offer.Date.Split('-')[2]), 0, 0, 0, new TimeSpan());

                                OfferBase tmp_send = new OfferBase(offer.Id.ToString(),
                                    OfferType.Sell,
                                    new OfferCategory(offer.Type),
                                    tmp_date,
                                    UnixTime.ToDateTimeOffset(agency.Last_update_stamp, App.Timezone),
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

                this.StatusBar.SetAsync("Документ сформирован, выбираем папку...", StatusString.Infinite);
                SaveFileDialog window = new SaveFileDialog();
                window.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);

                window.Title = "Сохранение документа в формате Яндекс.недвижимость...";
                window.FileName = "mls_feed.yrl";
                if ((bool)window.ShowDialog())
                {
                    this.StatusBar.SetAsync("Идёт сохранение, пожалуйста подождите...", StatusString.Infinite);
                    yandexDoc.Save(window.FileName);
                    this.StatusBar.SetAsync("Сохранение успешно завершено! Записано объектов: " + tmp_offer.Count(), StatusString.LongTime + StatusString.LongTime);
                }
                else this.StatusBar.SetAsync("Отмена операции, документ не сохранён...", StatusString.LongTime);
            });
        } //---метод SaveInFileMethod

        /// <summary>
        /// Метод загружает фиды из МЛС и обрабатывает полученные результаты.
        /// </summary>
        private void LoadMlsMethod()
        {
            this.Select_agency = null;

            #region load mls feed
            this.StatusBar.SetAsync("Загружаем из МЛС...", StatusString.Infinite);
            try
            {
                MlsServer.GetFeeds();
                if (MlsServer.Flats.Length == 0 || MlsServer.Houses.Length == 0)
                {
                    string message = "Ошибка загрузки фидов:";
                    if (MlsServer.Flats.Length == 0) message += " Не загружены квартиры!";
                    if (MlsServer.Houses.Length == 0) message += " Не загружены дома!";
                    throw new ArgumentException(message);
                }
            }
            catch(Exception ex)
            {
                this.StatusBar.SetAsync(ex.Message, StatusString.LongTime + StatusString.LongTime);
                this.Unblock = true;
                return;
            }
            #endregion

            #region check feed
            this.StatusBar.SetAsync("Обработка объектов...", StatusString.Infinite);

            //get new objects
            ListExt<Agency> tmp_agencys = new ListExt<Agency>();
            for(int i = 0; i < 2; i++)
            {
                string current_feed = "";
                MlsMode current_mode = MlsMode.Flat;

                if(i == 0)
                {
                    current_feed = MlsServer.Flats;
                    current_mode = MlsMode.Flat;
                }
                else if(i == 1)
                {
                    current_feed = MlsServer.Houses;
                    current_mode = MlsMode.House;
                }

                foreach(string feed_offer in current_feed.Split('\n'))
                {
                    if (feed_offer != null && feed_offer.Length > 0)
                    {
                        MlsOffer mls_offer = new MlsOffer(feed_offer, current_mode);
                        Agency feed_agency = new Agency(mls_offer.Agency);
                        try
                        {
                            feed_agency = tmp_agencys.FindFirst(new Func<Agency, bool>(obj =>
                            {
                                if (obj.Name == feed_agency.Name) return true;
                                else return false;
                            }));
                        }
                        catch
                        {
                            feed_agency.UpdateBindings(this.SelectChangesForAgency);
                            tmp_agencys.Add(feed_agency);
                        }
                        finally
                        {
                            feed_agency.AddOffer(mls_offer);
                        }
                    }
                }
            }
            //remove old objects with Delete status
            for(int agency_i = 0; agency_i < this.Agencys.Count; agency_i++)
            {
                if (this.Agencys[agency_i].Status == AgencyStatus.New) this.Agencys[agency_i].SetOldStatus();
                for (int offer_i = 0; offer_i < this.Agencys[agency_i].Offers.Count; offer_i++)
                {
                    if(this.Agencys[agency_i].Offers[offer_i].Status == OfferStatus.Delete)
                    {
                        this.Agencys[agency_i].Offers.RemoveAt(offer_i);
                        offer_i--;
                    }
                }
            }

            //equal old and new objects
            bool isNewAgency = false;
            foreach(Agency mls_agency in tmp_agencys)
            {
                try
                {
                    Agency current_agency = this.Agencys.FindFirst(new Func<Agency, bool>(obj =>
                    {
                        if (obj.Name == mls_agency.Name) return true;
                        else return false;
                    }));

                    foreach(MlsOffer current_offer in current_agency.Offers)
                    {
                        try
                        {
                            MlsOffer mls_offer = mls_agency.Offers.FindFirst(new Func<MlsOffer, bool>(obj =>
                            {
                                return obj.Equals(current_offer);
                            }));
                            current_offer.Merge(mls_offer);
                        }
                        catch
                        {
                            current_offer.SetDeleteStatus();
                        }
                    }
                }
                catch
                {
                    this.InvokeInMainThread(() =>
                    {
                        this.AddAgency(mls_agency);
                    });
                    if (!isNewAgency) isNewAgency = true;
                }
            }
            #endregion

            #region finish
            string status = "Готово!";
            if (isNewAgency) status += " Добавлены новые агенства!";
            this.StatusBar.SetAsync(status, StatusString.LongTime + StatusString.LongTime);
            this.OnPropertyChanged("CurrentUpdateTime");
            #endregion
        } //---метод LoadMlsMethod

        /// <summary>
        /// Метод, передаваемый в события агенства; срабатывает если <see cref="Agency.IsLoad"/> или <see cref="Agency.IsPicLoad"/> были изменены.
        /// </summary>
        /// <param name="model"><see cref="Agency"/>, которое должно быть назначено для <see cref="Select_agency"/>.</param>
        private void SelectChangesForAgency(Model model)
        {
            if(model != null && model is Agency agency && agency != this.Select_agency)
            {
                this.Select_agency = agency;
            }
        }

        /// <summary>
        /// Сохраняет результаты работы программы перед её закрытием. 
        /// </summary>
        /// <returns></returns>
        public override WindowClose CloseMethod()
        {
            this.StatusBar.SetAsync("Идёт сохранение, пожалуйста подождите!", StatusString.Infinite);
            this.SaveChangesMethod();
            return base.CloseMethod();
        }

        #region Команды
        /// <summary>
        /// Команда для кнопки, запускающая метод <see cref="LoadMlsMethod"/> (загрузка и обработка фидов из МЛС) в отдельном потоке.
        /// </summary>
        public RelayCommand Command_LoadMls
        {
            get
            {
                return new RelayCommand(obj =>
                {
                    Task.Run(() =>
                    {
                        this.Unblock = false;
                        this.LoadMlsMethod();
                        this.Unblock = true;
                    });
                }
                );
            }
        }

        /// <summary>
        /// Команда для кнопки, запускающая асинхронный метод <see cref="SaveInFileMethod"/> (сохранение объявлений в файл .yrl).
        /// </summary>
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

        /// <summary>
        /// Команда для кнопки, запускающая метод <see cref="SaveChangesMethod"/> (сохранение результатов работы) в отдельном потоке.
        /// </summary>
        public RelayCommand Command_SaveChanges
        {
            get
            {
                return new RelayCommand(obj =>
                {
                    Task.Run(() =>
                    {
                        this.Unblock = false;
                        this.StatusBar.SetAsync("Идёт сохранение результатов работы...", StatusString.LongTime);
                        this.SaveChangesMethod();
                        this.StatusBar.SetAsync("Сохранение успешно завершено!", StatusString.LongTime);
                        this.Unblock = true;
                    });

                });
            }
        }

        /// <summary>
        /// Команда для открытия окна настроек (<see cref="SettingsView"/>).
        /// </summary>
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

        /// <summary>
        /// Команда для перехода на сайт МЛС.
        /// </summary>
        public RelayCommand Command_GoGoMls
        {
            get
            {
                return new RelayCommand(obj =>
                {
                    System.Diagnostics.Process.Start("https://mls.kh.ua");
                });
            }
        }
        #endregion
    }
}
