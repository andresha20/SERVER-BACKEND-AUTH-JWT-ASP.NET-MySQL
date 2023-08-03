using APIToken.Models;
using APIToken.Utils;
using APIToken.Validations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace APIToken.Controllers
{
    public class RequestBody
    {
        public string? User_email { get; set; }
        public string? Password { get; set; }
    }

    [ApiController]
    [Route("api/auth/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly UserContext user_context;
        private readonly IConfiguration configuration;
        private readonly Token tokenUtil = new();

        public AuthController(UserContext userContext, IConfiguration configuration)
        {
            this.user_context = userContext;
            this.configuration = configuration;  
        }

        [HttpPost("login")]
        public async Task<ActionResult> Login(RequestBody body)
        {
            try
            {
                if (string.IsNullOrEmpty(body.User_email) || string.IsNullOrEmpty(body.Password)) { return BadRequest(); }
                var userInDB = await user_context.UserRegistrations.FirstAsync(u => u.User_email == body.User_email);
                if (userInDB == null) { return BadRequest(); }
                Validator validator = new();
                var isValidPassword = validator.VerifyPassword(body.Password, userInDB.Password, Convert.FromHexString(userInDB.Salt));
                if (!isValidPassword) { return Unauthorized(); }
                var key = configuration.GetSection("JWT:Key").Value;
                string token = tokenUtil.GenerateToken(userInDB, Request, key); 
                userInDB.Token = token;
                await user_context.SaveChangesAsync();
                return Ok(new { token });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("signup")]
        public async Task<ActionResult> CreateUser(UserModel user)
        {
            if (user == null) return BadRequest();
            try
            {
                var doesUserExist = await user_context.UserRegistrations.AsQueryable().Where(u => u.User_email == user.User_email).Select(u => u.User_id).FirstOrDefaultAsync();
                if (doesUserExist > 0) { return Conflict(); }
                Validator validator = new();
                var hashedPassword = validator.EncodePassword(user.Password, out var salt);
                user.Password = hashedPassword;
                user.Salt = salt;
                var key = configuration.GetSection("JWT:Key").Value;
                string token = tokenUtil.GenerateToken(user, Request, key);
                user.Token = token;
                user_context.UserRegistrations.Add(user);
                await user_context.SaveChangesAsync();
                return Ok(new { token } );
            } catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
