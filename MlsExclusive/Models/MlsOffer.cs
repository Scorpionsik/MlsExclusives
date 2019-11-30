using CoreWPF.MVVM;
using CoreWPF.MVVM.Interfaces;
using CoreWPF.Utilites;
using MlsExclusive.Utilites;
using MlsExclusive.Utilites.Enums;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Linq;

namespace MlsExclusive.Models
{
    /// <summary>
    /// Модель МЛС объявлений, с необходимым инструментарием.
    /// </summary>
    [Serializable]
    public class MlsOffer : Model, IModel
    {
        /// <summary>
        /// Id текущего объявления в МЛС базе.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Флаг, отмечающий, из какого фида было взято текущее объявление.
        /// </summary>
        public MlsMode Mode { get; set; }

        /// <summary>
        /// Хранит историю изменений в объявлении.
        /// </summary>
        public string Changes { get; set; }

        private string link;
        /// <summary>
        /// Ссылка на текущее объявление в базе АН Города.
        /// </summary>
        public string Link
        {
            get { return this.link; }
            set
            {
                this.link = value;
                //this.Command_select_model?.Execute();
                this.event_UpdateMlsOffer?.Invoke(null);
                this.OnPropertyChanged("Link");
            }
        }

        private event Action<Model> event_UpdateMlsOffer;
        /// <summary>
        /// Событие срабатывает при обновлении информации в объявлении.
        /// </summary>
        /// <remarks>
        /// Использование <see cref="Action{Model}"/> вместо <see cref="Action"/> обосновано обратной совместимостью со старыми сохранениями
        /// (ранее вместо этого ивента использовался <see cref="Model.Event_select_model"/>, который принимает в качестве параметра <see cref="Model"/>, но в текущей реализации также не использовал передаваемый параметр).
        /// </remarks>
        public event Action<Model> Event_UpdateMlsOffer
        {
            add {
                this.event_UpdateMlsOffer -= value;
                this.event_UpdateMlsOffer += value;
            }
            remove { this.event_UpdateMlsOffer -= value; }
        }

        private OfferStatus status;
        /// <summary>
        /// Статус объявления.
        /// </summary>
        public OfferStatus Status
        {
            get { return this.status; }
            set
            {
                this.status = value;
                //this.Command_select_model?.Execute();
                this.event_UpdateMlsOffer?.Invoke(null);
                this.OnPropertyChanged("Status");
            }
        }

        /// <summary>
        /// Количество комнат в объекте. 
        /// </summary>
        public int RoomCount { get; set; }

        /// <summary>
        /// Тип недвижимости (квартира, дом, участок и тд).
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// Район, в котором находится объект.
        /// </summary>
        public string District { get; set; }

        /// <summary>
        /// Ориентир, по которому можно отыскать объект.
        /// </summary>
        public string Guidemark { get; set; }

        /// <summary>
        /// Улица, на которой находится объект.
        /// </summary>
        public string Street { get; set; }

        /// <summary>
        /// Цена за объект.
        /// </summary>
        public double Price { get; set; }

        /// <summary>
        /// Этаж, на котором находится объект.
        /// </summary>
        public int Floor { get; set; }

        /// <summary>
        /// Этажность дома с объектом.
        /// </summary>
        public int Floors { get; set; }

        /// <summary>
        /// Общая площадь объекта.
        /// </summary>
        public int SqAll { get; set; }

        /// <summary>
        /// Жилая площадь объекта.
        /// </summary>
        public int SqLive { get; set; }

        /// <summary>
        /// Площадь кухни объекта.
        /// </summary>
        public int SqKitchen { get; set; }

        /// <summary>
        /// Площадь участка объекта (актуально для фидов <see cref="MlsMode.House"/>).
        /// </summary>
        public int SqArea { get; set; }

        /// <summary>
        /// Тип/количество балконов у объекта (актуально для фидов <see cref="MlsMode.Flat"/>).
        /// </summary>
        public string BalconyType { get; set; }

        /// <summary>
        /// Состояние для жилья.
        /// </summary>
        public string LiveStatus { get; set; }

        /// <summary>
        /// Агенство, выставившее текущее объявление.
        /// </summary>
        public string Agency { get; set; }

        /// <summary>
        /// Планировка комнат (актуально для фидов <see cref="MlsMode.Flat"/>).
        /// </summary>
        public string RoomsType { get; set; }

        /// <summary>
        /// Материал, из которого построено здание, где находится объект.
        /// </summary>
        public string Materials { get; set; }

        /// <summary>
        /// Тип санузла (раздельный, совмещённый и тд).
        /// </summary>
        public string BathroomType { get; set; }

        /// <summary>
        /// Как проведён газ к объекту (актуально для фидов <see cref="MlsMode.House"/>).
        /// </summary>
        public string GasValue { get; set; }

        /// <summary>
        /// Как проведена канализация к объекту (актуально для фидов <see cref="MlsMode.House"/>).
        /// </summary>
        public string SewerageValue { get; set; }

        /// <summary>
        /// Где находится санузел у объекта (актуально для фидов <see cref="MlsMode.House"/>).
        /// </summary>
        public string BathroomValue { get; set; }

        /// <summary>
        /// Телефоны агента, выставившего объявление.
        /// </summary>
        public List<string> Phones { get; set; }

        /// <summary>
        /// Фотографии объекта.
        /// </summary>
        public List<string> Photos { get; set; }

        /// <summary>
        /// Описание объекта; поскольку в фидах МЛС описания нету, оно формируется из параметров, указанных в объявлении.
        /// </summary>
        public string Description
        {
            get
            {
                string tmp_send = "В продаже ";

                if (RoomCount > 0) tmp_send += RoomCount + "-комн. ";

                tmp_send += Type;

                if (Guidemark != null && Guidemark != "") tmp_send += ", ориентир: " + Guidemark;
                if (Materials != null && Materials != "") tmp_send += ", материал: " + Materials;
                if (LiveStatus != null && LiveStatus != "") tmp_send += ", состояние: " + LiveStatus;
                if (BalconyType != null && BalconyType != "") tmp_send += ", балкон: " + BalconyType;
                if (RoomsType != null && RoomsType != "") tmp_send += ", комнаты " + RoomsType;
                if (BathroomType != null && BathroomType != "") tmp_send += ", санузел: " + BathroomType;
                if (BathroomValue != null && BathroomValue != "") tmp_send += ", тип санузла: " + BathroomValue;
                if (SewerageValue != null && SewerageValue != "") tmp_send += ", канализация: " + SewerageValue;
                if (GasValue != null && GasValue != "") tmp_send += ", газ: " + GasValue;

                return tmp_send + ".";
            }
        }

        /// <summary>
        /// Дата создания объявления.
        /// </summary>
        public string Date { get; set; }

        private double last_update_stamp;
        /// <summary>
        /// Unix timestamp последнего обновления из МЛС
        /// </summary>
        public double Last_update_stamp
        {
            get { return this.last_update_stamp; }
            set
            {
                this.last_update_stamp = value;
                this.OnPropertyChanged("Last_update_stamp");
                this.OnPropertyChanged("Last_update_date");
            }
        }

        /// <summary>
        /// Конвертирует <see cref="Last_update_stamp"/> в соответствующую строку.
        /// </summary>
        public string Last_update_date
        {
            get
            {
                DateTimeOffset tmp_date = UnixTime.ToDateTimeOffset(this.Last_update_stamp, App.Timezone);
                string month = tmp_date.Month.ToString();
                string day = tmp_date.Day.ToString();

                if (month.Length == 1) month = "0" + month;
                if (day.Length == 1) day = "0" + day;
                return tmp_date.Year + "-" + month + "-" + day;
            }
        }

        /// <summary>
        /// Формирует объявление, исходя из фида и <see cref="MlsMode"/>.
        /// </summary>
        /// <param name="mls_string">Принимает фид для обработки; фиды можно получить из <see cref="MlsServer"/>.</param>
        /// <param name="mode">Откуда был получен фид.</param>
        /// <exception cref="ArgumentNullException"/>
        /// <exception cref="ArgumentException"/>
        public MlsOffer(string mls_string, MlsMode mode)
        {
            if (mls_string == null) throw new ArgumentNullException("Фид не объявлен!", "mls_string");
            else if (mls_string.Length == 0) throw new ArgumentException("Передана пустая строка!", "mls_string");

            this.Mode = mode;
            this.Status = OfferStatus.New;
            this.Link = "";
            this.last_update_stamp = 0;
            string[] old_values = mls_string.Split('\t');
            ListExt<string> values = new ListExt<string>();
            foreach (string s in old_values)
            {
                values.Add(s.Replace("\"", ""));
            }

            //for all
            try
            {
                this.RoomCount = Convert.ToInt32(values[0]);
            }
            catch
            {
                this.RoomCount = 0;
            }

            this.District = values[2];
            this.Guidemark = values[3];
            this.Street = values[4];

            this.Price = Convert.ToDouble(values[5].Replace('.', ',')) * Convert.ToDouble(1000);
            try
            {
                this.Floor = Convert.ToInt32(values[6].Split('/')[0]);
                this.Floors = Convert.ToInt32(values[6].Split('/')[1]);
            }
            catch
            {
                this.Floor = 0;
                this.Floors = 0;
            }

            try
            {
                this.SqAll = Convert.ToInt32(values[8]);
            }
            catch { this.SqAll = 0; }
            try
            {
                this.SqLive = Convert.ToInt32(values[9]);
            }
            catch { this.SqLive = 0; }
            try
            {
                this.SqKitchen = Convert.ToInt32(values[10]);
            }
            catch { this.SqKitchen = 0; }
            this.LiveStatus = values[12];
            //for flat
            if (mode == MlsMode.Flat)
            {
                this.Agency = values[16];
                switch (values[1].ToLower())
                {
                    case "г":
                        this.Type = "гостинка";
                        break;
                    case "и":
                        this.Type = "квартира";
                        break;
                    case "п":
                        this.Type = "подселение";
                        break;
                }
                this.SqArea = 0;
                this.BalconyType = values[11];
                this.RoomsType = values[13];
                this.Materials = values[14];
                this.BathroomType = values[15];
                this.GasValue = "";
                this.SewerageValue = "";
                this.BathroomValue = "";
                try
                {
                    values[17] = values[17].Remove(values[17].Length - 1);
                    this.Phones = new List<string>(values[17].Split(','));
                }
                catch { this.Phones = new List<string>(); }
                if (values[18] != @"https://mls.kh.ua/photo/.jpg")
                {
                    this.Photos = new List<string>(values[18].Split(','));
                }
                else this.Photos = new List<string>();
                this.Date = values[19];
                this.Id = Convert.ToInt32(values[20]);
            }
            //for house
            else if (mode == MlsMode.House)
            {
                this.Agency = values[15];
                switch (values[1].ToLower())
                {
                    case "1":
                        this.Type = "пол-дома";
                        break;
                    case "д":
                        this.Type = "дача";
                        break;
                    case "у":
                        this.Type = "участок";
                        break;
                    case "ц":
                        this.Type = "дом";
                        break;
                }
                try
                {
                    this.SqArea = Convert.ToInt32(values[11]);
                }
                catch { this.SqArea = 0; }
                this.BalconyType = "";
                this.RoomsType = "";
                this.Materials = values[13];
                this.BathroomType = values[14];
                this.GasValue = values[16];
                this.SewerageValue = values[17];
                this.BathroomValue = values[18];
                try
                {
                    values[19] = values[19].Remove(values[19].Length - 1);
                    this.Phones = new List<string>(values[19].Split(','));
                }
                catch { this.Phones = new List<string>(); }
                if (values[20] != @"https://mls.kh.ua/photo/.jpg")
                {
                    this.Photos = new List<string>(values[20].Split(','));
                }
                else this.Photos = new List<string>();
                this.Date = values[21];

                this.Id = Convert.ToInt32(values[22]);
                MlsOffer.FixWrongValues(this);
            }
        } //---конструктор MlsOffer

        /// <summary>
        /// Метод для обновления даты последнего обновления из МЛС.
        /// </summary>
        /// <param name="model">Не используется в текущем методе.</param>
        public void UpdateDate(Model model)
        {
            this.Last_update_stamp = UnixTime.CurrentUnixTimestamp();
        }

        /// <summary>
        /// Не используется, необходим для реализации <see cref="IModel"/>.
        /// </summary>
        /// <returns>Триггерит исключение.</returns>
        /// <exception cref="NotImplementedException"/>
        public IModel Clone()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Сравнивает <see cref="MlsOffer"/> с текущим экземпляром.
        /// </summary>
        /// <param name="model"><see cref="MlsOffer"/> для сравнения.</param>
        /// <returns>Если <see cref="MlsOffer.Id"/> и <see cref="MlsOffer.Mode"/> равны, вернет true, иначе false; если в параметре передан не <see cref="MlsOffer"/>, вернет false</returns>
        public bool Equals(IModel model)
        {
            if (model is MlsOffer offer)
            {
                if (offer.Id == this.Id && this.Mode == offer.Mode) return true;
                else return false;
            }
            else return false;
        }

        /// <summary>
        /// Сравнивает <see cref="MlsOffer"/> с текущим экземпляром; 
        /// если они равны, производит поиск изменений; при их наличии, применяет изменения к текущему экземпляру и меняет <see cref="Status"/> на <see cref="OfferStatus.Modify"/>;
        /// если изменений нет,  меняет <see cref="Status"/> на <see cref="OfferStatus.NoChanges"/>.
        /// </summary>
        /// <param name="model"></param>
        public void Merge(IModel model)
        {
            if (this.Status != OfferStatus.Incorrect && this.Equals(model))
            {
                MlsOffer offer = model as MlsOffer;

                this.Changes = "";

                if (this.Price != offer.Price)
                {
                    this.Changes += "Цена ";
                    if (this.Price > offer.Price) this.Changes += "упала: ";
                    else this.Changes += "поднялась: ";

                    this.Changes += this.Price + " ---> " + offer.Price + "\n\n";

                    this.Price = offer.Price;
                }

                if (this.RoomCount != offer.RoomCount)
                {
                    this.Changes += "Кол-во комнат: " + this.RoomCount + " -- > " + offer.RoomCount + "\n";
                    this.RoomCount = offer.RoomCount;
                }

                if (this.Type != offer.Type)
                {
                    this.Changes += "Тип: " + this.Type + " -- > " + offer.Type + "\n";
                    this.Type = offer.Type;
                }
                if (this.District != offer.District)
                {
                    this.Changes += "Район: " + this.District + " -- > " + offer.District + "\n";
                    this.District = offer.District;
                }
                if (this.Guidemark != offer.Guidemark)
                {
                    this.Changes += "Ориентир: " + this.Guidemark + " -- > " + offer.Guidemark + "\n";
                    this.Guidemark = offer.Guidemark;
                }
                if (this.Street != offer.Street)
                {
                    this.Changes += "Улица: " + this.Street + " -- > " + offer.Street + "\n";
                    this.Street = offer.Street;
                }
                if (this.Floor != offer.Floor)
                {
                    this.Changes += "Этаж: " + this.Floor + " -- > " + offer.Floor + "\n";
                    this.Floor = offer.Floor;
                }
                if (this.Floors != offer.Floors)
                {
                    this.Changes += "Этажность: " + this.Floors + " -- > " + offer.Floors + "\n";
                    this.Floors = offer.Floors;
                }
                if (this.SqAll != offer.SqAll)
                {
                    this.Changes += "Общая площадь: " + this.SqAll + " -- > " + offer.SqAll + "\n";
                    this.SqAll = offer.SqAll;
                }
                if (this.SqLive != offer.SqLive)
                {
                    this.Changes += "Жилая площадь: " + this.SqLive + " -- > " + offer.SqLive + "\n";
                    this.SqLive = offer.SqLive;
                }
                if (this.SqKitchen != offer.SqKitchen)
                {
                    this.Changes += "Площадь кухни: " + this.SqKitchen + " -- > " + offer.SqKitchen + "\n";
                    this.SqKitchen = offer.SqKitchen;
                }
                if (this.SqArea != offer.SqArea)
                {
                    this.Changes += "Площадь участка: " + this.SqArea + " -- > " + offer.SqArea + "\n";
                    this.SqArea = offer.SqArea;
                }
                if (this.BalconyType != offer.BalconyType)
                {
                    this.Changes += "Балкон: " + this.BalconyType + " -- > " + offer.BalconyType + "\n";
                    this.BalconyType = offer.BalconyType;
                }
                if (this.LiveStatus != offer.LiveStatus)
                {
                    this.Changes += "Состояние: " + this.LiveStatus + " -- > " + offer.LiveStatus + "\n";
                    this.LiveStatus = offer.LiveStatus;
                }
                if (this.RoomsType != offer.RoomsType)
                {
                    this.Changes += "Комнаты: " + this.RoomsType + " -- > " + offer.RoomsType + "\n";
                    this.RoomsType = offer.RoomsType;
                }
                if (this.Materials != offer.Materials)
                {
                    this.Changes += "Материал: " + this.Materials + " -- > " + offer.Materials + "\n";
                    this.Materials = offer.Materials;
                }
                if (this.BathroomType != offer.BathroomType)
                {
                    this.Changes += "Санузел: " + this.BathroomType + " -- > " + offer.BathroomType + "\n";
                    this.BathroomType = offer.BathroomType;
                }
                if (this.GasValue != offer.GasValue)
                {
                    this.Changes += "Газ: " + this.GasValue + " -- > " + offer.GasValue + "\n";
                    this.GasValue = offer.GasValue;
                }
                if (this.SewerageValue != offer.SewerageValue)
                {
                    this.Changes += "Канализация: " + this.SewerageValue + " -- > " + offer.SewerageValue + "\n";
                    this.SewerageValue = offer.SewerageValue;
                }
                if (this.BathroomValue != offer.BathroomValue)
                {
                    this.Changes += "Наличие санузла: " + this.BathroomValue + " -- > " + offer.BathroomValue + "\n";
                    this.BathroomValue = offer.BathroomValue;
                }

                foreach (string phone in offer.Phones)
                {
                    if (!this.Phones.Contains(phone)) this.Phones.Add(phone);
                }

                foreach (string photo in offer.Photos)
                {
                    if (!this.Photos.Contains(photo)) this.Photos.Add(photo);
                }

                //set status
                if (this.Changes.Length > 0)
                {
                    if (this.Status != OfferStatus.Modify)
                    {
                        this.Status = OfferStatus.Modify;
                    }

                }
                else
                {
                    if (this.Status != OfferStatus.NoChanges)
                    {
                        this.Status = OfferStatus.NoChanges;
                    }
                }
            }
        } //---метод Equals

        /// <summary>
        /// Устанавливает <see cref="Status"/> текущего объявления на <see cref="OfferStatus.Delete"/>.
        /// </summary>
        public void SetDeleteStatus()
        {
            this.Status = OfferStatus.Delete;
        }

        public static void FixWrongValues(MlsOffer offer)
        {
            //районы
            switch (offer.District)
            {
                case "Шатиловка":
                case "Павловка":
                    offer.District = "Сосновая Горка";
                    break;
                case "Москалевка":
                case "З-д Шевченко":
                    offer.District = "Завод Шевченко";
                    break;
                case "Березовка":
                    offer.District = "Харьковский р-н";
                    break;
                case "м. пл. Восстания":
                case "м. Спортивная":
                    offer.District = "Гагарина проспект";
                    break;
                case "Лысая Гора":
                    offer.District = "Холодная Гора";
                    break;
                case "Хроли":
                    offer.District = "ХТЗ";
                    break;
                case "Б.Даниловка":
                    offer.District = "Большая Даниловка";
                    break;
                case "Госпром":
                case "Нагорный район":
                    offer.District = "Центр";
                    break;
            }

            //улицы
            switch (offer.Street)
            {
                case "Пушкинская":
                case "Пушкинская ул":
                    offer.Street = "Пушкинская (" + offer.District + ")";
                    break;
                case "Ньютона":
                case "Ньютона ул.":
                    offer.Street = "Ньютона (" + offer.District + ")";
                    break;
                case "Плехановская":
                case "Плехановская ул.":
                    offer.Street = "Плехановская (" + offer.District + ")";
                    break;
                case "Лугова":
                case "Лугова ул.":
                    offer.Street = "Лугова (" + offer.District + ")";
                    break;
                case "Ударный пер.":
                    offer.Street = "Ударный пер. (" + offer.District + ")";
                    break;
                case "Вяловская":
                case "Вяловская ул.":
                    offer.Street = "Вяловская (" + offer.District + ")";
                    break;
                case "Садовая":
                case "Садовая ул.":
                    offer.Street = "Садовая (" + offer.District + ")";
                    break;
                case "Садовый пер.":
                    offer.Street = "Садовый пер. (" + offer.District + ")";
                    break;
                case "Интернациональная":
                case "Интернациональная ул.":
                    offer.Street = "Интернациональная (" + offer.District + ")";
                    break;
                case "Студёный пер.":
                    offer.Street = "Студёный пер. (" + offer.District + ")";
                    break;
                case "Корчагинцев ул. Амосова":
                    offer.Street = "Амосова ул.";
                    break;
                case "Подсолнечный взд.":
                    offer.Street = "Подсолнечный въезд";
                    break;
                case "Ленина ул. Сумской шлях":
                    if(offer.Guidemark.Contains("Солоницевка")) offer.Street = "Сумской шлях ул. (Солоницевка пгт.)";
                    break;
                case "Ильича ул. Чуйковская":
                    offer.Street = "Чуйковская ул.";
                    break;
                case "Блюхера ул. Валентиновская":
                    offer.Street = "Валентиновская ул.";
                    break;
                case "Омский 1-й взд.":
                    offer.Street = "Омский 1-ый въезд";
                    break;
                case "Луначарского Независимости":
                    if (offer.District.Contains("Мерефа")) offer.Street = "Луначарского ул. (Мерефа)";
                    break;
                case "Лебединская":
                case "Лебединская ул.":
                    offer.Street = "Лебединская ул.";
                    break;
                case "Вальтера":
                case "Вальтера ул.":
                    offer.Street = "Академика Вальтера ул.";
                    break;
                case "Гв. Широнинцев":
                case "Гв. Широнинцев ул.":
                    if (offer.District.Contains("Салтовка")) offer.Street = "Гвардейцев Широнинцев ул. (Салтовка)";
                    else offer.Street = "Гвардейцев Широнинцев ул. (Северная Салтовка)";
                    break;
                case "Г. Сталинграда пр.":
                    offer.Street = "Героев Сталинграда пр. (" + offer.District + ")";
                    break;
                case "Садовый прзд.":
                    offer.Street = "Садовый проезд";
                    break;
                case "Ленина пр. Науки":
                    offer.Street = "Науки просп.("+ offer.District +")";
                    break;
                case "23 Августа":
                case "23 Августа ул.":
                    if (offer.District.Contains("Павлово Поле")) offer.Street = "23 Августа ул.";
                    break;
                case "Тракторостроителей пр.":
                    offer.Street = "Тракторостроителей просп.";
                    break;
                case "Дача 55":
                case "Дача 55 ул.":
                    offer.Street = "Дача 55 ул.";
                    break;
                case "Ак. Павлова ул.":
                case "Ак. Павлова":
                    if (offer.District.Contains("Салтовка")) offer.Street = "Академика Павлова ул. (Салтовка)";
                    break;
                case "Московский пр.":
                    if (offer.District.Contains("Гагарина проспект")) offer.Street = "Московский пр. (Гагарина проспект)";
                    break;
                case "Полтавский Шлях":
                    offer.Street = offer.Street + " (" + offer.District + ")";
                    break;
                case "Песочин":
                case "Чайковка":
                case "Циркуны":
                case "Коротыч":
                    offer.Street = "";
                    break;
                default:
                    offer.Street = offer.Street.Replace(" ул.", "");
                    break;
            }


        }

        /// <summary>
        /// Команда для кнопки, открывающая строку с объявлением в базе АН Города в браузере.
        /// </summary>
        public RelayCommand<string> Command_OpenLink
        {
            get
            {
                return new RelayCommand<string>(obj =>
                {
                    try
                    {
                        System.Diagnostics.Process.Start(obj);
                    }
                    catch { }
                },
                (obj) => obj != null && obj.Length > 0 && obj.ToLower().Contains("newcab.bee.th1.vps-private.net")
                );
            }
        }

        /// <summary>
        /// Команда для кнопки, копирующая в буфер обмена Windows ссылки на фотографии и адрес текущего объявления.
        /// </summary>
        public RelayCommand Command_CopyToClipboardsImages
        {
            get
            {
                return new RelayCommand(obj =>
                {
                    string tmp_copy = "";
                    foreach (string image in this.Photos)
                    {
                        tmp_copy += image + "\r\n";
                    }
                    if (this.Photos.Count > 0) tmp_copy += "\r\n\r\n";
                    tmp_copy += this.District + ", " + this.Guidemark + ", " + this.Street;
                    Clipboard.SetText(tmp_copy);
                },
                (obj) => this.Photos != null
                );
            }
        }

        public RelayCommand Command_SetIncorrectStatus
        {
            get
            {
                return new RelayCommand(obj =>
                {
                    this.Status = OfferStatus.Incorrect;
                });
            }
        }

        public RelayCommand Command_SetNoChangesStatus
        {
            get
            {
                return new RelayCommand(obj =>
                {
                    this.Status = OfferStatus.NoChanges;
                });
            }
        }
    }
}
