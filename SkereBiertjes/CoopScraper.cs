using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HtmlAgilityPack;

namespace SkereBiertjes
{
    public abstract class CoopScraper : Scraper
    {
        public string StandardURL;
        
        List<Beer> Scraper.getBeers()
        {
            throw new NotImplementedException();
        }

        string Scraper.getHTML()
        {


            return null;
        }

        List<Beer> Scraper.parseHTML()
        {
            throw new NotImplementedException();
        }
    }
}