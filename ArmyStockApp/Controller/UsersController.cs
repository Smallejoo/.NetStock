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
        [HttpsGet]
        public async Task<IActionResult> get(User user)
        {

        }
        //update email 
        [HttpsPut]
        public async Task<IActionResult> put(User user)
        {

        }
        //update password 
        [HttpsPut]
        public async Task<IActionResult> put(User user)
        {

        }
        // make new user 
        // delete user 



    }

}