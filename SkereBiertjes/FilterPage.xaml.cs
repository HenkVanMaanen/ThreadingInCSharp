using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace SkereBiertjes
{
    /// <summary>
    /// Filter page. Used for filtering search results on Shop, Brand and Type.
    /// </summary>
    public sealed partial class FilterPage : Page
    {

        public FilterPage()
        {
            this.InitializeComponent();
        }

        // Fired when the Save button is clicked (Bottom of the page)
        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {

        }

        // Fired when the Shop selection changes
        private void ShopComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // e.AddedItems[0].ToString(); <-- Get the current selection
        }

        // Fired when the Brand selection changes
        private void BrandComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        // Fired when the Type selection changes
        private void TypeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

    }
}