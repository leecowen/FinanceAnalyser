using System;
using System.Collections.Generic;
using System.Data.SQLite;

namespace FinanceAnalyser
{
    public class DatabaseConnector
    {
        public static SQLiteConnection InitializeDatabase()
        {
            //Location of the database
            string path = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

            //Establish connection to database file
            SQLiteConnection dbConnection;
            dbConnection = new SQLiteConnection("Data Source=" + path + "\\FADataBase.db");
            dbConnection.Open();            

            //Create a 'MatchedCategories' table is one does not already exist
            string sqlSelect = "create table if not exists MatchedCategories (Description string, Category string)";
            SQLiteCommand commandSelect = new SQLiteCommand(sqlSelect, dbConnection);

            return dbConnection;
        }

        /// <summary>
        /// Take the connection to the database and run SQL queries to pull all data from the 'MatchedCategories' table
        /// Reads all rows into 'matchedCategories' Dictionary
        /// </summary>
        /// <param name="dbConnection"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Takes 'newMatchedCategories' and the database connection to write new rows containing the new Description and Category
        /// </summary>
        /// <param name="newMatchedCategories"></param>
        /// <param name="dbConnection"></param>
        public static void SaveCategories(Dictionary<string, string> newMatchedCategories, SQLiteConnection dbConnection)
        {
            // Add each matched category into the database
            foreach(var category in newMatchedCategories)
            {
                string lineDescription = category.Key;
                string lineCategory = category.Value;

                string sqlInsert = "insert into MatchedCategories (Description, Category) values ('" + lineDescription + "', '" + lineCategory + "')";
                SQLiteCommand commandInsert = new SQLiteCommand(sqlInsert, dbConnection);
                commandInsert.ExecuteNonQuery();
            }
        }
    }
}
