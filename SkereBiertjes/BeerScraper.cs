using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using Windows.Storage;
using System.Threading.Tasks;

namespace SkereBiertjes
{
    public class BeerScraper
    {
        private ApplicationDataContainer localSettings = ApplicationData.Current.LocalSettings;
        private List<Scraper> scrapers;
        private DatabaseHandler databaseHandler;
        private int beersCount;
        private bool doneSearching;

        public BeerScraper(DatabaseHandler databaseHandler)
        {
            this.databaseHandler = databaseHandler;
            this.beersCount = 0;
            this.doneSearching = false;
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
            this.doneSearching = false;
            this.beersCount = 0;
            List<Beer> beersDB;
            foreach (Scraper scraper in this.scrapers)
            {
                beersDB = scraper.parseHTML();
                this.beersCount += beersDB.Count;
                this.databaseHandler.store(beersDB);
            }
            this.doneSearching = true;
        }

        public bool isDoneSearching()
        {
            return this.doneSearching;
        }

        public int getBeersCount()
        {
            return this.beersCount;
        }

        public List<Scraper> getScrapers()
        {
            return scrapers;
        }

        public List<Beer> search(string keyword, Filter filter) //Filters zijn merk, type, winkel, keyword; Filter is specifieker
        {
            while (!this.doneSearching)
            {
                Task.Delay(100);
            }

            List<Beer> beers = new List<Beer>();

            beers = this.databaseHandler.get();
            
            IDictionary<string, Object> typeFilter = new Dictionary<string, Object>();
            typeFilter["kratjes"] = 24;
            typeFilter["sixpack"] = 6;
            typeFilter["losse flesje"] = 1;
            
            if (localSettings.Values["multithreading_enabled"].ToString() == "True")
            {
                beers =    beers.Where(beer => filter.getBrand() == "" || beer.getBrand().ToLower().Contains(filter.getBrand().ToLower()))
                                .Where(beer => filter.getShop() == "" || beer.getShopName().ToLower().Contains(filter.getShop().ToLower()))
                                .Where(beer => filter.getType() == "" || beer.getBottleAmount().Equals(typeFilter[filter.getType().ToLower()]))
                                .Where(beer => keyword == "" || beer.getTitle().ToLower().Contains(keyword.ToLower()))
                                .OrderBy(beer => beer.getNormalizedPrice()).AsParallel().ToList();
            } else
            {
                beers = beers.Where(beer => filter.getBrand() == "" || beer.getBrand().ToLower().Contains(filter.getBrand().ToLower()))
                                .Where(beer => filter.getShop() == "" || beer.getShopName().ToLower().Contains(filter.getShop().ToLower()))
                                .Where(beer => filter.getType() == "" || beer.getBottleAmount().Equals(typeFilter[filter.getType().ToLower()]))
                                .Where(beer => keyword == "" || beer.getTitle().ToLower().Contains(keyword.ToLower()))
                                .OrderBy(beer => beer.getNormalizedPrice()).ToList();
            }

            return beers;
            
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