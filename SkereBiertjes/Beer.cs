using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace SkereBiertjes
{
    public class Beer
    {
        private string brand;
        private string title;
        private int volume;
        private int amountBottles;
        private string shop;
        private int priceNormalized;
        private string discount;
        private string url;

        public Beer(string brand, string title, int volume, int amountBottles, int priceNormalized, string discount, string shop, string url)
        {
            this.brand = brand;
            this.title = title;
            this.volume = volume;
            this.amountBottles = amountBottles;
            this.shop = shop;
            this.priceNormalized = priceNormalized;
            this.discount = discount;
            this.url = url;
        }

        public string getBrand()
        {
            return this.brand.Replace("'", "");
        }

        public string getTitle()
        {
            return this.title.Replace("'", "");
        }

        public int getVolume()
        {
            return this.volume;
        }
        public int getBottleAmount()
        {
            return this.amountBottles;
        }

        public int getNormalizedPrice()
        {
            return this.priceNormalized;
        }

        public string getDiscount()
        {
            return this.discount;
        }

        public string getShopName()
        {
            return this.shop;
        }

        public string getUrl()
        {
            return this.url;
        }

        public void printInfo()
        {
            bool debug = false;
            if (debug == true)
            {
                Debug.WriteLine("Brand: " + this.brand);
                Debug.WriteLine("Title: " + this.title);
                Debug.WriteLine("Volume: " + this.volume);
                Debug.WriteLine("Amount: " + this.amountBottles);
                Debug.WriteLine("shop: " + this.shop);
                Debug.WriteLine("priceNormalized: " + this.priceNormalized);
                Debug.WriteLine("discount: " + this.discount);
                Debug.WriteLine("url: " + this.url);
            }
        }
    }
}