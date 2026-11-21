using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using ArmyStockApp.Models;
using ArmyStockApp.Services;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Xunit;

public class ApiTests
{
    private static WebApplicationFactory<Program> Factory() => new WebApplicationFactory<Program>().WithWebHostBuilder(b => b.ConfigureServices(s => {
        s.RemoveAll<IUserService>(); s.AddSingleton<IUserService, MemoryUsers>();
    }));
    private static async Task<string> RegisterAndLogin(HttpClient client, string name = "alice")
    {
        var registration = await client.PostAsJsonAsync("/api/Users", new { userName=name, email=name+"@example.com", password="LongDemoPassword123" });
        Assert.Equal(HttpStatusCode.Created, registration.StatusCode);
        var json=await registration.Content.ReadAsStringAsync();
        Assert.DoesNotContain("password", json, StringComparison.OrdinalIgnoreCase);
        var login=await client.PostAsJsonAsync("/api/Users/login",new {userName=name,password="LongDemoPassword123"});
        Assert.Equal(HttpStatusCode.OK,login.StatusCode);
        return JsonDocument.Parse(await login.Content.ReadAsStringAsync()).RootElement.GetProperty("accessToken").GetString()!;
    }
    [Fact] public async Task LoginReturnsTokenAndMeNeverExposesPassword()
    {
        using var factory=Factory(); using var c=factory.CreateClient();
        var token=await RegisterAndLogin(c);
        c.DefaultRequestHeaders.Authorization=new("Bearer",token);
        var response=await c.GetAsync("/api/Users/me");
        Assert.Equal(HttpStatusCode.OK,response.StatusCode);
        var body=await response.Content.ReadAsStringAsync();
        Assert.Contains("alice",body); Assert.DoesNotContain("password",body,StringComparison.OrdinalIgnoreCase);
    }
    [Fact] public async Task IncorrectPasswordAndTamperedTokenAreRejected()
    {
        using var factory=Factory(); using var c=factory.CreateClient();
        await RegisterAndLogin(c);
        var response=await c.PostAsJsonAsync("/api/Users/login",new {userName="alice",password="wrong"});
        Assert.Equal(HttpStatusCode.Unauthorized,response.StatusCode);
        c.DefaultRequestHeaders.Authorization=new("Bearer","invalid-token");
        Assert.Equal(HttpStatusCode.Unauthorized,(await c.GetAsync("/api/Users/me")).StatusCode);
    }
    [Theory]
    [InlineData("GET","/api/Products")]
    [InlineData("POST","/api/Products")]
    [InlineData("PUT","/api/Products/012345678901234567890123")]
    [InlineData("DELETE","/api/Products/012345678901234567890123")]
    [InlineData("PATCH","/api/Users/changeEmail")]
    [InlineData("DELETE","/api/Users/me")]
    public async Task AnonymousRequestsCannotManageData(string method,string url)
    {
        using var factory=Factory(); using var c=factory.CreateClient();
        using var request=new HttpRequestMessage(new HttpMethod(method),url){Content=JsonContent.Create(new {})};
        Assert.Equal(HttpStatusCode.Unauthorized,(await c.SendAsync(request)).StatusCode);
    }
    [Fact] public async Task AccountChangesUseSignedInIdentity()
    {
        using var factory=Factory(); using var c=factory.CreateClient();
        var alice=await RegisterAndLogin(c); await RegisterAndLogin(c,"bob");
        c.DefaultRequestHeaders.Authorization=new("Bearer",alice);
        var patch=await c.PatchAsJsonAsync("/api/Users/changeEmail",new {userName="bob",password="LongDemoPassword123",newEmail="new@example.com"});
        Assert.Equal(HttpStatusCode.NoContent,patch.StatusCode);
        var users=factory.Services.GetRequiredService<IUserService>();
        Assert.Equal("new@example.com",(await users.GetByUserNameAsync("alice"))!.Email);
        Assert.Equal("bob@example.com",(await users.GetByUserNameAsync("bob"))!.Email);
    }
    [Fact] public async Task RegistrationValidatesInputAndRejectsDuplicate()
    {
        using var factory=Factory(); using var c=factory.CreateClient();
        Assert.Equal(HttpStatusCode.BadRequest,(await c.PostAsJsonAsync("/api/Users",new {userName="a",email="bad",password="short"})).StatusCode);
        await RegisterAndLogin(c);
        Assert.Equal(HttpStatusCode.Conflict,(await c.PostAsJsonAsync("/api/Users",new {userName="alice",email="alice@example.com",password="LongDemoPassword123"})).StatusCode);
    }
    [Fact] public async Task QueryStringLoginIsNotAnEndpoint()
    {
        using var factory=Factory(); using var c=factory.CreateClient();
        Assert.Equal(HttpStatusCode.MethodNotAllowed,(await c.GetAsync("/api/Users/login?userName=alice&password=secret")).StatusCode);
    }
    private class MemoryUsers : IUserService
    {
        private readonly Dictionary<string,User> users=new();
        public Task<bool> CreateAsync(User u){if(users.ContainsKey(u.UserName)||users.Values.Any(x=>x.Email==u.Email))return Task.FromResult(false);u.Id=Guid.NewGuid().ToString();u.Password=BCrypt.Net.BCrypt.HashPassword(u.Password);users[u.UserName]=u;return Task.FromResult(true);}
        public Task<User?> GetByUserNameAsync(string n)=>Task.FromResult(users.GetValueOrDefault(n));
        public async Task<User?> LogInCheckAsync(string n,string p){var u=await GetByUserNameAsync(n);return u is not null&&BCrypt.Net.BCrypt.Verify(p,u.Password)?u:null;}
        public async Task<bool> PatchEmailAsync(User u,string e){var found=await LogInCheckAsync(u.UserName,u.Password);if(found is null)return false;found.Email=e;return true;}
        public Task<bool> DeleteUserAsync(string e){var u=users.Values.FirstOrDefault(x=>x.Email==e);return Task.FromResult(u is not null&&users.Remove(u.UserName));}
    }
}
