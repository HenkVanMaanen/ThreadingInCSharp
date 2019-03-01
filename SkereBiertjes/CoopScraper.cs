using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SkereBiertjes
{
    public abstract class CoopScraper : Scraper
    {
        public string StandardURL;

        public Beer Beer { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public abstract Beer[] getBeers();

        string Scraper.getHTML()
        {
            throw new NotImplementedException();
        }

        Beer[] Scraper.parseHTML()
        {
            throw new NotImplementedException();
        }
    }
}