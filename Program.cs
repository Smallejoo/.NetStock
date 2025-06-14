using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MongoDB.Driver;                 // for MongoClient
using ArmyStockApp.Services;         // for ProductService
using ArmyStockApp.settings;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var key = "dev-secret-key";
var issuer = "your-app";



var builder = WebApplication.CreateBuilder(args);


builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = false,
            ValidateIssuerSigningKey = true,
            ValidIssuer = issuer,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key))
        };
    });

builder.Services.Configure<MongoDbSettings>(
    builder.Configuration.GetSection("MongoDB")); // look up at appsettings for MongoDB settings . 

builder.Services.AddSingleton<IMongoClient>(s =>
    new MongoClient(builder.Configuration.GetSection("MongoDB:ConnectionString").Value));
// adding to config mongo settings 


// 👇 Register services here
builder.Services.AddControllers(); // adds connection to ur apies . 

builder.Services.AddSingleton<UserService>();
builder.Services.AddSingleton<ProductService>();// adding our handeling layer.



var app = builder.Build();


// Middleware
app.UseAuthentication();  // 🔑 Check who the user is (token, etc.)
app.UseAuthorization();   // 🛂 Check if user is allowed to access specific route


app.MapControllers();    // Map controller routes to app 


app.UseStaticFiles(); // enable HTML/CSS/JS serving

app.Run();
