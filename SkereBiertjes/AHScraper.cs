using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SkereBiertjes
{
    public abstract class AHScraper : Scraper
    {
        public string StandardURL;

        public abstract Beer[] getBeers();

        private abstract String getHTML();

        private abstract Beer[] parseHTML();
    }
}