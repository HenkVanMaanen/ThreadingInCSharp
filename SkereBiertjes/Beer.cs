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
        private int volume;
        private int amountBottles;
        private string shop;
        private int priceNormalized;
        private string discount;
        private string url;

        public Beer(string brand, int volume, int amountBottles, int priceNormalized, string discount, string shop, string url)
        {
            this.brand = brand;
            this.volume = volume;
            this.amountBottles = amountBottles;
            this.shop = shop;
            this.priceNormalized = priceNormalized;
            this.discount = discount;
            this.url = url;
        }

        public string getBrand()
        {
            return this.brand;
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
            Debug.WriteLine("Brand: " + this.brand);
            Debug.WriteLine("Volume: " + this.volume);
            Debug.WriteLine("Amount: " + this.amountBottles);
            Debug.WriteLine("shop: " + this.shop);
            Debug.WriteLine("priceNormalized: " + this.priceNormalized);
            Debug.WriteLine("discount: " + this.discount);
            Debug.WriteLine("url: " + this.url);
        }
        
    }
}