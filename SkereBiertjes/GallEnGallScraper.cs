﻿using System;
using System.Collections.Generic;
using HtmlAgilityPack;
using System.Diagnostics;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace SkereBiertjes
{
    public class GallEnGallScraper : Scraper
    {
        public string StandardURL;

        private List<Beer> beers;

        public GallEnGallScraper()
        {
            //set data
            beers = new List<Beer>();
            StandardURL = @"Data/gall&gall.html";
        }

        string Scraper.getHTML()
        {
            return null;
        }


        List<Beer> Scraper.parseHTML()
        {
            List<Beer> beers = new List<Beer>();
            //get document
            var doc = new HtmlDocument();
            doc.OptionFixNestedTags = true;
            doc.Load(StandardURL);

            //select all articles
            var nodes = doc.DocumentNode.SelectNodes("//article[contains(@class,'product-block')]");

            //check if nodes are not null
            if (nodes == null)
            {
                Debug.WriteLine("No nodes selected");
                return null;
            }
            //check if document node is not null
            if (doc.DocumentNode == null)
            {
                Debug.WriteLine("DocumentNode == null");
                return null;
            }

            //loop over all articles
            foreach (var node in nodes)
            {
                //if node is null skip it
                if (node != null)
                {
                    //parse data from node
                    Beer beer = this.parseData(node);

                    beers.Add(beer);

                    //print info from beer
                    beer.printInfo();
                }
            }
            return beers;
        }

        //parse the article node to a beer
        private Beer parseData(HtmlNode node)
        {
            //get the node where the image is
            HtmlNodeCollection imageNode = node.SelectNodes(".//img[contains(@itemprop,'image')]");
            string ImageURL = "";

            //check if is not empty
            if (imageNode != null)
            {
                ImageURL = imageNode["img"].Attributes["src"].Value;
            }

            //parse to json
            JObject json = JObject.Parse(node.Attributes["data-tracking-impression"].Value);

            //parse to usable data
            string name = json["name"].ToString();
            int volume = this.parseNameToVolume(name);
            int price = Convert.ToInt32(Math.Round(Convert.ToDouble(json["price"].ToString()) * 100));
            string discount = parseDiscount(node.InnerText);

            //create beer
            Beer beer = CreateBeer(name, volume, price, discount, ImageURL);

            return beer;
        }

        private string parseDiscount(string text)
        {
            text = text.Remove(0, text.IndexOf('=') + 2);
            text = text.Remove(text.IndexOf('\n'), text.Length - text.IndexOf('\n'));
            text = text.Replace("\\", "");
            text = text.Replace("=\"", "=\\\"");
            text = text.Replace("\">", "\\\">");
            text = text.Replace(";", "");
            JObject json = JObject.Parse(text);
            if (json["product"]["sticker"].ToString() != "False")
            {
                return json["product"]["sticker"]["title"].ToString();
            }
            return "";
        }

        public List<Beer> getBeers()
        {
            return beers;
        }

        //create a beer with Gall&Gall allready in it;
        private Beer CreateBeer(string brand, int volume, int priceNormalized, string discount, string url)
        {
            return new Beer(brand, volume, priceNormalized, discount, "Gall&Gall", url);
        }

        //bc volume is only accisble in name, parse name to get volume in ml
        private int parseNameToVolume(string title)
        {
            if(title == "")
            {
                return -1;
            }

            string[] words = title.Split(' ');

            //will parse everything ending in CL
            if (words[words.Length - 1].Contains("CL"))
            {
                string word = words[words.Length - 1];
                word = word.Remove(word.Length - 2, 2);
                return Convert.ToInt32(Math.Round(Convert.ToDouble(word) * 10)); 
            }
            return 0;
        }
    }
}