using CoreWPF.MVVM;
using CoreWPF.MVVM.Interfaces;
using CoreWPF.Utilites;
using MlsExclusive.Utilites.Enums;
using System;
using System.Windows;

namespace MlsExclusive.Models
{
    [Serializable]
    public class MlsOffer : Model, IModel
    {
        public int Id { get; private set; }
        public MlsMode Mode { get; private set; }
        public string Changes { get; private set; }

        private string link;
        public string Link
        {
            get { return this.link; }
            set
            {
                this.link = value;
                this.OnPropertyChanged("Link");
            }
        }

        public OfferStatus Status { get; private set; }
        public int RoomCount { get; private set; }
        public string Type { get; private set; }
        public string District { get; private set; }
        public string Guidemark { get; private set; }
        public string Street { get; private set; }
        public double Price { get; private set; }
        public int Floor { get; private set; }
        public int Floors { get; private set; }
        public int SqAll { get; private set; }
        public int SqLive { get; private set; }
        public int SqKitchen { get; private set; }
        public int SqArea { get; private set; }
        public string BalconyType { get; private set; }
        public string LiveStatus { get; private set; }
        public string Agency { get; private set; }
        public string RoomsType { get; private set; }
        public string Materials { get; private set; }
        public string BathroomType { get; private set; }
        public string GasValue { get; private set; }
        public string SewerageValue { get; private set; }
        public string BathroomValue { get; private set; }
        public ListExt<string> Phones { get; private set; }
        public ListExt<string> Photos { get; private set; }
        public string Date { get; private set; }

        public MlsOffer(string mls_string, MlsMode mode)
        {
            this.Mode = mode;
            this.Status = OfferStatus.New;
            this.Link = "";
            string[] old_values = mls_string.Split('\t');
            ListExt<string> values = new ListExt<string>();
            foreach(string s in old_values)
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

            this.Price = Convert.ToDouble(values[5].Replace('.',',')) * Convert.ToDouble(1000);
            this.Floor = Convert.ToInt32(values[6].Split('/')[0]);
            this.Floors = Convert.ToInt32(values[6].Split('/')[1]);

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
                    this.Phones = new ListExt<string>(values[17].Split(','));
                }
                catch { this.Phones = new ListExt<string>(); }
                if(values[18] != @"https://mls.kh.ua/photo/.jpg")
                {
                    this.Photos = new ListExt<string>(values[18].Split(','));
                }
                else this.Photos = new ListExt<string>();
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
                    this.Phones = new ListExt<string>(values[19].Split(','));
                }
                catch { this.Phones = new ListExt<string>(); }
                if (values[20] != @"https://mls.kh.ua/photo/.jpg")
                {
                    this.Photos = new ListExt<string>(values[20].Split(','));
                }
                else this.Photos = new ListExt<string>();
                this.Date = values[21];

                this.Id = Convert.ToInt32(values[22]);
            }
        }

        public IModel Clone()
        {
            throw new NotImplementedException();
        }

        public bool Equals(IModel model)
        {
            if (model is MlsOffer offer)
            {
                if (offer.Id == this.Id && this.Mode == offer.Mode) return true;
                else return false;
            }
            else return false;
        }

        public void Merge(IModel model)
        {
            if(this.Equals(model))
            {
                MlsOffer offer = model as MlsOffer;
                this.Changes = "";

                if(this.Price != offer.Price)
                {
                    this.Changes += "Цена ";
                    if (this.Price > offer.Price) this.Changes += "упала: ";
                    else this.Changes += "поднялась: ";

                    this.Changes += this.Price + " ---> " + offer.Price + "\n\n";

                    this.Price = offer.Price;
                }

                if(this.RoomCount != offer.RoomCount)
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
               
                foreach(string phone in offer.Phones)
                {
                    if (!this.Phones.Contains(phone)) this.Phones.Add(phone);
                }

                foreach(string photo in offer.Photos)
                {
                    if (!this.Photos.Contains(photo)) this.Photos.Add(photo);
                }

                //set status
                if (this.Changes.Length > 0)
                {
                    if (this.Status != OfferStatus.Modify)
                    {
                        this.Status = OfferStatus.Modify;
                        this.Command_select_model?.Execute(null);
                    }

                }
                else
                {
                    if (this.Status != OfferStatus.NoChanges)
                    {
                        this.Status = OfferStatus.NoChanges;
                        this.Command_select_model?.Execute(null);
                    }
                }
            }
        }

        public void SetDeleteStatus()
        {
            this.Status = OfferStatus.Delete;
        }

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

        public RelayCommand Command_CopyToClipboardsImages
        {
            get
            {
                return new RelayCommand(obj =>
                {
                    string tmp_copy = "";
                    foreach (string image in this.Photos)
                    {
                        tmp_copy += image + "\n";
                    }
                    tmp_copy += "\n\n" + this.District + ", " + this.Guidemark + ", " + this.Street;
                    Clipboard.SetText(tmp_copy);
                },
                (obj) => this.Photos != null
                );
            }
        }
    }
}
