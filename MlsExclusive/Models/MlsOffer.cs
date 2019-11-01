using CoreWPF.MVVM;
using CoreWPF.MVVM.Interfaces;
using MlsExclusive.Utilites.Enums;
using System;

namespace MlsExclusive.Models
{
    [Serializable]
    public class MlsOffer : Model, IModel
    {
        public string CountRoom { get; private set; }

        private string type;
        public string Type
        {
            get { return this.type; }
            set
            {
                this.type = value;
            }
        }

        public MlsMode Mode { get; private set; }

        public string Agency { get; private set; }

        public MlsOffer(string mls_string, MlsMode mode)
        {
            this.Mode = mode;
            if(mode == MlsMode.Flat)this.Agency = mls_string.Split('\t')[16];
            else if(mode == MlsMode.House) this.Agency = mls_string.Split('\t')[15];
        }

        public IModel Clone()
        {
            throw new NotImplementedException();
        }

        public bool Equals(IModel model)
        {
            throw new NotImplementedException();
        }

        public void Merge(IModel model)
        {
            throw new NotImplementedException();
        }
    }
}
