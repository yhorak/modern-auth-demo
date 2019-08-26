using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Authentication;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using VsJwtAuthApp.Models;

namespace VsJwtAuthApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        [HttpPost("login")]
        public async Task<ActionResult<LoginResult>> Login([FromBody] LoginModel model,
            [FromServices] SignInManager<IdentityUser> signInManager,
            [FromServices] UserManager<IdentityUser> userManager,
            [FromServices] IConfiguration configuration
        )
        {
            var result = await signInManager.PasswordSignInAsync(model.Login, model.Password, false, false);

            if (!result.Succeeded) return BadRequest("User with such login and password does not exist");

            var user = await userManager.FindByNameAsync(model.Login);

            return new LoginResult
            {
                Token = generateToken(model.Login, user.Id, configuration)
            };
        }

        [HttpPost]
        public void Logout([FromBody] string value)
        {
            // we should ask our client to delete his key
        }

        private string generateToken(string login, string id, IConfiguration configuration)
        {
            var issuer = configuration["Jwt:Issuer"];
            var audience = configuration["Jwt:Audience"];
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"]));

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, "Demo"),
                new Claim(JwtRegisteredClaimNames.UniqueName, id),
                new Claim(ClaimTypes.NameIdentifier, login),
            };

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expires = DateTime.Now.AddHours(2);

            var token = new JwtSecurityToken(
                issuer,
                audience,
                claims,
                expires: expires,
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}