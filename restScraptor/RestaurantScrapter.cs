using HtmlAgilityPack;
using MongoDB.Bson;
using MongoDB.Driver;
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
            var restList = rest.GetResturants(5);

            var client = new MongoClient("mongodb://localhost:27017");
            var server = client.GetServer();
            var database = server.GetDatabase("Lucky");
            var collection = database.GetCollection("restaurants");

            collection.InsertBatch(restList);

            /*foreach (var document in collection.FindAll())
            {
                Console.WriteLine(document["name"]);
            }*/
             
            Console.Read();  
        }
    }
}
