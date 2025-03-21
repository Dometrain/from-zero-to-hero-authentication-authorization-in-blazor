using Auth0.AspNetCore.Authentication;
using Auth0WebApp;
using Auth0WebApp.Client.Pages;
using Auth0WebApp.Components;
using Auth0WebApp.Weather;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Yarp.ReverseProxy.Transforms;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents()
    .AddInteractiveWebAssemblyComponents()
    .AddAuthenticationStateSerialization(options => options.SerializeAllClaims = true);

builder.Services.AddAuth0WebAppAuthentication(options =>
{
    options.Domain = builder.Configuration["Auth0:Domain"]!;
    options.ClientId = builder.Configuration["Auth0:ClientId"]!;
    options.ClientSecret = builder.Configuration["Auth0:ClientSecret"]!;
}).WithAccessToken(tokenOptions =>
{
    tokenOptions.Audience = builder.Configuration["Auth0:Audience"]!;
}); 
builder.Services.AddTransient<LocalApiCallsHttpHandler>();
builder.Services.AddScoped<IWeatherForecaster, ClientWeatherForecaster>();
builder.Services.AddHttpClient<IWeatherForecaster, ClientWeatherForecaster>(httpClient =>
{
    httpClient.BaseAddress = new("https://localhost:7189");
}).ConfigurePrimaryHttpMessageHandler<LocalApiCallsHttpHandler>();

builder.Services.AddHttpContextAccessor();
builder.Services.AddHttpForwarder();
var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();


app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode()
    .AddInteractiveWebAssemblyRenderMode()
    .AddAdditionalAssemblies(typeof(Auth0WebApp.Client._Imports).Assembly);


app.MapGet("account/login", async (HttpContext context, string returnUrl = "/") =>
{
    var authenticationProperties = new LoginAuthenticationPropertiesBuilder()
         .WithRedirectUri(returnUrl)
         .Build();
    await context.ChallengeAsync(Auth0Constants.AuthenticationScheme, authenticationProperties);
});


app.MapGet("account/logout", async (HttpContext context) =>
{
    var authenticationProperties = new LogoutAuthenticationPropertiesBuilder()
         .WithRedirectUri("/")
         .Build();
    await context.SignOutAsync(Auth0Constants.AuthenticationScheme, authenticationProperties);
    await context.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
});

app.MapForwarder("/weatherforecast", "https://localhost:7202/", transformBuilder =>
{
    transformBuilder.AddRequestTransform(async transformContext =>
    {
        var accessToken = await transformContext.HttpContext.GetTokenAsync("access_token");
        transformContext.ProxyRequest.Headers.Authorization = new("Bearer", accessToken);
       
    });
       
}).RequireAuthorization();

app.Run();
