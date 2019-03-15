using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace SkereBiertjes
{
    public class JumboScraper : Scraper
    {
        public string StandardURL;

        public Beer Beer { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public Beer[] getBeers()
        {
            throw new NotImplementedException();
        }

        async Task<List<string>> Scraper.getHTML()
        {
            throw new NotImplementedException();
        }

        Beer[] Scraper.parseHTML()
        {
            throw new NotImplementedException();
        }
    }
}