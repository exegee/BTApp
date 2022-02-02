using System;
using System.Configuration;
using System.Text;

namespace BTApp.Models
{
    public class Recipe
    {
        /// <summary>
        /// Ogólne parametry receptur
        /// </summary>
        // Id receptury
        private int _recipeId; 
        // Numer receptury
        public int RecordNumber { private set; get; }
        // Nazwa receptury w panelu GOT
        private string _recipeName;
        // Nazwa rekordu, np. zlecenie z zew. systemu
        public string RecordName { private set; get; }
        // Data dodania/modyfikacji
        private string _date;
        // Długość bazy receptur
        private int _recordMax;

        /// <summary>
        /// Parametry receptury definiowane przez użytkownika
        /// </summary>
        private int _sumLength;
        private int _length;
        private int _quantity;
        private int _heater;

        /// <summary>
        /// Konstruktor receptury
        /// </summary>
        /// <param name="recordNumber"></param>
        /// <param name="recordName"></param>
        /// <param name="sumLength"></param>
        /// <param name="length"></param>
        /// <param name="quantity"></param>
        /// <param name="heater"></param>
        public Recipe(int recordNumber, string recordName, int sumLength, int length, int quantity, int heater)
        {
            _recipeId = Convert.ToInt32(ConfigurationManager.AppSettings["gotRecipeID"]);
            RecordNumber = recordNumber;
            _recipeName = ConfigurationManager.AppSettings["gotRecipeName"];
            RecordName = recordName;
            _recordMax = Convert.ToInt32(ConfigurationManager.AppSettings["gotRecordsNum"]); ;
            _date = DateTime.Now.ToString("MM/dd/yyyy");
            _sumLength = sumLength;
            _length = length;
            _quantity = quantity;
            _heater = heater;
        }

        /// <summary>
        /// Metoda pobiera treść receptury
        /// </summary>
        /// <returns></returns>
        public string GetBody()
        {
            var recipeBody = new StringBuilder();
            recipeBody.AppendLine(":GT2K_RECIPE,0");
            recipeBody.AppendLine($":RECIPE_ID,{_recipeId}");
            recipeBody.AppendLine($":RECIPE_NAME,\"{_recipeName}\"");
            recipeBody.AppendLine(":DEVICE_NUM,5"); // ilość zmiennych w recepturze
            recipeBody.AppendLine($":RECORD_NUM,{_recordMax}");
            recipeBody.AppendLine(":DATE_ORDER,YYYY/MM/DD hh:mm:ss");
            recipeBody.AppendLine(":LOCAL_TIME,GMT 00:00");
            recipeBody.AppendLine(":TIME_INF_ORDER,");
            recipeBody.AppendLine($",DEV_COMMENT,DEV_TYPE,DISP_TYPE,DEV_SIZE,{RecordNumber}");
            recipeBody.AppendLine($":RECORD_NAME,,,,,\"{RecordName}\"");
            recipeBody.AppendLine(":RECORD_ATTR,,,,,");
            recipeBody.AppendLine($":UPDATE,,,,,{_date}"); // format 2000/01/01 00:00:00
            recipeBody.AppendLine($"1,,BIN32,DEC,2,\"{_length}\"");
            recipeBody.AppendLine($"2,,BIN16,DEC,1,\"{_quantity}\"");
            recipeBody.AppendLine($"3,,BIN16,DEC,1,\"{_heater}\"");
            recipeBody.AppendLine($"4,,BIN32,DEC,2,\"{_sumLength}\"");
            recipeBody.AppendLine($"5,,STRING,ASC,5,\"{_date}\""); //5,,STRING,ASC,5,"KK        "
            return recipeBody.ToString();
        }
    }
}
