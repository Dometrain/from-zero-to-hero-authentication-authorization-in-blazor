using System.Net.Http.Json;

namespace Auth0WebApp.Weather;

public class ClientWeatherForecaster(HttpClient httpClient) : IWeatherForecaster
{
    public async Task<IEnumerable<WeatherForecast>> GetWeatherForecastAsync() =>
        await httpClient.GetFromJsonAsync<WeatherForecast[]>("/weatherforecast") ??
            throw new IOException("No weather forecast!");
}
