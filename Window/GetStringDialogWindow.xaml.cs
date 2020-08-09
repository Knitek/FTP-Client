using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace FTP_Client
{
    /// <summary>
    /// Interaction logic for GetStringDialogWindow.xaml
    /// </summary>
    public partial class GetStringDialogWindow : Window
    {
        string Result = "Cancel";
        GetStringModel model = new GetStringModel();
        public GetStringDialogWindow(string labelContent)
        {
            model.LabelContent = labelContent;
            DataContext = model;
            InitializeComponent();            
            this.ShowDialog();
        }

        private void Ok_Click(object sender, RoutedEventArgs e)
        {
            Result = "Ok";
            this.Close();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        public bool GetStatus()
        {
            if(Result=="Ok")
            { return true; }
            else
            {
                return false;
            }
        }

        public string GetResult()
        {
            return model.DialogText;
        }
    }


    public class GetStringModel : INotifyPropertyChanged
    {
        string labelContent { get; set; }
        string dialogText { get; set; }

        public string LabelContent
        {
            get { return labelContent; }
            set { if(labelContent!=value)
                {
                    labelContent = value;
                    RaisePropertyChanged("LabelContent");
                }
            }
        }
        public string DialogText
        {
            get { return dialogText; }
            set
            {
                if (dialogText != value)
                {
                    dialogText = value;
                    RaisePropertyChanged("DialogText");
                }
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
