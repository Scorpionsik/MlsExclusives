using CoreWPF.MVVM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MlsExclusive.Utilites
{
    public class StatusString : NotifyPropertyChanged
    {
        public static double LongTime = 6000;
        public static double ShortTime = 3000;
        public static double Infinite = 0;

        private string status;
        public string Status
        {
            get { return this.status; }
            private set
            {
                this.status = value;
                this.OnPropertyChanged("Status");
            }
        }

        public async void Set(string status, double milliseconds)
        {
            await Task.Run(() =>
            {
                this.Status = status;
                if (milliseconds > 0)
                {
                    Thread.Sleep((int)milliseconds);
                    this.Status = "";
                }
            });
        }

        public async void Clear()
        {
            await Task.Run(() =>
            {
                this.Status = "";
            });
        }
    }
}
