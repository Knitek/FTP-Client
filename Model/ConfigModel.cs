using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace FTP_Client.Model
{
    public class ConfigModel : INotifyPropertyChanged
    {
        string port { get; set; }

        public string LocalPath { get; set; }
        public string HostName { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string Port
        {
            get
            {
                if (string.IsNullOrWhiteSpace(port))
                    return "21";
                else
                    return port;
            }
            set
            {
                if (value != port)
                {
                    port = value;
                }
            }
        }
        public string RemoteFolder { get; set; }
        public DateTime LastUse { get; set; }


        public void FromOtherItem(ConfigModel source)
        {
            LocalPath = source.LocalPath;
            HostName = source.HostName;
            UserName = source.UserName;
            Password = source.Password;
            Port = source.Port;
            RemoteFolder = source.RemoteFolder;
            LastUse = source.LastUse;
        }
        [XmlIgnore]
        public bool CanTryConnect
        {
            get
            {
                if (HostName == null || UserName == null || Password == null)
                    return false;
                else
                    return true;
            }
        }
        [XmlIgnore]
        public bool LocalPathExists
        {
            get
            {
                if (string.IsNullOrWhiteSpace(LocalPath))
                    return false;
                if (System.IO.Directory.Exists(LocalPath))
                    return true;
                else
                    return false;
            }
        }

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
