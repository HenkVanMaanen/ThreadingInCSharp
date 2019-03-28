using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
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

        public MainPage()
        {
            this.suggestions = new ObservableCollection<string>();
            this.InitializeComponent();
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
                    possibleSuggestions.Add("Albert Heijn");
                    possibleSuggestions.Add("Jumbo");
                    possibleSuggestions.Add("Gal & Gal");
                    possibleSuggestions.Add("Coop");
                    possibleSuggestions.Add("PLUS");
                    possibleSuggestions.Add("Hertog Jan");
                    possibleSuggestions.Add("Grolsch");
                    possibleSuggestions.Add("Palm");
                    possibleSuggestions.Add("Bavaria");
                    possibleSuggestions.Add("Brand");
                    possibleSuggestions.Add("Heineken");
                    possibleSuggestions.Add("Jupiler");
                    possibleSuggestions.Add("Kordaat");
                    possibleSuggestions.Add("Kornuit");
                    possibleSuggestions.Add("Amstel");
                    possibleSuggestions.Add("Gulpener");

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


        private async void BeerSearchBox_QuerySubmitted(AutoSuggestBox sender,
            AutoSuggestBoxQuerySubmittedEventArgs args)
        {
            if (args.ChosenSuggestion == null)
            {
                progressRing.IsActive = true;
                EmptyStateElements.Visibility = Visibility.Collapsed;
                BeerItemsGrid.Visibility = Visibility.Collapsed;
                BigIcon.Text = SearchIcon;
                InfoGrid.Opacity = 0d;
                
                // Disable progress ring animation
                progressRing.IsActive = false;

                // Hide the Empty State Elements (Zoom icon)
                EmptyStateElements.Visibility = Visibility.Collapsed;

                // Beers found and something has been entered in the SearchBox!
                if (!string.IsNullOrWhiteSpace(args.QueryText) && this.beerScraper.getBeersCount() > 0)
                {
                    // Put beers in a new ArrayList for making up the grid
                    this.displayBeersOnScreen(args.QueryText);

                }
                // No beers found
                else if (this.beerScraper.getBeersCount() == 0)
                {
                    BeerItemsGrid.Visibility = Visibility.Collapsed;
                    EmptyStateElements.Visibility = Visibility.Visible;

                    BigIcon.Text = ErrorIcon;
                    EmptyStateTextBlock.Text = "We hebben helaas niks kunnen vinden...";
                }
                // Nothing entered inside the SearchBox
                else
                {
                    BeerItemsGrid.Visibility = Visibility.Collapsed;
                    EmptyStateElements.Visibility = Visibility.Visible;

                    EmptyStateTextBlock.Text = "Je zoekopdracht is leeg...";
                }
            }
        }

        private void displayBeersOnScreen(string search)
        {
            // Start the stopwatch and start getting beers
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();
            
            List<Beer> beers = this.beerScraper.search(search, this.filter);
            EmptyStateElements.Visibility = Visibility.Collapsed;
            
            // Stop stopwatch and get the elapsed time
            stopWatch.Stop();
            TimeSpan ts = stopWatch.Elapsed;

            var beerItemsSource = new ArrayList();
            foreach (var beer in beers)
            {
                var description =
                    $"{beer.getTitle()} - {beer.getBottleAmount()} x {(float)beer.getVolume() / 1000}L";
                var hasReduction = (string.IsNullOrWhiteSpace(beer.getDiscount()))
                    ? Visibility.Collapsed
                    : Visibility.Visible;
                float.TryParse(beer.getDiscount(), out var discount);


                beerItemsSource.Add(
                    new
                    {
                        BeerDescription = description,
                        ReductionPriceVisibility = hasReduction,
                        OriginalPrice = $"€ {String.Format("{0:0.00}", Convert.ToDecimal(discount) / 100)}",
                        Price = $"€ {String.Format("{0:0.00}", Convert.ToDecimal(beer.getNormalizedPrice()) / 100)}",
                        ImageUrl = beer.getUrl(),
                        ShopImageUrl = $"/Assets/shop/{beer.getShopName().Replace(" ", "").ToLower()}.png",
                    });


            }


            if (beers.Count == 0)
            {
                BeerItemsGrid.Visibility = Visibility.Collapsed;
                EmptyStateElements.Visibility = Visibility.Visible;

                BigIcon.Text = ErrorIcon;
                EmptyStateTextBlock.Text = "We hebben helaas niks kunnen vinden...";
            } else
            {
                // Show the grid
                BeerItemsGrid.Visibility = Visibility.Visible;

                // Add the beerItemsSource to the ItemsSource for the ItemsControl in the Xaml
                BeerItemsGrid.ItemsSource = beerItemsSource;

                // Display search time and amount of results
                InfoGrid.Opacity = 100d;
                var secondsText = ((double)((ts.Seconds * 1000) + ts.Milliseconds) / 1000 == 1d) ? "seconde" : "seconden";
                var resultsText = (beers.Count == 1) ? "resultaat" : "resultaten";
                TimingResults.Text = $"{beers.Count} {resultsText} in {(double)((ts.Seconds * 1000) + ts.Milliseconds) / 1000} {secondsText}";
            }
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

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if(e.Parameter is IDictionary<string, Object>)
            {
                IDictionary<string, Object> data = (IDictionary<string, Object>) e.Parameter;
                this.filter = (Filter) data["filter"];
                this.beerScraper = (BeerScraper) data["beerScraper"];
                
                this.displayBeersOnScreen("");
            }
            base.OnNavigatedTo(e);
        }
    }
}