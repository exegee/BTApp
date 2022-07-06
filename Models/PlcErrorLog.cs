using System;
using CsvHelper.Configuration.Attributes;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BTApp.Models
{
    public class PlcErrorLog
    {
        [Index(1)]
        public string Device { get; set; }
        [Index(2)]
        public string Code { get; set; }
        [Index(0)]
        public string Module { get; set; }
        [Index(3)]
        public string Description { get; set; }
        [Ignore]
        public bool PreviousState { get; set; }
        [Ignore]
        public bool CurrentState { get; set; }
        [Index(4)]
        public DateTime? OccurenceTime { get; set; }
    }
}
