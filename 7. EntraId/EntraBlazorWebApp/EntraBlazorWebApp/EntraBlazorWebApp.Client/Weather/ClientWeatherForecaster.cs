using Microsoft.AspNetCore.Components;
using System.Net;
using System.Net.Http.Json;

namespace EntraBlazorWebApp.Client.Weather;

public class ClientWeatherForecaster(HttpClient httpClient,NavigationManager navigationManager) : IWeatherForecaster
{
    public async Task<IEnumerable<WeatherForecast>> GetWeatherForecastAsync()
    {
        try
        {
            var req = await httpClient.GetAsync("/weatherforecast");
            req.EnsureSuccessStatusCode();
            return await req.Content.ReadFromJsonAsync<IEnumerable<WeatherForecast>>()??throw new Exception("No weather data");
        }
        catch (HttpRequestException ex) when (ex.StatusCode == HttpStatusCode.Unauthorized)
        {
            navigationManager.NavigateTo($"/authentication/login?returnUrl={navigationManager.Uri}", true);
            throw;
        }

    }
}
