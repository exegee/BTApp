using System.Collections.Generic;

namespace BTApp
{
    public enum PLCConnectionStatus
    {
        Connecting = 0,
        Connected = 1,
        NotConnected = 2        
    }
    public class Machine
    {
        public List<KeyValuePair<int, string>> State = new List<KeyValuePair<int,string>> (){
            new KeyValuePair<int, string>(0, "b/d"),
            new KeyValuePair<int, string>(1, "Manual"),
            new KeyValuePair<int, string>(2, "Wprowadzanie"),
            new KeyValuePair<int, string>(3, "Automat"),
            new KeyValuePair<int, string>(4, "Automat - przewijanie bez rozcinannia"),
            new KeyValuePair<int, string>(5, "Automat - przewijanie z rozcinaniem"),
            new KeyValuePair<int, string>(6, "Automat - arkuszowanie bez rozcinania"),
            new KeyValuePair<int, string>(7, "Automat - arkuszowanie z rozcinaniem"),
            new KeyValuePair<int, string>(100, "STOP awaryjny"),
            //new KeyValuePair<int, string>(101, "OTWARTO bramki serwisowe"),
            //new KeyValuePair<int, string>(110, "AWARIA w zespole rozwijacza"),
            //new KeyValuePair<int, string>(111, "AWARIA w zespole rozcinania"),
            //new KeyValuePair<int, string>(112, "AWARIA w zespole prowadzenia"),
            //new KeyValuePair<int, string>(113, "AWARIA w zespole nawijacza"),
        };

        //public List<KeyValuePair<int, string>> OperatingMode = new List<KeyValuePair<int, string>>(){
        //    new KeyValuePair<int, string> (0,"Recoiling"),
        //    new KeyValuePair<int, string>(1, "Sheeting")
        //};
    }

    public enum PLCOperatingMode
    {
        Unknown = 0,
        Recoiling_NoCutting = 1,
        Recoiling_Cutting = 2,
        Sheeting_NoCutting = 3,
        Sheeting_Cutting = 4
    }


}
