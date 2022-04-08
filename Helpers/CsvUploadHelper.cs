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
    public class CsvUploadHelper<T>
    {

        /// <summary>
        /// Metoda pobierająca listę wszystkich możliwych błędów sterownika z pliku csv z podanej ścieżki
        /// </summary>
        /// <param filePath="directoryPath"></param>
        public static List<T> GetErrorsFromCSV(string filePath)//(string filePath, string fileName)
        {
            List<T> errors = new List<T>();

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
            writer.Write(filePath);
            writer.Flush();
            stream.Position = 0;
            //convert csv file to PlcErrors list
            using (var reader = new StreamReader(stream))
            using (var csv = new CsvReader(reader, config))
            {
                var records = csv.GetRecords<T>();
                errors = records.ToList();
            }
            //}
            return errors;
        }
    }

}
