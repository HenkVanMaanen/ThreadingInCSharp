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
    //because all scrapers are basically the same, all scraper comments can be found in the AHScraper.
    //if there is data specific for this scraper only it will contain comments
    public class JumboScraper : Scraper
    {
        private string keyword = "bier";
        private List<Beer> beers;
        private bool benchmark = false;
        private List<string> benchmarkData;

        public JumboScraper()
        {
            beers = new List<Beer>();
        }

        public void setBenchmark(bool benchmark)
        {
            this.benchmark = benchmark;
        }

        public void setBenchmarkData(List<string> data)
        {
            this.benchmarkData = data;
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
            this.benchmarkData.Add("[JUMBO] Starting");
            List<Beer> beers = new List<Beer>();

            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();

            var pages = await getHTML();

            stopWatch.Stop();
            if (benchmark)
            {
                int ts = stopWatch.Elapsed.Milliseconds;
                this.benchmarkData.Add("[JUMBO] Getting html took: " + ts + " ms");
            }

            stopWatch.Restart();
            foreach (string page in pages)
            {
                //get document
                var doc = new HtmlDocument();
                doc.OptionFixNestedTags = true;
                doc.LoadHtml(page);

                //get all div with the class name jum-item-product
                var nodes = doc.DocumentNode.SelectNodes("//div[contains(@class, 'jum-item-product')]");

                if (nodes == null)
                {
                    Debug.WriteLine("no nodes selected");
                    break;
                }

                //loop over all nodes
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
            
            stopWatch.Stop();
            if (benchmark)
            {
                int ts = stopWatch.Elapsed.Milliseconds;
                this.benchmarkData.Add("[JUMBO] parsing html took: " + ts + " ms");
            }
            return beers;
        }

        List<Beer> Scraper.getBeers()
        {
            return beers;
        }

        private Beer parseData(HtmlNode node)
        {
            //select title tag
            string title = node.SelectSingleNode(".//a[contains(@data-jum-action, 'quickView')]").InnerHtml;
            string brand = this.parseToBrand(title);

            int volume;
            try
            {
                //select size tag
                HtmlNode node2 = node.SelectSingleNode(".//span[contains(@class, 'jum-pack-size')]");
                if (node2 == null)
                {
                    volume = 0;
                } else
                {
                    string text = node2.InnerHtml;
                    if (text.ToLower().Contains("cl"))
                    {
                        string textCL = text.Split(' ')[2];
                        volume = Convert.ToInt32(textCL) * 10;
                    } else if (text.ToLower().Contains("ml"))
                    {
                        string textML = text.Split(' ')[0];
                        volume = Convert.ToInt32(textML);
                    }
                    else if (text.ToLower().Contains("l"))
                    {
                        string textL = text.Split(' ')[2];
                        textL = textL.Replace(',', '.');
                       
                        volume = Convert.ToInt32(Math.Round(Convert.ToDouble(textL) * 10));
                    } else
                    {
                        volume = 0;
                    }
                }
            }
            catch
            {
                volume = 0;
            }

            int bottleAmount;
            try
            {
                //get node that contains bottleamount
                HtmlNode node2 = node.SelectSingleNode(".//span[contains(@class, 'jum-pack-size')]");
                if(node2 == null)
                {
                    bottleAmount = 1;
                } else
                {
                    string text = node2.InnerHtml;
                    //cl and liters will contain a bottle amount, ml will not
                    if ((text.ToLower().Contains("cl") || text.ToLower().Contains("l")) && !text.ToLower().Contains("ml"))
                    {
                        bottleAmount = Convert.ToInt32(text.Split(' ')[0]);
                    }
                    else
                    {
                        bottleAmount = 1;
                    }
                }
            } catch
            {
                bottleAmount = 1;
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


