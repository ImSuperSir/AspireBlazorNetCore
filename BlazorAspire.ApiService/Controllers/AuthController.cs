using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BlazorAspire.Model.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace BlazorAspire.ApiService.Controllers
{
    [Route("api/[controller]")]
    // [Authorize(Roles="Admin")]
    [ApiController]
    public class AuthController(IConfiguration configuration) : ControllerBase
    {

        [HttpPost("login")]
        public ActionResult<LoginResponseModel> Login([FromBody] LoginModel model)
        {
            if ((model.UserName == "admin" && model.Password == "admin")
            || model.UserName == "user" && model.Password == "user")
            {
                var token = GenerateJwtToken(model.UserName);
                // var tokenExpired = DateTime.Now.AddMinutes(30).Ticks;
                return Ok(new LoginResponseModel { Token = token });
            }
            return Unauthorized();
        }

        private string GenerateJwtToken(string userName)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.Name, userName),
                new Claim(ClaimTypes.Role, userName == "admin" ? "Admin" : "User")
            };

            string secret = configuration.GetValue<string>("Jwt:Secret")!;
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: "laluro", //configuration.GetValue<string>("Jwt:Issuer"),
                audience: "laluro", //configuration.GetValue<string>("Jwt:Audience"),
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(30),
                signingCredentials: creds
            );

            
            return new JwtSecurityTokenHandler().WriteToken(token);

            // Generate JWT token
        }

    }
}
