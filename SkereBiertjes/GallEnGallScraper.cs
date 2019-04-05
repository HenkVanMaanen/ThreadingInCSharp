using System;
using System.Collections.Generic;
using HtmlAgilityPack;
using System.Diagnostics;
using Newtonsoft.Json.Linq;

using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace SkereBiertjes
{
    public class GallEnGallScraper : Scraper
    {
        public string StandardURL;
        private string keyword = "bier";
        private List<Beer> beers;

        public GallEnGallScraper()
        {
            //set data
            beers = new List<Beer>();
            StandardURL = @"Data/gall&gall.html";
        }

        async Task<List<string>> getHTML()
        {
            var pages = new List<string>();

            using (var httpClient = new HttpClient())
            {
                

                for (var p = 1; p < 18; p++)
                {
                    var response = await httpClient.GetAsync("https://www.gall.nl/shop/" + keyword + "/?page=" + p);
                    pages.Add(await response.Content.ReadAsStringAsync());
                }

                return pages;
            }
        }


        async Task<List<Beer>> Scraper.parseHTML()
        {
            List<Beer> beers = new List<Beer>();
            //get document
            List<string> pages = await getHTML();

            foreach (string page in pages)
            {

                var doc = new HtmlDocument();
                doc.OptionFixNestedTags = true;
                doc.LoadHtml(page);

                //select all articles
                var nodes = doc.DocumentNode.SelectNodes("//article[contains(@class,'product-block')]");
                //check if nodes are not null
                if (nodes == null)
                {
                    Debug.WriteLine("No nodes selected");
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
            }
            
            return beers;
        }

        List<Beer> Scraper.getBeers()
        {
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
            string title = json["name"].ToString();
            string brand = this.parseToBrand(title);
            int volume = this.parseNameToVolume(title);
            int price = Convert.ToInt32(Math.Round(Convert.ToDouble(json["price"].ToString())));

            JObject jsonParsed;
            string discount;
            int bottleAmount;

            try
            {
                jsonParsed = parseToJson(node.InnerText);
                if (jsonParsed == null)
                {
                    if (node.InnerText.Contains("24"))
                    {
                        bottleAmount = 24;
                    }else if (node.InnerText.Contains("12"))
                    {
                        bottleAmount = 12;
                    }
                    else if (node.InnerText.Contains("6"))
                    {
                        bottleAmount = 6;
                    }
                    else
                    {
                        bottleAmount = 1;
                    }
                    discount = "";
                } else
                {
                    discount = parseToDiscount(jsonParsed);
                    bottleAmount = parseToAmountDiscount(jsonParsed);
                }
            }
            catch
            {
                discount = "";
                bottleAmount = 1;
            }

            //create beer
            Beer beer = CreateBeer(brand, title, volume, bottleAmount, price, discount, ImageURL);

            return beer;
        }

        //create a beer with Gall&Gall allready in it;
        private Beer CreateBeer(string brand, string title, int volume, int bottleAmount, int priceNormalized, string discount, string url)
        {
            return new Beer(brand, title, volume, bottleAmount, priceNormalized, discount, "Gall en Gall", url);
        }

        private string parseToBrand(string title)
        {
            List<string> brands = new List<string>{
                "Grolsch",
                "Heineken",
                "Amstel",
                "Bavaria",
                "Jupiler",
            };
            
            foreach(string brand in brands)
            {
                if (title.ToLower().Contains(brand.ToLower()))
                {
                    return brand;
                }
            }

            return "";
        }

        //bc volume is only accisble in name, parse name to get volume in ml
        private int parseNameToVolume(string title)
        {
            if (title == "")
            {
                return 300;
            }

            string[] words = title.Split(' ');

            //will parse everything ending in CL
            if (words[words.Length - 1].Contains("CL"))
            {
                try
                {
                    string word = words[words.Length - 1];
                    if (word.ToLower().Contains("x"))
                    {
                        words = word.ToLower().Split('x');
                        words = words[1].ToLower().Split("cl");
                        return Convert.ToInt32(Math.Round(Convert.ToDouble(words[0]) * 10));
                    }
                    else
                    {
                        word = word.Remove(word.Length - 2, 2);
                        return Convert.ToInt32(Math.Round(Convert.ToDouble(word) * 10));
                    }
                } catch
                {
                    return 300;
                }
            }
            return 300;
        }

        private JObject parseToJson(string text)
        {
            if(text.IndexOf("data-tracking-click=") > 0)
            {
                text = text.Remove(0, text.IndexOf("data-tracking-click=") + "data-tracking-click=".Length + 1);
                text = text.Remove(text.IndexOf('\n'), text.Length - text.IndexOf('\n'));
                text = text.Replace("\\", "");
                text = text.Replace("=\"", "=\\\"");
                text = text.Replace("\">", "\\\">");
                text = text.Replace(";", "");
                return JObject.Parse(text);
            }
            return null;
        }

        private string parseToDiscount(JObject json)
        {
            if (json["product"]["sticker"].ToString() != "False")
            {
                return json["product"]["sticker"]["title"].ToString();
            }
            return "";
        }

        private int parseToAmountDiscount(JObject json)
        {
            if (json["availability"]["available_as_package"].ToString() == "False")
            {
                return 1;
            }
            return Convert.ToInt32(json["availability"]["available_as_package"]["qos"].ToString());
        }
    }
}