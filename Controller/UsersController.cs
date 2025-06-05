using ArmyStockApp.Models;
using ArmyStockApp.Services;
using Microsoft.AspNetCore.Mvc;

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
        public async Task<IActionResult> LogIn(string userName,string password)
        {
            var Authenticated =await _service.LogInCheckAsync(userName,password);
            if (Authenticated != null)
            {
                return Ok(userName);    
            }
            
         return BadRequest("Wrong password or username");
        }

        //update email 
        [HttpPatch("ChangeEmail")]
        public async Task<IActionResult> PatchEmail(User user,string newEmail)
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
        public async Task<IActionResult> PatchPassword(User user, string newPassword)
        {
            var Authenticated = await _service.LogInCheckAsync(user.userName, user.password);
           if (Authenticated != null)
            {
                var Success = await _service.PatchPasswordAsync(user, newPassword);
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