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
        private CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

        public ObservableCollection<object> BeerItems
        {
            get { return this.beerItems; }
        }

        public MainPage()
        {
            this.InitializeComponent();
            this.suggestions = new ObservableCollection<string>();
            beerSearchBox.IsEnabled = false;
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

        /**
         * Currently not implemented, maybe later
         */
        private void BeerSearchBox_SuggestionChosen(AutoSuggestBox sender, AutoSuggestBoxSuggestionChosenEventArgs args)
        {
            // Set sender.Text. You can use args.SelectedItem to build your text string.
        }

        /**
         * UI method to handle things when the app is busy searching beers
         */
        private void uiSearchMode()
        {
            EmptyStateElements.Visibility = Visibility.Collapsed;
            BeerItemScrollViewer.Opacity = 0.25d;
            BigIcon.Text = SearchIcon;
            progressRing.IsActive = true;
        }

        /**
         * UI method to handle things when the app has found no beers to display
         */
        private void uiDoneSearchingResultEmpty()
        {
            EmptyStateElements.Visibility = Visibility.Visible;
            BeerItemScrollViewer.Opacity = 0;
            BigIcon.Text = ErrorIcon;
            EmptyStateTextBlock.Text = "Geen biertjes gevonden...\nProbeer het opnieuw met een andere zoekopdracht!";
            TimingResults.Text = "Geen resultaten";
            progressRing.IsActive = false;
        }

        /**
        * UI method to handle things when the app has found more than zero beers to display
        */
        private void uiDoneSearchingResultsNotEmpty()
        {
            EmptyStateElements.Visibility = Visibility.Collapsed;
            BeerItemScrollViewer.Opacity = 1d;
            progressRing.IsActive = false;
        }

        /**
         * When the user presses enter or clicks on the search button, this method is called
         */
        private async void BeerSearchBox_QuerySubmitted(AutoSuggestBox sender,
            AutoSuggestBoxQuerySubmittedEventArgs args)
        {
            if (args.ChosenSuggestion == null)
            {
                uiSearchMode();

                // Hide the Empty State Elements (Zoom icon)
                EmptyStateElements.Visibility = Visibility.Collapsed;

                // Beers found and something has been entered in the SearchBox!
                if (!string.IsNullOrWhiteSpace(args.QueryText) && this.beerScraper.getBeersCount() > 0)
                {
                    // Submit cancellation for previous tasks so no new Beer Items will be added to the Grid
                    cancellationTokenSource.Cancel();

                    await Dispatcher.RunAsync(CoreDispatcherPriority.High, async () =>
                    {
                        var userInputText = args.QueryText;

                        // Reset the token
                        cancellationTokenSource.Dispose();
                        cancellationTokenSource = new CancellationTokenSource();

                        Thread t1 = new Thread(async () => { await displayBeersOnScreenAsync(userInputText, cancellationTokenSource.Token); });
                        t1.Start();
                    });
                }
                // Nothing entered inside the SearchBox
                else
                {
                    EmptyStateTextBlock.Text = "Je zoekopdracht is leeg...";
                    uiDoneSearchingResultEmpty();
                }
            }
        }

        /**
         * This async method creates a row in the Grid for
         * particular beer item. It needs to be executed immediately.
         */
        private async Task UpdateGridBeer(object row, CancellationToken cancellationToken)
        {
            // UI THREAD STUFF
            if (!cancellationToken.IsCancellationRequested)
            {
                await Dispatcher.RunAsync(CoreDispatcherPriority.High, () => { beerItems.Add(row); });
            }
        }

        /**
         * Method to handle displaying beers on the Grid.
         * It creates several different Tasks so that the UI will
         * not be blocked!
         */
        private async Task<bool> displayBeersOnScreenAsync(string search, CancellationToken cancellationToken)
        {
            bool bRet = true;
            List<Task> tasks = new List<Task>();

            try
            {
                // Backend stuff is put in the Task below
                tasks.Add(Task.Run(async () =>
                    {
                        Stopwatch stopWatch = new Stopwatch();
                        stopWatch.Start();

                        // UI THREAD STUFF
                        await Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
                            () =>
                            {
                                beerItems.Clear();
                                TimingResults.Text =
                                    "Verbinden met de webshops en bezig met ophalen van meters bier...\nMoment geduld alstublieft!";
                            });

                        // Get the List of beers
                        beers = this.beerScraper.search(search, this.filter);

                        // UI THREAD STUFF
                        // Going to start adding beers to the Beer Grid
                        await Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
                            () =>
                            {
                                beerItems.Clear();
                                TimingResults.Text = "Inladen van de opgehaalde biertjes... Dit kan even duren!";
                                beerSearchBox.IsEnabled = true;
                            });

                        // Foreach loop to add every beer to the UI Grid
                        foreach (var beer in beers)
                        {
                            if (cancellationToken.IsCancellationRequested)
                            {
                                break;
                            }

                                var description =
                                $"{beer.getTitle()} - {beer.getBottleAmount()} x {(float) beer.getVolume() / 1000}L";
                            var hasReduction = (string.IsNullOrWhiteSpace(beer.getDiscount()))
                                ? Visibility.Collapsed
                                : Visibility.Visible;
                            float.TryParse(beer.getDiscount(), out var discount);

                            // Add current beer to separate UI thread
                            // where it will be added to the Grid
                            await UpdateGridBeer(new
                            {
                                BeerDescription = description,
                                ReductionPriceVisibility = hasReduction,
                                OriginalPrice = $"€ {String.Format("{0:0.00}", Convert.ToDecimal(discount) / 100)}",
                                Price =
                                    $"€ {String.Format("{0:0.00}", Convert.ToDecimal(beer.getNormalizedPrice()) / 100)}",
                                ImageUrl = beer.getUrl(),
                                ShopImageUrl = $"/Assets/shop/{beer.getShopName().Replace(" ", "").ToLower()}.png",
                            }, cancellationTokenSource.Token);
                        }

                        // Done searching and filtering beers. Stop the StopWatch!
                        TimeSpan ts = stopWatch.Elapsed;
                        var secondsText = ((double) ((ts.Seconds * 1000) + ts.Milliseconds) / 1000 == 1d)
                            ? "seconde"
                            : "seconden";
                        var resultsText = (beers.Count == 1) ? "resultaat" : "resultaten";

                        // UI THREAD STUFF
                        await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                        {
                            if (beers.Count > 0)
                            {
                                uiDoneSearchingResultsNotEmpty();
                            }
                            else
                            {
                                uiDoneSearchingResultEmpty();
                            }

                            TimingResults.Text =
                                $"{beers.Count} {resultsText} in {(double) ((ts.Seconds * 1000) + ts.Milliseconds) / 1000} {secondsText}";
                        });
                    })
                );

                Task.WaitAll(tasks.ToArray());
            }
            catch (Exception agEx)
            {
                Debug.WriteLine(agEx.StackTrace);
                Debug.WriteLine(agEx.Message);
                bRet = false;
            }

            return bRet;
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

        /**
         * This method below will be called everytime the user
         * navigates to the MainPage view (aka 'home')
         */
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (e.Parameter is IDictionary<string, Object>)
            {
                IDictionary<string, Object> data = (IDictionary<string, Object>) e.Parameter;
                this.filter = (Filter) data["filter"];
                this.beerScraper = (BeerScraper) data["beerScraper"];

                // Start loading in the beers from the async method "displayBeersOnScreenAsync"
                // in a specific thread so UI will not be blocked

                uiSearchMode();
                Thread t1 = new Thread(async () => { await displayBeersOnScreenAsync("", cancellationTokenSource.Token); });
                t1.Start();
            }

            base.OnNavigatedTo(e);
        }
    }
}