using BTApp.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

namespace BTApp.Common
{
    public class FTPClient
    {
        private string _ipAddress;
        private int _port;
        private static string _username = "Admin";
        private static string _password = "test";
        private string _ftpPath;
        private string _recipeFileNameTemplate;
        private FtpWebRequest _ftpWebRequest;
        private DebugMode _debugMode;
        private int _recordMax;
        private string _GOTRecipeExt;

        /// <summary>
        /// Konstruktor
        /// </summary>
        public FTPClient()
        {
            // ----------------------------------
            // Pobranie wartości konfiguracyjnych
            // ----------------------------------
            _GOTRecipeExt = ConfigurationManager.AppSettings["gotRecordExtension"];
            _ipAddress = ConfigurationManager.AppSettings["gotIPAddress"];
            _port = Convert.ToInt32(ConfigurationManager.AppSettings["gotFTPPort"]);
            _recipeFileNameTemplate = ConfigurationManager.AppSettings["gotRecipeFileNameTemplate"];
            _recordMax = Convert.ToInt32(ConfigurationManager.AppSettings["gotRecordsNum"]); ;
            _ftpPath = string.Concat("ftp://", _ipAddress, ":", _port ,"//Project1//recipe//");
            _debugMode = new DebugMode();
            //DeleteEmptyRecipes();
            // ----------------------------------
        }

        /// <summary>
        /// Metoda pobiera pliki receptur z GOT
        /// </summary>
        /// <param name="fileExt"></param>
        /// <returns></returns>
        private List<string> GetRecipesList(string fileExt)
        {
            NewFtpWebRequest(_ftpPath);
            List<string> files = new List<string>();
            try
            {
                _ftpWebRequest.Method = WebRequestMethods.Ftp.ListDirectory;
                FtpWebResponse ftpWebResponse = (FtpWebResponse)_ftpWebRequest.GetResponse();
                StreamReader streamReader = new StreamReader(ftpWebResponse.GetResponseStream());
                string line = streamReader.ReadLine();
                while (!string.IsNullOrEmpty(line))
                {
                    if (line.Contains(fileExt))
                    {
                        files.Add(line);
                        line = streamReader.ReadLine();
                    }
                    else
                    {
                        line = streamReader.ReadLine();
                    }
                }
                streamReader.Close();
                ftpWebResponse.Close();
            }
            catch(WebException e)
            {
                _debugMode.ConsoleWriteLine($"FTP error: {e.Message}");
            }
            return files;
        }
        /// <summary>
        /// Metoda usuwa puste receptury z GOT
        /// </summary>
        /// <param name="files"></param>
        /// <returns></returns>
        public bool DeleteEmptyRecipes(List<string> files = null)
        {
            bool result = false;

            if (files == null)
            {
                files = GetRecipesList(_GOTRecipeExt);
            }
            
            foreach(string fileName in files)
            {
                string filePath = _ftpPath + fileName;
                bool isEmpty = CheckIfRecipeIsEmpty(filePath);
                if (isEmpty)
                {
                    try
                    {
                        _debugMode.ConsoleWriteLine($"File {fileName} is an empty recipe. Deleting...");
                        NewFtpWebRequest(filePath);
                        _ftpWebRequest.Method = WebRequestMethods.Ftp.DeleteFile;
                        FtpWebResponse ftpWebResponse = (FtpWebResponse)_ftpWebRequest.GetResponse();
                        ftpWebResponse.Close();
                        result = (ftpWebResponse.StatusCode == FtpStatusCode.FileActionOK) ? true : false;
                    }
                    catch(Exception)
                    {
                        _debugMode.ConsoleWriteLine("FTP Error: Error deleteing file from GOT.");
                    }
                }
            }
            return result;
        }
        /// <summary>
        /// Metoda sprawdza czy plik receptury jest pusty
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        private bool CheckIfRecipeIsEmpty(string path)
        {
            NewFtpWebRequest(path);
            _ftpWebRequest.Method = WebRequestMethods.Ftp.DownloadFile;
            string line = string.Empty;
            try
            {
                using (Stream stream = _ftpWebRequest.GetResponse().GetResponseStream())
                using (StreamReader reader = new StreamReader(stream))
                {
                    for (int i = 1; i < 12; i++)
                    {
                        line = reader.ReadLine();
                    }
                }
            }
            catch(WebException e)
            {
                _debugMode.ConsoleWriteLine($"FTP error: {e.Message}");
            }
            return (line.Last() == 'N') ? true : false;
           
        }
        /// <summary>
        /// Metoda tworzy nowe zadanie FTP
        /// </summary>
        /// <param name="ftpPath"></param>
        private void NewFtpWebRequest(string ftpPath)
        {
            _ftpWebRequest = (FtpWebRequest)WebRequest.Create(ftpPath);
            _ftpWebRequest.Credentials = new NetworkCredential(_username, _password);
        }
        /// <summary>
        /// Metoda zwraca listę receptur z flagami pusta/zajęta
        /// </summary>
        /// <returns></returns>
        private List<KeyValuePair<int,bool>> GetEmptyRecipeRecords()
        {
            DeleteEmptyRecipes();
            List<string> gotRecipes = GetRecipesList(_GOTRecipeExt);
            List<KeyValuePair<int, bool>> recipeRecords = new List<KeyValuePair<int, bool>>();
            for(int i=1; i < _recordMax + 1; i++)
            {
                recipeRecords.Add(new KeyValuePair<int, bool>(i, false));
            }
            foreach(string recipeName in gotRecipes)
            {
                int recipeNumber = Int32.Parse(recipeName.Substring(9,4));
                recipeRecords[recipeNumber-1] = new KeyValuePair<int, bool>(recipeNumber, true);
            }

            return recipeRecords;
        }

        /// <summary>
        /// Metoda tworzy nowy plik na serwerze FTP na podstawie listy zlecen
        /// </summary>
        /// <param name="orders"></param>
        /// <returns></returns>
        public bool TransferOrdersAsRecipes(List<Order> orders)
        {
            if (orders == null)
            {
                return false;
            }
            List<KeyValuePair<int, bool>> recipeRecords = GetEmptyRecipeRecords();
            List<Recipe> recipes = new List<Recipe>();
            int i = 0;
            foreach (Order order in orders)
            {
                int firstEmptyRecord = recipeRecords.First(recipe => recipe.Value == false).Key + i;
                string recordName = String.Concat(firstEmptyRecord.ToString(), "_", order.Id);
                recipes.Add(new Recipe(firstEmptyRecord,recordName,order.SumLength,order.Length,order.Quantity,order.Heater));
                i++;
            }
            try
            {
                foreach(Recipe recipe in recipes)
                {
                    string recipePath = _ftpPath + _recipeFileNameTemplate + recipe.RecordNumber.ToString().PadLeft(4,'0') + ".CSV";
                    NewFtpWebRequest(recipePath);
                    _ftpWebRequest.Method = WebRequestMethods.Ftp.UploadFile;
                    byte[] recipeContent = Encoding.ASCII.GetBytes(recipe.GetBody());
                    _ftpWebRequest.ContentLength = recipeContent.Length;
                    using (Stream requestStream = _ftpWebRequest.GetRequestStream())
                    {
                        requestStream.Write(recipeContent, 0, recipeContent.Length);
                    }
                    using (FtpWebResponse ftpWebResponse = (FtpWebResponse)_ftpWebRequest.GetResponse())
                    {
                        _debugMode.ConsoleWrite($"Upload File Complete, status {ftpWebResponse.StatusDescription}");
                    }
                }
                return true;
            }
            catch(WebException e)
            {
                _debugMode.ConsoleWriteLine($"FTP error: {e.Message}");
                return false;
            }
        }

    }
}
