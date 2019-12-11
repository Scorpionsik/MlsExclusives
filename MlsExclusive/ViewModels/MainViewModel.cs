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
using System.Windows;
using System.Text.RegularExpressions;

namespace MlsExclusive.ViewModels
{
    /// <summary>
    /// Логика для <see cref="MainView"/>.
    /// </summary>
    public class MainViewModel : ViewModel
    {
        private bool showFilters;
        /// <summary>
        /// Флаг, отображающий интерфейс с выбором агенства и типа фида для просмотра; если стоит false, эти фильтры скрываются, и в списке отображаются все объявления.
        /// </summary>
        public bool ShowFilters
        {
            get { return this.showFilters; }
            set
            {
                this.showFilters = value;
                this.OnPropertyChanged("ShowFilters");
                this.OnPropertyChanged("OffersColumn");
                this.OnPropertyChanged("OffersColumnSpan");
                this.OnPropertyChanged("Current_offers");
            }
        } 

        public int OffersColumn
        {
            get
            {
                if (this.ShowFilters) return 2;
                else return 0;
            }
        }

        public int OffersColumnSpan
        {
            get
            {
                if (this.ShowFilters) return 1;
                else return 3;
            }
        }

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

        private bool unlockFlags;
        /// <summary>
        /// Флаг-индикатор, который передается в контекстное меню агенств; отмечает, заюлокированы флаги <see cref="Agency.IsLoad"/> и <see cref="Agency.IsPicLoad"/> или нет.
        /// </summary>
        public bool UnlockFlags
        {
            get { return this.unlockFlags; }
            set
            {
                this.unlockFlags = value;
                this.OnPropertyChanged("UnlockFlags");
            }
        }

        private event Action<MlsOffer> event_ScrollIntoCurrentOffers;
        /// <summary>
        /// Событие прокручивает тиблицу с <see cref="Current_offers"/> до выбранного объявления; используется для поиска и отображения объявлений внутри <see cref="Current_offers"/>.
        /// </summary>
        public event Action<MlsOffer> Event_ScrollIntoCurrentOffers
        {
            add
            {
                this.event_ScrollIntoCurrentOffers -= value;
                this.event_ScrollIntoCurrentOffers += value;
            }
            remove
            {
                this.event_ScrollIntoCurrentOffers -= value;
            }
        }

        /// <summary>
        /// Обновляемая строка для статус-бара окна.
        /// </summary>
        public StatusString StatusBar { get; private set; }

        private string search_string;
        /// <summary>
        /// Текстовое поле для поиска.
        /// </summary>
        public string Search_string
        {
            get { return this.search_string; }
            set
            {
                this.search_string = value;
                this.OnPropertyChanged("Search_string");
            }
        }

        /// <summary>
        /// Коллекция возможных вариаций поиска.
        /// </summary>
        public ListExt<MainSearchMode> SearchModes { get; private set; }

        private MainSearchMode select_Searchmode;
        /// <summary>
        /// Выбранная вариация поиска.
        /// </summary>
        public MainSearchMode Select_Searchmode
        {
            get { return this.select_Searchmode; }
            set
            {
                this.select_Searchmode = value;
                this.OnPropertyChanged("Select_Searchmode");
            }
        }

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
        /// Добавляет в заголовок название выбранного агенства.
        /// </summary>
        public override string Title
        {
            get
            {
                string tmp_send = base.Title;

                if (this.Select_agency != null) tmp_send += ": выбрано агенство " + this.Select_agency.Name;

                return tmp_send;
            }
            set => base.Title = value;
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
                this.OnPropertyChanged("Title");
            }
        }

        /// <summary>
        /// Возвращает время последней выгрузки из МЛС. 
        /// </summary>
        public string CurrentUpdateTime
        {
            get
            {
                return MlsServer.GetUpdateTime().ToString();
            }
        }

        /// <summary>
        /// Отображаемая коллекция объявлений; при отображении учитываются <see cref="Select_agency"/>, <see cref="Select_mode"/> и <see cref="Select_filter"/>.
        /// </summary>
        public ListExt<MlsOffer> Current_offers
        {
            get
            {
                if (this.ShowFilters)
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
                else
                {
                    ListExt<MlsOffer> tmp_send = new ListExt<MlsOffer>();
                    foreach(Agency a in this.Agencys)
                    {
                           if(a.IsLoad) tmp_send.AddRange(a.Offers);
                    }
                    return tmp_send.FindRange(this.Select_filter.Value);
                }
            }
        }

        /// <summary>
        /// Инициализация логики окна <see cref="MainView"/>.
        /// </summary>
        public MainViewModel()
        {
            this.Title = "МЛС Эксклюзивы";
            this.StatusBar = new StatusString();
            this.unblock = true;
            this.unlockFlags = false;
            this.showFilters = true;

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
                if ((offer.Status != OfferStatus.Delete && offer.Status != OfferStatus.Incorrect) && (!MlsOffer.CheckLinkMethod(offer.Link))) return true;
                else return false;
            }));
            this.select_filter = this.Filters.ElementAt(0);

            this.Modes = new ListExt<MlsMode>(Enum.GetValues(typeof(MlsMode)).Cast<MlsMode>());
            this.SearchModes = new ListExt<MainSearchMode>(Enum.GetValues(typeof(MainSearchMode)).Cast<MainSearchMode>());
            this.select_Searchmode = MainSearchMode.Text;
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
        /// <param name="write_status">Режим записи.</param>
        private async void SaveInFileMethod(WriteStatus write_status)
        {
            await Task.Run(() =>
            {
                if (write_status == WriteStatus.Select && !this.Select_agency.IsLoad)
                {
                    this.StatusBar.SetAsync("Для выбранного агенства (" + this.Select_agency.Name + ") установлен флаг запрета выгрузки, отмена записи...", StatusString.LongTime + StatusString.LongTime);
                }
                else
                {
                    //this.StatusBar.SetAsync("Идёт создание документа, пожалуйста подождите...", StatusString.Infinite);

                    ListExt<Agency> agencys = new ListExt<Agency>();
                    List<OfferBase> tmp_offer = new List<OfferBase>();

                    if (write_status == WriteStatus.All) agencys.AddRange(this.Agencys);
                    else if (write_status == WriteStatus.Select) agencys.Add(this.Select_agency);

                    foreach (Agency agency in agencys)
                    {
                        if (agency.IsLoad)
                        {
                            foreach (MlsOffer offer in agency.Offers)
                            {
                                if ((!MlsOffer.CheckLinkMethod(offer.Link)) && (offer.Status != OfferStatus.Incorrect && offer.Status != OfferStatus.Delete))
                                {
                                    DateTimeOffset tmp_date = new DateTimeOffset(int.Parse(offer.Date.Split('-')[0]), int.Parse(offer.Date.Split('-')[1]), int.Parse(offer.Date.Split('-')[2]), 0, 0, 0, new TimeSpan());

                                    string current_type = offer.Type;
                                    if (current_type == "гостинка" || current_type == "подселение") current_type = "квартира";
                                    else if (current_type == "пол-дома") current_type = "дом";

                                    OfferBase tmp_send = new OfferBase(offer.Id.ToString(),
                                        OfferType.Sell,
                                        new OfferCategory(current_type),
                                        tmp_date,
                                        UnixTime.ToDateTimeOffset(offer.Last_update_stamp, App.Timezone),
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

                    if (tmp_offer.Count == 0)
                    {
                        this.StatusBar.SetAsync("Документ пустой, отмена записи...", StatusString.LongTime + StatusString.LongTime);
                    }
                    else
                    {
                        XmlDocument yandexDoc = OfferBase.GetYandexDoc(tmp_offer);
                        this.StatusBar.SetAsync("Документ сформирован, выбираем папку...", StatusString.Infinite);
                        SaveFileDialog window = new SaveFileDialog();
                        window.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);

                        window.Title = "Сохранение документа в формате Яндекс.недвижимость...";
                        window.FileName = "mls_feed";
                        if (write_status == WriteStatus.Select) window.FileName += "_" + this.Select_agency.Name.Replace(" ", "");
                        window.FileName += ".yrl";
                        if ((bool)window.ShowDialog())
                        {
                            this.StatusBar.SetAsync("Идёт сохранение, пожалуйста подождите...", StatusString.Infinite);
                            yandexDoc.Save(window.FileName);

                            string tmp_status = "Сохранение успешно завершено!";
                            if (write_status == WriteStatus.Select) tmp_status += " (агенство " + this.Select_agency.Name + ")";

                            this.StatusBar.SetAsync(tmp_status + " Записано объектов: " + tmp_offer.Count(), StatusString.LongTime + StatusString.LongTime);
                        }
                        else this.StatusBar.SetAsync("Отмена операции, документ не сохранён...", StatusString.LongTime);
                    }
                }
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
                        MlsOffer.FixWrongValues(mls_offer);
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
                if (this.Agencys[agency_i].Status != AgencyStatus.Old)
                {
                    this.Agencys[agency_i].SetOldStatus();
                }
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
                            //MlsOffer.FixWrongValues(mls_offer);
                            current_offer.Merge(mls_offer);
                            if (current_offer.Status != OfferStatus.NoChanges && current_offer.Status != OfferStatus.Incorrect) current_agency.SetEditStatus();
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
                    switch(MessageBox.Show("Начать загрузку из МЛС?", "МЛС Сервер", MessageBoxButton.YesNo, MessageBoxImage.Question))
                    {
                        case MessageBoxResult.None:
                        case MessageBoxResult.Cancel:
                        case MessageBoxResult.No:
                            this.StatusBar.SetAsync("Отмена выгрузки в МЛС...", StatusString.LongTime);
                            return;
                    }

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
                    if (this.Select_agency != null)
                    {
                        WriteSelectView window = new WriteSelectView(this.Select_agency.Name);
                        if ((bool)window.Show())
                        {
                            this.SaveInFileMethod(window.Result);
                        }
                        else this.StatusBar.SetAsync("Отмена записи...", StatusString.LongTime);
                    }
                    else this.SaveInFileMethod(WriteStatus.All);
                }
                
                );
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

        /// <summary>
        /// Команда устанавливает <see cref="UnlockFlags"/> и <see cref="Agency.UnlockFlags"/>  в каждом агенстве внутри <see cref="Agencys"/>; параметр передается через CommandParameter.
        /// </summary>
        public RelayCommand<string> Command_SetUnlockFlag
        {
            get
            {
                return new RelayCommand<string>(obj =>
                {
                    switch (obj)
                    {
                        case "true":
                            this.UnlockFlags = true;
                            break;
                        case "false":
                            this.UnlockFlags = false;
                            break;
                    }
                    foreach(Agency a in this.Agencys)
                    {
                        a.UnlockFlags = this.UnlockFlags;
                    }
                },
                (obj) => obj != null && (obj == "true" || obj == "false")
                );
            }
        }

        /// <summary>
        /// Команда ищет объявление с учётом введенных данных и выбранной вариации поиска.
        /// </summary>
        public RelayCommand<string> Command_searchInCurrentOffers
        {
            get
            {
                return new RelayCommand<string>(search =>
                {
                    int startIndex = 0, allSteps = this.Select_agency.Offers.Count;
                    bool finish = false;
                    if (this.Select_agency.Select_offer != null) startIndex = this.Select_agency.Offers.IndexOf(this.Select_agency.Select_offer) + 1;

                    for (int step = 0; step < allSteps && !finish; step++, startIndex++)
                    {
                        if (startIndex >= allSteps) startIndex = 0;
                        

                        switch (this.Select_Searchmode)
                        {
                            case MainSearchMode.Price:
                                double price = -1;
                                try
                                {
                                    price = Convert.ToDouble(search.Replace(" ","").Replace("$",""));
                                }
                                catch
                                {
                                    MessageBox.Show("Введен неверный формат цены!", "Ошибка поиска!", MessageBoxButton.OK, MessageBoxImage.Error);
                                    finish = true;
                                }

                                if (!finish)
                                {
                                    if (this.Select_agency.Offers[startIndex].Price == price)
                                    {
                                        if (this.Select_mode != this.Select_agency.Offers[startIndex].Mode) this.Select_mode = this.Select_agency.Offers[startIndex].Mode;
                                        this.Select_agency.Select_offer = this.Select_agency.Offers[startIndex];
                                        finish = true;
                                    }
                                }
                                break;
                            case MainSearchMode.Text:
                                if (this.Select_agency.Offers[startIndex].Description.ToLower().Contains(search.ToLower()))
                                {
                                    if (this.Select_mode != this.Select_agency.Offers[startIndex].Mode) this.Select_mode = this.Select_agency.Offers[startIndex].Mode;
                                    this.Select_agency.Select_offer = this.Select_agency.Offers[startIndex];
                                    finish = true;
                                }
                                break;
                            case MainSearchMode.Id:
                                int id = -1;
                                try
                                {
                                    id = Convert.ToInt32(search.Replace("yandex", "").Replace("_", "").Replace(" ",""));
                                }
                                catch
                                {
                                    MessageBox.Show("Введен неверный ID!", "Ошибка поиска!", MessageBoxButton.OK, MessageBoxImage.Error);
                                    finish = true;
                                }

                                if (!finish)
                                {
                                    if(this.Select_agency.Offers[startIndex].Id == id)
                                    {
                                        if (this.Select_mode != this.Select_agency.Offers[startIndex].Mode) this.Select_mode = this.Select_agency.Offers[startIndex].Mode;
                                        this.Select_agency.Select_offer = this.Select_agency.Offers[startIndex];
                                        finish = true;
                                    }
                                }
                                break;
                        }
                    }

                    if (!finish) this.StatusBar.SetAsync("По вашему запросу ничего не найдено!", StatusString.LongTime);
                    else
                    {
                        this.event_ScrollIntoCurrentOffers?.Invoke(this.Select_agency.Select_offer);
                        this.StatusBar.SetAsync("Найдено!", StatusString.ShortTime);
                    }
                },
                (search) => (search != null && search.Length > 0) && 
                (this.Select_agency != null && this.Select_agency.Offers != null && this.Select_agency.Offers.Count > 0) &&
                ((this.Select_Searchmode == MainSearchMode.Price && new Regex(@"^[\d \$]+$").IsMatch(search)) || 
                (this.Select_Searchmode == MainSearchMode.Text) || 
                (this.Select_Searchmode == MainSearchMode.Id && new Regex(@"^(yandex)?[_]{0,2}[\d]+[ ]*$").IsMatch(search)))
                );
            }
                
        }

        /// <summary>
        /// Команда срабатывает при вводе текста в <see cref="Search_string"/>; пытается подставить максимально подходящий <see cref="MainSearchMode"/> в зависимости от ввода.
        /// </summary>
        public RelayCommand<string> Command_AutoSwitchFindMode
        {
            get
            {
                return new RelayCommand<string>(obj =>
                {
                    if (new Regex(@"^[\d \$]+$").IsMatch(obj))
                    {
                        if (this.Select_Searchmode != MainSearchMode.Price) this.Select_Searchmode = MainSearchMode.Price;
                    }
                    else if (new Regex(@"^(yandex)?[_]{0,2}[\d]+[ ]*$").IsMatch(obj))
                    {
                        if(this.Select_Searchmode != MainSearchMode.Id) this.Select_Searchmode = MainSearchMode.Id;
                    }
                    else
                    {
                        if (this.Select_Searchmode != MainSearchMode.Text) this.Select_Searchmode = MainSearchMode.Text;
                    }
                },
                (obj) => obj != null && obj.Length > 0
                );
            }
        }
        #endregion
    }
}
