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

        public GUI()
        {
            this.beerScraper = new BeerScraper();
            Task T1 = new Task(() => { beerScraper.start();});
            T1.Start();

            Debug.WriteLine("Beerscraper started");

            T1.Wait();
            Debug.WriteLine("Beerscraper done");
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