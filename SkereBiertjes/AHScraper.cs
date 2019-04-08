using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using HtmlAgilityPack;
using Newtonsoft.Json.Linq;
using System.Diagnostics;

namespace SkereBiertjes
{
    public class AHScraper : Scraper
    {
        private string keyword = "bier";
        private List<Beer> beers;
        private bool benchmark = false;
        private List<string> benchmarkData;

        public AHScraper()
        {
            beers = new List<Beer>();
        }
        
        //set benchmark if benchmark is running
        public void setBenchmark(bool benchmark)
        {
            this.benchmark = benchmark;
        }

        //set benchmark data
        public void setBenchmarkData(List<string> data)
        {
            this.benchmarkData = data;
        }
        
        async Task<List<string>> getHTML()
        {
            var pages = new List<string>();

            using (var httpClient = new HttpClient())
            {
                

                for (var p = 0; p < 17; p++)
                {
                    var response = await httpClient.GetAsync("https://www.ah.nl/zoeken/api/products/search?query=" + keyword + "&page=" + p);
                    pages.Add(await response.Content.ReadAsStringAsync());
                }

                return pages;
            }
        }

        async Task<List<Beer>> Scraper.parseHTML()
        {
            this.benchmarkData.Add("[AH] Starting");
            List<Beer> beers = new List<Beer>();

            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();

            //getting the html from the AH
            var pages = await getHTML();

            stopWatch.Stop();
            if (benchmark)
            {
                int ts = stopWatch.Elapsed.Milliseconds;
                this.benchmarkData.Add("[AH] Getting html took: " + ts + " ms");
            }

            stopWatch.Restart();
            //loop over all pages and parse each page
            foreach (string page in pages)
            {
                //set current page into a HTMLDocument
                var doc = new HtmlDocument();
                doc.OptionFixNestedTags = true;
                doc.LoadHtml(page);

                JToken jsonProducts = null;
                try
                {
                    //clean the text so it can be parsed to JSON
                    string text = doc.DocumentNode.InnerHtml;
                    JObject json;
                    if (text[0].Equals("\"") && text[text.Length].Equals("\""))
                    {
                        text = text.Remove(text.Length , 1).Remove(0, 1);
                    }
                    //add extra backslashes to Quotes otherwise Json cant be parsed
                    text = text.Replace("href=\"", "href=\\\"");
                    text = text.Replace("\">", "\\\">");
                    json = JObject.Parse(text);
                    
                    jsonProducts = json.SelectToken("products");
                } catch
                {
                    //skip this page if the page has invalid data on it.
                    break;
                }

                //loop over every product
                foreach (JToken product in jsonProducts)
                {
                    //parse product into a beer opject
                    Beer beer = this.parseData(product);
                    
                    beers.Add(beer);

                    //print info from beer if it is enabled in the scraper
                    beer.printInfo();
                }
            }

            stopWatch.Stop();
            if (benchmark)
            {
                int ts = stopWatch.Elapsed.Milliseconds;
                this.benchmarkData.Add("[AH] parsing html took: " + ts + " ms");
            }
            return beers;
        }
        
        List<Beer> Scraper.getBeers()
        {
            return beers;
        }

        //parse the article node to a beer
        private Beer parseData(JToken json)
        {
            //parse json to usable data
            string title = json["title"].ToString();
            string brand = this.parseToBrand(title);
            int volume = this.parseNameToVolume(json["unitSize"].ToString());
            int priceNormalized = Convert.ToInt32(Math.Round(Convert.ToDouble(json["price"]["now"].ToString()) * 100));
            string discount = "";
            int bottleAmount = this.parseNameToBottle(json["unitSize"].ToString());

            string url = getURL(json);

            return CreateBeer(brand, title, volume, bottleAmount, priceNormalized, discount, url);
        }

        private string getURL(JToken json)
        {
            //if the images exists return image, otherwise return empty string
            try
            {
                if (json["images"].Count() == 0)
                {
                    return "";
                }
                else
                {
                    return json["images"][0]["url"].ToString();
                }
            } catch
            {
                return "";
            }
        }

        private string parseToBrand(string title)
        {
            //parse title to a brand, this function will check if one of the listed brands is in the title.
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

        private int parseNameToBottle(string text)
        {
            //parse title to bottle amount, the name will always contain the amount of bottles, if its not there it will always be 1
            if (text == "")
            {
                return 1;
            }

            string[] words = text.Split(' ');

            if (words[1].Contains("x"))
            {
                string word = words[0];
                return Convert.ToInt32(word);
            }

            return 1;
        }

        private int parseNameToVolume(string text)
        {
            if(text == "")
            {
                return 0;
            }

            string[] words = text.Split(' ');
            string word;
            //will parse everything ending in CL
            if (words[words.Length - 1].Contains("cl"))
            {
                word = words[words.Length - 2];
                return Convert.ToInt32(Math.Round(Convert.ToDouble(word) * 10));
            }
            //will parse everything ending in l
            if (words[words.Length - 1].Contains("l"))
            {
                word = words[words.Length - 2].Replace(",", ".");
                return Convert.ToInt32(Math.Round(Double.Parse(word) * 1000));
            }
            
            return 1;
        }

        //create a beer with AlbertHeijn allready in it;
        private Beer CreateBeer(string brand, string title, int volume, int bottleAmount, int priceNormalized, string discount, string url)
        {
            return new Beer(brand, title, volume, bottleAmount, priceNormalized, discount, "Albert Heijn", url);
        }
    }
}