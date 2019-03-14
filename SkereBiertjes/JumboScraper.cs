using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using HtmlAgilityPack;

namespace SkereBiertjes
{
    public class JumboScraper : Scraper
    {
        public string StandardURL;
        
        public JumboScraper()
        {
            StandardURL = @"Data/jumbo.html";
        }

        public List<Beer> parseHTML()
        {
            List<Beer> beers = new List<Beer>();
            //get document
            var doc = new HtmlDocument();
            doc.OptionFixNestedTags = true;
            doc.Load(StandardURL);

            var nodes = doc.DocumentNode.SelectNodes("//div[contains(@class, 'jum-item-product')]");

            if (nodes == null)
            {
                Debug.WriteLine("no nodes selected");
                return null;
            }

            foreach (var node in nodes)
            {
                if (node != null)
                {
                    Beer beer = this.parseData(node);

                    beers.Add(beer);

                    beer.printInfo();
                }
            }

            return beers;
        }

        List<Beer> Scraper.getBeers()
        {
            throw new NotImplementedException();
        }

        string Scraper.getHTML()
        {
            throw new NotImplementedException();
        }

        private Beer parseData(HtmlNode node)
        {
            string brand = node.SelectSingleNode(".//a[contains(@data-jum-action, 'quickView')]").InnerHtml;
            int volume = node.SelectSingleNode(".//span[contains(@class, 'jum-pack-size')]").InnerHtml;
            int priceNormalized = 0;
            string discount = "";
            string url = "";
            
            return CreateBeer(brand, volume, priceNormalized, discount, url); ;
        }

        //create a beer with jumbo allready in it;
        private Beer CreateBeer(string brand, int volume, int priceNormalized, string discount, string url)
        {
            return new Beer(brand, volume, priceNormalized, discount, "Jumbo", url);
        }
    }
}