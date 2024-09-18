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
                var token = GenerateJwtToken(model.UserName, isRefreshToken: false);
                var refreshToken = GenerateJwtToken(model.UserName, isRefreshToken: true);
                // var tokenExpired = DateTime.Now.AddMinutes(30).Ticks;
                return Ok(new LoginResponseModel
                {
                    Token = token,
                    RefreshToken = refreshToken,
                    TokenExpired = DateTimeOffset.UtcNow.AddHours(1).ToUnixTimeSeconds()
                });
            }
            return Unauthorized();
        }

        [HttpGet("loginByRefreshToken")]
        public ActionResult<LoginResponseModel> LoginByRefreshToken(string refreshToken)
        {
            var secret = configuration.GetValue<string>("Jwt:RefreshTokenSecret");
            var claimsPrincipal = GetClaimsPrinciplaFromToken(refreshToken, secret);
            if (claimsPrincipal != null)
            {
                return new StatusCodeResult(StatusCodes.Status401Unauthorized);
                // var userName = claimsPrincipal.FindFirst(ClaimTypes.Name)?.Value;
                // var token = GenerateJwtToken(userName, isRefreshToken: false);
                // var newRefreshToken = GenerateJwtToken(userName, isRefreshToken: true);
                // return Ok(new LoginResponseModel
                // {
                //     Token = token,
                //     RefreshToken = newRefreshToken,
                //     TokenExpired = DateTimeOffset.UtcNow.AddHours(1).ToUnixTimeSeconds()
                // });
            }

            var userName = claimsPrincipal.FindFirstValue(ClaimTypes.Name);
            var token = GenerateJwtToken(userName, isRefreshToken: false);
            var newRefreshToken = GenerateJwtToken(userName, isRefreshToken: true);

            return Ok(new LoginResponseModel
            {
                Token = token,
                RefreshToken = newRefreshToken,
                TokenExpired = DateTimeOffset.UtcNow.AddHours(1).ToUnixTimeSeconds()
            });


        }

        private ClaimsPrincipal GetClaimsPrinciplaFromToken(string token, string secret)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(secret);
            try
            {
                var principal = tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateAudience = true,
                    ValidAudience = "laluro",
                    ValidateIssuer = true,
                    ValidIssuer = "laluro",
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key)
                }, out var validatedToken);

                return principal;
            }
            catch //(Exception ex)
            {
                return null;
            }

        }

        private string GenerateJwtToken(string userName, bool isRefreshToken = false)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.Name, userName),
                new Claim(ClaimTypes.Role, userName == "admin" ? "Admin" : "User")
            };

            string secret = configuration.GetValue<string>($"Jwt:{(isRefreshToken ? "RefreshTokenSecret" : "Secret")}")!;
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: "laluro", //configuration.GetValue<string>("Jwt:Issuer"),
                audience: "laluro", //configuration.GetValue<string>("Jwt:Audience"),
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(isRefreshToken ? 24 : 1),
                signingCredentials: creds
            );


            return new JwtSecurityTokenHandler().WriteToken(token);

            // Generate JWT token
        }

    }
}
