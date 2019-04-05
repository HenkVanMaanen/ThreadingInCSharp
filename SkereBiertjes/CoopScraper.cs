using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HtmlAgilityPack;

using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using System.Diagnostics;

namespace SkereBiertjes
{
    public class CoopScraper : Scraper
    {
        private string keyword = "bier";
        private List<Beer> beers; 

        public CoopScraper()
        {
            beers = new List<Beer>();
        }

        
        async Task<List<string>> getHTML()
        {
            var pages = new List<string>();

            using (var httpClient = new HttpClient())
            {

                var response = await httpClient.GetAsync("https://www.coop.nl/actions/ViewAjax-Start?PageNumber=1&PageSize=5000&SortingAttribute=&ViewType=&TargetPipeline=ViewParametricSearch-ProductPaging&SearchTerm=" + keyword + "&SearchParameter=%26%40QueryTerm%3D" + keyword + "&AjaxCall=true");
                pages.Add(await response.Content.ReadAsStringAsync());

                return pages;
            }
        }

        async Task<List<Beer>> Scraper.parseHTML()
        {
            List<Beer> beers = new List<Beer>();
            var pages = await getHTML();

            foreach (string page in pages)
            {
                var doc = new HtmlDocument();
                doc.OptionFixNestedTags = true;
                doc.LoadHtml(page);

                JArray json = JArray.Parse(doc.DocumentNode.InnerHtml);

                foreach (JToken product in json)
                {
                    Beer beer = this.parseData(product);

                    beers.Add(beer);
                    this.beers.Add(beer);
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
            string title = json["productJson"]["name"].ToString();
            string brand = this.parseToBrand(title);
            int priceNormalized = getPrice(json);
            string discount = "";
            int bottleAmount = parseNameToBottle(title);
            string jsonText = json["productSubText"].ToString();
            string txt = jsonText.Remove(jsonText.Length - 3, 3);
            int volume = Convert.ToInt32(Math.Round(Convert.ToDouble(txt))) / bottleAmount;
            string url = json["image540"].ToString();

            return CreateBeer(brand, title, volume, bottleAmount, priceNormalized, discount, url);
        }

        private int getPrice(JToken json)
        {
            int number;
            string text = json["productJson"]["price"].ToString();
            Debug.WriteLine(text);
            if (text.Contains("."))
            {
                if (text.Split(".")[1].Length == 1)
                {
                    number = Convert.ToInt32(Math.Round(Convert.ToDouble(json["productJson"]["price"].ToString()) * 10));
                }
                else
                {
                    number = Convert.ToInt32(Math.Round(Convert.ToDouble(json["productJson"]["price"].ToString())));
                }
            } else if (json["productJson"]["price"].ToString().Contains('.'))
            {
                number = Convert.ToInt32(Math.Round(Convert.ToDouble(json["productJson"]["price"].ToString())));
            } else
            {
                number = Convert.ToInt32(Math.Round(Convert.ToDouble(json["productJson"]["price"].ToString()) * 100));
            }
            Debug.WriteLine(number);
            return number;
        }

        //create a beer with AlbertHeijn allready in it;
        private Beer CreateBeer(string brand, string title, int volume, int bottleAmount, int priceNormalized, string discount, string url)
        {
            return new Beer(brand, title, volume, bottleAmount, priceNormalized, discount, "Coop", url);
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

            if (words.Length > 4 && words[words.Length - 3].Contains("x"))
            {
                string word = words[words.Length - 4];
                return Convert.ToInt32(word);
            }

            return 1;
        }
    }
}