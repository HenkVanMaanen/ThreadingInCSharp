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
using System.Collections;
using Windows.Storage;

namespace SkereBiertjes
{

    /// <summary>
    /// The app root. Contains the navigation bar, basic layout every page got, and includes all the pages.
    /// </summary>
    public sealed partial class AppRoot : Page
    {

        private BeerScraper beerScraper;
        private DatabaseHandler databaseHandler;
        private Filter filter;
        private ApplicationDataContainer localSettings = ApplicationData.Current.LocalSettings;

        public AppRoot()
        {
            this.InitializeComponent();
            this.filter = new Filter("", "", "");
            localSettings.Values["multithreading_enabled"] = true;
            //create database handler, send this instance to all other classes.
            this.databaseHandler = new DatabaseHandler("SkereBiertjesV6.db");
            this.databaseHandler.delete();
            this.beerScraper = new BeerScraper(databaseHandler);
            
            Task T1 = new Task(() => {
                beerScraper.startFindingFirstBeers();
            });

            T1.Start();
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
                
                switch (item.Tag.ToString())
                {
                    case "home":
                        IDictionary<string, Object> data = new Dictionary<string, Object>();
                        data["filter"] = this.filter;
                        data["beerScraper"] = this.beerScraper;

                        ContentFrame.Navigate(typeof(MainPage), data);
                        break;
                    case "filters":
                        ContentFrame.Navigate(typeof(FilterPage), this.filter);
                        break;
                    default:
                        ContentFrame.Navigate(typeof(MainPage), this.databaseHandler);
                        break;
                }
            }
        }

        
    }
}
