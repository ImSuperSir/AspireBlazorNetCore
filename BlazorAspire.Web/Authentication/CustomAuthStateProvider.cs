using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Threading.Tasks;
using BlazorAspire.Model.Models;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;

namespace BlazorAspire.Web.Authentication;

public class CustomAuthStateProvider(ProtectedLocalStorage protectedLocalStorage) : AuthenticationStateProvider
{
    public async override Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        var sessionModel = (await protectedLocalStorage.GetAsync<LoginResponseModel>("sessionState")).Value;
        var identity = sessionModel == null ? new ClaimsIdentity() : GetClaimsIdentity(sessionModel.Token);
        var user = new ClaimsPrincipal(identity);
        return new AuthenticationState(user);


        // if (string.IsNullOrEmpty(token))
        // {
        //     return new AuthenticationState(new ClaimsPrincipal());
        // }

    }

    public async Task MarkUserAsAuthenticated(LoginResponseModel model)
    {
        await protectedLocalStorage.SetAsync("sessionState", model);
        var identity = GetClaimsIdentity(model.Token);
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
        await protectedLocalStorage.DeleteAsync("sessionState");
        var identity = new ClaimsIdentity();
        var user = new ClaimsPrincipal(identity);
        NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(user)));
    }
}
