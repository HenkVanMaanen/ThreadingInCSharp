using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using Microsoft.Data.Sqlite;
using MySql.Data.MySqlClient;

namespace SkereBiertjes
{
    class DatabaseHandler
    {
        string _databaseName;

        //create databasehandler
        public DatabaseHandler(string databaseName)
        {
            //set database name and run the setup
            this._databaseName = databaseName;
            this.setup();
        }

        public string databaseName { get => _databaseName; set => _databaseName = value; }

        //database setup.
        //create the table if it doenst exists yet.
        private void setup()
        {
            using (SqliteConnection db = new SqliteConnection("Filename=" + databaseName))
            {
                db.Open();

                //create create query
                String tableCommand = "CREATE TABLE IF NOT EXISTS " +
                                        "beers (" +
                                            "id INTEGER PRIMARY KEY AUTOINCREMENT, " +
                                            "brand NVARCHAR(50) NOT NULL," +
                                            "volume int NOT NULL," +
                                            "priceNormalized int NOT NULL," +
                                            "discount string DEFAULT NULL," +
                                            "shop string NOT NULL," +
                                            "url string DEFAULT NULL" +
                                        "); ";

                SqliteCommand createTable = new SqliteCommand(tableCommand, db);

                createTable.ExecuteReader();

                db.Close();
            }

        }
        
        //get all beers from database and return it as a List<Beer>
        public List<Beer> get()
        {
            List<Beer> beers = new List<Beer>();

            //open database
            using (SqliteConnection db = new SqliteConnection("Filename=" + databaseName))
            {
                db.Open();

                //create select query
                SqliteCommand selectCommand = new SqliteCommand("SELECT * from beers", db);

                //run select query
                SqliteDataReader query = selectCommand.ExecuteReader();
                
                while (query.Read())
                {
                    //add every single beer to the beers list
                    beers.Add(
                        new Beer(
                            query.GetString(1),
                            query.GetInt32(2),
                            query.GetInt32(3),
                            query.GetString(4),
                            query.GetString(5),
                            query.GetString(6)
                        )
                    );
                }

                db.Close();
            }

            //return list
            return beers;
        }

        //insert all beers that are in a list into the database
        public bool store(List<Beer> Beers)
        {
            try
            {
                //create start of the insert query
                StringBuilder sCommand = new StringBuilder("INSERT INTO beers (brand, volume, priceNormalized, discount, shop, url) VALUES ");

                using (SqliteConnection db = new SqliteConnection("Filename=" + databaseName))
                {
                    List<string> Rows = new List<string>();

                    foreach (Beer beer in Beers)
                    {
                        //add for every beer a new line to the insert query
                        Rows.Add(string.Format("('{0}', '{1}', '{2}', '{3}', '{4}', '{5}')",
                            MySqlHelper.EscapeString(beer.getBrand()),
                            MySqlHelper.EscapeString(beer.getVolume().ToString()),
                            MySqlHelper.EscapeString(beer.getNormalizedPrice().ToString()),
                            MySqlHelper.EscapeString(beer.getDiscount()),
                            MySqlHelper.EscapeString(beer.getShopName()),
                            MySqlHelper.EscapeString(beer.getUrl()))
                        );
                    }
                    sCommand.Append(string.Join(",", Rows));
                    sCommand.Append(";");

                    //open the db connection and run the query containing all information
                    db.Open();
                    SqliteCommand insertSql = new SqliteCommand(sCommand.ToString(), db);
                    insertSql.ExecuteReader();
                   
                    db.Close();
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine("Exception: " + e.Message);
            }

            return true;
        }
        
        //remove all information
        public void delete()
        {
            using (SqliteConnection db = new SqliteConnection("Filename=" + databaseName))
            {
                //create delete query
                SqliteCommand insertSql = new SqliteCommand("TRUNCATE TABLE beers", db);

                //run the delete query
                try
                {
                    db.Open();

                    insertSql.ExecuteReader();

                    db.Close();
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                }
            }
        }
    }
}