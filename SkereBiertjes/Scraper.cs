using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SkereBiertjes
{
    public interface Scraper
    {
        List<Beer> getBeers();

        string getHTML();

        List<Beer> parseHTML();
    }
}