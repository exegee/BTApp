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


        public static void AddRecordsToCSV(string path, List<T> records)
        {
            if (!File.Exists(path))
            {
                File.Create(path).Close();
            }

            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                // Don't write the header again.
                HasHeaderRecord = false,
                Delimiter = ";"
            };
            using (var stream = File.Open(path, FileMode.Append))
            using (var writer = new StreamWriter(stream))
            using (var csv = new CsvWriter(writer, config))
            {
                csv.WriteRecords(records);
            }
        }

        public static void ClearRecords(string path)
        {
            if (!File.Exists(path))
            {
                File.Create(path).Close();
            }

            List<T> records = new List<T>();

            using (var writer = new StreamWriter(path))
            using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
            {
                csv.WriteRecords(records);
            }
        }



    }

}
