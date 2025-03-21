using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

public class Auth0Service(HttpClient httpClient, IConfiguration config) : IAuth0Service
{
    private string _accessToken;

    private async Task<string> GetAccessTokenAsync()
    {
        if (!string.IsNullOrEmpty(_accessToken))
            return _accessToken;

        var domain = config["Auth0:Domain"];
        var clientId = config["Auth0:ClientId"];
        var clientSecret = config["Auth0:ClientSecret"];
        var audience = config["Auth0:Audience"];

        var requestBody = new
        {
            client_id = clientId,
            client_secret = clientSecret,
            audience = audience,
            grant_type = "client_credentials"
        };

        var requestContent = new StringContent(JsonSerializer.Serialize(requestBody), System.Text.Encoding.UTF8, "application/json");

        var response = await httpClient.PostAsync($"https://{domain}/oauth/token", requestContent);
        response.EnsureSuccessStatusCode();

        var responseString = await response.Content.ReadAsStringAsync();
        using var jsonDoc = JsonDocument.Parse(responseString);
        _accessToken = jsonDoc.RootElement.GetProperty("access_token").GetString();

        return _accessToken;
    }


    public async Task<List<Auth0User>> GetUsersAsync()
    {
        var token = await GetAccessTokenAsync();
        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var response = await httpClient.GetAsync($"https://{config["Auth0:Domain"]}/api/v2/users");
        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<List<Auth0User>>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
    }
}
