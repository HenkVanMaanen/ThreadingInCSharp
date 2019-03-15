﻿using System;
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
            scrapers.Add(new GallEnGallScraper());
            //scrapers.Add(new JumboScraper());

            foreach (Scraper scraper in this.scrapers)
            {
                this.databaseHandler.store(scraper.parseHTML());
            }
        }

        public Scraper[] getScrapers()
        {
            throw new System.NotImplementedException();
        }

        public List<Beer> search(string keyword, Filter filter) //Filters zijn merk, type, winkel, keyword; Filter is specifieker
        {
            List<Beer> b = new List<Beer>();
            b = this.databaseHandler.get();
            
            if (filter.getBrand() != "")
            {
                IEnumerable<Beer> filterQuery = from element in b
                                                where element.getBrand().Contains(filter.getBrand())
                                                select element;
            }
            if (filter.getShop() != "")
            {
                IEnumerable<Beer> filterQuery = from element in b
                                                where element.getShopName().Contains(filter.getShop())
                                                select element;
            }
            if (filter.getType() != "")
            {
                IEnumerable<Beer> filterQuery = from element in b
                                                where element.getShopName().Contains(filter.getType())
                                                select element;
            }


            IEnumerable<Beer> searchQuery = from element in b
                                          where element.getBrand().Contains(keyword)
                                          select element;

            List<Beer> beers = new List<Beer>();
            foreach (Beer beer in searchQuery)
            {
                beers.Add(beer);
                Debug.WriteLine("Bier is: " + beer.getBrand());
            }
            return beers;
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