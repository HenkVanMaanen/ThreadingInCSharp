using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace SkereBiertjes
{
    public class AHScraper : Scraper
    {
        public string keyword = "bier";

        public Beer Beer { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public Beer[] getBeers()
        {
            throw new NotImplementedException();
        }

        async Task<List<string>> Scraper.getHTML()
        {
            var pages = new List<string>();

            using (var httpClient = new HttpClient())
            {
                

                for (var p = 0; p < 17; p++)
                {
                    var response = await httpClient.GetAsync("https://www.ah.nl/zoeken/api/products/search?query=" + keyword + "&page=" + p);
                    pages.Add(await response.Content.ReadAsStringAsync());
                }

                return pages;
            }
        }

        Beer[] Scraper.parseHTML()
        {
            throw new NotImplementedException();
        }
    }
}