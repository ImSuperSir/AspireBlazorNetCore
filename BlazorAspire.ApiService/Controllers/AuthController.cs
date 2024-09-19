using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using BlazorAspire.BL.Services;
using BlazorAspire.Model.Entities;
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
    public class AuthController(IConfiguration configuration, IAuthService authService) : ControllerBase
    {

        [HttpPost("login")]
        public async Task<ActionResult<LoginResponseModel>> Login([FromBody] LoginModel model)
        {

            var user =  await authService.GetUserByLoginAsync(model.UserName, model.Password);    

            if (user != null)
            {
                var token = GenerateJwtToken(user, isRefreshToken: false);
                var refreshToken = GenerateJwtToken(user, isRefreshToken: true);

                await authService.AddRefreshTokenModelAsync(new RefreshTokenModel
                {
                    UserId = user.Id,
                    RefreshToken = refreshToken
                });
                
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
        public async Task<ActionResult<LoginResponseModel>> LoginByRefreshToken(string refreshToken)
        {
            var refreshTokenModel = await authService.GetRefreshTokenModelAsync(refreshToken);
            if (refreshTokenModel == null)
            {
                //return Unauthorized();
                return new StatusCodeResult(StatusCodes.Status400BadRequest);
            }

            var token = GenerateJwtToken(refreshTokenModel.User, isRefreshToken: false);
            var newRefreshToken = GenerateJwtToken(refreshTokenModel.User, isRefreshToken: true);

            await authService.AddRefreshTokenModelAsync( new RefreshTokenModel
            {
                UserId = refreshTokenModel.UserId,
                RefreshToken = newRefreshToken
            });


            return Ok(new LoginResponseModel
            {
                Token = token,
                RefreshToken = newRefreshToken,
                TokenExpired = DateTimeOffset.UtcNow.AddHours(1).ToUnixTimeSeconds()
            });


        }

        // private ClaimsPrincipal GetClaimsPrinciplaFromToken(string token, string secret)
        // {
        //     var tokenHandler = new JwtSecurityTokenHandler();
        //     var key = Encoding.UTF8.GetBytes(secret);
        //     try
        //     {
        //         var principal = tokenHandler.ValidateToken(token, new TokenValidationParameters
        //         {
        //             ValidateAudience = true,
        //             ValidAudience = "laluro",
        //             ValidateIssuer = true,
        //             ValidIssuer = "laluro",
        //             ValidateLifetime = true,
        //             ValidateIssuerSigningKey = true,
        //             IssuerSigningKey = new SymmetricSecurityKey(key)
        //         }, out var validatedToken);

        //         return principal;
        //     }
        //     catch //(Exception ex)
        //     {
        //         return null;
        //     }

        // }

        private string GenerateJwtToken(UserModel user, bool isRefreshToken = false)
        {
            var claims = new List<Claim>()
            {
                new Claim(ClaimTypes.Name, user.UserName)
            };

            claims.AddRange(user.UserRoles.Select(r => new Claim(ClaimTypes.Role, r.Role.RoleName)));

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
