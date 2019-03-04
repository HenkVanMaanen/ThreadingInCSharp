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
            this._databaseName = databaseName;
            this.setup();
        }

        public string databaseName { get => _databaseName; set => _databaseName = value; }

        //check if database exists insert tables if it doesnt
        private void setup()
        {
            using (SqliteConnection db = new SqliteConnection("Filename=" + databaseName))
            {
                db.Open();

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
        
        public List<Beer> get()
        {
            List<Beer> beers = new List<Beer>();

            using (SqliteConnection db = new SqliteConnection("Filename=" + databaseName))
            {
                db.Open();

                SqliteCommand selectCommand = new SqliteCommand("SELECT * from beers", db);

                SqliteDataReader query = selectCommand.ExecuteReader();

                int idx = 0;
                while (query.Read())
                {
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
                    idx++;
                }

                db.Close();
            }

            return beers;
        }

        public bool store(List<Beer> Beers)
        {
           
            try
            {
                StringBuilder sCommand = new StringBuilder("INSERT INTO beers (brand, volume, priceNormalized, discount, shop, url) VALUES ");

                using (SqliteConnection db = new SqliteConnection("Filename=" + databaseName))
                {
                    List<string> Rows = new List<string>();

                    foreach (Beer beer in Beers)
                    {
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
        
        public void delete()
        {
            using (SqliteConnection db = new SqliteConnection("Filename=" + databaseName))
            {
                SqliteCommand insertSql = new SqliteCommand("DELETE FROM beers", db);

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