using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Threading.Tasks;
using BlazorAspire.Model.Models;
using BlazorAspire.Web.Authentication;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.AspNetCore.Identity.Data;
using Newtonsoft.Json;

namespace BlazorAspire.Web;

public class ApiClient(HttpClient httpClient, ProtectedLocalStorage protectedLocalStorage, AuthenticationStateProvider authenticationStateProvider)
{
    public async Task SetAuthorizationHeader()
    {
        var sessionState = (await protectedLocalStorage.GetAsync<LoginResponseModel>("sessionState")).Value;
        if (sessionState != null && !string.IsNullOrEmpty(sessionState.Token))
        {
            if (sessionState.TokenExpired < DateTimeOffset.UtcNow.ToUnixTimeSeconds())
            {
                await ((CustomAuthStateProvider)authenticationStateProvider).MarkUserAsLoggedOut();
            }
            else if (sessionState.TokenExpired < DateTimeOffset.UtcNow.AddMinutes(10).ToUnixTimeSeconds())
            {
                var res = await httpClient.GetFromJsonAsync<LoginResponseModel>($"/api/Auth/loginByRefreshToken?refreshToken={sessionState.RefreshToken}");
                if (res != null && !string.IsNullOrEmpty(res.Token))
                {
                    await ((CustomAuthStateProvider)authenticationStateProvider).MarkUserAsAuthenticated(res);
                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", res.Token);
                    //await protectedLocalStorage.SetAsync("sessionState", res.Content);
                }
                else
                {
                    await ((CustomAuthStateProvider)authenticationStateProvider).MarkUserAsLoggedOut();
                }
            }
            else
            {
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", sessionState.Token);
            }
        }
    }

    public async Task<T> GetFromJsonAsync<T>(string path)
    {
        await SetAuthorizationHeader();
        return await httpClient.GetFromJsonAsync<T>(path);
    }
    public async Task<T1> PostAsync<T1, T2>(string path, T2 postModel)
    {
        await SetAuthorizationHeader();
        var res = await httpClient.PostAsJsonAsync(path, postModel);
        if (res != null && res.IsSuccessStatusCode)
        {
            return JsonConvert.DeserializeObject<T1>(await res.Content.ReadAsStringAsync());
        }
        return default;
    }
    public async Task<T1> PutAsync<T1, T2>(string path, T2 postModel)
    {
        await SetAuthorizationHeader();
        var res = await httpClient.PutAsJsonAsync(path, postModel);
        if (res != null && res.IsSuccessStatusCode)
        {
            return JsonConvert.DeserializeObject<T1>(await res.Content.ReadAsStringAsync());
        }
        return default;
    }
    public async Task<T> DeleteAsync<T>(string path)
    {
        await SetAuthorizationHeader();
        return await httpClient.DeleteFromJsonAsync<T>(path);
    }
}

// public record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
// {
//     public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
// }
