using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkereBiertjes
{
    public interface Scraper
    {
        //retuurn all beers from the scraper
        List<Beer> getBeers();

        //get html, parse html and return a Task
        Task<List<Beer>> parseHTML();

        //set if benchmark is on or not
        void setBenchmark(bool benchmark);

        //set benchmark list where data can be pusht on
        void setBenchmarkData(List<string> data);
    }
}