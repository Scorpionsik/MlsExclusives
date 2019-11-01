using CoreWPF.MVVM;
using CoreWPF.Utilites;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MlsExclusive.Models
{
    [Serializable]
    public class Agency : Model
    {
        private bool isload;
        public bool IsLoad
        {
            get { return this.isload; }
            set
            {
                this.isload = value;
                this.OnPropertyChanged("IsLoad");
            }
        }

        private string name;
        public string Name
        {
            get { return this.name; }
            set
            {
                this.name = value;
                this.OnPropertyChanged("Name");
            }
        }

        public ListExt<MlsOffer> Offers { get; private set; }

        public Agency(string name)
        {
            this.name = name;
            this.Offers = new ListExt<MlsOffer>();
            
        }
    }
}
