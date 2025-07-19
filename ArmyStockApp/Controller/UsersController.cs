using ArmyStockApp.Models;
using ArmyStockApp.Services;
using Microsoft.AspNetCore.Mvc;

namespace ArmyStockApp.Controllers;

/// <summary>
/// API endpoints for managing users and authentication.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly UserService _service;

    public UsersController(UserService service)
    {
        _service = service;
    }

    /// <summary>
    /// Authenticate a user.
    /// </summary>
    [HttpGet("login")]
    public async Task<IActionResult> Login([FromQuery] string userName, [FromQuery] string password)
    {
        var user = await _service.LogInCheckAsync(userName, password);
        if (user is null)
        {
            return Unauthorized();
        }
        return Ok(user);
    }

    /// <summary>
    /// Update a user's email address.
    /// </summary>
    [HttpPatch("changeEmail")]
    public async Task<IActionResult> ChangeEmail([FromBody] ChangeEmailRequest request)
    {
        var success = await _service.PatchEmailAsync(new User { UserName = request.UserName, Password = request.Password }, request.NewEmail);
        if (!success)
        {
            return BadRequest("Wrong password or username");
        }
        return Ok();
    }

    /// <summary>
    /// Create a new user.
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> Post(User user)
    {
        var created = await _service.CreateAsync(user);
        if (!created)
        {
            return Conflict("User or email already exists");
        }
        return Ok(user);
    }

    /// <summary>
    /// Delete a user by email address.
    /// </summary>
    [HttpDelete]
    public async Task<IActionResult> Delete([FromQuery] string email)
    {
        var deleted = await _service.DeleteUserAsync(email);
        if (!deleted)
        {
            return NotFound();
        }
        return Ok();
    }

    /// <summary>
    /// DTO for changing email.
    /// </summary>
    public record ChangeEmailRequest(string UserName, string Password, string NewEmail);
}

