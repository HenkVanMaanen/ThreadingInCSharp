using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using Microsoft.Data.Sqlite;

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
                                            "price int NOT NULL," +
                                            "volume int NOT NULL" +
                                            "priceNormalized int NOT NULL" +
                                            "discount string DEFAULT NULL" +
                                            "shop string NOT NULL" +
                                            "url string DEFAULT NULL" +
                                        "); ";

                SqliteCommand createTable = new SqliteCommand(tableCommand, db);

                createTable.ExecuteReader();

                db.Close();
            }

        }

        public bool store(Beer[] Beers)
        {

            try
            {
                StringBuilder sCommand = new StringBuilder("INSERT INTO beers (brand, price, volume, priceNormalized, discount, shop, url) VALUES ");

                using (SqliteConnection db = new SqliteConnection("Filename=" + databaseName))
                {
                    List<string> Rows = new List<string>();

                    foreach (Beer beer in Beers)
                    {
                        Rows.Add(string.Format("('{0}', '{1}'), '{2}'), '{3}'), '{4}'), '{5}'), '{6}')"),
                            MySqlHelper.EscapeString(beer.getBrand()),
                            MySqlHelper.EscapeString(beer.getPrice()),
                            MySqlHelper.EscapeString(beer.getVolume()),
                            MySqlHelper.EscapeString(beer.getNormalizedPrice()),
                            MySqlHelper.EscapeString(beer.getDiscount()),
                            MySqlHelper.EscapeString(beer.getShopName()),
                            MySqlHelper.EscapeString(beer.getUrl()));
                    }
                    sCommand.Append(string.Join(",", Rows));
                    sCommand.Append(";");


                    db.Open();

                    using (MySqlCommand myCmd = new MySqlCommand(sCommand.ToString(), db))
                    {
                        myCmd.CommandType = CommandType.Text;
                        myCmd.ExecuteNonQuery();
                    }

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

        }
    }
}