using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HtmlAgilityPack;

using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace SkereBiertjes
{
    public class CoopScraper : Scraper
    {
        private string StandardURL;
        private string keyword = "bier";
        private List<Beer> beers; 

        public CoopScraper()
        {
            StandardURL = @"Data/coop.json";
            beers = new List<Beer>();
        }

        
        async Task<List<string>> Scraper.getHTML()
        {
            var pages = new List<string>();

            using (var httpClient = new HttpClient())
            {

                var response = await httpClient.GetAsync("https://www.coop.nl/actions/ViewAjax-Start?PageNumber=1&PageSize=5000&SortingAttribute=&ViewType=&TargetPipeline=ViewParametricSearch-ProductPaging&SearchTerm=" + keyword + "&SearchParameter=%26%40QueryTerm%3D" + keyword + "&AjaxCall=true");
                pages.Add(await response.Content.ReadAsStringAsync());

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