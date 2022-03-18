using BTApp.Commands;
using BTApp.Common;
using BTApp.Events;
using BTApp.Helpers;
using BTApp.Models;
using BTApp.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Configuration;
using System.Runtime.CompilerServices;
using System.Windows;

namespace BTApp.ViewModels
{
    public class MainWindowViewModel : WindowService, INotifyPropertyChanged
    {
        private PLCConnectionStatus _isPLCConnected { get; set; }
        public PLCConnectionStatus isPLCConnected
        {
            get
            {
                return _isPLCConnected;
            }
            set
            {
                _isPLCConnected = value;
                NotifyPropertyChanged();
            }
        }
        private bool _isFilePresent { get; set; } = false;
        public bool isFilePresent
        {
            get
            {
                return _isFilePresent;
            }
            set
            {
                _isFilePresent = value;
                NotifyPropertyChanged();
            }
        }
        private string _machineStatus { get; set; }
        public string machineStatus
        {
            get
            {
                return _machineStatus;
            }
            set
            {
                _machineStatus = value;
                NotifyPropertyChanged();
            }
        }
        private float _currentLengthExecuted { get; set; }
        public float CurrentLengthExecuted
        {
            get
            {
                return _currentLengthExecuted;
            }
            set
            {
                _currentLengthExecuted = value;
                NotifyPropertyChanged();
            }
        }
        private float _orderTotalLength { get; set; }
        public float OrderTotalLength
        {
            get
            {
                return _orderTotalLength;
            }
            set
            {
                _orderTotalLength = value;
                NotifyPropertyChanged();
            }
        }
        private float _orderHeater { get; set; }
        public float OrderHeater
        {
            get
            {
                return _orderHeater;
            }
            set
            {
                _orderHeater = value;
                NotifyPropertyChanged();
            }
        }
        private int _orderQuantity { get; set; }
        public int OrderQuantity
        {
            get
            {
                return _orderQuantity;
            }
            set
            {
                _orderQuantity = value;
                NotifyPropertyChanged();
            }
        }
        private float _orderLength { get; set; }
        public float OrderLength
        {
            get
            {
                return _orderLength;
            }
            set
            {
                _orderLength = value;
                NotifyPropertyChanged();
            }
        }

        private int inputPLCData;

        public int InputPLCData
        {
            get { return inputPLCData; }
            set 
            { 
                inputPLCData = value;
                NotifyPropertyChanged();
            }
        }

        private int outputPLCData;

        public int OutputPLCData
        {
            get { return outputPLCData; }
            set 
            { 
                outputPLCData = value;
                NotifyPropertyChanged();
            }
        }

        private Settings settings;
        public Settings Settings
        {
            get { return settings; }
            set 
            { 
                settings = value;
                NotifyPropertyChanged();
            }
        }

        private const string PLC_TEST_REG = "D4509";



        public LastOrderCommand ShowLastOrderCommand { get; private set; }
        public OnWindowLoadedCommand OnWindowLoadedCommand { get; private set; }
        public OnWindowUnloadedCommand OnWindowUnloadedCommand { get; private set; }
        public WriteDataTestCommand onWriteDataCommand { get; set; }
        public ReadDatatestCommand onReadDatatestCommand { get; set; }
        public SelectFolderCommand onSelectFilePathCommand { get; set; }
        public RestoreDefaultSettingsCommand onRestoreDefaultSettingsCommand { get; set; }
        public SaveSettingsInDBCommand onSaveSettingsInDBCommand { get; set; }
        public ClearErrorHistoryCommand onClearErrorHistoryCommand { get; set; }

        public ObservableCollection<Device> devices = new ObservableCollection<Device>();
        public ObservableCollection<Order> order;
        public List<PlcError> ActivePlcErrors { get; set; }
        public List<PlcError> LogedErrors { get; set; }

        private PLC _plc;
        private FolderScan _folderScan;
        private OrderConverter _orderConverter;
        private FTPClient _fTPClient;
        private List<Order> _lastOrder = new List<Order>();

        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Konstruktor
        /// </summary>
        public MainWindowViewModel()
        {
            
            _folderScan = new FolderScan();
            _orderConverter = new OrderConverter();
            _folderScan.fileDetected += _folderScan_fileDetected;
            _orderConverter.OrderProcessingFinished += _orderConverter_OrderProcessingFinished;
            ShowLastOrderCommand = new LastOrderCommand(DisplayLastOrderView);
            OnWindowLoadedCommand = new OnWindowLoadedCommand(OnWindowLoaded);
            OnWindowUnloadedCommand = new OnWindowUnloadedCommand(OnWindowUnloaded);
            onWriteDataCommand = new WriteDataTestCommand(this);
            onReadDatatestCommand = new ReadDatatestCommand(this);
            onClearErrorHistoryCommand = new ClearErrorHistoryCommand(this);
            onSelectFilePathCommand = new SelectFolderCommand(this);
            onRestoreDefaultSettingsCommand = new RestoreDefaultSettingsCommand(this);
            onSaveSettingsInDBCommand = new SaveSettingsInDBCommand(this);
            ActivePlcErrors = new List<PlcError>();
            LogedErrors = new List<PlcError>();
            Settings = new Settings();
            SeedSettingsData();
            //testing

        }

        /// <summary>
        /// Event zamnkięcia okna
        /// </summary>
        void OnWindowUnloaded()
        {
            _plc.End();
            _folderScan.End();
            Application.Current.Shutdown();
        }

        /// <summary>
        /// Event wyrenderowania nowego okna
        /// </summary>
        void OnWindowLoaded()
        {
            _folderScan.Begin();
            _fTPClient = new FTPClient();
            _plc = new PLC(Settings.PlcIPAddress,Settings.PlcPassword,Settings.PlcConnectionTimeout,Settings.PlcRefreshRate);
            _plc.Begin();
            _plc.PLCStatusChange += _plc_PLCStatusChange;
            _plc.MachineStatusChange += _plc_MachineStatusChange;
            _plc.MonitorValueChange += _plc_MonitorValueChange;
            _plc.DataChange += _dataChange;
            _plc.ActiveErrorChange += _plc_ActiveErrorChange;
            _plc.LogedErrorChange += _plc_LogedErrorChange;
            GetErrorLogs();
        }

        /// <summary>
        /// Event zmiany monitorowanych danych
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _plc_MonitorValueChange(object sender, EventArgs e)
        {
            PLC plc = sender as PLC;
            devices = new ObservableCollection<Device>(plc.Devices);
            OrderLength = devices[0].Value / 1000f;
            OrderQuantity = devices[1].Value;
            OrderHeater = devices[2].Value /10f;
            CurrentLengthExecuted = devices[3].Value / 1000f;
            OrderTotalLength = devices[4].Value / 1000f;
        }

        private void _dataChange(object sender, EventArgs e)
        {
            PLC plc = sender as PLC;
            InputPLCData = plc.inputData;
        }

        /// <summary>
        /// Event zmiany stanu maszyny
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _plc_MachineStatusChange(object sender, EventArgs e)
        {
            PLC plc = sender as PLC;
            var test = new Machine().State;
            machineStatus = new Machine().State.Find(state => state.Key == plc.MachineStatusCode).Value;
        }

        /// <summary>
        /// Event zmiany stanu połączenia ze sterownikiem PLC
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _plc_PLCStatusChange(object sender, System.EventArgs e)
        {
            PLC plc = sender as PLC;
            isPLCConnected = plc.isConnected;
        }

        private void _plc_ActiveErrorChange(object sender, System.EventArgs e)
        { 
            PLC plc = sender as PLC;
            ActivePlcErrors = new List<PlcError>(plc.ActiveErrors);
            NotifyPropertyChanged("ActivePlcErrors");

        }

        private void _plc_LogedErrorChange(object sender, System.EventArgs e)
        {
            GetErrorLogs();
        }

        /// <summary>
        /// Event zmiany atrybutu wymagany do odświeżenia informacji w UI
        /// </summary>
        /// <param name="propertyName"></param>
        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// Event wykrycia pliku "wyslij.txt" 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _folderScan_fileDetected(object sender, EventArgs e)
        {
            FolderScan folderScan = sender as FolderScan;
            string fullPath = folderScan.importFileFullPath;
            _orderConverter.ProcessOrder(fullPath);
        }

        /// <summary>
        /// Event zakończenia przetwarzania danych zlecenia
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _orderConverter_OrderProcessingFinished(object sender, OrderProcessingEventArgs e)
        {
            _lastOrder = e.orders;
            _fTPClient.TransferOrdersAsRecipes(_lastOrder);
            _plc.NewOrderFlag = true;
            //_fTPClient.GetFileList(".CSV");
            //var recipe = new Recipe(1, 3, "Zlecenie", "ZLECENIE TEST", 20, "2000/01/01 03:18:54", 4563, 235, 12, 35);
            //var recipeBody = recipe.GetBody();
            //_plc.BatchSendOrderToPLC(e.orders, "D30000");
        }

        /// <summary>
        /// Metoda wywołuje nowe okno 
        /// </summary>
        private void DisplayLastOrderView()
        {
            ShowWindow(new LastOrderViewModel(_lastOrder), "Ostatnie zlecenie");
        }

        public int ReadWord()
        {
            return _plc.ReadSingleWord(PLC_TEST_REG , out inputPLCData);
        }

        public void WriteWord()
        {
            _plc.WriteSingleWord(PLC_TEST_REG, outputPLCData);
        }

        private void GetErrorLogs()
        {
            List<PlcError> errors = DatabaseHelper.Read<PlcError>();

            //LogedErrors.Clear();

            //foreach(PlcError error in errors)
            //{
            //    LogedErrors.Add(error);
            //}
            LogedErrors = new List<PlcError>(errors);
            NotifyPropertyChanged("LogedErrors");
        }

        public void ClearErrorlog()
        {
            DatabaseHelper.DeleteAll<PlcError>();
            GetErrorLogs();
        }

        public void ChangeFilePath(string type, string path)
        {
            switch (type)
            {
                case "Import":
                    Settings.ImportPath = path;
                    NotifyPropertyChanged("Settings");
                    break;
                case "Export":
                    Settings.ExportPath = path;
                    NotifyPropertyChanged("Settings");
                    break;
                default:
                    break;
            }
        }

        public void RestoreDefaultSettings()
        {
            MessageBoxResult result = MessageBox.Show("Sure you want to set defaults?", "Confirm", MessageBoxButton.YesNo);
            if (result == MessageBoxResult.Yes)
            {
                ReturnToDefault();
                _plc.updateSettings(Settings);
                NotifyPropertyChanged("Settings");
                DatabaseHelper.DeleteAll<Settings>();
            }
        }

        public void SaveSettingsInDB()
        {
            _plc.updateSettings(Settings);

            if(Settings.SettingsId != null)
            {
                DatabaseHelper.Update(Settings);
            }
            else
            {
                DatabaseHelper.Insert(Settings);
            }
            MessageBox.Show("Settings saved", "Info", MessageBoxButton.OK);

        }

        private void ReturnToDefault()
        {
                Settings.ImportPath = ConfigurationManager.AppSettings[nameof(Settings.ImportPath)];
                Settings.ExportPath = ConfigurationManager.AppSettings[nameof(Settings.ExportPath)];
                Settings.FileName = ConfigurationManager.AppSettings[nameof(Settings.FileName)];
                Settings.PlcIPAddress = ConfigurationManager.AppSettings[nameof(Settings.PlcIPAddress)];
                Settings.PlcPassword = ConfigurationManager.AppSettings[nameof(Settings.PlcPassword)];
                Settings.PlcConnectionTimeout = Convert.ToInt32(ConfigurationManager.AppSettings[nameof(Settings.PlcConnectionTimeout)]);
                Settings.PlcRefreshRate = Convert.ToInt32(ConfigurationManager.AppSettings[nameof(Settings.PlcRefreshRate)]);
                Settings.Debug = Convert.ToBoolean(ConfigurationManager.AppSettings[nameof(Settings.Debug)]);
                Settings.MoveFileRetryTime = Convert.ToInt32(ConfigurationManager.AppSettings[nameof(Settings.MoveFileRetryTime)]);
                Settings.GOTRecordsNum = Convert.ToInt32(ConfigurationManager.AppSettings[nameof(Settings.GOTRecordsNum)]);
                Settings.GOTRecipeName = ConfigurationManager.AppSettings[nameof(Settings.GOTRecipeName)];
                Settings.GOTRecipeID = Convert.ToInt32(ConfigurationManager.AppSettings[nameof(Settings.GOTRecipeID)]);
                Settings.GOTRecordExtension = ConfigurationManager.AppSettings[nameof(Settings.GOTRecordExtension)];
                Settings.GOTIPAddress = ConfigurationManager.AppSettings[nameof(Settings.GOTIPAddress)];
                Settings.GOTFTPPort = Convert.ToInt32(ConfigurationManager.AppSettings[nameof(Settings.GOTFTPPort)]);
                Settings.GOTRecipeFileNameTemplate = ConfigurationManager.AppSettings[nameof(Settings.GOTRecipeFileNameTemplate)];
                Settings.ServiceUri = ConfigurationManager.AppSettings[nameof(Settings.ServiceUri)];
        }
        private void SeedSettingsData()
        {
            List<Settings> setList = DatabaseHelper.Read<Settings>();
            if (setList.Count == 0)
            {
                ReturnToDefault();
            }
            else
            {
                Settings.SettingsId = setList[0].SettingsId;
                Settings.ImportPath = setList[0].ImportPath;
                Settings.ExportPath = setList[0].ExportPath;
                Settings.FileName = setList[0].FileName;
                Settings.PlcIPAddress = setList[0].PlcIPAddress;
                Settings.PlcPassword = setList[0].PlcPassword;
                Settings.PlcConnectionTimeout = setList[0].PlcConnectionTimeout;
                Settings.PlcRefreshRate = setList[0].PlcRefreshRate;
                Settings.Debug = setList[0].Debug;
                Settings.MoveFileRetryTime = setList[0].MoveFileRetryTime;
                Settings.GOTRecordsNum = setList[0].GOTRecordsNum;
                Settings.GOTRecipeName = setList[0].GOTRecipeName;
                Settings.GOTRecipeID = setList[0].GOTRecipeID;
                Settings.GOTRecordExtension = setList[0].GOTRecordExtension;
                Settings.GOTIPAddress = setList[0].GOTIPAddress;
                Settings.GOTFTPPort = setList[0].GOTFTPPort;
                Settings.GOTRecipeFileNameTemplate = setList[0].GOTRecipeFileNameTemplate;
                Settings.ServiceUri = setList[0].ServiceUri;
            }
        }

    }
}
