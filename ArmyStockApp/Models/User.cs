public class User
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; }

    public string UserName { get; set; }
    public string Email { get; set; }
    public string PasswordHash { get; set; }  // Store hashed password
}
