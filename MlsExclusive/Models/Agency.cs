using CoreWPF.MVVM;
using CoreWPF.Utilites;
using MlsExclusive.Utilites.Enums;
using System;
using System.Collections.Specialized;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace MlsExclusive.Models
{
    [Serializable]
    public class Agency : Model
    {
        private AgencyStatus status;
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

        public string StatusString
        {
            get
            {
                if (this.Status == AgencyStatus.New) return "Новое агенство!";
                else return "";
            }
        }

        private double last_update_stamp;
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

        public string Last_update_date
        {
            get
            {
                return UnixTime.ToDateTimeOffset(this.Last_update_stamp, App.Timezone).ToString();
            }
        }

        private bool isload;
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
                //чтобы при нажатии на CheckBox было выбрано агенство-хозяин CheckBox
                this.Command_select_model?.Execute(null);
                this.OnPropertyChanged("IsLoad");
            }
        }

        private bool ispicload;
        public bool IsPicLoad
        {
            get { return this.ispicload; }
            set
            {

                this.ispicload = value;
                if(!this.IsChanges)this.IsChanges = true;
                //чтобы при нажатии на CheckBox было выбрано агенство-хозяин CheckBox
                this.Command_select_model?.Execute(null);
                this.OnPropertyChanged("IsPicLoad");
            }
        }

        private bool ischanges;
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

        private string name;
        public string Name
        {
            get { return this.name; }
            set
            {
                this.name = value.Replace("\"", "");
                this.OnPropertyChanged("Name");
            }
        }

        public ListExt<MlsOffer> Offers { get; private set; }

        public Agency(string name)
        {
            this.status = AgencyStatus.New;
            this.last_update_stamp = UnixTime.CurrentUnixTimestamp();
            this.ischanges = true;
            this.isload = true;
            this.ispicload = false;
            this.name = name;
            this.Offers = new ListExt<MlsOffer>();
        }

        public void UpdateBindings(Action<Model> event_select_model)
        {
            this.Event_select_model = new Action<Model>(event_select_model);
            foreach(MlsOffer offer in this.Offers)
            {
                this.SetBindings(offer);
            }
            this.Offers.CollectionChanged += new NotifyCollectionChangedEventHandler(this.SetBindings);
        }

        private void SetBindings(MlsOffer offer)
        {
            offer.Event_select_model = new Action<Model>(this.SetBindings);
        }

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

        private void SetBindings(object obj, NotifyCollectionChangedEventArgs e)
        {
            if (!this.IsChanges) this.IsChanges = true;
        }

        private void SetBindings(Model model)
        {
            if (!this.IsChanges) this.IsChanges = true;
        }

        public void SetOldStatus()
        {
            if (this.Status == AgencyStatus.New) this.Status = AgencyStatus.Old;
        }

        public static string Serialize(Agency agency, string folder_path)
        {
            if (folder_path.Contains("\\") && folder_path[folder_path.Length - 1] != '\\') folder_path = folder_path + "\\";
            else if (folder_path.Contains("/") && folder_path[folder_path.Length - 1] != '/') folder_path = folder_path + "/";
            agency.IsChanges = false;
            //if(agency.Status == AgencyStatus.New) agency.Status = AgencyStatus.Old;
            BinaryFormatter formatter = new BinaryFormatter();
            using (FileStream fs = new FileStream(folder_path + agency.Name + ".agency", FileMode.OpenOrCreate))
            {
                formatter.Serialize(fs, agency);
                //fs.Close();
            }
            return folder_path + agency.Name + ".agency";
        }

        public static Agency Deserialize(string path)
        {
            if (File.Exists(path))
            {
                BinaryFormatter formatter = new BinaryFormatter();
                using (FileStream fs = new FileStream(path, FileMode.Open))
                {
                    Agency agency = (Agency)formatter.Deserialize(fs);
                    //fs.Close();
                    return agency;
                }
            }
            else return null;
        }
    }
}
