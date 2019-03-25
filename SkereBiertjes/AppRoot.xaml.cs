using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;
using System.Threading.Tasks;
using System.Diagnostics;

namespace SkereBiertjes
{

    /// <summary>
    /// The app root. Contains the navigation bar, basic layout every page got, and includes all the pages.
    /// </summary>
    public sealed partial class AppRoot : Page
    {

        private BeerScraper beerScraper;
        private List<Beer> beers;

        public AppRoot()
        {
            this.InitializeComponent();

            this.beers = new List<Beer>();
            this.beerScraper = new BeerScraper();
            
            Task T1 = new Task(() => {
                beerScraper.startFindingFirstBeers();
            });

            T1.Start();
            T1.Wait();

            this.beers = this.beerScraper.getBeers();

            Debug.WriteLine(this.beers.Count);
        }

        // Runs when the NavView is loaded
        private void NavView_Loaded(object sender, RoutedEventArgs args)
        {
            // Set the initial SelectedItem to 'home'
            foreach (NavigationViewItemBase item in NavView.MenuItems)
            {
                if (item is NavigationViewItem && item.Tag.ToString() == "home")
                {
                    NavView.SelectedItem = item;
                    break;
                }
            }

            // Navigate to home (MainPage)
            ContentFrame.Navigate(typeof(MainPage));

            // Change 'Settings' text to 'Instellingen'
            var settings = (NavigationViewItem)NavView.SettingsItem;
            settings.Content = "Instellingen";
        }

        // Fires when the user clicks a menu item
        private void NavView_SelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
        {
            // If Settings was clicked, navigate to the Settings page
            if (args.IsSettingsSelected)
            {
                ContentFrame.Navigate(typeof(SettingsPage));
            }
            
            // Else some other menu item was clicked
            else
            {
                NavigationViewItem item = args.SelectedItem as NavigationViewItem;

                switch(item.Tag.ToString())
                {
                    case "home":
                        ContentFrame.Navigate(typeof(MainPage), this.beers);
                        break;
                    case "filters":
                        ContentFrame.Navigate(typeof(FilterPage));
                        break;
                    default:
                        ContentFrame.Navigate(typeof(MainPage));
                        break;
                }
            }
        }

        
    }
}
