using BTApp.Commands;
using BTApp.Common;
using BTApp.Events;
using BTApp.Models;
using BTApp.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
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


        public LastOrderCommand ShowLastOrderCommand { get; private set; }
        public OnWindowLoadedCommand OnWindowLoadedCommand { get; private set; }
        public OnWindowUnloadedCommand OnWindowUnloadedCommand { get; private set; }
        public ObservableCollection<Device> devices = new ObservableCollection<Device>();
        public ObservableCollection<Order> order;

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
             _plc = new PLC();
            _plc.Begin();
            _plc.PLCStatusChange += _plc_PLCStatusChange;
            _plc.MachineStatusChange += _plc_MachineStatusChange;
            _plc.MonitorValueChange += _plc_MonitorValueChange;
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
    }
}
