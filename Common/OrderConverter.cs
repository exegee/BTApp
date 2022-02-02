using BTApp.Events;
using BTApp.Helpers;
using BTApp.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;

namespace BTApp.Common
{
    public class OrderConverter
    {
        private string[] _splitter = { "\t" };
        private DebugMode _debugMode;
        private IOHelper _iOHelper;
        private OrderProcessingEventArgs orderProcessingEventArgs;

        public event EventHandler<OrderProcessingEventArgs> OrderProcessingFinished;

        /// <summary>
        /// Konstruktor
        /// </summary>
        public OrderConverter()
        {
            orderProcessingEventArgs = new OrderProcessingEventArgs();
            _debugMode = new DebugMode();
            _iOHelper = new IOHelper();
        }

        /// <summary>
        /// Metoda odczytuje i przetwarza plik wyslij.txt
        /// </summary>
        /// <param name="fullPath"></param>
        public void ProcessOrder(string fullPath)
        {
            _debugMode.ConsoleWriteLine("Processing order...");

            List<Order> orders = new List<Order>();
            string[] lines = File.ReadAllLines(fullPath);

            // Nowa nazwa pliku
            string newFileName = lines[0];
            // Id zlecenia
            string orderId = lines[0].Split('-')[3];
            // Temperatura SPS
            string[] lineNo2 = lines[1].Split(_splitter, StringSplitOptions.None);
            int heater;
            Int32.TryParse(lineNo2[1], out heater);
            // Całkowita długość zlecenia
            int totalLength;
            Int32.TryParse(lines[2], out totalLength);
            int orderSize = CheckOrderSize(lines);
            for(int i = orderSize; i>0; i--)
            {
                // Długość i ilość odcinków do wykonania
                string[] pcs_line = lines[lines.Length - i - 2].Split(_splitter, StringSplitOptions.None);
                int length;
                int quantity;
                Int32.TryParse(pcs_line[0], out length);
                Int32.TryParse(pcs_line[1], out quantity);
                orders.Add(new Order
                {
                    Id = orderId,
                    Length = length,
                    Quantity = quantity,
                    Heater = heater,
                    SumLength = totalLength,
                    ImportDate = DateTime.Now
                });
            }
            orderProcessingEventArgs.orders = orders;
            _iOHelper.SaveFile(fullPath, GetNewPath(newFileName)); // !!!!!!!!!!!!!!!!!!!
            OnOrderProcessingFinished();
        }

        /// <summary>
        /// Metoda sprawdza ilosc wymiarów w zleceniu
        /// </summary>
        /// <param name="lines"></param>
        /// <returns></returns>
        private int CheckOrderSize(string[] lines)
        {
            int orderLength;
            orderLength = lines.Length - 5;
            _debugMode.ConsoleWriteLine("Found ", orderLength, "order/s.");
            return orderLength;
        }

        /// <summary>
        /// Event zakończenia przetwarzania zlecenia
        /// </summary>
        public void OnOrderProcessingFinished()
        {
            _debugMode.ConsoleWriteLine("Processing order complete.");
            OrderProcessingFinished?.Invoke(this, orderProcessingEventArgs);
        }

        /// <summary>
        /// Metoda pobiera nową ścieżke zapisu
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        private string GetNewPath(string fileName)
        {
            // Pobranie wartości "exportPath" z ustawień aplikacji
            string exportPath = ConfigurationManager.AppSettings["exportPath"];

            _iOHelper.CheckIfDirectoryExist(exportPath);
            string newPath = string.Concat(exportPath,"\\" , fileName, ".txt");

            return newPath;
        }
    }
}
