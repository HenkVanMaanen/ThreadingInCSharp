using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkereBiertjes
{
    public interface Scraper
    {
        Beer Beer { get; set; }

        Beer[] getBeers();
        Task<List<string>> getHTML();
        Beer[] parseHTML();
    }
}