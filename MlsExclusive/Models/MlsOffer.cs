using CoreWPF.MVVM.Interfaces;
using MlsExclusive.Utilites.Enums;
using System;

namespace MlsExclusive.Models
{
    [Serializable]
    public class MlsOffer : IModel
    {
        public MlsMode Mode { get; private set; }

        public MlsOffer(string mls_string, MlsMode mode)
        {
            this.Mode = mode;

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
