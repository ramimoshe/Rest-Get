using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestaurantScrapter.Model
{
    class Restaurant
    {
        [BsonElement("name")]
        public string Name { get; set; }
        [BsonElement("address")]
        public string Address { get; set; }
        [BsonElement("description")]
        public string Desc { get; set; }
        [BsonElement("classifications")]
        public List<string> Classifications { get; set; }
        [BsonElement("rating")]
        public string Rating { get; set; }
    }
}
