using ArmyStockApp.Services;
using ArmyStockApp.settings;
using Microsoft.AspNetCore.Authentication.BearerToken;
using MongoDB.Driver;

var builder = WebApplication.CreateBuilder(args);
builder.Services.Configure<MongoDbSettings>(builder.Configuration.GetSection("MongoDB"));
builder.Services.AddSingleton<IMongoClient>(_ => new MongoClient(
    builder.Configuration["MongoDB:ConnectionString"] ?? "mongodb://localhost:27017"));
builder.Services.AddSingleton<IUserService, UserService>();
builder.Services.AddSingleton<ProductService>();
builder.Services.AddControllers();
builder.Services.AddAuthentication(BearerTokenDefaults.AuthenticationScheme).AddBearerToken();
builder.Services.AddAuthorization();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
var app = builder.Build();
if (app.Environment.IsDevelopment()) { app.UseSwagger(); app.UseSwaggerUI(); }
else { app.UseHttpsRedirection(); }
app.UseDefaultFiles();
app.UseStaticFiles();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();
public partial class Program { }
