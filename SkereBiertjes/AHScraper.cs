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
        private string StandardURL;
        private string keyword = "bier";
        private List<Beer> beers;

        public AHScraper()
        {
            StandardURL = @"Data/coop.json";
            beers = new List<Beer>();
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

        List<Beer> Scraper.parseHTML()
        {
            throw new NotImplementedException();
        }
        
        List<Beer> Scraper.getBeers()
        {
            return beers;
        }
    }
}