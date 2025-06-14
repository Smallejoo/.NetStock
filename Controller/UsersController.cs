using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using ArmyStockApp.Models;
using ArmyStockApp.Services;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace ArmyStockApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly UserService _service;

        public UsersController(UserService service)
        {
            _service = service;
        }

        //check if got one 
        [HttpGet("LogIn")]
        public async Task<IActionResult> LogIn([FromQuery] string userName,[FromQuery]string password)
        {
            
            var Authenticated =await _service.LogInCheckAsync(userName,password); // check if user exists in our MongoDB
            if (Authenticated != null)
            {
                var key = Encoding.UTF8.GetBytes("this_is_a_very_strong_key_12345678!"); //+ you need this key to be 16+ chars . 
                 // starting to work on a token for security and Authintication 
                var tokenHandler = new JwtSecurityTokenHandler();  // creating an object of a token 
                var tokenDescription = new SecurityTokenDescriptor // configing 
                {
                    Subject = new ClaimsIdentity(new[] { new Claim(ClaimTypes.Name, userName) }),
                    Expires = DateTime.UtcNow.AddHours(1),   // its life expectency 
                    Issuer = "ArmyStock",
                    SigningCredentials =new SigningCredentials(new SymmetricSecurityKey(key)
                    ,SecurityAlgorithms.HmacSha256Signature)
                };
                var token = tokenHandler.CreateToken(tokenDescription); // we actually create one 
                return Ok(new{token=tokenHandler.WriteToken(token)} );   // make a string out of it and send it to the front
            }
            
         return Unauthorized("Wrong password or username");
        }

        //update email 
        [HttpPatch("ChangeEmail")]
        public async Task<IActionResult> PatchEmail(LogInfo user,[FromQuery] string newEmail)
        {
            var Authenticated = await _service.LogInCheckAsync(user.userName, user.password);
            if (Authenticated!=null)
            {
                  var Success = await _service.PatchEmailAsync(user,newEmail);

                if (Success)
                {
                    return Ok(user);
                }
   
            }
           
            return BadRequest("Wrong password or username");
            
        }


        //update password 
        [HttpPatch("ChangePassword")]
        public async Task<IActionResult> PatchPassword(LogInfo user, string newPassword)
        {
            var Authenticated = await _service.LogInCheckAsync(user.userName, user.password);
           if (Authenticated != null)
            {
                var Success = await _service.PatchPasswordAsync(user, newPassword,false);
                if (Success)
                    return Ok(user);

            }
            return BadRequest("Wrong password or username"); 

        }
         [HttpPatch("ForgotPassword")]
        public async Task<IActionResult> PatchPasswordForgot(LogInfo user, string newPassword)
        {       // here insted of the password you will send email . 
            var Authenticated = await _service.LogInCheckForgotAsync(user.userName, user.password);
           if (Authenticated != null)
            {
                var Success = await _service.PatchPasswordAsync(user, newPassword,true);
                if (Success)
                    return Ok(user);

            }
            return BadRequest("Wrong password or username"); 

        }
        // make new user 
        [HttpPost]
        public async Task<IActionResult> PostNewUser(User user)
        {
            var Authenticated = await _service.LogInCheckAsync(user.userName, user.password);
            if (Authenticated == null)
            {
                var Success = await _service.CreateAsync(user);
                if (Success)
                    return Ok(user);
            }
              return BadRequest("UserName already taken"); 
        }
        // delete user 
        [HttpDelete]
        public async Task<IActionResult> Delete(User user)
        {
            var Authenticated = await _service.LogInCheckAsync(user.userName, user.password);
            if (Authenticated != null)
            {

                var Success = await _service.DeleteUserAsync(user.email);
                return  Ok("User deleted");
            }
            return BadRequest("No such user "); 

        }

    }

}