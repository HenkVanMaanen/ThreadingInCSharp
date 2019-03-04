﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SkereBiertjes
{
    public class Beer
    {
        private string brand;
        private int volume;
        private string shop;
        private int priceNormalized;
        private string discount;
        private string url;

        public Beer(string brand, int volume, int priceNormalized, string discount, string shop, string url)
        {
            this.brand = brand;
            this.volume = volume;
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
        
    }
}