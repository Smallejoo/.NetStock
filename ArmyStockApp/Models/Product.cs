using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;


namespace ArmyStockApp.Models
{
    public class Product
    {
        [BsonId] // main field 
        [BsonRepresentation(BsonType.ObjectId)] // mongo special ID . 
        public string Id { get; set; }

        public string Name { get; set; }
        public string Description { get; set; }
        public int Stock { get; set; }
        public decimal Price { get; set; }
    }
}
