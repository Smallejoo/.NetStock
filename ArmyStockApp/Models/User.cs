using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ArmyStockApp.Models;

/// <summary>
/// Basic user record persisted in MongoDB.
/// </summary>
public class User
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = string.Empty;

    public string UserName { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Plain text password supplied during registration/login.
    /// Stored hashed in the database by <see cref="UserService"/>.
    /// </summary>
    public string Password { get; set; } = string.Empty;
}
