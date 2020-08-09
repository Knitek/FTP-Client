using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FTP_Client.Model;
using FTP_Client.Controls;
using System.Security;
using System.Security.Permissions;
using System.Security.AccessControl;

namespace FTP_Client.ViewModel
{
    public class AddEditViewModel : INotifyPropertyChanged
    {
        ConfigModel selectedItem { get; set; }
        string title { get; set; }

        public ConfigModel SelectedItem
        {
            get { return selectedItem; }
            set
            {
                if (value != selectedItem)
                {
                    selectedItem = value;
                    RaisePropertyChanged("SelectedItem");
                }
            }
        }
        public string Title
        {
            get { return title; }
            set
            {
                if(value!=title)
                {
                    title = value;
                    RaisePropertyChanged("Title");
                }
            }
        } 

        public string ReturnCommand { get; set; }
        

        public CommandBase OkCommand { get; set; }
        public CommandBase CancelCommand { get; set; }

        public AddEditViewModel(Action action)
        {
            Title = "Add";
            SelectedItem = new ConfigModel();
            SetUpCommands();
            exit = action;
        }
        public AddEditViewModel(Action action, ConfigModel item)
        {
            exit = action;
            if(item != null)
            {
                SelectedItem = new ConfigModel();
                SelectedItem.FromOtherItem(item);
                Title = "Edit";
                SetUpCommands();
            }
            else
            {
                exit.Invoke();
            }
            
        }

        private Action exit { get; set; }

        private void SetUpCommands()
        {
            OkCommand = new CommandBase(Ok);
            CancelCommand = new CommandBase(Cancel);
        }

        private void Ok()
        {
            ReturnCommand = "Ok";
            exit.Invoke();
        }
        private void Cancel()
        {
            ReturnCommand = "Cancel";
            exit.Invoke();
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void RaisePropertyChanged(string property)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        }
    }
}
