using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SkereBiertjes
{
    public abstract class JumboScraper : Scraper
    {
        public string StandardURL;
        
        public List<Beer> parseHTML()
        {
            throw new NotImplementedException();
        }

        List<Beer> Scraper.getBeers()
        {
            throw new NotImplementedException();
        }

        string Scraper.getHTML()
        {
            throw new NotImplementedException();
        }
    }
}