using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Newtonsoft.Json;

namespace BlazorAspire.Web;

public class ApiClient(HttpClient httpClient, ProtectedLocalStorage protectedLocalStorage)
{
    public async Task SetAuthorizationHeader()
    {
        var token = (await protectedLocalStorage.GetAsync<string>("authtoken")).Value;
        if (!string.IsNullOrEmpty(token))
        {
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
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
        if(res != null && res.IsSuccessStatusCode)
        {
            return JsonConvert.DeserializeObject<T1>(await res.Content.ReadAsStringAsync());
        }
        return default;
    }
    public async Task<T1> PutAsync<T1, T2>(string path, T2 postModel)
    {
        await SetAuthorizationHeader();
        var res = await httpClient.PutAsJsonAsync(path, postModel);
        if(res != null && res.IsSuccessStatusCode)
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
