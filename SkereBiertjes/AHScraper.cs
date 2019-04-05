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
        private string StandardURL;
        private string keyword = "bier";
        private List<Beer> beers;

        public AHScraper()
        {
            StandardURL = @"Data/albertheijn.json";
            beers = new List<Beer>();
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
            Debug.WriteLine("Starting AH");
            List<Beer> beers = new List<Beer>();
            var pages = await getHTML();

            foreach (string page in pages)
            {
                var doc = new HtmlDocument();
                doc.OptionFixNestedTags = true;
                doc.LoadHtml(page);

                JToken jsonProducts = null;
                try
                {
                    string text = doc.DocumentNode.InnerHtml;
                    JObject json;
                    if (text[0].Equals("\"") && text[text.Length].Equals("\""))
                    {
                        text = text.Remove(text.Length , 1).Remove(0, 1);
                    }
                    text = text.Replace("href=\"", "href=\\\"");
                    text = text.Replace("\">", "\\\">");
                    json = JObject.Parse(text);
                    
                    jsonProducts = json.SelectToken("products");
                } catch
                {
                    break;
                }

                foreach (JToken product in jsonProducts)
                {
                    Beer beer = this.parseData(product);

                    beers.Add(beer);

                    //print info from beer
                    beer.printInfo();
                }
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
            if (text == "")
            {
                return 0;
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