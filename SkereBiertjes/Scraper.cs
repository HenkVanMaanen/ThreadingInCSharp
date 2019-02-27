using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SkereBiertjes
{
    public interface Scraper
    {
        Beer Beer { get; set; }

        Beer[] getBeers();
        string getHTML();
        Beer[] parseHTML();
    }
}