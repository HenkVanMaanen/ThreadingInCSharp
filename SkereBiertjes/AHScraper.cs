﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using HtmlAgilityPack;
using Newtonsoft.Json.Linq;

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


        async Task<List<string>> Scraper.getHTML()
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

        List<Beer> Scraper.parseHTML()
        {
            List<Beer> beers = new List<Beer>();

            var doc = new HtmlDocument();
            doc.OptionFixNestedTags = true;
            doc.Load(StandardURL);

            JObject json = JObject.Parse(doc.DocumentNode.InnerHtml);

            JToken jsonProducts = json.SelectToken("products");

            foreach(JToken product in jsonProducts)
            {
                Beer beer = this.parseData(product);

                beers.Add(beer);

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
            string brand = json["title"].ToString();
            int volume = this.parseNameToVolume(json["unitSize"].ToString());
            int priceNormalized = Convert.ToInt32(Math.Round(Convert.ToDouble(json["price"]["now"].ToString()) * 100));
            string discount = "";
            int bottleAmount = this.parseNameToBottle(json["unitSize"].ToString());
            string url = json["images"][0]["url"].ToString();

            return CreateBeer(brand, volume, bottleAmount, priceNormalized, discount, url);
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

            //will parse everything ending in CL
            if (words[words.Length - 1].Contains("cl"))
            {
                string word = words[words.Length - 2];
                return Convert.ToInt32(Math.Round(Convert.ToDouble(word) * 10));
            }
            //will parse everything ending in l
            if (words[words.Length - 1].Contains("l"))
            {
                string word = words[words.Length - 2].Replace(",", ".");

                return Convert.ToInt32(Math.Round(Double.Parse(word) * 1000));
            }
            
            return 1;
        }
        //create a beer with AlbertHeijn allready in it;
        private Beer CreateBeer(string brand, int volume, int bottleAmount, int priceNormalized, string discount, string url)
        {
            return new Beer(brand, volume, bottleAmount, priceNormalized, discount, "Albert Heijn", url);
        }
    }
}