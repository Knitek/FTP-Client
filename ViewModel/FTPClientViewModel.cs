using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FTP_Client.Model;
using FTP_Client.Controls;
using System.Windows;

namespace FTP_Client.ViewModel
{
    public class FTPClientViewModel : INotifyPropertyChanged
    {
        public string title = "FTP Client";
        public string version = "20181017 v1.1.3";

        ObservableCollection<DataListRowModel> dataList { get; set; }
        ObservableCollection<ConfigModel> configs { get; set; }
        ConfigModel configData { get; set; }
        string statusText { get; set; }
        FluentFTP.FtpClient client { get; set; }
        Action ClearStatusAction { get; set; }
        Action<double> ProgressUpdate { get; set; }
        DataListRowModel selectedListRow { get; set; }
        int taskCounter { get; set; }
        double progress { get; set; }
        bool fastDelete { get; set; }
        string movePath { get; set; }
        

        public IProgress<double> iProgres { get; set; }
        public double Progress
        {
            get { return progress; }
            set
            {
                if(progress!=value)
                {
                    progress = value;
                    RaisePropertyChanged("Progress");
                }
            }
        }
        public int TaskCounter
        {
            get { return taskCounter; }
            set
            {
                if(taskCounter!=value)
                {
                    taskCounter = value;
                    if (taskCounter > 1000)
                        taskCounter = 0;
                }
            }
        }
        public ObservableCollection<DataListRowModel> DataList
        {
            get { return dataList; }
            set
            {
                if (dataList != value)
                {
                    dataList = value;
                    RaisePropertyChanged("DataList");
                }
            }
        }
        public ObservableCollection<ConfigModel> Configs
        {
            get { return configs; }
            set
            {
                if(configs!=value)
                {
                    configs = value;
                    RaisePropertyChanged("Configs");
                }
            }
        }

        public ConfigModel ConfigData
        {
            get { return configData; }
            set { if (value != configData)
                {
                    configData = value;
                    RaisePropertyChanged("ConfigData");
                }
            }
        }
        public string ConnectionStatus
        {
            get
            {
                if(client!=null)
                {
                    if(client.IsConnected)
                    {
                        return "Connected";
                    }
                    else
                    {
                        return "Disconnected";
                    }
                }
                return "Not set";
            }
        }
        public string StatusText
        {
            get { return statusText; }
            set {
                if (statusText!=value)
                {
                    statusText = value;
                    if (statusText.ToLower().Contains("connected") || statusText.ToLower().Contains("disconnected"))
                        RaisePropertyChanged("ConnectionStatus");

                    RaisePropertyChanged("StatusText");
                    taskCounter++;
                    Task.Factory.StartNew(ClearStatusAction);  
                }
            }
        }
        public DataListRowModel SelectedListRow
        {
            get { return selectedListRow; }
            set
            {
                if(selectedListRow!=value)
                {
                    selectedListRow = value;
                    RaisePropertyChanged("SelectedListRow");
                    Progress = 0;
                }
            }
        }
        public bool FastDelete
        {
            get { return fastDelete; }
            set
            {
                if(fastDelete!=value)
                {
                    fastDelete = value;
                    RaisePropertyChanged("FastDelete");
                }
            }
        }
        public string MovePath
        {
            get { return movePath; }
            set
            {
                if(value!=movePath)
                {
                    movePath = value;
                    RaisePropertyChanged("MoveFileSelected");
                }
            }
        }
        public bool MoveFileSelected
        {
            get
            {
                if (string.IsNullOrWhiteSpace(movePath))
                    return false;
                else
                    return true;
            }
        }

        public CommandBase OpenLocalDirectoryCommand { get; set; }
        public CommandBase GoToDirectoryCommand { get; set; }
        public CommandBase ConnectCommand { get; set; }
        public CommandBase ConnectToCommand { get; set; }
        public CommandBase AddConfigCommand { get; set; }
        public CommandBase EditConfigCommand { get; set; }
        public CommandBase DeleteConfigCommand { get; set; }
        public CommandBase DisconnectCommand { get; set; }
        public CommandBase UploadCommand { get; set; }
        public CommandBase DownloadCommand { get; set; }
        public CommandBase CreateDirectoryCommand { get; set; }
        public CommandBase DeleteCommand { get; set; }
        public CommandBase MoveCommand { get; set; }
        public CommandBase MoveHereCommand { get; set; }
        public CommandBase AboutWindowCommand { get; set; }
        public CommandBase ExitCommand { get; set; }

#region START_UP
        public FTPClientViewModel()
        {
            CheckConfigSetup();
            DataList = new ObservableCollection<DataListRowModel>();
            ClearStatusAction = new Action(async() => 
            {
                int z = TaskCounter;
                await Task.Delay(TimeSpan.FromSeconds(2.5));
                if(z==TaskCounter)
                    StatusText = "";
            });
            ProgressUpdate = new Action<double>((double d) => { this.Progress = d; });
            iProgres = new Progress<double>(ProgressUpdate);

            OpenLocalDirectoryCommand = new CommandBase(OpenDirectory);
            GoToDirectoryCommand = new CommandBase(GoToDirectory);
            ConnectCommand = new CommandBase(Connect);
            ConnectToCommand = new CommandBase(ConnectTo);
            AddConfigCommand = new CommandBase(AddConfig);
            EditConfigCommand = new CommandBase(EditConfig);
            DeleteConfigCommand = new CommandBase(DeleteConfig);
            DisconnectCommand = new CommandBase(Disconnect);
            UploadCommand = new CommandBase(Upload);
            DownloadCommand = new CommandBase(Download);
            CreateDirectoryCommand = new CommandBase(CreateDirectory);
            DeleteCommand = new CommandBase(Delete);
            MoveCommand = new CommandBase(Move);
            MoveHereCommand = new CommandBase(MoveHere);
            AboutWindowCommand = new CommandBase(AboutWindow);
            ExitCommand = new CommandBase(Exit);            
        }
        private void CheckConfigSetup()
        {
            try
            {
                string dataPath = ToolsLib.Tools.ReadAppSettingPath("defaultDataDirectory");
                if (dataPath == null)
                {
                    ToolsLib.Tools.WriteAppSetting("defaultDataDirectory", "Data\\");
                    dataPath = ToolsLib.Tools.ReadAppSettingPath("defaultDataDirectory");
                }
                string errorPath = System.IO.Path.Combine(dataPath, "ErrorLogs\\");
                if (ToolsLib.Tools.ReadAppSettingPath("errorLogPath") == null || !System.IO.Directory.Exists(ToolsLib.Tools.ReadAppSettingPath("errorLogPath")))
                    ToolsLib.Tools.WriteAppSetting("errorLogPath", errorPath);
                else
                    errorPath = ToolsLib.Tools.ReadAppSettingPath("errorLogPath");

                if (!System.IO.Directory.Exists(dataPath))
                    System.IO.Directory.CreateDirectory(dataPath);
                if (!System.IO.Directory.Exists(errorPath))
                    System.IO.Directory.CreateDirectory(errorPath);                                
            }
            catch(Exception exc)
            {
                MessageBox.Show(exc.Message);
            }

            string configpath = System.IO.Path.Combine(ToolsLib.Tools.ReadAppSettingPath("defaultDataDirectory"), "Configs.xml");
            if (System.IO.File.Exists(configpath))
            {
                Configs = ToolsLib.Tools.Deserialize<ObservableCollection<ConfigModel>>(configpath);
            }         
            else
            {
                Configs = new ObservableCollection<ConfigModel>();
            }
        }
        public void OnClose()
        {
            try
            {
                string path = System.IO.Path.Combine(ToolsLib.Tools.ReadAppSettingPath("defaultDataDirectory"), "Configs.xml");
                ToolsLib.Tools.Serialize(Configs, path);
                if(client!= null && client.IsConnected)
                    client.Disconnect();
            }
            catch(Exception exc)
            {
                ToolsLib.Tools.ExceptionLogAndShow(exc, "OnClose()");
            }
        }
        #endregion
#region CommandFunctions
        private void OpenDirectory()
        {
            if(ConfigData!=null && ConfigData.LocalPathExists)
            {
                System.Diagnostics.Process.Start(ConfigData.LocalPath);
            }
            else
            {
                StatusText = "LoaclPath not set";
            }
        }
        private void GoToDirectory()
        {
            if (SelectedListRow == null || !client.IsConnected) return;
            if (SelectedListRow.Type.Equals("DIR") || SelectedListRow.Type.Equals("BACK"))
            {
                try
                {
                    ListCombine(SelectedListRow.FullPath);
                }
                catch (Exception exc)
                {
                    MessageBox.Show(exc.Message);
                    return;
                }   
            }
        }
        private void Connect()
        {
            try
            {                
                if (ConfigData == null || !ConfigData.CanTryConnect) return;

                client = new FluentFTP.FtpClient(ConfigData.HostName, int.Parse(ConfigData.Port), ConfigData.UserName, ConfigData.Password);
                client.Connect();
                ListCombine(ConfigData.RemoteFolder ?? "");
                StatusText = "Connected";
                ConfigData.LastUse = DateTime.Now;
            }
            catch (Exception)
            {
                MessageBox.Show("Can't connect");
            }            
        }
        private void ConnectTo()
        {
            try
            {                
                AddEditWindow addEditWindow = new AddEditWindow();
                if (addEditWindow.model.ReturnCommand?.ToLower() == "ok")
                {
                    if (client.IsConnected)
                        client.Disconnect();

                    var customConfig = addEditWindow.model.SelectedItem;

                    if (customConfig == null || !customConfig.CanTryConnect) return;

                    client = new FluentFTP.FtpClient(customConfig.HostName, int.Parse(customConfig.Port), customConfig.UserName, customConfig.Password);
                    client.Connect();
                    ListCombine(customConfig.RemoteFolder ?? "");
                    StatusText = "Connected";
                }
            }
            catch(Exception exc)
            {
                ToolsLib.Tools.ExceptionLogAndShow(exc, "ConnectTo");
            }
        }
        private void AddConfig()
        {
            try
            {
                AddEditWindow addEditWindow = new AddEditWindow();
                if((addEditWindow?.model?.ReturnCommand?.ToLower() ?? "cancel") == "ok")
                {
                    Configs.Add(addEditWindow.model.SelectedItem);
                    StatusText = addEditWindow.model.SelectedItem.HostName + " added to config list.";
                }
            }
            catch(Exception exc)
            {
                ToolsLib.Tools.ExceptionLogAndShow(exc, "addconfig()");
            }
        }
        private void EditConfig()
        {
            if (ConfigData == null)
            {
                StatusText = "Select config for edit.";
                return;
            }
            try
            {
                AddEditWindow addEditWindow = new AddEditWindow(ConfigData);
                if ((addEditWindow?.model?.ReturnCommand?.ToLower() ?? "calncel")== "ok")
                {
                    ConfigData.FromOtherItem(addEditWindow.model.SelectedItem);
                    StatusText = "To use changes please reconnect.";
                }
            }
            catch(Exception exc)
            {
                ToolsLib.Tools.ExceptionLogAndShow(exc, "EditConfig()");
            }
        }
        private void DeleteConfig()
        {
            try
            {
                if (ConfigData == null) return;
                if(MessageBox.Show("Do you really want delete config for host: "+ConfigData.HostName,"Question",MessageBoxButton.YesNo)==MessageBoxResult.Yes)
                {
                    Configs.Remove(ConfigData);
                    StatusText = "Deleted";
                }
            }
            catch(Exception exc)
            {
                ToolsLib.Tools.ExceptionLogAndShow(exc, "deleteconfig()");
            }
        }
        private void Disconnect()
        {
            try
            {
                if (client?.IsConnected ?? false)
                    client.Disconnect();
                else
                    return;
                StatusText = "Disconnected";
                ListClear();
            }
            catch(Exception exc)
            {
                ToolsLib.Tools.ExceptionLogAndShow(exc, "disconnect()");
            }
        }
        private async void Upload()
        {
            try
            {
                if (DataList.Count == 0) return;
                var tmp = SelectFile();
                if (string.IsNullOrWhiteSpace(tmp)) return;
                var remotePath = System.IO.Path.Combine(DataList.FirstOrDefault(x => x.Name.Equals(".")).FullPath.TrimEnd('.'), System.IO.Path.GetFileName(tmp));

                using (System.IO.StreamReader stream = new System.IO.StreamReader(tmp))
                {
                    if (await client.UploadAsync(stream.BaseStream, remotePath, FluentFTP.FtpExists.Overwrite, true, new System.Threading.CancellationToken(), progress: iProgres))
                    {
                        StatusText = "Successfull upload";
                        ListCombine(DataList.FirstOrDefault(x => x.Name.Equals(".")).FullPath);
                    }
                    else
                    {
                        StatusText = "Error while upload";
                    }
                }
            }
            catch (Exception exc)
            {
                ToolsLib.Tools.ExceptionLogAndShow(exc, "Upload");
            }
        }
        private async void Download()
        {
            try
            {
                if (SelectedListRow == null || (SelectedListRow.Type != "FILE")) return;
                if (!ConfigData.LocalPathExists) { StatusText = "LocalPath not exists"; return; }
                string localPath = System.IO.Path.Combine(ConfigData.LocalPath, SelectedListRow.Name);
                string remotePath = SelectedListRow.FullPath;

                if (await client.DownloadFileAsync(localPath, remotePath, true, FluentFTP.FtpVerify.Delete, new System.Threading.CancellationToken(), progress: iProgres))
                {
                    StatusText = "Successfull download";
                }
                else
                {
                    StatusText = "Error while Download";
                }
            }
            catch (Exception exc)
            {
                ToolsLib.Tools.ExceptionLogAndShow(exc, "Download");
            }
        }
        private void CreateDirectory()
        {
            try
            {
                if (DataList.Count == 0) return;
                var remotePath = DataList.FirstOrDefault(x => x.Name.Equals(".")).FullPath.TrimEnd('.');
                GetStringDialogWindow getStringDialog = new GetStringDialogWindow("Enter directory name");
                if(getStringDialog.GetStatus())
                {
                    string dirName = getStringDialog.GetResult();
                    if (string.IsNullOrWhiteSpace(dirName) || dirName.Contains("/") || dirName.Contains("\\") || dirName.Contains("."))
                    {
                        StatusText = "Wrong dir name";
                        return;
                    }                       
                    remotePath = System.IO.Path.Combine(remotePath, dirName);

                    client.CreateDirectory(remotePath);
                    ListCombine(DataList.FirstOrDefault(x => x.Name.Equals(".")).FullPath);
                }

            }
            catch (Exception exc)
            {
                ToolsLib.Tools.ExceptionLogAndShow(exc, "CreateDirectory");
            }
        }
        private void Delete()
        {
            if (SelectedListRow == null) return;
            try
            {
                if (SelectedListRow.Type == "DIR")
                {
                    if (!FastDelete)
                    {
                        if (MessageBox.Show("Do you want delete directory: " + SelectedListRow.FullPath, "Security question", MessageBoxButton.YesNo) == MessageBoxResult.No)
                        {
                            return;
                        }
                    }
                    client.DeleteDirectory(SelectedListRow.FullPath);
                    ListCombine(DataList.FirstOrDefault(x => x.Name.Equals(".")).FullPath);
                }
                else if (SelectedListRow.Type == "FILE")
                {
                    if (!FastDelete)
                    {
                        if (MessageBox.Show("Do you want delete directory: " + SelectedListRow.FullPath, "Security question", MessageBoxButton.YesNo) == MessageBoxResult.No)
                        {
                            return;
                        }
                    }
                    client.DeleteFile(SelectedListRow.FullPath);
                    ListCombine(DataList.FirstOrDefault(x => x.Name.Equals(".")).FullPath);
                }
            }
            catch (Exception exc)
            {
                ToolsLib.Tools.ExceptionLogAndShow(exc, "DeleteCommand");
            }
        }       
        private void Move()
        {            
            if (SelectedListRow.Type == "FILE")
            {                
                MovePath = SelectedListRow.FullPath;
                StatusText = System.IO.Path.GetFileName(SelectedListRow.FullPath) + " selected to move";
                return;            
            }            
        }
        private void MoveHere()
        {
            if (!string.IsNullOrWhiteSpace(MovePath))
            {
                var remotePath = System.IO.Path.Combine(DataList.FirstOrDefault(x => x.Name.Equals(".")).FullPath.TrimEnd('.'), System.IO.Path.GetFileName(MovePath));
                try
                {
                    if (client.FileExists(remotePath))
                    {
                        StatusText = "File alredy exists";
                        MovePath = string.Empty;
                        return;
                    }
                    else if (client.MoveFile(MovePath, remotePath))
                    {
                        ListCombine(DataList.FirstOrDefault(x => x.Name.Equals(".")).FullPath);
                        StatusText = "File moved";
                    }
                    else
                    {
                        StatusText = "Error: Can't move";
                        return;
                    }
                }
                catch (Exception exc)
                {
                    ToolsLib.Tools.ExceptionLogAndShow(exc, "MoveFile");
                }
                MovePath = string.Empty;
                return;
            }
        }
        private void AboutWindow()
        {
            new ToolsLib.Wpf.AboutWindow(title, version, "Simple ftp client for operations on single files");
        }
        private void Exit()
        {
            App.Current.MainWindow.Close();
        }
        #endregion
#region ToolFunctions
        private void ListCombine(string path = "")
        {
            try
            {
                if (!client.IsConnected) return;
                FluentFTP.FtpListItem[] list;
                list = client.GetListing(path);                
                var tmplist = new ObservableCollection<DataListRowModel>();
                var dirlist = list.Where(x => x.Type == FluentFTP.FtpFileSystemObjectType.Directory);
                var filelist = list.Where(x => x.Type == FluentFTP.FtpFileSystemObjectType.File);

                #region defaultdir
                string parentPath = path.TrimEnd('/');
                if (parentPath != "")
                    parentPath = parentPath.Substring(0, parentPath.LastIndexOf('/'));
                if(parentPath=="")
                    parentPath = "/";

                if(path!="/")
                    tmplist.Add(new DataListRowModel()
                    {
                        Name = "..",
                        FullPath = parentPath,
                        Type = "BACK",
                        Info = "FullPath: " + parentPath
                    });
                tmplist.Add(new DataListRowModel()
                {
                    Name=".",
                    FullPath = path,
                    Type = "BACK",
                    Info = "FullPath: " + path
                });
                #endregion
                foreach (var row in dirlist)
                {
                    tmplist.Add(new DataListRowModel()
                    {
                        Name = row.Name,
                        FullPath = row.FullName,
                        Type = "DIR",
                        Info = "Modified: " + row.Modified.ToString() + Environment.NewLine +
                        "Chmod: " + row.Chmod.ToString() + Environment.NewLine +
                        "FullPath: " + row.FullName
                    });
                }
                foreach (var row in filelist)
                {
                    tmplist.Add(new DataListRowModel()
                    {
                        Name = row.Name,
                        FullPath = row.FullName,
                        Type = "FILE",
                        Info = "Size: " + ((row.Size < 1024) ? row.Size.ToString() + "B" : ((row.Size < 1024 * 1024) ? (row.Size / 1024).ToString() + "KB" : (row.Size / (1024 * 1024)).ToString() + "MB")) + Environment.NewLine +
                        "Modified: " + row.Modified.ToString() + Environment.NewLine +
                        "Chmod: " + row.Chmod.ToString() + Environment.NewLine +
                        "FullPath: " + row.FullName
                    });
                }                
                DataList = tmplist;
            }
            catch(Exception exc)
            {
                ToolsLib.Tools.ExceptionLogAndShow(exc, "ListCombine()");
            }
        }
        private void ListClear()
        {
            DataList = new ObservableCollection<DataListRowModel>();
        }
        private string SelectFile()
        {
            string tmp = String.Empty;
            using (var dialog = new System.Windows.Forms.OpenFileDialog())
            {
                System.Windows.Forms.DialogResult result = dialog.ShowDialog();
                tmp = dialog.FileName;
                if (result == System.Windows.Forms.DialogResult.OK)
                {
                    return tmp;
                }
                else
                    return null;
            }
        }
        #endregion

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
