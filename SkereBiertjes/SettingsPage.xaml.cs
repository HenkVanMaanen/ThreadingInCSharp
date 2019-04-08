using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace SkereBiertjes
{
    public sealed partial class SettingsPage : Page
    {
        private ApplicationDataContainer localSettings = ApplicationData.Current.LocalSettings;
        private BeerScraper beerScraper;
        private CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
        private ObservableCollection<string> benchmarkTest = new ObservableCollection<string>();

        public SettingsPage()
        {
            this.InitializeComponent();
            var multithreadingEnabled = localSettings.Values["multithreading_enabled"];

            if (multithreadingEnabled == null)
            {
                this.multithreadingToggle.IsOn = true;
                localSettings.Values["multithreading_enabled"] = true;
            }
            else
            {
                this.multithreadingToggle.IsOn = (bool) multithreadingEnabled;
            }
        }

        // Benchmark start button clicked
        private async void BenchmarkStart_Click(object sender, RoutedEventArgs e)
        {
            cancellationTokenSource.Cancel();

            //set this data list into all scrapers and beerscraper, this way we can call this data object.
            List<string> data = new List<string>();
            this.beerScraper.setBenchmarkData(data);

            Task T1 = new Task(() => {
                beerScraper.startFindingFirstBeers(true);
            });

            T1.Start();


            cancellationTokenSource.Dispose();
            cancellationTokenSource = new CancellationTokenSource();
            
            //benchmark data
            int totalGetting = 0;
            int totalParsing = 0;
            int totalGettingDone = 0;
            int totalParsingDone = 0;
            
            bool multithread = (bool) localSettings.Values["multithreading_enabled"];
            //infinite loop
            //not best pratice needs to change in the future
            while (true)
            {
                //if data contains new benchmark data print it out
                if (data.Count > 0)
                {
                    //loop over all new benchmark data
                    foreach (string txt in data.ToList())
                    {
                        //if current text contains getting it will get the time and add it to totalgetting or it will replace totalgetting based on multithread on or off.
                        //this should give a slight insight in the difference off multithreading or not.
                        await UpdateBenchMarkGrid(txt, cancellationTokenSource.Token);
                        if (txt.Contains("Getting") && !txt.Contains("Total"))
                        {
                            totalGettingDone++;
                            if (multithread)
                            {
                                totalGetting = Convert.ToInt32(txt.Split(" ")[4]);
                            }
                            else
                            {
                                totalGetting += Convert.ToInt32(txt.Split(" ")[4]);
                            }

                            //if all scrapers are done getting the html it will send the string to the benchmark page
                            if (totalGettingDone == this.beerScraper.getScrapers().Count)
                            {
                                data.Add("[Getting Total] " + totalGetting + "ms");
                            }

                        }
                        //if current text contains parsing it will get the time and add it to totalparsing or it will replace totalparsing based on multithread on or off.
                        //this should give a slight insight in the difference off multithreading or not.
                        else if (txt.Contains("parsing") && !txt.Contains("Total")) 
                        {
                            totalParsingDone++;
                            if (multithread)
                            {
                                totalParsing = Convert.ToInt32(txt.Split(" ")[4]);
                            }
                            else
                            {
                                totalParsing += Convert.ToInt32(txt.Split(" ")[4]);
                            }

                            //if all scrapers are done parsing the html it will send the string to the benchmark page
                            if (totalGettingDone == this.beerScraper.getScrapers().Count)
                            {
                                data.Add("[Parsing Total] " + totalParsing + "ms");
                            }
                        }
                        //remove current data;
                        data.Remove(txt);
                    }
                }
                //wait for 100 ms
                await Task.Delay(100);
            }
        }
        
        /**
         * This async method creates a row in the Grid for
         * particular benchmark item. It needs to be executed immediately.
         */
        private async Task UpdateBenchMarkGrid(string row, CancellationToken cancellationToken)
        {
            // UI THREAD STUFF
            if (!cancellationToken.IsCancellationRequested)
            {
                await Dispatcher.RunAsync(CoreDispatcherPriority.High, () => { benchmarkTest.Add(row); });
            }
        }

        // Multithreading toggled
        private void Multithreading_Toggled(object sender, RoutedEventArgs e)
        {
            ToggleSwitch toggleSwitch = sender as ToggleSwitch;

            if (toggleSwitch != null)
            {
                if (toggleSwitch.IsOn == true)
                {
                    // Turn multithreading ON
                    localSettings.Values["multithreading_enabled"] = true;
                }
                else
                {
                    // Turn multithreading OFF
                    localSettings.Values["multithreading_enabled"] = false;
                }
            }
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (e.Parameter is BeerScraper)
            {
                // Reset the token

                this.beerScraper = (BeerScraper) e.Parameter;
            }
            base.OnNavigatedTo(e);
        }
    }
}