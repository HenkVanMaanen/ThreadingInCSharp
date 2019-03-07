using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace SkereBiertjes
{
    public sealed partial class SettingsPage : Page
    {
        public SettingsPage()
        {
            this.InitializeComponent();
        }

        // Benchmark start button clicked
        private void BenchmarkStart_Click(object sender, RoutedEventArgs e)
        {

        }

        // Multithreading toggled
        private void Multithreading_Toggled(object sender, RoutedEventArgs e)
        {
            if (sender is ToggleSwitch toggleSwitch)
            {
                if (toggleSwitch.IsOn == true)
                {
                    // Turn multithreading ON
                }
                else
                {
                    // Turn multithreading OFF
                }
            }
        }
    }
}
