using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace SkereBiertjes
{
    public class GUI
    {
        private BeerScraper beerScraper;
        private Filter filter;

        public GUI()
        {
            this.filter = new Filter("Brand", "", "");
        }

        private void search(System.String keyword)
        {
            throw new System.NotImplementedException();
        }

        private void toggleMultithreading()
        {
            throw new System.NotImplementedException();
        }

        private void refreshBeers()
        {
            throw new System.NotImplementedException();
        }

        private string[] setFilters()
        {
            throw new System.NotImplementedException();
        }

        private void quit()
        {
            throw new System.NotImplementedException();
        }
    }
}