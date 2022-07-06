using BTApp.Helpers;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CsvHelper.Configuration.Attributes;

namespace BTApp.Models
{
    public class Settings
    {
        //[Index(0)]
        //public string ImportPath { get; set; }
        //[Index(1)]
        //public string ExportPath { get; set; }
        //[Index(2)]
        //public string FileName { get; set; }
        [Index(0)]
        public string PlcIPAddress { get; set; }
        [Index(1)]
        public string PlcPassword { get; set; }
        [Index(2)]
        public int PlcConnectionTimeout { get; set; }
        [Index(3)]
        public int PlcRefreshRate { get; set; }
        [Ignore]
        public bool Debug { get; set; }
        //[Index(8)]
        //public int MoveFileRetryTime {get; set; }
        //[Index(9)]
        //public int GOTRecordsNum { get; set; }
        //[Index(10)]
        //public string GOTRecipeName { get; set; }
        //[Index(11)]
        //public int GOTRecipeID { get; set; }
        //[Index(12)]
        //public string GOTRecordExtension { get; set; }
        //[Index(13)]
        //public string GOTIPAddress { get; set; }
        //[Index(14)]
        //public int GOTFTPPort { get; set; }
        //[Index(15)]
        //public string GOTRecipeFileNameTemplate { get; set; }
        //[Index(16)]
        //public string ServiceUri { get; set; }

    }
}
