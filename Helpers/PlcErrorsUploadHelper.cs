using BTApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using CsvHelper;
using System.Globalization;
using CsvHelper.Configuration;

namespace BTApp.Helpers
{
    public class PlcErrorsUploadHelper
    {

        /// <summary>
        /// Metoda pobierająca listę wszystkich możliwych błędów sterownika z pliku csv z podanej ścieżki
        /// </summary>
        /// <param filePath="directoryPath"></param>
        public static List<PlcError> GetErrorsFromCSV()//(string filePath, string fileName)
        {
            List<PlcError> errors = new List<PlcError>();

            //if (Directory.Exists(filePath))
            //{
                var config = new CsvConfiguration(CultureInfo.InvariantCulture)
                {
                    HasHeaderRecord = true,
                    //MissingFieldFound = null,
                    Delimiter = ";"
                };
            //Create data stream from app resources MA595_FailuresList.csv
            var stream = new MemoryStream();
            var writer = new StreamWriter(stream);
            writer.Write(Properties.Resources.Evromat2022_MA595_IO_v1_7);
            writer.Flush();
            stream.Position = 0;
            //convert csv file to PlcErrors list
            using (var reader = new StreamReader(stream))
            using (var csv = new CsvReader(reader, config))
            {
                var records = csv.GetRecords<PlcError>();
                errors = records.ToList();
            }
            //}
            return errors;
        }
    }

}
