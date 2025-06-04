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
        public async Task<IActionResult> Get(User user)
        {
            var existing =await _service.LogInCheckAsync(user.userName, user.password);
            if (existing != null)
            {
                return OK(user);    
            }
            
         return BadRequest("Wrong password or username");
        }

        //update email 
        [HttpPatch("ChangeEmail")]
        public async Task<IActionResult> PatchEmail(User user)
        {
            var Sucsess = await _service.PatchEmailAsync(user);

            if (Sucsess != null)
            {
                return OK(user);
            }

            return BadRequest("Wrong password or username");
            
        }
        

        //update password 
        [HttpPatch("ChangePassword")]
        public async Task<IActionResult> Patch(User user)
        {
            

        }
        // make new user 
        [HttpPost]
        public async Task<IActionResult> Post(User user)
        {

        }
        // delete user 
        [HttpDelete]
        public async Task<IActionResult> Delete(User user)
        {


        }

    }

}