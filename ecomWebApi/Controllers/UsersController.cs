using ecomWebApi.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace ecomWebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly MyDbContext dbContext;
        public UsersController(MyDbContext dbContext)
        {
            this.dbContext = dbContext;
        }
        [HttpPost]
        [Route("Registration")]
        public IActionResult Registration(UserDTO userDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Check if email already exists for the same role
            var objUser = dbContext.Users.FirstOrDefault(x => x.Email == userDTO.Email && x.Role == userDTO.Role);
            if (objUser != null)
            {
                return BadRequest($"User with email {userDTO.Email} and role {userDTO.Role} already exists");
            }

            // Add new user
            var newUser = new User
            {
                FirstName = userDTO.FirstName,
                LastName = userDTO.LastName,
                Email = userDTO.Email,
                Password = userDTO.Password,
                Role = userDTO.Role,
            };

            dbContext.Users.Add(newUser);
            dbContext.SaveChanges();

            // Return only necessary data
            return Ok(new
            {
                newUser.UserId,
                newUser.FirstName,
                newUser.LastName,
                newUser.Email,
                newUser.Role
            });
        }



        [HttpPost]
        [Route("Login")]
        public IActionResult Login(LoginDTO loginDTO)
        {
            var user = dbContext.Users.FirstOrDefault(x => x.Email == loginDTO.Email && x.Password == loginDTO.Password && x.Role == loginDTO.Role);

            if (user != null)
            {
                return Ok(new
                {
                    UserId = user.UserId,
                    Email = user.Email,
                    Role = user.Role
                });
            }

            return Unauthorized();
        }



        //Get all Users
        [HttpPost]
        [Route("GetUsers")]
        public IActionResult GetUsers()
        {
            return Ok(dbContext.Users.ToList());
        }

        [HttpPost]
        [Route("GetUser")]
        public IActionResult GetUser(int id)
        {
            var user = dbContext.Users.FirstOrDefault(x => x.UserId == id);
            if(user != null)
            {
                return Ok(user);
            }
            else
            {

            }
                return NoContent();
        }
    }
}
