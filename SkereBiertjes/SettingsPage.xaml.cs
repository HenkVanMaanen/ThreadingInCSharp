using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace SkereBiertjes
{
    public sealed partial class SettingsPage : Page
    {
        private ApplicationDataContainer localSettings = ApplicationData.Current.LocalSettings;

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
        private void BenchmarkStart_Click(object sender, RoutedEventArgs e)
        {
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
    }
}