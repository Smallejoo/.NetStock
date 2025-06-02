using ArmyStockApp.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using ArmyStockApp.settings;


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
        public async Task<User> GetByEmailAsync(string email)
        {
            var theOne = await _Users.Find(u => u.email == email).FirstOrDefaultAsync();
            return theOne;
        }

        public async Task<User> GetByUserNameAsync(string userName)
        {
            var theOne = await _Users.Find(u => u.userName == userName).FirstOrDefaultAsync();
            return theOne;
        }

        public async Task<bool> CreateAsync(User user)
        {
            var existing = _Users.Find(u => u.userName == user.userName || u.email == user.email).FirstOrDefaultAsync();
            if (existing)
            {
                return false;
            }
            user.password = BCrypt.Net.BCrypt.HashPassword(user.password);
            await _Users.InsertOneAsync(user);
            return true;
        }
        public async Task<User> LogInCheckAsync(string userName, string password)
        {

            var theOne = await _Users.Find(u => u.userName == userName && u.password == password).FirstOrDefaultAsync();
            return theOne;

        }

        public async Task<bool> DeleteUserAsync(string email)
        {
            var existing = await _Users.Find(u => u.email == email).FirstOrDefaultAsync();
            if (existing==null)
            {
                return false;
            }
            _Users.DeleteAsync(u => u.email == email);
            return true;
        }

    }
}