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
        private string StandardURL;
        private List<Beer> beers;
        
        public JumboScraper()
        {
            beers = new List<Beer>();
            private string keyword = "bier";
        StandardURL = @"Data/jumbo.html";
        }

    async Task<List<string>> Scraper.getHTML()
    {
        var pages = new List<string>();

        using (var httpClient = new HttpClient())
        {


            for (var p = 0; p < 42; p++)
            {
                var response = await httpClient.GetAsync("https://www.jumbo.com/producten?PageNumber=" + p + "&SearchTerm=" + keyword);
                pages.Add(await response.Content.ReadAsStringAsync());
            }

            return pages;
        }
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
            return beers;
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