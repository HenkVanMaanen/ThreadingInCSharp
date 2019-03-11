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
            Debug.WriteLine(doc.ParseErrors);
            Debug.WriteLine("Count" + nodes.Count);

            foreach (var node in nodes)
            {
                if (node != null)
                {
                    var data = node.Attributes;
                    foreach (var dataTrackingImpression in node.Attributes)
                    {
                        if (dataTrackingImpression.Name == "data-tracking-impression")
                        {
                            var Value = dataTrackingImpression.Value;
                            JObject json = JObject.Parse(Value);
                            Debug.WriteLine(json);
                            Beer beer = CreateBeer(json["name"].ToString(), 55, Int32.Parse(json["price"].ToString()), "", "");
                            Debug.WriteLine(beer);

                        }
                    }

                }
            }
            return null;
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
    }
}