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
                filtered.Clear();
                filterQuery = from element in b
                              where element.getBrand().ToLower().Contains(filter.getBrand().ToLower())
                              select element;

                foreach (Beer beer in filterQuery)
                {
                    filtered.Add(beer);
                }
                b.Clear();
                b = filtered.ConvertAll(beer => new Beer(beer.getBrand(), beer.getTitle(), beer.getVolume(), beer.getBottleAmount(), beer.getNormalizedPrice(), beer.getDiscount(), beer.getShopName(), beer.getUrl()));
            }
            if (filter.getShop() != "")
            {
                filtered.Clear();
                filterQuery = from element in b
                              where element.getShopName().ToLower().Contains(filter.getShop().ToLower())
                              select element;

                foreach (Beer beer in filterQuery.ToList())
                {
                    filtered.Add(beer);
                }
                b.Clear();
                b = filtered.ConvertAll(beer => new Beer(beer.getBrand(), beer.getTitle(), beer.getVolume(), beer.getBottleAmount(), beer.getNormalizedPrice(), beer.getDiscount(), beer.getShopName(), beer.getUrl()));
            }
            if (filter.getType() != "")
            {
                filtered.Clear();
                int amountOfBottles = 0;
                if(filter.getType().ToLower() == "kratjes")
                {
                    amountOfBottles = 24;
                } else if(filter.getType().ToLower() == "sixpack")
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

                foreach (Beer beer in filterQuery)
                {
                    filtered.Add(beer);
                }
                b.Clear();
                b = filtered.ConvertAll(beer => new Beer(beer.getBrand(), beer.getTitle(), beer.getVolume(), beer.getBottleAmount(), beer.getNormalizedPrice(), beer.getDiscount(), beer.getShopName(), beer.getUrl()));
            }

            if (keyword != "")
            {
                IEnumerable<Beer> searchQuery = from element in b
                                                where element.getTitle().ToLower().Contains(keyword.ToLower())
                                                select element;

                foreach (Beer beer in searchQuery)
                {
                    beers.Add(beer);
                }
                b.Clear();
                b = beers.ConvertAll(beer => new Beer(beer.getBrand(), beer.getTitle(), beer.getVolume(), beer.getBottleAmount(), beer.getNormalizedPrice(), beer.getDiscount(), beer.getShopName(), beer.getUrl()));

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