using System;
using System.Collections.Generic;
using HtmlAgilityPack;
using System.Diagnostics;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace SkereBiertjes
{
    public class GallEnGallScraper : Scraper
    {
        public string StandardURL;

        private List<Beer> beers;

        public GallEnGallScraper()
        {
            beers = new List<Beer>();
            StandardURL = @"Data/gall&gall.html";
        }

        string Scraper.getHTML()
        {
            var doc = new HtmlDocument();
            doc.OptionFixNestedTags = true;
            doc.Load(StandardURL);

            var nodes = doc.DocumentNode.SelectNodes("//article[contains(@class,'product-block')]");

            if (nodes == null)
            {
                Debug.WriteLine("No nodes selected");
                return null;
            }
            if (doc.DocumentNode == null)
            {
                Debug.WriteLine("DocumentNode == null");
                return null;
            }

            foreach (var node in nodes)
            {
                if (node != null)
                {
                    HtmlNodeCollection node2 = node.SelectNodes(".//img[contains(@itemprop,'image')]");
                    Debug.WriteLine(node2["img"].Attributes["src"].Value);

                    foreach (var dataTrackingImpression in node.Attributes)
                    {
                        if (dataTrackingImpression.Name == "data-tracking-impression")
                        {
                            JObject json = JObject.Parse(dataTrackingImpression.Value);
                            Beer beer = this.parseData(json);
                            beer.printInfo();
                        }
                    }

                }
            }
            return null;
        }
         
        private Beer parseData(JObject json)
        {
            string name = json["name"].ToString();
            int volume = this.parseNameToVolume(name);
            int price = Convert.ToInt32(Math.Round(Convert.ToDouble(json["price"].ToString()) * 100));
            string discount = "";
            string url = "";
            Beer beer = CreateBeer(name, volume, price, discount, url);


            return beer;
        }

        List<Beer> Scraper.parseHTML()
        {
            throw new NotImplementedException();
        }

        public List<Beer> getBeers()
        {
            return beers;
        }

        private Beer CreateBeer(string brand, int volume, int priceNormalized, string discount, string url)
        {
            return new Beer(brand, volume, priceNormalized, discount, "Gall&Gall", url);
        }

        private int parseNameToVolume(string title)
        {
            if(title == "")
            {
                return -1;
            }

            string[] words = title.Split(' ');
            if (words[words.Length - 1].Contains("CL"))
            {
                string word = words[words.Length - 1];
                word = word.Remove(word.Length - 2, 2);
                return Convert.ToInt32(Math.Round(Convert.ToDouble(word) * 10)); 
            }
            return 0;


        }
    }
}