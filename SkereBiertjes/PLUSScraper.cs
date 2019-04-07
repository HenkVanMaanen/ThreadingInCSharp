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
        private string keyword = "bier";
        private List<Beer> beers;

        public PLUSScraper()
        {
            StandardURL = @"Data/plus.html";
            beers = new List<Beer>();
        }

        async Task<List<Beer>> Scraper.parseHTML()
        {
            Debug.WriteLine("Starting PLUS");
            List<Beer> beers = new List<Beer>();
            var pages = await getHTML();

            foreach (string page in pages)
            {
                //get document
                var doc = new HtmlDocument();
                doc.OptionFixNestedTags = true;
                doc.LoadHtml(page);

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
            }
            return beers;
        }

        List<Beer> Scraper.getBeers()
        {
            throw new NotImplementedException();
        }

        async Task<List<string>> getHTML()
        {
            var pages = new List<string>();

            using (var httpClient = new HttpClient())
            {

                var response = await httpClient.GetAsync("https://www.plus.nl/INTERSHOP/web/WFS/PLUS-website-Site/nl_NL/-/EUR/ViewTWSearch-ProductPaging?PageNumber=1&PageSize=500&SortingAttribute=&tn_cid=333333-10000&tn_q=" + keyword +  "&tn_sort=Sorteeroptie%2520Zoeken&SelectedTabName=solrTabs1");
                pages.Add(await response.Content.ReadAsStringAsync());

                return pages;
            }
        }

        private Beer parseData(HtmlNode node)
        {
            HtmlNode infoNode = node.SelectSingleNode(".//div[contains(@class, 'product-tile__info')]");
            
            string title = infoNode.Attributes["data-name"].Value;
            string brand = this.parseToBrand(title);
            int bottleAmount = this.parseToAmount(title);

            int priceNormalized;
            try
            {
                priceNormalized = this.getPrice(infoNode);
            } catch
            {
                priceNormalized = 0;
            }

            string data = node.SelectSingleNode(".//span[contains(@class, 'product-tile__quantity')]").InnerHtml;
            int totalVolume = Convert.ToInt32(Regex.Match(data, @"\d+").Value);
            int volume = totalVolume / bottleAmount;
            string discount = this.getDiscount(infoNode);
            string url = "https://plus.nl" + node.SelectSingleNode(".//img[contains(@class, 'lazy')]").Attributes["data-src"].Value.Replace("&#47;", "/");
            return CreateBeer(brand, title, volume, bottleAmount, priceNormalized, discount, url);
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
        
        private int getPrice(HtmlNode node)
        {
            if(node == null)
            {
                return 0;
            }

            HtmlNode HtmlDiscountPriceNode = node.SelectSingleNode(".//div[contains(@class, 'text-clover')]");

            return Convert.ToInt32(Math.Round(Convert.ToDouble(node.Attributes["data-price"].Value)));
        }

        private string getDiscount(HtmlNode node)
        {
            if (node == null)
            {
                return "";
            }

            HtmlNode HtmlDiscountPriceNode = node.SelectSingleNode(".//div[contains(@class, 'text-clover')]");
            if (HtmlDiscountPriceNode != null)
            {
                return HtmlDiscountPriceNode.InnerText.Replace("\n", "").Replace("\r", "").Replace(" ", "");
            }
            return "";
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
        private Beer CreateBeer(string brand, string title, int volume, int bottleAmount, int priceNormalized, string discount, string url)
        {
            return new Beer(brand, title, volume, bottleAmount, priceNormalized, discount, "Plus", url);
        }
    }
}