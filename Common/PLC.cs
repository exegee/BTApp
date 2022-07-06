using BTApp.Helpers;
using BTApp.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.IO;
using System.Threading;

namespace BTApp.Common
{

    public class PLC
    {
        public PLCConnectionStatus isConnected { get; private set; } = PLCConnectionStatus.Connecting;
        public int MachineStatusCode { get; private set; } = 0;
        public int EncoderValue { get; private set; } = 0;
        public bool NewOrderFlag { get; set; }

        public int inputData { get; set; }
        public int outputData { get; set; }

        public string deviceReg { get; set; }
        private List<PlcError> _possiblePlcErrorsList;
        public List<PlcError> ActiveErrors;

        //PLC constants
        private const string PLC_READY_REG = "SM400";
        private const string PLC_MACHINESTATE_REG = "D4500";
        //private const string PLC_LOADORDER_REG = "M4400";
        static readonly object _locker = new object();

        private volatile ACTETHERLib.ActLCPUTCP _ActLCPUTCP;
        //private Thread _tCheckConneciton;
        //private Thread _tGetMachineStatus;
        //private Thread _tSendDataToPLC;
        //private Thread _tGetEncoderValue;
        private Thread _tMonitor;
        private Thread _tAsync;
        private DebugMode _debugMode;
        private static string _plcPassword;
        private string _plcIPAddress;
        private int _plcConnectionTimeout;
        private int _plcRefreshRate;

        public event EventHandler PLCStatusChange;
        public event EventHandler DataChange;
        public event EventHandler MachineStatusChange;
        public event EventHandler MonitorValueChange;
        public event EventHandler ActiveErrorChange;
        public event EventHandler LogedErrorChange;

        /// <summary>
        /// Lista rejestrów do pobrania
        /// </summary>
        public List<Device> Devices = new List<Device>()
        {
         new Device{Name="D4501", Size=1, Value=0, OldValue=0, Comment="Operating Mode"},//
         new Device{Name="D4012", Size=2, Value=0, OldValue=0, Comment="Recoiling Speed"},//
         new Device{Name="D4000", Size=2, Value=0, OldValue=0, Comment="Current sheet lenght executed"},//
         new Device{Name="D5860", Size=2, Value=0, OldValue=0, Comment="Total Lenght"},//
         new Device{Name="D2102", Size=2, Value=0, OldValue=0, Comment="Actual decoiler load"},//???predkosc
         new Device{Name="D802", Size=2, Value=0, OldValue=0, Comment="Actual recoiler load"},//
         new Device{Name="D4010", Size=2, Value=0, OldValue=0, Comment="Cutting speed"},//
         new Device{Name="D5962", Size=1, Value=0, OldValue=0, Comment="Already cut sheets"},//
         new Device{Name="D5966", Size=1, Value=0, OldValue=0, Comment="Total amount of sheets"},//
         new Device{Name="D4516", Size=1, Value=0, OldValue=0, Comment="Stripes Width"},
         new Device{Name="D4517", Size=1, Value=0, OldValue=0, Comment="Stripes Number"},
         new Device{Name="D5960", Size=2, Value=0, OldValue=0, Comment="Stripes length"},//
         new Device{Name="D4502", Size=2, Value=0, OldValue=0, Comment="Service Gates"}//
        };

        /// <summary>
        /// Konstruktor
        /// </summary>
        public PLC(string iPAddress, string password, int timeout, int refreshTime)
        {
            _debugMode = new DebugMode();
            // Pobierz adres IP sterownika PLC z pliku koniguracyjnego
            //_iPAddress = ConfigurationManager.AppSettings["plcIPAddress"];
            //// Pobierz wartość maksymalnego czasu na uzyskanie polaczenia
            //_connectionTimout = Convert.ToInt32(ConfigurationManager.AppSettings["plcConnectionTimout"]);
            //_plcRefreshRate = Convert.ToInt32(ConfigurationManager.AppSettings["plcRefreshRate"]);
            //_plcPassword = ConfigurationManager.AppSettings["plcPassword"];
            _possiblePlcErrorsList = CsvUploadHelper<PlcError>.GetErrorsFromCSV(Properties.Resources.Evromat2022_MA595_Awarie);
            _tMonitor = new Thread(wMonitor);
            _tMonitor.SetApartmentState(ApartmentState.STA);
            _debugMode.ConsoleWriteLine("Connecting to PLC...");
            ActiveErrors = new List<PlcError>();
            _plcPassword = password;
            _plcIPAddress = iPAddress;
            _plcConnectionTimeout = timeout;
            _plcRefreshRate = refreshTime;

        }
        
        /// <summary>
        /// Metoda rozpoczyna wątek monitora PLC
        /// </summary>
        public void Begin()
        {
            _tMonitor.Start();
        }

        /// <summary>
        /// Metoda zamyka wątek monitora PLC
        /// </summary>
        public void End()
        {
            //_ActLCPUTCP.Close();
            _tMonitor. Abort();
        }

        /// <summary>
        /// Wątek monitora PLC - zapis/odczyt danych
        /// </summary>
        private void wMonitor()
        {
            //_ActLCPUTCP = NewConnection();
            int data = 0;
            bool flag = false;
            int status = 0;

            int connectionStatus;
            PLCConnectionStatus isConnectedMem = PLCConnectionStatus.Connecting;
            int currentMachineStateCode = 0;
            // Bufor danych pobranych z PLC
            int[] readBuffer = new int[16];
            // Otwórz połączenie z PLC
            while (true)
            {
                lock (_locker)
                {
                    _ActLCPUTCP = NewConnection();
                    status = _ActLCPUTCP.Open();

                    // Sprawdz polaczenie z PLC
                    try
                    {
                        status = _ActLCPUTCP.GetDevice(PLC_READY_REG, out connectionStatus);
                        isConnected = (connectionStatus == 1 && status == 0) ? PLCConnectionStatus.Connected : PLCConnectionStatus.NotConnected;
                        if (isConnected != isConnectedMem)
                        {
                            isConnectedMem = isConnected;
                            OnPLCStatusChange();
                        }
                    }
                    catch (Exception e)
                    {
                        _debugMode.ConsoleWriteLine("PLC connection failed: " + e.Message);
                        _ActLCPUTCP.Close();
                    }
                    // Jeśli połączenie aktywne odczytaj pozostałe dane
                    if (isConnected == PLCConnectionStatus.Connected)
                    {
                        try
                        {
                            // Pobierz stan maszyny i wyzwól event jeśli inny niż zapamiętany
                            _ActLCPUTCP.GetDevice(PLC_MACHINESTATE_REG, out currentMachineStateCode);

                            //check for PLC Errors
                            CheckForPlcErrors();

                            //Try Read Single Word - To Delete
                            ReadSingleWord("D4509", out data);
                            inputData = data;
                            
                            OnDataStatusChange();



                           if (currentMachineStateCode != MachineStatusCode)
                            {
                                MachineStatusCode = currentMachineStateCode;
                                OnMachineStatusChange();
                            }
                            // Monitor
                            foreach (Device device in Devices)
                            {
                                _ActLCPUTCP.ReadDeviceBlock(device.Name, device.Size, out readBuffer[0]);
                                ushort b0 = (ushort)readBuffer[0];
                                ushort b1 = (ushort)readBuffer[1];
                                device.Value = b1 << 16 | b0;
                                if (device.Value != device.OldValue)
                                {
                                    if (device.Value < 0)
                                    {
                                        device.Value = 0;
                                    }
                                    device.OldValue = device.Value;
                                    OnMonitorValueChange();
                                }
                            }

                            //if (NewOrderFlag)
                            //{
                            //    _ActLCPUTCP.SetDevice(PLC_LOADORDER_REG, 1);
                            //    NewOrderFlag = false;
                            //}

                        }
                        catch (Exception e)
                        {
                            _debugMode.ConsoleWriteLine(e.Message);
                            _ActLCPUTCP.Close();
                            _debugMode.ConsoleWriteLine("PLC data read failed" + e.Message);
                        }
                    }
                    else
                    {
                        currentMachineStateCode = 0;
                        MachineStatusCode = currentMachineStateCode;
                        OnMachineStatusChange();
                    }

                    //_ActLCPUTCP.Close();
                    //GC.Collect();
                    _ActLCPUTCP.Close();
                    Thread.Sleep(_plcRefreshRate);
                }
            }

        }

        
        /// <summary>
        /// Event zmiany stanu połączenia ze sterownikiem PLC
        /// </summary>
        public void OnPLCStatusChange()
        {
            //Debug
            _debugMode.ConsoleWriteLine($"PLC connection status: {isConnected}");

            //Wywołaj event zmiany statusu PLC
            PLCStatusChange?.Invoke(this, EventArgs.Empty);
        }

        public void OnDataStatusChange()
        {
            //Debug
            //_debugMode.ConsoleWriteLine($"PLC connection status: {isConnected}");

            //Wywołaj event zmiany statusu PLC
            DataChange?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Event zmiany stanu maszyny
        /// </summary>
        public void OnMachineStatusChange()
        {
            //Debug
            _debugMode.ConsoleWriteLine($"Machine status code: {MachineStatusCode}");

            //Wywołaj event zmiany stanu maszyny
            MachineStatusChange?.Invoke(this, EventArgs.Empty);
        }

        public void OnMonitorValueChange()
        {
            // Wywowaj event zmiany stanu wartości monitora
            MonitorValueChange?.Invoke(this, EventArgs.Empty);
        }

        public void OnActiveErrorChange()
        {
            // Wywowaj event zmiany stanu wartości monitora
            ActiveErrorChange?.Invoke(this, EventArgs.Empty);
        }

        public void OnLogedErrorChange()
        {
            LogedErrorChange?.Invoke(this, EventArgs.Empty);
        }

        public bool updateSettings(Settings settings)
        {
            bool result = false;

            if (settings != null)
            {
                if(settings.PlcConnectionTimeout > 0 &&
                    settings.PlcRefreshRate > 0 &&
                    settings.PlcPassword != null &&
                    settings.PlcIPAddress != null)
                {
                    _plcConnectionTimeout = settings.PlcConnectionTimeout;
                    _plcRefreshRate = settings.PlcRefreshRate;
                    _plcPassword = settings.PlcPassword;
                    _plcIPAddress = settings.PlcIPAddress;
                    result = true;
                }
            }
            return result;
        }

        public void updateSettings(string iPAddress, string password, int timeout, int refreshTime)
        {
            _plcConnectionTimeout = timeout;
            _plcRefreshRate = refreshTime;
            _plcPassword = password;
            _plcIPAddress = iPAddress;
        }

        #region Helper connection methods

        private ACTETHERLib.ActLCPUTCP NewConnection()
        {
            var newConnection = new ACTETHERLib.ActLCPUTCP();
            newConnection.ActHostAddress = _plcIPAddress;
            newConnection.ActPassword = _plcPassword;
            newConnection.ActTimeOut = _plcConnectionTimeout;
            return newConnection;
        }

        private void _WriteSingleWord()
        {
            lock(_locker)
            { 
                _ActLCPUTCP = NewConnection();
                _ActLCPUTCP.Open();
                try
                {
                    _ActLCPUTCP.SetDevice(deviceReg, outputData);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("BLAD");
                }

                _ActLCPUTCP.Close();
            }
        }
        //for async use
        public void WriteSingleWord(string device, int data)
        {
            outputData = data;
            deviceReg = device;
            _tAsync = new Thread(_WriteSingleWord);
            _tAsync.SetApartmentState(ApartmentState.STA);
            _tAsync.Start();
            _tAsync.Join();
        }

        //for sync use
        public int ReadSingleWord(string device, out int data)
        {
            int status = -1;
            try
            {
                status = _ActLCPUTCP.GetDevice(device, out data);
            }
            catch (Exception ex) { 
                _debugMode.ConsoleWriteLine(ex.Message);
                data = -1;
            }
            
            return status;
        }

        //public void ReadSingleWord(string device, out int data)
        //{
        //    _tAsync = new Thread(wMonitor);
        //    _tAsync.SetApartmentState(ApartmentState.STA);
        //    _tAsync.Start();
        //}

        private void CheckForPlcErrors()
        {
            Console.WriteLine("looking for errors");
            ActiveErrors.Clear();
            string szDevice = findFirstWordAddress(_possiblePlcErrorsList[0].Device);//find the beggining of a word
            int BufferSize = findBufferSize(szDevice, _possiblePlcErrorsList[_possiblePlcErrorsList.Count-1].Device);
            int status = 0;
            int iSize = BufferSize;
            int[] inputData = new int[BufferSize];
            
            //get data
            status = _ActLCPUTCP.ReadDeviceBlock(szDevice, iSize, out inputData[0]);
            //do some calculations
            int offset = 0;
            string firstDevice = _possiblePlcErrorsList[0].Device.Substring(1, _possiblePlcErrorsList[0].Device.Length - 1);
            string firstWord = szDevice.Substring(1, szDevice.Length - 1);
            offset = Math.Abs(int.Parse(firstDevice) - int.Parse(firstWord));

            int mask;
            PlcError tempVal;
            int startPoint;
            int endPoint;
            int index;

            for (int i =0; i < inputData.Length; i++)
            {
                //manage indexes of particular byte: for first is 4...15, after 0...15
                if (i==0)
                {
                    startPoint = offset;
                    endPoint = 16;
                }
                else
                {
                    startPoint = 0;
                    endPoint = 16;
                }

                for (int j = startPoint; j < endPoint; j++)
                {
                    mask = 1;
                    index = (i * 16) + j - offset;

                    //exit from function on the end of error list
                    if (index < _possiblePlcErrorsList.Count)
                    {

                        tempVal = _possiblePlcErrorsList[index];

                        //if error occures
                        if ((inputData[i] & (mask << j)) != 0)
                        {
                            tempVal.PreviousState = tempVal.CurrentState;
                            tempVal.CurrentState = true;
                            Console.WriteLine($"Error occured on adress: {tempVal.Device}");
                            ActiveErrors.Add(tempVal);
                            if (!tempVal.PreviousState)
                            {
                                tempVal.OccurenceTime = DateTime.Now;//update date only when change from 0 to 1
                            }
                            OnActiveErrorChange();
                        }
                        else
                        {
                            tempVal.PreviousState = tempVal.CurrentState;
                            tempVal.CurrentState = false;
                            tempVal.OccurenceTime = null;
                            //Console.WriteLine($"No error on adress: {tempVal.Device}");
                            //check if error was active
                            if (tempVal.PreviousState)
                            {
                                //TODO log old error in log list (probably .db would be the best)
                                tempVal.OccurenceTime = DateTime.Now;
                                try
                                {
                                    //DatabaseHelper.Insert(tempVal);
                                    List<PlcErrorLog> list = new List<PlcErrorLog>();
                                    PlcErrorLog newItem = new PlcErrorLog();
                                    newItem.Device = tempVal.Device;
                                    newItem.Code = tempVal.Code;
                                    newItem.Module = tempVal.Module;
                                    newItem.Description = tempVal.Description;
                                    newItem.PreviousState = tempVal.PreviousState;
                                    newItem.CurrentState = tempVal.CurrentState;
                                    newItem.OccurenceTime = tempVal.OccurenceTime;
                                    list.Add(newItem);
                                    CsvUploadHelper<PlcErrorLog>.AddRecordsToCSV(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "ErrorLogs.csv"),list);
                                    OnLogedErrorChange();
                                    OnActiveErrorChange();
                                }catch (Exception ex)
                                {
                                    _debugMode.ConsoleWriteLine("PLC error log failed: " + ex.Message);
                                }
                            }

                        }
                    }
                    else
                    {
                        return;
                    }

                }
            }
            //Console.WriteLine($"status: {status}");
            //Console.WriteLine($"first byte: {data}");
            //Console.WriteLine($"first byte: {inputData[0]}");
            //Console.WriteLine($"second byte: {inputData[1]}");
            //Console.WriteLine($"third byte: {inputData[2]}");
        }

        private string findFirstWordAddress(string device)
        {
            device = device.Substring(1, device.Length - 1);
            int deviceInt = int.Parse(device);
            int adress = (deviceInt / 16) * 16;
            return "M" + adress.ToString();
        }

        private int findBufferSize(string firstAddress, string lastAddress)
        {
            firstAddress = firstAddress.Substring(1, firstAddress.Length - 1);
            int begin = int.Parse(firstAddress);
            lastAddress = lastAddress.Substring(1, lastAddress.Length - 1);
            int end = int.Parse(lastAddress);

            return (int)Math.Ceiling((end - begin) / 16.0);

        }

            #endregion

    }

}
