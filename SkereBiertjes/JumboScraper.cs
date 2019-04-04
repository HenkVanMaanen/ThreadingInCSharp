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
        private string keyword = "bier";
        private List<Beer> beers;

        public JumboScraper()
        {
            StandardURL = @"Data/jumbo.html";
            beers = new List<Beer>();
        }

        async Task<List<string>> getHTML()
        {
            var pages = new List<string>();

            using (var httpClient = new HttpClient())
            {


                for (var p = 0; p < 4; p++)
                {
                    var response = await httpClient.GetAsync("https://www.jumbo.com/producten?PageNumber=" + p + "&SearchTerm=" + keyword);
                    string c = await response.Content.ReadAsStringAsync();
                    pages.Add(c);
                   
                }

                return pages;
            }
        }

        async Task<List<Beer>> Scraper.parseHTML()
        {
            List<Beer> beers = new List<Beer>();
            var pages = await getHTML();

            foreach (string page in pages)
            {
                //get document
                var doc = new HtmlDocument();
                doc.OptionFixNestedTags = true;
                doc.LoadHtml(page);

                var nodes = doc.DocumentNode.SelectNodes("//div[contains(@class, 'jum-item-product')]");

                if (nodes == null)
                {
                    Debug.WriteLine("no nodes selected");
                    break;
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
            }
            return beers;
        }

        List<Beer> Scraper.getBeers()
        {
            return beers;
        }

        private Beer parseData(HtmlNode node)
        {
            string title = node.SelectSingleNode(".//a[contains(@data-jum-action, 'quickView')]").InnerHtml;
            string brand = this.parseToBrand(title);

            int volume;
            try
            {
                volume = Convert.ToInt32(node.SelectSingleNode(".//span[contains(@class, 'jum-pack-size')]").InnerHtml.Split(' ')[2]) * 10;
            } catch
            {
                volume = 0;
            }

            int bottleAmount;
            try
            {
                bottleAmount = Convert.ToInt32(node.SelectSingleNode(".//span[contains(@class, 'jum-pack-size')]").InnerHtml.Split(' ')[0]); ;
            } catch
            {
                bottleAmount = 0;
            }

            int priceNormalized;
            try
            {
                priceNormalized = Convert.ToInt32(node.SelectSingleNode(".//input[contains(@id, 'PriceInCents')]").Attributes["value"].Value);
            } catch
            {
                priceNormalized = 0;
            }

            string discount = this.getDiscount(node);
            string url = this.getURL(node);

            return CreateBeer(brand, title, volume, bottleAmount, priceNormalized, discount, url); ;
        }

        private string parseToBrand(string title)
        {
            List<string> brands = new List<string>{
                "Grolsch",
                "Heineken",
                "Amstel",
                "Bavaria",
                "Jupiler",
            };

            foreach (string brand in brands)
            {
                if (title.ToLower().Contains(brand.ToLower()))
                {
                    return brand;
                }
            }

            return "";
        }

        private string getURL(HtmlNode node)
        {
            if (node == null)
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
            if (node == null)
            {
                return "";
            }
            node = node.SelectSingleNode(".//div[contains(@class, 'jum-promotion')]");
            if (node == null)
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
        private Beer CreateBeer(string brand, string title, int volume, int bottleAmount, int priceNormalized, string discount, string url)
        {
            return new Beer(brand, title, volume, bottleAmount, priceNormalized, discount, "Jumbo", url);
        }
    }
}


