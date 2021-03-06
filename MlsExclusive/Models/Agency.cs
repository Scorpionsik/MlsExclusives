using CoreWPF.MVVM;
using CoreWPF.Utilites;
using MlsExclusive.Utilites.Enums;
using System;
using System.Collections.Specialized;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Windows.Data;
using MlsExclusive.ViewModels;
using System.Collections.Generic;
using MessagePack;
using System.Text.RegularExpressions;
using System.Security.Cryptography;
using MlsExclusive.Utilites;

namespace MlsExclusive.Models
{
    /// <summary>
    /// Модель агенства, с необходимым инструментарием.
    /// </summary>
    [MessagePackObject(keyAsPropertyName: true)]
    [Serializable]
    public class Agency : Model
    {
        [IgnoreMember]
        private bool unlockFlags;
        /// <summary>
        /// Флаг, блокирующий флаги <see cref="Agency.IsLoad"/>, <see cref="Agency.IsPicLoad"/>. 
        /// </summary>
        /// <remarks>
        /// Сделано, чтобы пользователь случайно не переключал вышеуказанные флаги при выборе агенства в списке.
        /// </remarks>
        public bool UnlockFlags
        {
            get { return this.unlockFlags; }
            set
            {
                this.unlockFlags = value;
                this.OnPropertyChanged("UnlockFlags");
            }
        }

        [IgnoreMember]
        private AgencyStatus status;
        /// <summary>
        /// Флаг, отмечающий, новое агенство добавлено в приложение или нет.
        /// </summary>
        public AgencyStatus Status
        {
            get { return this.status; }
            private set
            {
                this.status = value;
                this.IsChanges = true;
                this.OnPropertyChanged("Status");
                this.OnPropertyChanged("StatusString");
            }
        }

        /// <summary>
        /// Возвращает <see cref="Status"/> в соответствующей вариации строки.
        /// </summary>
        /// <remarks>
        /// Свойство используется вместо соответствующего <see cref="IValueConverter"/>, до того как было принято решение использовать их; оставлено для совместимости со старыми сохранениями.
        /// </remarks>
        [IgnoreMember]
        public string StatusString
        {
            get
            {
                if (this.Status == AgencyStatus.New) return "Новое агенство!";
                else if(this.Status == AgencyStatus.Edit) return "Есть изменения!";
                else return "";
            }
        }

        [IgnoreMember]
        private double last_update_stamp;
        /// <summary>
        /// Время, когда в последний раз агенство или его объекты были обновлены (флаг <see cref="IsChanges"/> устанавливался в true).
        /// </summary>
        public double Last_update_stamp
        {
            get
            {
                return this.last_update_stamp;
            }
            set
            {
                this.last_update_stamp = value;
                this.OnPropertyChanged("last_update_stamp");
                this.OnPropertyChanged("Last_update_date");
            }
        }

        /// <summary>
        /// Возвращает <see cref="Last_update_stamp"/> в формате <see cref="DateTimeOffset.ToString()"/> (используется смещение во времени, взятое из <see cref="App.Timezone"/>).
        /// </summary>
        [IgnoreMember]
        public string Last_update_date
        {
            get
            {
                return UnixTime.ToDateTimeOffset(this.Last_update_stamp, App.Timezone).ToString();
            }
        }

        [IgnoreMember]
        private bool isload;
        /// <summary>
        /// Флаг, отмечающий, нужно ли сохранять объекты текущего агенства при выгрузках. <para>Внмание: если флаг установлен в false, он также установит <see cref="IsPicLoad"/> в false.</para>
        /// </summary>
        /// <remarks>
        /// true, если нужно сохранять, иначе false.
        /// </remarks>
        public bool IsLoad
        {
            get { return this.isload; }
            set
            {
                this.isload = value;
                if (!this.isload)
                {
                    this.IsPicLoad = false;
                }
                if (!this.IsChanges) this.IsChanges = true;
                this.Command_select_model?.Execute();
                this.OnPropertyChanged("IsLoad");
            }
        }

        [IgnoreMember]
        private bool ispicload;
        /// <summary>
        /// Флаг, отмечающий, сохранять ли фотографии объектов при выгрузках.
        /// </summary>
        /// <remarks>
        /// true, если нужно сохранять, иначе false.
        /// </remarks>
        public bool IsPicLoad
        {
            get { return this.ispicload; }
            set
            {

                this.ispicload = value;
                if(!this.IsChanges)this.IsChanges = true;
                this.Command_select_model?.Execute();
                this.OnPropertyChanged("IsPicLoad");
            }
        }

        [IgnoreMember]
        private bool ischanges;
        /// <summary>
        /// Флаг, отмечающий, есть ли изменения в свойствах агенства или внутри объектов.
        /// </summary>
        /// <remarks>
        /// true, если изменения есть, иначе false.
        /// </remarks>
        public bool IsChanges
        {
            get { return this.ischanges; }
            private set
            {
                this.ischanges = value;
                if (this.ischanges)
                {
                    this.Last_update_stamp = UnixTime.CurrentUnixTimestamp();
                }
                this.OnPropertyChanged("IsChanges");
            }
        }

        [IgnoreMember]
        private string name;
        /// <summary>
        /// Название агенства.
        /// </summary>
        public string Name
        {
            get { return this.name; }
            set
            {
                this.name = value.Replace("\"", "");
                this.OnPropertyChanged("Name");
            }
        }

        /// <summary>
        /// Объявления, которые выставило текущее агенство.
        /// </summary>
        public ListExt<MlsOffer> Offers { get; private set; }

        [NonSerialized]
        [IgnoreMember]
        private MlsOffer select_offer;

        /// <summary>
        /// Выбранное объявление из коллекции <see cref="Offers"/>.
        /// </summary>
        [IgnoreMember]
        public MlsOffer Select_offer
        {
            get { return this.select_offer; }
            set
            {
                this.select_offer = value;
                //this.Command_select_model?.Execute();
                this.OnPropertyChanged("Select_offer");
            }
        }

        /// <summary>
        /// Экземпляр класса с указанным названием.
        /// </summary>
        /// <param name="name">Название агенства.</param>
        [SerializationConstructor]
        public Agency(string name)
        {
            this.status = AgencyStatus.New;
            this.unlockFlags = false;
            this.last_update_stamp = UnixTime.CurrentUnixTimestamp();
            this.ischanges = true;
            this.isload = true;
            this.ispicload = false;
            this.name = name;
            this.Offers = new ListExt<MlsOffer>();
        }

        /// <summary>
        /// Метод для обновления привязок объектов к агенству.
        /// </summary>
        /// <param name="event_select_model">Метод, полученный из <see cref="MainViewModel"/> для обновления выбранного агенства.</param>
        public void UpdateBindings(Action<Model> event_select_model)
        {
            this.Event_select_model += new Action<Model>(event_select_model);
            foreach(MlsOffer offer in this.Offers)
            {
                this.SetBindings(offer);
                MlsOffer.FixWrongValues(offer);
            }
            this.Offers.CollectionChanged += new NotifyCollectionChangedEventHandler(this.SetBindings);
        }

        /// <summary>
        /// Привязывает события для <see cref="MlsOffer"/>.
        /// </summary>
        /// <param name="offer"><see cref="MlsOffer"/>, к которому будет осуществена привязка.</param>
        private void SetBindings(MlsOffer offer)
        {
            offer.Event_select_model += new Action<Model>(this.SetSelectOffer);
            offer.Event_UpdateMlsOffer += new Action<Model>(this.SetBindings);
            offer.Event_UpdateMlsOffer += new Action<Model>(offer.UpdateDate);
        }

        /// <summary>
        /// Привязывается к событию Event_select_model у <see cref="MlsOffer"/>; обновляет выбранное объявление.
        /// </summary>
        /// <param name="model"><see cref="MlsOffer"/> для работы.</param>
        private void SetSelectOffer(Model model)
        {
            if(model != null && model is MlsOffer offer && this.Select_offer != offer)
            {
                this.Select_offer = offer;
            }
        }

        /// <summary>
        /// Метод добавляет новый объект или обновляет существующий, если был получен повтор.
        /// </summary>
        /// <param name="offer">Объект для добавления/обновления в коллекции <see cref="Offers"/>.</param>
        public void AddOffer(MlsOffer offer)
        {
            try
            {
                MlsOffer tmp = this.Offers.FindFirst(new Func<MlsOffer, bool>(obj =>
                 {
                     return offer.Equals(obj);
                 }));
                tmp.Merge(offer);
            }
            catch
            {
                this.SetBindings(offer);
                this.Offers.Add(offer);
            }
        }

        /// <summary>
        /// Метод для доавления коллекции объектов.
        /// </summary>
        /// <param name="collection">Коллекция объектов для добавления.</param>
        public void AddOfferRange(IEnumerable<MlsOffer> collection)
        {
            foreach(MlsOffer m in collection)
            {
                this.AddOffer(m);
            }
        }

        /// <summary>
        /// Метод для события обновления объекта, привязывается к <see cref="Offers"/>, событие CollectionChanged.
        /// </summary>
        /// <param name="obj">Не используется в текущем методе.</param>
        /// <param name="e">Не используется в текущем методе.</param>
        private void SetBindings(object obj, NotifyCollectionChangedEventArgs e)
        {
            if (!this.IsChanges) this.IsChanges = true;
        }

        /// <summary>
        /// Метод для события обновления объекта, привязывается к <see cref="MlsOffer"/> в коллекции <see cref="Offers"/>.
        /// </summary>
        /// <param name="model">Не используется в текущем методе.</param>
        private void SetBindings(Model model)
        {
            if (!this.IsChanges) this.IsChanges = true;
        }

        /// <summary>
        /// Устанавливает <see cref="Status"/> в <see cref="AgencyStatus.Old"/>.
        /// </summary>
        public void SetOldStatus()
        {
            if (this.Status != AgencyStatus.Old) this.Status = AgencyStatus.Old;
        }

        /// <summary>
        /// Устанавливает <see cref="Status"/> в <see cref="AgencyStatus.Edit"/>.
        /// </summary>
        public void SetEditStatus()
        {
            if (this.Status != AgencyStatus.Edit) this.Status = AgencyStatus.Edit;
        }

        /// <summary>
        /// Статический метод, сериализует <see cref="Agency"/> в файл *.agency, где * равна <see cref="Agency.Name"/>.
        /// </summary>
        /// <param name="agency"><see cref="Agency"/> для сериализации.</param>
        /// <param name="folder_path">Папка для сохранения сериализованного <see cref="Agency"/>.</param>
        /// <param name="mode">Режим сохранения.</param>
        /// <returns>Возвращает полный путь к сериализованному <see cref="Agency"/>.</returns>
        public static string Serialize(Agency agency, string folder_path, AgencySerializeMode mode = AgencySerializeMode.Default)
        {
            if (folder_path.Contains("\\") && folder_path[folder_path.Length - 1] != '\\') folder_path = folder_path + "\\";
            else if (folder_path.Contains("/") && folder_path[folder_path.Length - 1] != '/') folder_path = folder_path + "/";
            agency.IsChanges = false;
            //if(agency.Status == AgencyStatus.New) agency.Status = AgencyStatus.Old;
            switch (mode)
            {
                case AgencySerializeMode.MessagePack:
                    byte[] bytes = MessagePackSerializer.Serialize(agency);
                    
                    File.WriteAllText(folder_path + agency.Name + ".json", MessagePackSerializer.ToJson(bytes));
                    break;
                case AgencySerializeMode.MessagePackNotJson:
                    byte[] bytes2 = MessagePackSerializer.Serialize(agency);

                    File.WriteAllBytes(folder_path + agency.Name + ".agnc", Crypt.Encrypt(bytes2, "ugly goblin"));
                    break;
                default:
                    BinaryFormatter formatter = new BinaryFormatter();
                    using (FileStream fs = new FileStream(folder_path + agency.Name + ".agency", FileMode.OpenOrCreate))
                    {
                        formatter.Serialize(fs, agency);
                        //fs.Close();
                    }
                    break;
            }
            return folder_path + agency.Name + ".agency";
        }

        /// <summary>
        /// Статический метод, десериализует <see cref="Agency"/> из файла.
        /// </summary>
        /// <param name="path">Путь к файлу *.agency для десериализации</param>
        /// <param name="mode">Режим сохранения.</param>
        /// <returns>возвращает десериализованный <see cref="Agency"/>.</returns>
        public static Agency Deserialize(string path, AgencySerializeMode mode = AgencySerializeMode.Default)
        {
            if (File.Exists(path))
            {
                
                switch (mode)
                {
                    case AgencySerializeMode.MessagePack:
                        if (!new Regex(@"\.json$").IsMatch(path)) return null;
                        string json = File.ReadAllText(path);
                        return MessagePackSerializer.Deserialize<Agency>(MessagePackSerializer.FromJson(json), MessagePack.Resolvers.ContractlessStandardResolverAllowPrivate.Instance);
                    case AgencySerializeMode.MessagePackNotJson:
                        if (!new Regex(@"\.agnc$").IsMatch(path)) return null;
                        byte[] bytes = File.ReadAllBytes(path);

                        return MessagePackSerializer.Deserialize<Agency>(Crypt.Decrypt(bytes, "ugly goblin"), MessagePack.Resolvers.ContractlessStandardResolverAllowPrivate.Instance);
                    default:
                        if (!new Regex(@"\.agency$").IsMatch(path)) return null;
                        BinaryFormatter formatter = new BinaryFormatter();
                        using (FileStream fs = new FileStream(path, FileMode.Open))
                        {
                            Agency agency = (Agency)formatter.Deserialize(fs);
                            //fs.Close();
                            return agency;
                        }
                }
            }
            else return null;
        }

        /// <summary>
        /// Команда-датчик, необходим для блокировки флага <see cref="IsPicLoad"/>, если <see cref="IsLoad"/> равен false.
        /// </summary>
        [IgnoreMember]
        public RelayCommand Command_blockIsPicLoad
        {
            get
            {
                return new RelayCommand(obj => { }, (obj) => this.IsLoad);
            }
        }
    }
}
