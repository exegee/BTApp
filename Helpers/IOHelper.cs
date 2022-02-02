using BTApp.Common;
using System;
using System.IO;

namespace BTApp.Helpers
{
    public class IOHelper
    {
        private DebugMode _debugMode;

        /// <summary>
        /// Konstruktor
        /// </summary>
        public IOHelper()
        {
            _debugMode = new DebugMode();
        }

        /// <summary>
        /// Metoda sprawdza czy folder istnieje, jeśli nie tworzy go
        /// </summary>
        /// <param name="directoryPath"></param>
        public void CheckIfDirectoryExist(string directoryPath)
        {
            try
            {
                if (Directory.Exists(directoryPath))
                {
                    _debugMode.ConsoleWriteLine("Directory exists: ", directoryPath);
                    return;
                }
                DirectoryInfo di = Directory.CreateDirectory(directoryPath);
                _debugMode.ConsoleWriteLine("Created directory: ", directoryPath);
            }
            catch (Exception e)
            {
                _debugMode.ConsoleWriteLine(e.Message);
            }
        }

        /// <summary>
        /// Metoda zapisuje plik pod nową nazwa i w nowej lokalizacji
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="path"></param>
        public void SaveFile(string oldPath, string newPath)
        {
            
            File.Move(oldPath, newPath);
            _debugMode.ConsoleWriteLine("Moved and renamed file to: ", newPath);
        }
    }
}
