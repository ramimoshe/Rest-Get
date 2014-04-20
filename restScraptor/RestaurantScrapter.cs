using HtmlAgilityPack;
using RestaurantScrapter.Scrapers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace restScraptor
{
    public class RestaurantScraptor
    {
        static void Main(string[] args)
        {
            var rest = new Rest();


            var restCategotries = rest.getRestaurantCategories();
            var restList = rest.GetResturants();


             
            
            Console.Read();  
        }
    }
}
