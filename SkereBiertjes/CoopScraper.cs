using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HtmlAgilityPack;

using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using HtmlAgilityPack;
using Newtonsoft.Json.Linq;

namespace SkereBiertjes
{
    public class CoopScraper : Scraper
    {
        private string StandardURL;
        private string keyword = "bier";
        private List<Beer> beers; 

        public CoopScraper()
        {
            StandardURL = @"Data/coop.json";
            beers = new List<Beer>();
        }

        
        async Task<List<string>> Scraper.getHTML()
        {
            var pages = new List<string>();

            using (var httpClient = new HttpClient())
            {

                var response = await httpClient.GetAsync("https://www.coop.nl/actions/ViewAjax-Start?PageNumber=1&PageSize=5000&SortingAttribute=&ViewType=&TargetPipeline=ViewParametricSearch-ProductPaging&SearchTerm=" + keyword + "&SearchParameter=%26%40QueryTerm%3D" + keyword + "&AjaxCall=true");
                pages.Add(await response.Content.ReadAsStringAsync());

                return pages;
            }
        }

        List<Beer> Scraper.parseHTML()
        {
            List<Beer> beers = new List<Beer>();

            var doc = new HtmlDocument();
            doc.OptionFixNestedTags = true;
            doc.Load(StandardURL);

            JArray json = JArray.Parse(doc.DocumentNode.InnerHtml);

            foreach (JToken product in json)
            {
                Beer beer = this.parseData(product);

                beers.Add(beer);
                this.beers.Add(beer);
                //print info from beer
                beer.printInfo();
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
            string brand = json["productJson"]["name"].ToString();
            int priceNormalized = Convert.ToInt32(Math.Round(Convert.ToDouble(json["productJson"]["price"].ToString()) * 100));
            string discount = "";
            int bottleAmount = parseNameToBottle(brand);
            string jsonText = json["productSubText"].ToString();
            string txt = jsonText.Remove(jsonText.Length - 3, 3);
            int volume = Convert.ToInt32(Math.Round(Convert.ToDouble(txt))) / bottleAmount;
            string url = json["image540"].ToString();

            return CreateBeer(brand, volume, bottleAmount, priceNormalized, discount, url);
        }

        //create a beer with AlbertHeijn allready in it;
        private Beer CreateBeer(string brand, int volume, int bottleAmount, int priceNormalized, string discount, string url)
        {
            return new Beer(brand, volume, bottleAmount, priceNormalized, discount, "Albert Heijn", url);
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