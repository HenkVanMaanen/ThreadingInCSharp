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
            //create the database handler
            this.databaseHandler = new DatabaseHandler("SkereBiertjesV2.db");
            this.scrapers = new List<Scraper>();
            this.databaseHandler.delete();
            //create fake data
            List<Beer> beers = new List<Beer> {
                new Beer("Grolsch", 330, 5,  800, "", "Jumbo", "http://pils.com"),
                new Beer("Grolsch", 330, 5,  799, "", "Jumbo", "http://pils.com"),
            };

            
        }

        public void start()
        {
            //add the fake data to the database.
            //this.databaseHandler.store(beers);

            //get all data from the database.
            //this.beers = this.databaseHandler.get();
            //scrapers.Add(new GallEnGallScraper());
            //scrapers.Add(new JumboScraper());
            scrapers.Add(new PLUSScraper());

            foreach (Scraper scraper in this.scrapers)
            {
                this.databaseHandler.store(scraper.parseHTML());
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

        public void getData()
        {
            throw new System.NotImplementedException();
        }

        public int timeToScrape()
        {
            throw new System.NotImplementedException();
        }
    }
}