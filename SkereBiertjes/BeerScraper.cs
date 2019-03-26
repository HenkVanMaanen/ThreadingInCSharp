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

        public BeerScraper(DatabaseHandler databaseHandler)
        {
            this.databaseHandler = databaseHandler;
            this.beers = new List<Beer>();

            //set all scrapers
            this.scrapers = new List<Scraper>
            {
                new GallEnGallScraper(),
                new JumboScraper(),
                new PLUSScraper(),
                new AHScraper(),
                new CoopScraper(),
            };
        }

        public void startFindingFirstBeers()
        {
            List<Beer> beersDB;
            foreach (Scraper scraper in this.scrapers)
            {
                beersDB = scraper.parseHTML();
                this.beers = this.beers.Concat(beersDB).ToList();
                this.databaseHandler.store(beersDB);
            }
        }

        public List<Beer> getBeers()
        {
            Debug.WriteLine(this.beers.Count);
            return this.beers;
        }

        public List<Scraper> getScrapers()
        {
            return scrapers;
        }

        public List<Beer> search(string keyword, Filter filter) //Filters zijn merk, type, winkel, keyword; Filter is specifieker
        {
            List<Beer> b = new List<Beer>();
            List<Beer> filtered = new List<Beer>();
            List<Beer> beers = new List<Beer>();

            b = this.databaseHandler.get();

            IEnumerable<Beer> filterQuery = null;

            if (filter.getBrand() != "")
            {
                filterQuery = from element in b
                              where element.getBrand().Contains(filter.getBrand())
                              select element;
            }
            if (filter.getShop() != "")
            {
                filterQuery = from element in b
                              where element.getShopName().Contains(filter.getShop())
                              select element;
            }
            if (filter.getType() != "")
            {
                int amountOfBottles = 0;
                if(filter.getType() == "kratjes")
                {
                    amountOfBottles = 24;
                } else if(filter.getType() == "blikjes")
                {
                    amountOfBottles = 6;
                }
                else
                {
                    amountOfBottles = 1;
                }

                filterQuery = from element in b
                              where element.getBottleAmount().Equals(amountOfBottles)
                              select element;
            }

            if (filterQuery != null)
            {
                foreach (Beer beer in filterQuery)
                {
                    filtered.Add(beer);
                }
                b = filtered;
            }

            if (keyword != "")
            {
                IEnumerable<Beer> searchQuery = from element in b
                                                where element.getBrand().Contains(keyword)
                                                select element;

                foreach (Beer beer in searchQuery)
                {
                    beers.Add(beer);
                }
                b = beers;
            }
            return b;
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

    public class Filter
    {
        private string brand;
        private string shop;
        private string type;

        public Filter(string brand, string shop, string type)
        {
            this.brand = brand;
            this.shop = shop;
            this.type = type;
        }

        public void setBrand(string brand)
        {
            this.brand = brand;
        }

        public void setShop(string shop)
        {
            this.shop = shop;
        }

        public void setType(string type)
        {
            this.type = type;
        }

        public string getBrand()
        {
            return brand;
        }

        public string getShop()
        {
            return shop;
        }

        public string getType()
        {
            return type;
        }
    }
}