using System;
using System.Configuration;
using System.IO;

namespace BTApp.Common
{
    public class DebugMode
    {
        private bool _debugMode = false;
        private static string logFile = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop),"Log.txt");//"C:\\Users\\WKS6\\Desktop\\Log.txt"; 

        /// <summary>
        /// Konstruktor
        /// </summary>
        public DebugMode()
        {
            // Pobranie wartości "debug" z ustawień aplikacji
            _debugMode = Convert.ToBoolean(ConfigurationManager.AppSettings["debug"]);
        }

        /// <summary>
        /// Metoda wyświetla wiadomość na konsoli jeśli debug=true
        /// </summary>
        /// <param name="message"></param>
        public void ConsoleWriteLine(string message)
        {
            if (_debugMode)
            {
                string text = GetTime() + message;
                Console.WriteLine(text);
                WriteLineToLogFile(text);
            }
        }
        /// <summary>
        /// Metoda wyświetla wiadomość na konsoli jeśli debug=true i condition=true
        /// </summary>
        /// <param name="message"></param>
        /// <param name="condition"></param>
        public void ConsoleWriteLine(string message1, int value, string message2)
        {
            if (_debugMode)
            {
                string text = GetTime() + $"{message1}: {value} {message2}";
                Console.WriteLine(text);
                WriteLineToLogFile(text);
            }
        }
        /// <summary>
        /// Metoda wyświetla dwie nastepujące stringi po sobie
        /// </summary>
        /// <param name="message1"></param>
        /// <param name="message2"></param>
        public void ConsoleWriteLine(string message1, string message2)
        {
            if (_debugMode)
            {
                string text = GetTime() + $"{message1} {message2}";
                Console.WriteLine(text);
                WriteLineToLogFile(text);
            }
        }
        /// <summary>
        /// Metoda wyświetla dwie nastepujące stringi po sobie
        /// </summary>
        /// <param name="message1"></param>
        /// <param name="message2"></param>
        /// <param name="message3"></param>
        public void ConsoleWriteLine(string message1, string message2, string message3)
        {
            if (_debugMode)
            {
                string text = GetTime() + $"{message1} {message2} {message3}";
                Console.WriteLine(text);
                WriteLineToLogFile(text);
            }
        }
        /// <summary>
        /// Metoda wyświetla dwie nastepujące stringi po sobie
        /// </summary>
        /// <param name="message1"></param>
        /// <param name="message2"></param>
        /// <param name="message3"></param>
        /// <param name="message4"></param>
        public void ConsoleWriteLine(string message1, string message2, string message3, string message4)
        {
            if (_debugMode)
            {
                string text = GetTime() + $"{message1} {message2} {message3} {message4}";
                Console.WriteLine(text);
                WriteLineToLogFile(text);
            }
        }
        /// <summary>
        /// Metoda wyświetla wiadomosc w konsoli
        /// </summary>
        /// <param name="message"></param>
        public void ConsoleWrite(string message)
        {
            if (_debugMode)
            {
                string text = GetTime() + $"{message}";
                Console.Write(text);
                WriteLineToLogFile(text);
            }
        }
        /// <summary>
        /// Metoda zwraca aktualny czas
        /// </summary>
        /// <returns></returns>
        private string GetTime()
        {
            return DateTime.Now.ToString() + ": ";
        }
        /// <summary>
        /// Metoda zapisuje logi konsoli do pliku Log.txt
        /// </summary>
        /// <param name="text"></param>
        public void WriteLineToLogFile(string text)
        {
            try
            {
                using (StreamWriter w = File.AppendText(logFile))
                {
                    w.WriteLine(text);
                }
            }
            catch (Exception e)
            {
                ConsoleWrite(e.Message);
            }
        }

        public static void WriteErrorToLogFile(string text)
        {
            try
            {
                using (StreamWriter w = File.AppendText(logFile))
                {
                    string txt = DateTime.Now.ToString() + ": " + text;
                    w.WriteLine(txt);
                }
            }
            catch (Exception e)
            {
                //ConsoleWrite(e.Message);
            }
        }
    }
}
