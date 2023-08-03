using APIToken.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using APIToken.Utils;
using Newtonsoft.Json.Linq;

namespace APIToken.Controllers
{   
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly UserContext user_context;
        private readonly IConfiguration configuration;
        private readonly Token tokenUtil = new();

        public UserController(UserContext userContext, IConfiguration configuration)
        {
            user_context = userContext;
            this.configuration = configuration;
        }

        [HttpGet("validatesession")]
        public ActionResult ValidateUser()
        {
            String email = User.FindFirstValue(ClaimTypes.Email)!;
            String username = User.FindFirstValue(ClaimTypes.Name)!;
            String id = User.FindFirstValue("Id")!;
            if (id == null || email == null || username == null) { return BadRequest(); };
            return Ok(new { email, username, id });
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserModel>>> GetUsers()
        {
            if (user_context == null)
            {
                return NotFound();
            }
            return await user_context.UserRegistrations.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<UserModel>> GetUser(int id)
        {
            if (user_context == null) { return NotFound(); }
            var user = await user_context.UserRegistrations.FindAsync(id);
            if (user == null) { return NotFound(); }
            return Ok(user);
        }

        [HttpPatch("{id}")]
        public async Task<ActionResult> LogoutUser(int id)
        {
            if (id == 0) { return BadRequest(); };
            if (user_context == null) { return NotFound(); }
            var user = await user_context.UserRegistrations.FindAsync(id);
            if (user == null) { return NotFound(); }
            user.Token = "";
            user_context.Entry(user).State = EntityState.Modified;
            try
            {
                await user_context.SaveChangesAsync();
                String message = $"User with ID {id} logged off successfully.";
                return Ok(new { message });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<UserModel>> EditUser(UserModel user, int id)
        {
            if (id != user.User_id)
            {
                return BadRequest();
            }
            var key = configuration.GetSection("JWT:Key").Value;
            string token = tokenUtil.GenerateToken(user, Request, key);
            user.Token = token;
            user_context.Entry(user).State = EntityState.Modified;
            try
            {
                await user_context.SaveChangesAsync();
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteUser(int id)
        {
            try
            {
                if (user_context.UserRegistrations == null)
                {
                    return NotFound();
                }
                var user = await user_context.UserRegistrations.FindAsync(id);
                if (user == null)
                {
                    return NotFound();
                }
                user_context.UserRegistrations.Remove(user);
                await user_context.SaveChangesAsync();
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
