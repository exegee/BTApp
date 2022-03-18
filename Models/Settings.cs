using BTApp.Helpers;
using SQLite;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BTApp.Models
{
    public class Settings
    {
        [PrimaryKey, AutoIncrement]
        public int? SettingsId { get; set; }
        public string ImportPath { get; set; }
        public string ExportPath { get; set; }
        public string FileName { get; set; }
        public string PlcIPAddress { get; set; }
        public string PlcPassword { get; set; }
        public int PlcConnectionTimeout { get; set; }
        public int PlcRefreshRate { get; set; }
        public bool Debug { get; set; }
        public int MoveFileRetryTime {get; set; }
        public int GOTRecordsNum { get; set; }
        public string GOTRecipeName { get; set; }
        public int GOTRecipeID { get; set; }
        public string GOTRecordExtension { get; set; }
        public string GOTIPAddress { get; set; }
        public int GOTFTPPort { get; set; }
        public string GOTRecipeFileNameTemplate { get; set; }
        public string ServiceUri { get; set; }

    }
}
