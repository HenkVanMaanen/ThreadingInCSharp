using System;
using System.Collections.Generic;
using HtmlAgilityPack;
using System.Diagnostics;
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
        
            doc.LoadHtml(StandardURL);

            var nodes = doc.DocumentNode.SelectNodes("//div[contains(@class,'product-block dotted vertical bier-speciaal-bier-tripel')]");

            foreach (var node in nodes)
            {

                Debug.WriteLine(node.Attributes["value"].Value);
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
    }
}