using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;

namespace BlazorAspire.Web.Authentication;

public class CustomAuthStateProvider(ProtectedLocalStorage protectedLocalStorage) : AuthenticationStateProvider
{
    public async override Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        var token = (await protectedLocalStorage.GetAsync<string>("authtoken")).Value;
        var identity = string.IsNullOrEmpty(token) ? new ClaimsIdentity() : GetClaimsIdentity(token);
        var user = new ClaimsPrincipal(identity);
        return new AuthenticationState(user);


        // if (string.IsNullOrEmpty(token))
        // {
        //     return new AuthenticationState(new ClaimsPrincipal());
        // }

    }

    public async Task MarkUserAsAuthenticated(string token)
    {
        await protectedLocalStorage.SetAsync("authtoken", token);
        var identity = GetClaimsIdentity(token);
        var user = new ClaimsPrincipal(identity);
        NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(user)));
    }

    private ClaimsIdentity GetClaimsIdentity(string token)
    {
        var handler = new JwtSecurityTokenHandler();
        var jwtToken = handler.ReadJwtToken(token);
        var claims = jwtToken.Claims;

        return new ClaimsIdentity(claims, "jwt");
    }

    public async Task MarkUserAsLoggedOut()
    {
        await protectedLocalStorage.DeleteAsync("authtoken");
        var identity = new ClaimsIdentity();
        var user = new ClaimsPrincipal(identity);
        NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(user)));
    }
}
