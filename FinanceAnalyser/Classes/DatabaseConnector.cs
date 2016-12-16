using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;

namespace FinanceAnalyser.Classes
{
    public class DatabaseConnector
    {
        public static SQLiteConnection InitializeDatabase()
        {
            //Establish connection to database file
            SQLiteConnection dbConnection;

            string path = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

            dbConnection = new SQLiteConnection("Data Source=" + path + "\\FADataBase.db");
            dbConnection.Open();            

            //Create a 'MatchedCategories' table is one does not already exist
            string sqlSelect = "create table if not exists MatchedCategories (Description string, Category string)";
            SQLiteCommand commandSelect = new SQLiteCommand(sqlSelect, dbConnection);

            //To be deleted
            var tableExists = commandSelect.ExecuteNonQuery();

            return dbConnection;
        }

        public static Dictionary<string, string> LoadSavedCategories(SQLiteConnection dbConnection)
        {
            Dictionary<string, string> matchedCategories = new Dictionary<string, string>();

            string sqlSelect = "select * from MatchedCategories";
            SQLiteCommand commandSelect = new SQLiteCommand(sqlSelect, dbConnection);
            SQLiteDataReader reader = commandSelect.ExecuteReader();
            while (reader.Read())
            {
                string description = reader["Description"].ToString();
                string category = reader["Category"].ToString();

                matchedCategories.Add(description, category);
            }

            return matchedCategories;
        }

        public static void SaveCategories(Dictionary<string, string> matchedCategories, SQLiteConnection dbConnection)
        {
            // Add each matched category into the database
            foreach(var category in matchedCategories)
            {
                string lineDesc = category.Key;
                string lineCategory = category.Value;

                string sqlInsert = "insert into MatchedCategories (Description, Category) values ('" + lineDesc + "', '" + lineCategory + "')";
                SQLiteCommand commandInsert = new SQLiteCommand(sqlInsert, dbConnection);
                commandInsert.ExecuteNonQuery();
            }
        }
    }
}
