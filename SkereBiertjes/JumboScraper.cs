using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using HtmlAgilityPack;

using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

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

        async Task<List<string>> Scraper.getHTML()
        {
            throw new NotImplementedException();
        }

        private Beer parseData(HtmlNode node)
        {
            string brand = node.SelectSingleNode(".//a[contains(@data-jum-action, 'quickView')]").InnerHtml;
            int volume = Convert.ToInt32(node.SelectSingleNode(".//span[contains(@class, 'jum-pack-size')]").InnerHtml.Split(' ')[2]) * 10;
            int bottleAmount = Convert.ToInt32(node.SelectSingleNode(".//span[contains(@class, 'jum-pack-size')]").InnerHtml.Split(' ')[0]); ;
            int priceNormalized = Convert.ToInt32(node.SelectSingleNode(".//input[contains(@id, 'PriceInCents')]").Attributes["value"].Value);
            string discount = this.getDiscount(node);
            string url = this.getURL(node);
            
            return CreateBeer(brand, volume, bottleAmount, priceNormalized, discount, url); ;
        }

        private string getURL(HtmlNode node)
        {
            if(node == null)
            {
                return "";
            }
            node = node.SelectSingleNode(".//dd[contains(@class, 'jum-item-figure')]");
            if (node == null)
            {
                return "";
            }
            node = node.SelectSingleNode(".//img");
            if (node == null)
            {
                return "";
            }
            return node.Attributes["data-jum-src"].Value.Replace("&#47;", "/");
        }

        private string getDiscount(HtmlNode node)
        {
            if(node == null)
            {
                return "";
            }
            node = node.SelectSingleNode(".//div[contains(@class, 'jum-promotion')]");
            if(node == null)
            {
                return "";
            }
            node = node.SelectSingleNode(".//img");
            if (node == null)
            {
                return "";
            }
            return node.Attributes["alt"].Value;
        }

        //create a beer with jumbo allready in it;
        private Beer CreateBeer(string brand, int volume, int bottleAmount, int priceNormalized, string discount, string url)
        {
            return new Beer(brand, volume, bottleAmount, priceNormalized, discount, "Jumbo", url);
        }
    }
}