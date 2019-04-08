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

            // Set the multithreading Toggle button to the correct value from local settings
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

        /**
         * Start the benchmark (on UI button click)
         */
        private async void BenchmarkStart_Click(object sender, RoutedEventArgs e)
        {
            cancellationTokenSource.Cancel();

            List<string> data = new List<string>();
            this.beerScraper.setBenchmarkData(data);

            Task T1 = new Task(() => {
                beerScraper.startFindingFirstBeers(true);
            });

            T1.Start();


            cancellationTokenSource.Dispose();
            cancellationTokenSource = new CancellationTokenSource();

            int totalGetting = 0;
            int totalParsing = 0;
            int totalGettingDone = 0;
            int totalParsingDone = 0;
            bool multithread = (bool) localSettings.Values["multithreading_enabled"];
            while (true)
            {
                if (data.Count > 0)
                {
                    foreach (string txt in data.ToList())
                    {
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

                            if (totalGettingDone == this.beerScraper.getScrapers().Count)
                            {
                                data.Add("[Getting Total] " + totalGetting + "ms");
                            }

                        }
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

                            if (totalGettingDone == this.beerScraper.getScrapers().Count)
                            {
                                data.Add("[Parsing Total] " + totalParsing + "ms");
                            }
                        }
                        data.Remove(txt);
                    }
                }
                await Task.Delay(100);
            }
        }

        private async Task UpdateBenchMarkGrid(string row, CancellationToken cancellationToken)
        {
            // UI THREAD STUFF
            if (!cancellationToken.IsCancellationRequested)
            {
                await Dispatcher.RunAsync(CoreDispatcherPriority.High, () => { benchmarkTest.Add(row); });
            }
        }

        /**
         * Toggle multithreading (on UI button click)
         */
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