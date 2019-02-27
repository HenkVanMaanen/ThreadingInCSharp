using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SkereBiertjes
{
    public class BeerScraper
    {
        private List<Scraper> scrapers;
        private DatabaseHandler databaseHandler;

        public Scraper Scraper
        {
            get => default(Scraper);
            set
            {
            }
        }

        public DatabaseHandler DatabaseHandler
        {
            get => default(DatabaseHandler);
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