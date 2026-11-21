using System.Security.Claims;
using ArmyStockApp.Contracts;
using ArmyStockApp.Models;
using ArmyStockApp.Services;
using Microsoft.AspNetCore.Authentication.BearerToken;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
namespace ArmyStockApp.Controllers;
[ApiController]
[Route("api/[controller]")]
public class UsersController(IUserService service) : ControllerBase
{
    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginRequest request)
    {
        var user = await service.LogInCheckAsync(request.UserName, request.Password);
        if (user is null) return Unauthorized();
        var identity = new ClaimsIdentity(new[] {
            new Claim(ClaimTypes.NameIdentifier, user.Id), new Claim(ClaimTypes.Name, user.UserName)
        }, BearerTokenDefaults.AuthenticationScheme);
        return SignIn(new ClaimsPrincipal(identity), BearerTokenDefaults.AuthenticationScheme);
    }
    [HttpPost]
    public async Task<IActionResult> Register(RegisterRequest request)
    {
        var user = new User { UserName = request.UserName, Email = request.Email, Password = request.Password };
        if (!await service.CreateAsync(user)) return Conflict("User or email already exists");
        return StatusCode(StatusCodes.Status201Created, new UserResponse(user.UserName, user.Email));
    }
    [Authorize, HttpGet("me")]
    public async Task<IActionResult> Me()
    {
        var user = await service.GetByUserNameAsync(User.Identity!.Name!);
        return user is null ? NotFound() : Ok(new UserResponse(user.UserName, user.Email));
    }
    [Authorize, HttpPatch("changeEmail")]
    public async Task<IActionResult> ChangeEmail(ChangeEmailRequest request)
    {
        var success = await service.PatchEmailAsync(new User {
            UserName = User.Identity!.Name!, Password = request.Password
        }, request.NewEmail);
        return success ? NoContent() : Unauthorized();
    }
    [Authorize, HttpDelete("me")]
    public async Task<IActionResult> DeleteMe(DeleteAccountRequest request)
    {
        var user = await service.LogInCheckAsync(User.Identity!.Name!, request.Password);
        if (user is null) return Unauthorized();
        return await service.DeleteUserAsync(user.Email) ? NoContent() : NotFound();
    }
}
