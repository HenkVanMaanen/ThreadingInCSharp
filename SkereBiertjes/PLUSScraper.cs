using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HtmlAgilityPack;
using System.Diagnostics;
using System.Text.RegularExpressions;

using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace SkereBiertjes
{
    public class PLUSScraper : Scraper
    {
        public string StandardURL;

        public PLUSScraper()
        {
            StandardURL = @"Data/plus.html";
        }

        public List<Beer> parseHTML()
        {
            List<Beer> beers = new List<Beer>();
            //get document
            var doc = new HtmlDocument();
            doc.OptionFixNestedTags = true;
            doc.Load(StandardURL);

            var nodes = doc.DocumentNode.SelectNodes("//div[contains(@class, 'product-list-block')]");

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
            HtmlNode infoNode = node.SelectSingleNode(".//div[contains(@class, 'product-tile__info')]");
            
            string brand = infoNode.Attributes["data-name"].Value;
            int bottleAmount = this.parseToAmount(brand);
            int priceNormalized = Convert.ToInt32(Math.Round(Convert.ToDouble(infoNode.Attributes["data-price"].Value) * 100));
            string data = node.SelectSingleNode(".//span[contains(@class, 'product-tile__quantity')]").InnerHtml;
            int totalVolume = Convert.ToInt32(Regex.Match(data, @"\d+").Value);
            int volume = totalVolume / bottleAmount;
            string discount = "";
            string url = "";
            return CreateBeer(brand, volume, bottleAmount, priceNormalized, discount, url);
        }

        private int parseToAmount(string title)
        {
            if (title == null)
            {
                return 1;
            }
            int[] size = new int[]{
                6,
                12,
                24
            };

            for(int idx = 0; idx < size.Length; idx++)
            {
                if (title.Contains(size[idx].ToString()))
                {
                    return size[idx];
                }
            }


            return 1;
        }
        //create a beer with jumbo allready in it;
        private Beer CreateBeer(string brand, int volume, int bottleAmount, int priceNormalized, string discount, string url)
        {
            return new Beer(brand, volume, bottleAmount, priceNormalized, discount, "PLUS", url);
        }
    }
}