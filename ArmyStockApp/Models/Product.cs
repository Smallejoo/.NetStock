using System.ComponentModel.DataAnnotations;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
namespace ArmyStockApp.Models;
public class Product
{
    [BsonId, BsonRepresentation(BsonType.ObjectId)] public string? Id { get; set; }
    [Required, StringLength(120)] public string Name { get; set; } = "";
    [StringLength(2000)] public string Description { get; set; } = "";
    [Range(0, int.MaxValue)] public int Stock { get; set; }
    [Range(typeof(decimal), "0", "1000000000")] public decimal Price { get; set; }
}
