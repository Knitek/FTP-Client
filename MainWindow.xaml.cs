using System;
using System.Collections.Generic;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using FTP_Client.ViewModel;
using FTP_Client.Model;

namespace FTP_Client
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        FTPClientViewModel model { get; set; }
        public MainWindow()
        {
            InitializeComponent();
            model = new FTPClientViewModel();
            DataContext = model;
            ToolsLib.Tools.CheckForUpdates(model.title, model.version);
        }

        private void ftpWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            model.OnClose();
        }
    }
}
