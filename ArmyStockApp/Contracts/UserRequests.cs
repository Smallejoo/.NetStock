using System.ComponentModel.DataAnnotations;
namespace ArmyStockApp.Contracts;
public record RegisterRequest(
    [Required, StringLength(64, MinimumLength = 3)] string UserName,
    [Required, EmailAddress] string Email,
    [Required, StringLength(64, MinimumLength = 12)] string Password);
public record LoginRequest([Required] string UserName, [Required] string Password);
public record ChangeEmailRequest([Required] string Password, [Required, EmailAddress] string NewEmail);
public record DeleteAccountRequest([Required] string Password);
public record UserResponse(string UserName, string Email);
