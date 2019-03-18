using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkereBiertjes
{
    public interface Scraper
    {
        List<Beer> getBeers();
        Task<List<string>> getHTML();
        List<Beer> parseHTML();
    }
}