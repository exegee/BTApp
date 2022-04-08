using BTApp.Helpers;
using CsvHelper.Configuration.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BTApp.Models
{
    public  class ServiceGates
    {
        public List<ServiceGate> Gates { get; set; }

        public ServiceGates(int PLCRawData)
        {
            Gates = CsvUploadHelper<ServiceGate>.GetErrorsFromCSV(Properties.Resources.Lista_Bramek);
            
            restoreDataFromDW(PLCRawData);

        }

        public void updateState(int PLCRawData)
        {
            restoreDataFromDW(PLCRawData);
        }

        private void restoreDataFromDW(int data)
        {
            int mask = 0;

            for (int i = 0; i < 18; i++)
            {
                ServiceGate gate = Gates.Where(g => g.Number == i+1).FirstOrDefault();
                mask = (int)Math.Pow(2, i);
                gate.Active = (data & mask) != 0;
            }
        }

    }

    public class ServiceGate
    {
        [Index(0)]
        public int Number { get; set; }

        [Ignore]
        public bool Active { get; set; }
        [Index(1)]
        public string Description { get; set; }
    }


}
