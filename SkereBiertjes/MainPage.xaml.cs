using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Markup;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.ApplicationModel.Core;
using Windows.UI.Core;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace SkereBiertjes
{
    /// <summary>
    /// The main page of the app. Users can use the searchbar to find their prefered beers,
    /// to see what beers have a discount and what the other prices are from low to high.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private ObservableCollection<string> suggestions;
        private static string SearchIcon = "\uE1A3";
        private static string ErrorIcon = "\uE783";
        private List<Beer> beers;
        private Filter filter;
        private BeerScraper beerScraper;
        private ObservableCollection<object> beerItems = new ObservableCollection<object>();
        public ObservableCollection<object> BeerItems { get { return this.beerItems; } }

        public MainPage()
        {
            this.suggestions = new ObservableCollection<string>();
            this.InitializeComponent();
            BigIcon.Text = SearchIcon;
        }

        private void BeerSearchBox_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        {
            suggestions.Clear();
            // Only get results when it was a user typing, 
            // otherwise assume the value got filled in by TextMemberPath 
            // or the handler for SuggestionChosen.
            if (args.Reason == AutoSuggestionBoxTextChangeReason.UserInput)
            {
                if (!string.IsNullOrWhiteSpace(sender.Text))
                {
                    var possibleSuggestions = new ObservableCollection<string>();
                    possibleSuggestions.Clear();
                    //possibleSuggestions.Add("Albert Heijn");
                    //possibleSuggestions.Add("Jumbo");
                    //possibleSuggestions.Add("Gal & Gal");
                    //possibleSuggestions.Add("Coop");
                    //possibleSuggestions.Add("PLUS");
                    //possibleSuggestions.Add("Hertog Jan");
                    //possibleSuggestions.Add("Grolsch");
                    //possibleSuggestions.Add("Palm");
                    //possibleSuggestions.Add("Bavaria");
                    //possibleSuggestions.Add("Brand");
                    //possibleSuggestions.Add("Heineken");
                    //possibleSuggestions.Add("Jupiler");
                    //possibleSuggestions.Add("Kordaat");
                    //possibleSuggestions.Add("Kornuit");
                    //possibleSuggestions.Add("Amstel");
                    //possibleSuggestions.Add("Gulpener");

                    foreach (string possibleSuggestion in possibleSuggestions)
                    {
                        if (possibleSuggestion.ToLower().Contains(sender.Text.ToLower().Trim()))
                        {
                            suggestions.Add(possibleSuggestion);
                        }
                    }

                    sender.ItemsSource = suggestions;
                }
            }
        }


        private void BeerSearchBox_SuggestionChosen(AutoSuggestBox sender, AutoSuggestBoxSuggestionChosenEventArgs args)
        {
            // Set sender.Text. You can use args.SelectedItem to build your text string.
        }

        private void uiSearchMode()
        {
            EmptyStateElements.Visibility = Visibility.Collapsed;
            BeerItemsGrid.Visibility = Visibility.Collapsed;
            BigIcon.Text = SearchIcon;
            InfoGrid.Opacity = 0d;
        }

        private void uiDoneSearchingResultEmpty()
        {
            BeerItemsGrid.Visibility = Visibility.Collapsed;
            EmptyStateElements.Visibility = Visibility.Visible;
            progressRing.IsActive = false;
        }

        private async void BeerSearchBox_QuerySubmitted(AutoSuggestBox sender,
            AutoSuggestBoxQuerySubmittedEventArgs args)
        {
            progressRing.IsActive = true;

            if (args.ChosenSuggestion == null)
            {
                uiSearchMode();

                // Hide the Empty State Elements (Zoom icon)
                EmptyStateElements.Visibility = Visibility.Collapsed;

                // Beers found and something has been entered in the SearchBox!
                if (!string.IsNullOrWhiteSpace(args.QueryText) && this.beerScraper.getBeersCount() > 0)
                {
                    // Put beers in a new ArrayList for making up the grid
                    displayBeersOnScreenAsync(args.QueryText);
                }
                // No beers found
                else if (this.beerScraper.getBeersCount() == 0)
                {
                    BigIcon.Text = ErrorIcon;
                    EmptyStateTextBlock.Text = "We hebben helaas niks kunnen vinden...";
                    uiDoneSearchingResultEmpty();
                }
                // Nothing entered inside the SearchBox
                else
                {
                    EmptyStateTextBlock.Text = "Je zoekopdracht is leeg...";
                    uiDoneSearchingResultEmpty();
                }
            }
        }

        private void displayBeersOnScreenAsync(string search)
        {
            // Backend stuff is put in the Task below
            Task.Run(() =>
            {
                List<object> beersGridCollection = new List<object>();
                beers = this.beerScraper.search(search, this.filter);
                
                foreach (var beer in beers)
                {
                    var description =
                        $"{beer.getTitle()} - {beer.getBottleAmount()} x {(float) beer.getVolume() / 1000}L";
                    var hasReduction = (string.IsNullOrWhiteSpace(beer.getDiscount()))
                        ? Visibility.Collapsed
                        : Visibility.Visible;
                    float.TryParse(beer.getDiscount(), out var discount);

                    Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => beerItems.Add( new
                    {
                        BeerDescription = description,
                        ReductionPriceVisibility = hasReduction,
                        OriginalPrice = $"€ {String.Format("{0:0.00}", Convert.ToDecimal(discount) / 100)}",
                        Price =
                                $"€ {String.Format("{0:0.00}", Convert.ToDecimal(beer.getNormalizedPrice()) / 100)}",
                        ImageUrl = beer.getUrl(),
                        ShopImageUrl = $"/Assets/shop/{beer.getShopName().Replace(" ", "").ToLower()}.png",
                    }));
                }

                // UI Work is done below
                Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    BeerItemsGrid.Visibility = Visibility.Visible;
                    progressRing.IsActive = false;
                });
            });
        }

        private void EmptyStateTextBlock_Loaded(object sender, RoutedEventArgs e)
        {
            // Als je nog meer onbenullige teksten hebt, kun je die in onderstaande array toevoegen.
            var searchTextArray = new[]
            {
                "Zuinig zuipen? Gebruik de zoekbalk hierboven!",
                "HAAA, BIER!!! ZOEKEN MAAR!",
                "Zoek naar goedkope biertjes met de zoekbalk hierboven.",
                "Probleem\'n? Poar neem\'n! Zoek naar goedkope biertjes hierboven",
                "Lekker fris pils voor een lekker fris prijsje!",
            };

            Random rnd = new Random();
            int rnd_num = rnd.Next(0, (searchTextArray.Length));
            EmptyStateTextBlock.Text = searchTextArray[rnd_num];
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            progressRing.IsActive = true;
            if (e.Parameter is IDictionary<string, Object>)
            {
                IDictionary<string, Object> data = (IDictionary<string, Object>) e.Parameter;
                this.filter = (Filter) data["filter"];
                this.beerScraper = (BeerScraper) data["beerScraper"];

                uiSearchMode();
                var dispatcher = CoreWindow.GetForCurrentThread().Dispatcher;
                dispatcher.RunAsync(CoreDispatcherPriority.Normal,
                   () => { displayBeersOnScreenAsync(""); });
            }

            base.OnNavigatedTo(e);
        }
    }
}