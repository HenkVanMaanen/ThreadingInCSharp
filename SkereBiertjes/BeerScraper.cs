using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace SkereBiertjes
{
    public class BeerScraper
    {
        private List<Scraper> scrapers;
        private DatabaseHandler databaseHandler;
        private List<Beer> beers;

        public BeerScraper()
        {
            scrapers = new List<Scraper>
            {
                new AHScraper(),
                new CoopScraper(),
            };

            //create the database handler
            this.databaseHandler = new DatabaseHandler("SkereBiertjesV1.db");

            //create fake data
            List<Beer> beers = new List<Beer> {
                new Beer("Grolsch", 330,  799, "", "Jumbo", "http://pils.com"),
                new Beer("Grolsch", 330,  800, "", "Jumbo", "http://pils.com"),
                new Beer("Grolsch", 330,  799, "", "Jumbo", "http://pils.com"),
                new Beer("Grolsch", 330,  800, "", "Jumbo", "http://pils.com"),
                new Beer("Grolsch", 330,  799, "", "Jumbo", "http://pils.com"),
                new Beer("Grolsch", 330,  800, "", "Jumbo", "http://pils.com"),
                new Beer("Grolsch", 330,  799, "", "Jumbo", "http://pils.com"),
                new Beer("Grolsch", 330,  800, "", "Jumbo", "http://pils.com"),
                new Beer("Grolsch", 330,  799, "", "Jumbo", "http://pils.com"),
                new Beer("Grolsch", 330,  800, "", "Jumbo", "http://pils.com"),
                new Beer("Grolsch", 330,  799, "", "Jumbo", "http://pils.com"),
                new Beer("Grolsch", 330,  800, "", "Jumbo", "http://pils.com"),
                new Beer("Grolsch", 330,  799, "", "Jumbo", "http://pils.com"),
                new Beer("Grolsch", 330,  800, "", "Jumbo", "http://pils.com"),
                new Beer("Grolsch", 330,  799, "", "Jumbo", "http://pils.com"),
                new Beer("Grolsch", 330,  800, "", "Jumbo", "http://pils.com"),
                new Beer("Grolsch", 330,  799, "", "Jumbo", "http://pils.com"),
                new Beer("Grolsch", 330,  800, "", "Jumbo", "http://pils.com"),
                new Beer("Grolsch", 330,  799, "", "Jumbo", "http://pils.com"),
                new Beer("Grolsch", 330,  800, "", "Jumbo", "http://pils.com"),
                new Beer("Grolsch", 330,  799, "", "Jumbo", "http://pils.com"),
                new Beer("Grolsch", 330,  800, "", "Jumbo", "http://pils.com"),
                new Beer("Grolsch", 330,  799, "", "Jumbo", "http://pils.com"),
                new Beer("Grolsch", 330,  800, "", "Jumbo", "http://pils.com"),
                new Beer("Grolsch", 330,  799, "", "Jumbo", "http://pils.com"),
                new Beer("Grolsch", 330,  800, "", "Jumbo", "http://pils.com"),
                new Beer("Grolsch", 330,  799, "", "Jumbo", "http://pils.com"),
                new Beer("Grolsch", 330,  800, "", "Jumbo", "http://pils.com"),
                new Beer("Grolsch", 330,  799, "", "Jumbo", "http://pils.com"),
                new Beer("Grolsch", 330,  800, "", "Jumbo", "http://pils.com"),
                new Beer("Grolsch", 330,  799, "", "Jumbo", "http://pils.com"),
                new Beer("Grolsch", 330,  800, "", "Jumbo", "http://pils.com"),
                new Beer("Grolsch", 330,  799, "", "Jumbo", "http://pils.com"),
                new Beer("Grolsch", 330,  800, "", "Jumbo", "http://pils.com"),
                new Beer("Grolsch", 330,  799, "", "Jumbo", "http://pils.com"),
                new Beer("Grolsch", 330,  800, "", "Jumbo", "http://pils.com"),
                new Beer("Grolsch", 330,  799, "", "Jumbo", "http://pils.com"),
                new Beer("Grolsch", 330,  800, "", "Jumbo", "http://pils.com"),
                new Beer("Grolsch", 330,  799, "", "Jumbo", "http://pils.com"),
                new Beer("Grolsch", 330,  800, "", "Jumbo", "http://pils.com"),
                new Beer("Grolsch", 330,  799, "", "Jumbo", "http://pils.com"),
                new Beer("Grolsch", 330,  800, "", "Jumbo", "http://pils.com"),
                new Beer("Grolsch", 330,  799, "", "Jumbo", "http://pils.com"),
                new Beer("Grolsch", 330,  800, "", "Jumbo", "http://pils.com"),
                new Beer("Grolsch", 330,  799, "", "Jumbo", "http://pils.com"),
                new Beer("Grolsch", 330,  800, "", "Jumbo", "http://pils.com"),
                new Beer("Grolsch", 330,  799, "", "Jumbo", "http://pils.com"),
                new Beer("Grolsch", 330,  800, "", "Jumbo", "http://pils.com"),
                new Beer("Grolsch", 330,  799, "", "Jumbo", "http://pils.com"),
                new Beer("Grolsch", 330,  800, "", "Jumbo", "http://pils.com"),
                new Beer("Grolsch", 330,  799, "", "Jumbo", "http://pils.com"),
                new Beer("Grolsch", 330,  800, "", "Jumbo", "http://pils.com"),
                new Beer("Grolsch", 330,  799, "", "Jumbo", "http://pils.com"),
                new Beer("Grolsch", 330,  800, "", "Jumbo", "http://pils.com"),
                new Beer("Grolsch", 330,  799, "", "Jumbo", "http://pils.com"),
                new Beer("Grolsch", 330,  800, "", "Jumbo", "http://pils.com"),
                new Beer("Grolsch", 330,  799, "", "Jumbo", "http://pils.com"),
                new Beer("Grolsch", 330,  800, "", "Jumbo", "http://pils.com"),
                new Beer("Grolsch", 330,  799, "", "Jumbo", "http://pils.com"),
                new Beer("Grolsch", 330,  800, "", "Jumbo", "http://pils.com"),
                new Beer("Grolsch", 330,  799, "", "Jumbo", "http://pils.com"),
                new Beer("Grolsch", 330,  800, "", "Jumbo", "http://pils.com"),
                new Beer("Grolsch", 330,  799, "", "Jumbo", "http://pils.com"),
                new Beer("Grolsch", 330,  800, "", "Jumbo", "http://pils.com"),
                new Beer("Grolsch", 330,  799, "", "Jumbo", "http://pils.com"),
                new Beer("Grolsch", 330,  800, "", "Jumbo", "http://pils.com"),
                new Beer("Grolsch", 330,  799, "", "Jumbo", "http://pils.com"),
                new Beer("Grolsch", 330,  800, "", "Jumbo", "http://pils.com"),
                new Beer("Grolsch", 330,  799, "", "Jumbo", "http://pils.com"),
                new Beer("Grolsch", 330,  800, "", "Jumbo", "http://pils.com"),
                new Beer("Grolsch", 330,  799, "", "Jumbo", "http://pils.com"),
                new Beer("Grolsch", 330,  800, "", "Jumbo", "http://pils.com"),
                new Beer("Grolsch", 330,  799, "", "Jumbo", "http://pils.com"),
                new Beer("Grolsch", 330,  800, "", "Jumbo", "http://pils.com"),
                new Beer("Grolsch", 330,  799, "", "Jumbo", "http://pils.com"),
                new Beer("Grolsch", 330,  800, "", "Jumbo", "http://pils.com"),
                new Beer("Grolsch", 330,  799, "", "Jumbo", "http://pils.com"),
                new Beer("Grolsch", 330,  800, "", "Jumbo", "http://pils.com"),
                new Beer("Grolsch", 330,  799, "", "Jumbo", "http://pils.com"),
                new Beer("Grolsch", 330,  800, "", "Jumbo", "http://pils.com"),
                new Beer("Grolsch", 330,  799, "", "Jumbo", "http://pils.com"),
                new Beer("Grolsch", 330,  800, "", "Jumbo", "http://pils.com"),
                new Beer("Grolsch", 330,  799, "", "Jumbo", "http://pils.com"),
                new Beer("Grolsch", 330,  800, "", "Jumbo", "http://pils.com"),
                new Beer("Grolsch", 330,  799, "", "Jumbo", "http://pils.com"),
                new Beer("Grolsch", 330,  800, "", "Jumbo", "http://pils.com"),
                new Beer("Grolsch", 330,  799, "", "Jumbo", "http://pils.com"),
                new Beer("Grolsch", 330,  800, "", "Jumbo", "http://pils.com"),
                new Beer("Grolsch", 330,  799, "", "Jumbo", "http://pils.com"),
                new Beer("Grolsch", 330,  800, "", "Jumbo", "http://pils.com"),
                new Beer("Grolsch", 330,  799, "", "Jumbo", "http://pils.com"),
                new Beer("Grolsch", 330,  800, "", "Jumbo", "http://pils.com"),
                new Beer("Grolsch", 330,  799, "", "Jumbo", "http://pils.com"),
                new Beer("Grolsch", 330,  800, "", "Jumbo", "http://pils.com"),
                new Beer("Grolsch", 330,  799, "", "Jumbo", "http://pils.com"),
                new Beer("Grolsch", 330,  800, "", "Jumbo", "http://pils.com"),
                new Beer("Grolsch", 330,  799, "", "Jumbo", "http://pils.com"),
                new Beer("Grolsch", 330,  800, "", "Jumbo", "http://pils.com"),
                new Beer("Grolsch", 330,  799, "", "Jumbo", "http://pils.com"),
                new Beer("Grolsch", 330,  800, "", "Jumbo", "http://pils.com"),
                new Beer("Grolsch", 330,  799, "", "Jumbo", "http://pils.com"),
                new Beer("Grolsch", 330,  800, "", "Jumbo", "http://pils.com"),
                new Beer("Grolsch", 330,  799, "", "Jumbo", "http://pils.com"),
                new Beer("Grolsch", 330,  800, "", "Jumbo", "http://pils.com"),
                new Beer("Grolsch", 330,  799, "", "Jumbo", "http://pils.com"),
                new Beer("Grolsch", 330,  800, "", "Jumbo", "http://pils.com"),
                new Beer("Grolsch", 330,  799, "", "Jumbo", "http://pils.com"),
                new Beer("Grolsch", 330,  800, "", "Jumbo", "http://pils.com"),
                new Beer("Grolsch", 330,  799, "", "Jumbo", "http://pils.com"),
                new Beer("Grolsch", 330,  800, "", "Jumbo", "http://pils.com"),
                new Beer("Grolsch", 330,  799, "", "Jumbo", "http://pils.com"),
                new Beer("Grolsch", 330,  800, "", "Jumbo", "http://pils.com"),
                new Beer("Grolsch", 330,  799, "", "Jumbo", "http://pils.com"),
                new Beer("Grolsch", 330,  800, "", "Jumbo", "http://pils.com"),
                new Beer("Grolsch", 330,  799, "", "Jumbo", "http://pils.com"),
                new Beer("Grolsch", 330,  800, "", "Jumbo", "http://pils.com"),
                new Beer("Grolsch", 330,  799, "", "Jumbo", "http://pils.com"),
                new Beer("Grolsch", 330,  800, "", "Jumbo", "http://pils.com"),
                new Beer("Grolsch", 330,  799, "", "Jumbo", "http://pils.com"),
                new Beer("Grolsch", 330,  800, "", "Jumbo", "http://pils.com"),
                new Beer("Grolsch", 330,  799, "", "Jumbo", "http://pils.com"),
                new Beer("Grolsch", 330,  800, "", "Jumbo", "http://pils.com"),
                new Beer("Grolsch", 330,  799, "", "Jumbo", "http://pils.com"),
                new Beer("Grolsch", 330,  800, "", "Jumbo", "http://pils.com"),
                new Beer("Grolsch", 330,  799, "", "Jumbo", "http://pils.com"),
                new Beer("Grolsch", 330,  800, "", "Jumbo", "http://pils.com"),
                new Beer("Grolsch", 330,  799, "", "Jumbo", "http://pils.com"),
                new Beer("Grolsch", 330,  800, "", "Jumbo", "http://pils.com"),
                new Beer("Grolsch", 330,  799, "", "Jumbo", "http://pils.com"),
                new Beer("Grolsch", 330,  800, "", "Jumbo", "http://pils.com"),
                new Beer("Grolsch", 330,  799, "", "Jumbo", "http://pils.com"),
                new Beer("Grolsch", 330,  800, "", "Jumbo", "http://pils.com"),
                new Beer("Grolsch", 330,  799, "", "Jumbo", "http://pils.com"),
                new Beer("Grolsch", 330,  800, "", "Jumbo", "http://pils.com"),
                new Beer("Grolsch", 330,  799, "", "Jumbo", "http://pils.com"),
                new Beer("Grolsch", 330,  800, "", "Jumbo", "http://pils.com"),
                new Beer("Grolsch", 330,  799, "", "Jumbo", "http://pils.com"),
                new Beer("Grolsch", 330,  800, "", "Jumbo", "http://pils.com"),
                new Beer("Grolsch", 330,  799, "", "Jumbo", "http://pils.com"),
                new Beer("Grolsch", 330,  800, "", "Jumbo", "http://pils.com"),
                new Beer("Grolsch", 330,  799, "", "Jumbo", "http://pils.com"),
                new Beer("Grolsch", 330,  800, "", "Jumbo", "http://pils.com"),
                new Beer("Grolsch", 330,  799, "", "Jumbo", "http://pils.com"),
                new Beer("Grolsch", 330,  800, "", "Jumbo", "http://pils.com"),
                new Beer("Grolsch", 330,  799, "", "Jumbo", "http://pils.com"),
                new Beer("Grolsch", 330,  800, "", "Jumbo", "http://pils.com"),
                new Beer("Grolsch", 330,  799, "", "Jumbo", "http://pils.com"),
                new Beer("Grolsch", 330,  800, "", "Jumbo", "http://pils.com"),
                new Beer("Grolsch", 330,  799, "", "Jumbo", "http://pils.com"),
                new Beer("Grolsch", 330,  800, "", "Jumbo", "http://pils.com"),
            };

            //add the fake data to the database.
            this.databaseHandler.store(beers);

            //get all data from the database.
            this.beers = this.databaseHandler.get();

            this.getData();
        }

        public Scraper Scraper
        {
            get => default(Scraper);
            set
            {
            }
        }

        public Scraper[] getScrapers()
        {
            throw new System.NotImplementedException();
        }

        public Beer[] search(string keyword, System.String filters)
        {
            throw new System.NotImplementedException();
        }

        async public void getData()
        {
            foreach (Scraper scraper in scrapers) {
                var w = System.Diagnostics.Stopwatch.StartNew();
                List<string> pages = await scraper.getHTML();
                w.Stop();
                Debug.WriteLine(w.ElapsedMilliseconds);
            }
        }

        public int timeToScrape()
        {
            throw new System.NotImplementedException();
        }
    }
}