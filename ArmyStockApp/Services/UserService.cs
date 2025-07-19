using ArmyStockApp.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using ArmyStockApp.settings;

/// <summary>
/// Service providing basic CRUD operations for <see cref="User"/> documents.
/// </summary>


namespace ArmyStockApp.Services
{

    public class UserService
    {
        private readonly IMongoCollection<User> _Users;

        public UserService(IOptions<MongoDbSettings> settings, IMongoClient client)
        {
            var db = client.GetDatabase(settings.Value.DatabaseName);
            _Users = db.GetCollection<User>("Users");
        }

        public async Task<bool> PatchEmailAsync(User user, string newEmail)
        {
           var existing = await LogInCheckAsync(user.UserName, user.Password);
            if (existing != null)
            {
                var filterd = Builders<User>.Filter.Eq(u => u.Id, existing.Id);
                var update = Builders<User>.Update.Set(u => u.Email, newEmail);

                var result = await _Users.UpdateOneAsync(filterd, update);
                return result.ModifiedCount > 0;
            }
            return false;
        }

        public async Task<User> GetByEmailAsync(string email)
        {
            var theOne = await _Users.Find(u => u.Email == email).FirstOrDefaultAsync();
            return theOne;
        }

        public async Task<User> GetByUserNameAsync(string userName)
        {
            var theOne = await _Users.Find(u => u.UserName == userName).FirstOrDefaultAsync();
            return theOne;
        }

        public async Task<bool> CreateAsync(User user)
        {
            var existing = await _Users.Find(u => u.UserName == user.UserName || u.Email == user.Email).FirstOrDefaultAsync();
            if (existing== null)
            {
                user.Password = BCrypt.Net.BCrypt.HashPassword(user.Password);
                await _Users.InsertOneAsync(user);
                return true;
            }
            return false;
        }
        public async Task<User?> LogInCheckAsync(string userName, string password)
        {
            var theOne = await _Users.Find(u => u.UserName == userName).FirstOrDefaultAsync();
            if (theOne != null && BCrypt.Net.BCrypt.Verify(password, theOne.Password))
            {
                return theOne;
            }
            return null;
        }

        public async Task<bool> DeleteUserAsync(string email)
        {
            var existing = await _Users.Find(u => u.Email == email).FirstOrDefaultAsync();
            if (existing == null)
            {
                return false;
            }
            await _Users.DeleteOneAsync(u => u.Email == email);
            return true;
        }

    }
}