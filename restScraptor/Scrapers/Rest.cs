using HtmlAgilityPack;
using RestaurantScrapter.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace RestaurantScrapter.Scrapers
{
    class Rest //: IRestaurantScrapter
    {
        //"http://www.rest.co.il/Search.aspx?regions=4&city=172"
        public static readonly string tempUrl = "http://www.rest.co.il/Search.aspx?regions={0}&city={1}";
        public static readonly string baseUrl = "http://www.rest.co.il/";
        public static readonly string allRestaurantsByPage = "http://www.rest.co.il/search.aspx?page={0}";
        public static readonly string categoriesUrl = "http://www.rest.co.il/ajax/SearchFilerAllItems.aspx?page=1&filter=types&multi=1&pagename=Search.aspx&_=1397994066119";
        public static readonly string restaurantInfoUrl = "http://www.rest.co.il/Reviews.aspx?restId={0}";
        // example: http://www.rest.co.il/Reviews.aspx?restId=7858

        public Dictionary<string, string> getRestaurantCategories()
        {
            Dictionary<string, string> categoriesResult = new Dictionary<string, string>();

            HtmlWeb hw = new HtmlWeb();
            hw.OverrideEncoding = Encoding.GetEncoding("ISO-8859-8");
            HtmlDocument doc = hw.Load(categoriesUrl);

            var baseAnchor = "//div[@id='divItems']";
            var categoriesNodes = doc.DocumentNode.SelectSingleNode(baseAnchor).SelectNodes(".//a");

            foreach (var item in categoriesNodes)
            {
                var categoryId = item.Attributes.First(c => c.Name == "id").Value;
                categoriesResult.Add(categoryId, item.InnerHtml);
                
            }

            return categoriesResult;
 
        }

        public List<Restaurant> GetResturants(int numberOfPages)
        { 
            int i=1;
            List<Restaurant> restaurants = new List<Restaurant>();
            List<Restaurant> partialRestaurants = GetResturantsByPage(i);

            while (partialRestaurants != null && i < numberOfPages)
            {
                restaurants.AddRange(partialRestaurants);
                partialRestaurants = GetResturantsByPage(++i);
                Console.WriteLine("Page number: " + i);
            }

            return restaurants;
        }


        private List<Restaurant> GetResturantsByPage(int pageNumber)
        {
            List<Restaurant> rests = new List<Restaurant>();

            HtmlWeb hw = new HtmlWeb();
            hw.OverrideEncoding = Encoding.GetEncoding("ISO-8859-8");
            HtmlDocument doc = hw.Load(String.Format(allRestaurantsByPage, pageNumber));

            var baseAddress = "//div[@class='item_box']";

            HtmlNodeCollection rest = doc.DocumentNode.SelectNodes(baseAddress);
            if (rest == null) return null;

            var restInfo = rest.ToList();

            foreach (var item in rest)
            {
                var nameNode = item.SelectSingleNode(".//div[@class='name']");
                if (nameNode == null) continue;

                var name = nameNode.SelectSingleNode(".//a").InnerText;
                var descNode = item.SelectSingleNode(".//div[@class='descript']").SelectSingleNode(".//a");
                string desc = "";
                if (descNode != null)
                    desc = descNode.InnerText;
                var address = item.SelectSingleNode(".//div[@class='contact']").SelectSingleNode(".//a").InnerText;
                var classificationsNode = item.SelectSingleNode(".//div[@class='category']");
                var classifications = new Dictionary<Guid, string>();
                if (classificationsNode != null)
                    classifications = item.SelectSingleNode(".//div[@class='category']").SelectNodes(".//span").ToDictionary(k => Guid.NewGuid(), v => v.InnerText);

                var restIdNode = item.SelectSingleNode(".//div[@class='item_img']").SelectSingleNode(".//a");
                string restId = "";
                if (restIdNode == null)
                    continue;
                restId = restIdNode.GetAttributeValue("restid",null);
                if (restId == null)
                    continue;

                string rating = GetResturantRating(restId);
                if (restId != null && rating != null)
                    rests.Add(new Restaurant() { Name = name, Desc = desc, Address = address, Classifications = classifications.Values.ToList(), Rating = rating });
            }

            return rests;
        }

        public string GetResturantRating(string restId)
        {
            HtmlWeb hw = new HtmlWeb();
            hw.PreRequest = OnPreRequest;
            hw.OverrideEncoding = Encoding.GetEncoding("ISO-8859-8");
            HtmlDocument doc = hw.Load(String.Format(restaurantInfoUrl, restId));
            //itemprop="ratingValue"
            var ratingElement = "//span[@itemprop='ratingValue']";

            if (IsRedirect(doc) == true)
                return null;

            return doc.DocumentNode.SelectSingleNode(ratingElement).InnerText;
        }

        private static bool OnPreRequest(HttpWebRequest request)
        {
            request.AllowAutoRedirect = false;
            return true;
        }

        private bool IsRedirect(HtmlDocument doc)
        {
            if (doc.DocumentNode.SelectSingleNode(".//title").InnerText == "Object moved") 
                return true;
            return false;
        }
    }
}
