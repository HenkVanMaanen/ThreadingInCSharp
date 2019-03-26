using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace SkereBiertjes
{
    /// <summary>
    /// Filter page. Used for filtering search results on Shop, Brand and Type.
    /// </summary>
    public sealed partial class FilterPage : Page
    {

        private Filter filter;

        public FilterPage()
        {
            this.InitializeComponent();
        }

        private void setFilterState()
        {
            if (this.filter == null)
            {
                return;
            }

            string brand = this.filter.getBrand();

            if(brand != null)
            {
                List<string> lstItems = BrandComboBox.Items
                                .Select(item => item.ToString())
                                .ToList();

                BrandComboBox.SelectedIndex = lstItems.FindIndex(a => a.Equals(brand));
            }

            string shop = this.filter.getShop();

            if (shop != null)
            {
                List<string> lstItems = ShopComboBox.Items
                                .Select(item => item.ToString())
                                .ToList();

                ShopComboBox.SelectedIndex = lstItems.FindIndex(a => a.Equals(shop));
            }

            string type = this.filter.getType();

            if (type != null)
            {
                List<string> lstItems = TypeComboBox.Items
                                .Select(item => item.ToString())
                                .ToList();

                TypeComboBox.SelectedIndex = lstItems.FindIndex(a => a.Equals(type));
            }
        }

        // Fired when the Save button is clicked (Bottom of the page)
        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {

        }

        // Fired when the Shop selection changes
        private void ShopComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // e.AddedItems[0].ToString(); <-- Get the current selection
            this.filter.setShop(e.AddedItems[0].ToString());
        }

        // Fired when the Brand selection changes
        private void BrandComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            this.filter.setBrand(e.AddedItems[0].ToString());

        }

        // Fired when the Type selection changes
        private void TypeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            this.filter.setType(e.AddedItems[0].ToString());

        }
        
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (e.Parameter is Filter)
            {
                this.filter = (Filter) e.Parameter;
                this.setFilterState();
            }
            base.OnNavigatedTo(e);
        }
    }
}