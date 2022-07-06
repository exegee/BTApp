using BTApp.Helpers;
using BTApp.Models;
using System;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Threading;

namespace BTApp.Common
{
    public class FolderScan
    {
        public string importFileFullPath { get; private set; }

        private string _scanDirectoryPath;
        private string _exportDirectoryPath;
        private string _importFileName;
        private string _exportFileName;
        private int _moveFileRetryTime;
        private bool _fileDetected = false;
        private Thread _tScanFolder;
        private DebugMode _debugMode;
        private IOHelper _iOHelper;

        public event EventHandler fileDetected;

        /// <summary>
        /// Konstruktor
        /// </summary>
        public FolderScan(string importPath, string exportPath, string fileName)
        {
            // ----------------------------------
            // Pobranie wartości konfiguracyjnych
            // ----------------------------------
            //_scanDirectoryPath = ConfigurationManager.AppSettings["importPath"];
            //_exportDirectoryPath = ConfigurationManager.AppSettings["exportPath"];
            //_importFileName = ConfigurationManager.AppSettings["fileName"];
            _scanDirectoryPath = importPath;
            _exportDirectoryPath = exportPath;
            _importFileName = fileName;

            try
            {
                _moveFileRetryTime = Convert.ToInt32(ConfigurationManager.AppSettings["moveFileRetryTime"]);
            }
            catch (Exception e)
            {
                _debugMode.ConsoleWriteLine(e.Message);
            }
            // ----------------------------------

            _debugMode = new DebugMode();
            _iOHelper = new IOHelper();

            // Uruchom wątek skanowania katalogu
            _tScanFolder = new Thread(()=> {
                ScanDirectory(_importFileName);
            });
        }

        //public bool updateSettings(Settings settings)
        //{
        //    bool result = false;

        //    if (settings != null)
        //    {
        //        _scanDirectoryPath = settings.ImportPath;
        //        _exportDirectoryPath = settings.ExportPath;
        //        _importFileName = settings.FileName;
        //        result = true;
        //    }
        //    return result;
        //}
        /// <summary>
        /// Metoda uruchamia wątek skowania folderu
        /// </summary>
        public void Begin()
        {
            _tScanFolder.Start();
        }

        /// <summary>
        /// Metoda zatrzymuje wątek skanowania folderu
        /// </summary>
        public void End()
        {
            _tScanFolder.Abort();
        }

        /// <summary>
        /// Metoda skanowania katalogu w poszukiwaniu pliku o nazwie i rozszerzeniu zdefiniowanym w pliku konfiguracyjnym
        /// W tym przypadku "wyslij.txt"
        /// </summary>
        private void ScanDirectory(string fileName)
        {
            // Sprawdź czy katalog istnieje, jeśli nie utwórz go
            _iOHelper.CheckIfDirectoryExist(_scanDirectoryPath);
            // Pętla skanowania w wątku
            while (true)
            {
                try
                {
                    if (CheckForFile(_scanDirectoryPath, fileName))
                    {
                        importFileFullPath = GetFullPath(_scanDirectoryPath, _importFileName);
                        _exportFileName = GetExportFileName(importFileFullPath);
                        if (CheckForFile(_exportDirectoryPath, _exportFileName))
                        {
                            _debugMode.ConsoleWriteLine("File \"", _exportFileName, "\" exists in", _exportDirectoryPath);
                            _debugMode.ConsoleWriteLine($"Retrying in {_moveFileRetryTime} seconds...");

                            Thread.Sleep(_moveFileRetryTime*1000);
                        }
                        else
                        {
                            OnFileDetected();
                            _fileDetected = true;
                        }
                    }
                    else
                    {
                        _fileDetected = false;
                    }
                }
                catch (Exception e)
                {
                    _debugMode.ConsoleWriteLine(e.Message);
                }
                Thread.Sleep(1000);
            }
        }

        /// <summary>
        /// Event wykrycia pliku "wyslij.txt"
        /// </summary>
        public void OnFileDetected()
        {
            if (!_fileDetected)
            {
                _debugMode.ConsoleWriteLine("Found ", _importFileName, " file!");
                fileDetected?.Invoke(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Metoda zwraca pełną ścieżkę pliku
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        private string GetFullPath(string filePath, string fileName)
        {
            string fullPath = string.Concat(filePath, "\\", fileName);
            return fullPath;
        }

        /// <summary>
        /// Metoda sprawdza czy istnieja pliki o znanej nazwie w danym katalogu
        /// </summary>
        /// <param name="dir"></param>
        /// <param name="file"></param>
        /// <returns></returns>
        private bool CheckForFile(string dir, string file)
        {
            return Directory.GetFiles(dir, file).Length > 0 ? true : false;
        }

        /// <summary>
        /// Metoda pobiera nazwe eksportowanego pliku
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        private string GetExportFileName(string path)
        {
            return File.ReadLines(path).First() + ".txt";           
        }
    }
}
