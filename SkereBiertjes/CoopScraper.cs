using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace SkereBiertjes
{
    public class CoopScraper : Scraper
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

                var response = await httpClient.GetAsync("https://www.coop.nl/actions/ViewAjax-Start?PageNumber=1&PageSize=5000&SortingAttribute=&ViewType=&TargetPipeline=ViewParametricSearch-ProductPaging&SearchTerm=" + keyword + "&SearchParameter=%26%40QueryTerm%3D" + keyword + "&AjaxCall=true");
                pages.Add(await response.Content.ReadAsStringAsync());

                return pages;
            }
        }

        Beer[] Scraper.parseHTML()
        {
            throw new NotImplementedException();
        }
    }
}