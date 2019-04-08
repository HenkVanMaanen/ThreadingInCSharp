using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using Windows.Storage;
using System.Threading;
using System.Threading.Tasks;
using Windows.Foundation;

namespace SkereBiertjes
{
    public class BeerScraper
    {
        private ApplicationDataContainer localSettings = ApplicationData.Current.LocalSettings;
        private List<Scraper> scrapers;
        private DatabaseHandler databaseHandler;
        private int beersCount;
        private bool doneSearching;
        private List<string> benchmarkData;

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

        //set list where data can be push on for benchmark 
        public void setBenchmarkData(List<string> data)
        {
            this.benchmarkData = data;
        }

        //start scraping the internet 
        public async void startFindingFirstBeers(bool benchmark)
        {
            this.doneSearching = false;
            this.beersCount = 0;
            var scraperFinishedCount = 0;
            Mutex mut = new Mutex();

            //loops over every scraper and will start parseing every scraper
            foreach (Scraper scraper in this.scrapers)
            {
                if (benchmark)
                {
                    scraper.setBenchmark(benchmark);
                    scraper.setBenchmarkData(this.benchmarkData);
                } else
                {
                    scraper.setBenchmarkData(new List<string>());
                }

                var workToDo = new WaitCallback(async o =>
                {
                    List<Beer> beersDB = new List<Beer>();
                    beersDB = await scraper.parseHTML();
                    this.beersCount += beersDB.Count;
                    this.databaseHandler.store(beersDB);

                    mut.WaitOne();
                    scraperFinishedCount++;
                    mut.ReleaseMutex();
                });

                ThreadPool.QueueUserWorkItem(workToDo);
            }
            
            // Wait for scrapers to finish otherwise gui shows zero results
            while (scraperFinishedCount != scrapers.Count)
            {
                await Task.Delay(300);
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

        public List<Beer> search(string keyword, Filter filter)
        {
            //wait untill the startFindingFirstBeers is done
            while (!this.doneSearching)
            {
                Task.Delay(100);
            }

            List<Beer> beers = new List<Beer>();

            //get data from database
            beers = this.databaseHandler.get();
            
            //insert typefilter in a list so we can compare numbers instead of text.
            IDictionary<string, Object> typeFilter = new Dictionary<string, Object>();
            typeFilter["kratjes"] = 24;
            typeFilter["sixpack"] = 6;
            typeFilter["losse flesje"] = 1;

            //if multithread is on us PLINQ else use LINQ
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
    }

    //filter class, this class will be used for filtering current dataset
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