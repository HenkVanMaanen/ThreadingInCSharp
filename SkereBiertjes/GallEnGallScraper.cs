using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HtmlAgilityPack;
using System.Diagnostics;

namespace SkereBiertjes
{
    public class GallEnGallScraper : Scraper
    {
        public string StandardURL;

        private List<Beer> beers;

        public GallEnGallScraper()
        {
            beers = new List<Beer>();
            StandardURL = "C:/Users/chiel/Persoonlijk/SCHOOL TI STENDEN/Technische informatica jaar 3/Periode 3/C# multithreading/C# multithreading/Documenten/Test data/gall&gall.html";
        }

        string Scraper.getHTML()
        {
            var doc = new HtmlDocument();
            doc.Load(StandardURL);

            var node = doc.DocumentNode.SelectSingleNode("//body");

            Debug.WriteLine(node);
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