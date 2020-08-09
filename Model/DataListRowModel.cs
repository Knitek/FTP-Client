using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FTP_Client.Model
{
    public class DataListRowModel : INotifyPropertyChanged
    {
        string name { get; set; }
        string type { get; set; }
        public string Name
        {
            get { return name; }
            set
            {
                if (name != value)
                {
                    name = value;
                    RaisePropertyChanged("Name");
                }
            }
        }
        public string Type
        {
            get { return type; }
            set
            {
                if (value!=type)
                {
                    type = value;
                    RaisePropertyChanged("Type");
                }
            }
        }

        public string FullPath { get; set; }
        public string Info { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
        private void RaisePropertyChanged(string property)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(property));
            }
        }
    }
}
