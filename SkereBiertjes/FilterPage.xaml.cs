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
            //set all filters to the state it was in.
            if (this.filter == null)
            {
                return;
            }

            string brand = this.filter.getBrand();

            if (brand != null)
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

        /**
         * Change the prefered shop (on UI combobox selection changed)
         */
        private void ShopComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string shop = e.AddedItems[0].ToString(); // Get the current selection
            if (shop.ToLower().Contains("geen winkel"))
            {
                this.filter.setShop("");
            }
            else
            {
                this.filter.setShop(shop);
            }
        }

        /**
         * Change the prefered brand (on UI combobox selection changed)
         */
        private void BrandComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string brand = e.AddedItems[0].ToString(); // Get the current selection

            if (brand.ToLower().Contains("geen merk"))
            {
                this.filter.setBrand("");
            }
            else
            {
                this.filter.setBrand(brand);
            }
        }

        /**
         * Change the prefered volume of beer (on UI combobox selection changed)
         */
        private void TypeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {            
            string type = e.AddedItems[0].ToString(); // Get the current selection

            if (type.ToLower().Contains("geen type"))
            {
                this.filter.setType("");
            }
            else
            {
                this.filter.setType(type);
            }
        }
        
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            //use only one filter object for all pages, this will make sure the filter will always be the same
            if (e.Parameter is Filter)
            {
                this.filter = (Filter) e.Parameter;
                this.setFilterState();
            }
            base.OnNavigatedTo(e);
        }
    }
}