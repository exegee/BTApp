using BTApp.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Threading;

namespace BTApp.Common
{

    public class PLC
    {
        public PLCConnectionStatus isConnected { get; private set; } = PLCConnectionStatus.Connecting;
        public int MachineStatusCode { get; private set; } = 0;
        public int EncoderValue { get; private set; } = 0;
        public bool NewOrderFlag { get; set; }

        private volatile ACTETHERLib.ActLCPUTCP _ActLCPUTCP;
        //private Thread _tCheckConneciton;
        //private Thread _tGetMachineStatus;
        //private Thread _tSendDataToPLC;
        //private Thread _tGetEncoderValue;
        private Thread _tMonitor;
        private DebugMode _debugMode;
        private static string _plcPassword = "Er55";
        private string _iPAddress;
        private int _connectionTimout;
        private int _plcRefreshRate;

        public event EventHandler PLCStatusChange;
        public event EventHandler MachineStatusChange;
        public event EventHandler MonitorValueChange;

        /// <summary>
        /// Lista rejestrów do pobrania
        /// </summary>
        public List<Device> Devices = new List<Device>()
        {
         new Device{Name="D4501", Size=2, Value=0, OldValue=0, Comment="Długość w zleceniu"},
         new Device{Name="D4503", Size=1, Value=0, OldValue=0, Comment="Ilość z zleceniu"},
         new Device{Name="D4504", Size=1, Value=0, OldValue=0, Comment="Grzałka"},
         new Device{Name="D4505", Size=2, Value=0, OldValue=0, Comment="Wartość enkodera"},
         new Device{Name="D4507", Size=2, Value=0, OldValue=0, Comment="Całkowita długość"},
        };

        /// <summary>
        /// Konstruktor
        /// </summary>
        public PLC()
        {
            _debugMode = new DebugMode();
            // Pobierz adres IP sterownika PLC z pliku koniguracyjnego
            _iPAddress = ConfigurationManager.AppSettings["plcIPAddress"];
            // Pobierz wartość maksymalnego czasu na uzyskanie polaczenia
            _connectionTimout = Convert.ToInt32(ConfigurationManager.AppSettings["plcConnectionTimout"]);
            _plcRefreshRate = Convert.ToInt32(ConfigurationManager.AppSettings["plcRefreshRate"]);

            _tMonitor = new Thread(wMonitor);
            _tMonitor.SetApartmentState(ApartmentState.STA);

            _debugMode.ConsoleWriteLine("Connecting to PLC...");

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
            _ActLCPUTCP = new ACTETHERLib.ActLCPUTCP();
            _ActLCPUTCP.ActHostAddress = _iPAddress;
            _ActLCPUTCP.ActPassword = _plcPassword;
            _ActLCPUTCP.ActTimeOut = _connectionTimout;
            int connectionStatus;
            PLCConnectionStatus isConnectedMem = PLCConnectionStatus.Connecting;
            int currentMachineStateCode = 0;
            // Bufor danych pobranych z PLC
            int[] readBuffer = new int[16];
            // Otwórz połączenie z PLC
            while (true)
            {
                _ActLCPUTCP.Open();

                // Sprawdz polaczenie z PLC
                try
                {
                    _ActLCPUTCP.GetDevice("SM400", out connectionStatus);
                    isConnected = (connectionStatus == 1) ? PLCConnectionStatus.Connected : PLCConnectionStatus.NotConnected;
                    if (isConnected != isConnectedMem)
                    {
                        isConnectedMem = isConnected;
                        OnPLCStatusChange();
                    }
                }
                catch(Exception e)
                {
                    _debugMode.ConsoleWriteLine(e.Message);
                    _ActLCPUTCP.Close();
                }
                // Jeśli połączenie aktywne odczytaj pozostałe dane
                if (isConnected == PLCConnectionStatus.Connected)
                {
                    try
                    {
                        // Pobierz stan maszyny i wyzwól event jeśli inny niż zapamiętany
                        _ActLCPUTCP.GetDevice("D4500", out currentMachineStateCode);
                        if (currentMachineStateCode != MachineStatusCode)
                        {
                            MachineStatusCode = currentMachineStateCode;
                            OnMachineStatusChange();
                        }
                        // Monitor
                        foreach(Device device in Devices)
                        {
                            _ActLCPUTCP.ReadDeviceBlock(device.Name, device.Size, out readBuffer[0]);
                            ushort b0 = (ushort)readBuffer[0];
                            ushort b1 = (ushort)readBuffer[1];
                            device.Value = b1 << 16 | b0;
                            if(device.Value != device.OldValue)
                            {
                                if (device.Value < 0)
                                {
                                    device.Value = 0;
                                }
                                device.OldValue = device.Value;
                                OnMonitorValueChange();
                            }
                        }

                        if (NewOrderFlag)
                        {
                            _ActLCPUTCP.SetDevice("M4400", 1);
                            NewOrderFlag = false;
                        }

                    }
                    catch (Exception e)
                    {
                        _debugMode.ConsoleWriteLine(e.Message);
                        _ActLCPUTCP.Close();
                    }
                }
                
                //_ActLCPUTCP.Close();
                //GC.Collect();
                Thread.Sleep(_plcRefreshRate);
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
    }

}
