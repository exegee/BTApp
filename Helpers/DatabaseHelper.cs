//using BTApp.Common;
//using SQLite;
//using System;
//using System.Collections.Generic;
//using System.IO;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace BTApp.Helpers
//{
//    public class DatabaseHelper
//    {
//        private static string dbBTAppFile = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "BTAppDB.db");

//        public static bool Insert<T>(T item)
//        {
//            bool result = false;
//            DebugMode.WriteErrorToLogFile(dbBTAppFile);

//            using(SQLiteConnection conn = new SQLiteConnection(dbBTAppFile))
//            {
//                conn.CreateTable<T>();
//                int rows = conn.Insert(item);
//                if(rows > 0)
//                {
//                    result = true;
//                }
//            }

//            return result;
//        }
        
//        public static bool Update<T>(T item)
//        {
//            bool result = false;
//            DebugMode.WriteErrorToLogFile(dbBTAppFile);
//            using (SQLiteConnection conn = new SQLiteConnection(dbBTAppFile))
//            {
//                conn.CreateTable<T> ();
//                int rows = conn.Update (item);
//                if( rows > 0)
//                {
//                    result = true;
//                }
//            }
//            return result;
//        }

//        public static bool Delete<T>(T item)
//        {
//            bool result = false;
//            DebugMode.WriteErrorToLogFile(dbBTAppFile);
//            using (SQLiteConnection conn = new SQLiteConnection(dbBTAppFile))
//            {
//                conn.CreateTable<T>();
//                int rows = conn.Delete(item);
//                if (rows > 0)
//                {
//                    result = true;
//                }
//            }
//            return result;
//        }

//        public static List<T>Read<T>() where T: new()
//        {
//            List<T> list = new List<T>();
//            DebugMode.WriteErrorToLogFile(dbBTAppFile);
//            try
//            {
//                using (SQLiteConnection conn = new SQLiteConnection(dbBTAppFile))
//                {
//                    conn.CreateTable<T>();
//                    list = conn.Table<T>().ToList();
//                }
//            }catch (Exception ex)
//            {
//                DebugMode.WriteErrorToLogFile("Read errors failed: " + ex.Message);
//            }


//            return list;
//        }

//        public static bool DeleteAll<T>()
//        {
//            bool result = false;
//            DebugMode.WriteErrorToLogFile(dbBTAppFile);
//            using (SQLiteConnection conn = new SQLiteConnection(dbBTAppFile))
//            {
//                conn.CreateTable<T>();
//                int rows = conn.DeleteAll<T>();
//                if (rows > 0)
//                {
//                    result = true;
//                }
//            }
//            return result;
//        }

//    }
//}
