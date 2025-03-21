using EntraBlazorWebApp;
using EntraBlazorWebApp.Client.Pages;
using EntraBlazorWebApp.Client.Weather;
using EntraBlazorWebApp.Components;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.Identity.Web;
using Microsoft.Identity.Web.UI;
using System.Net;
using Yarp.ReverseProxy.Transforms;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents()
    .AddMicrosoftIdentityConsentHandler()
    .AddInteractiveWebAssemblyComponents()
    .AddAuthenticationStateSerialization(options=>options.SerializeAllClaims=true);

builder.Services.AddHttpForwarder();
builder.Services.AddHttpContextAccessor();
builder.Services.AddTransient<LocalApiCallsHttpHandler>();
builder.Services.AddHttpClient<IWeatherForecaster, ClientWeatherForecaster>(httpClient =>
{
    httpClient.BaseAddress = new("https://localhost:7270"); //Address to the local server not the API
}).ConfigurePrimaryHttpMessageHandler<LocalApiCallsHttpHandler>();


//Auth related
builder.Services.AddControllersWithViews().AddMicrosoftIdentityUI();
builder.Services.AddCascadingAuthenticationState();
builder.Services.AddAuthentication(OpenIdConnectDefaults.AuthenticationScheme)
.AddMicrosoftIdentityWebApp(options =>
{
    builder.Configuration.Bind("EntraId", options);
})
.EnableTokenAcquisitionToCallDownstreamApi()
.AddInMemoryTokenCaches();
builder.Services.AddAuthorization(options =>{});



var app = builder.Build();

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
app.MapLoginAndLogout();
app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode()
    .AddInteractiveWebAssemblyRenderMode()
    .AddAdditionalAssemblies(typeof(EntraBlazorWebApp.Client._Imports).Assembly);
var urlToWeatherApi = "https://localhost:7288/";
string[] scopes = new[] { "api://WeatherApi/Read" };
app.MapForwarder("/weatherforecast", urlToWeatherApi, transformBuilder =>
{
    transformBuilder.AddRequestTransform(async transformContext =>
    {
        try
        {
            var user = transformContext.HttpContext.User;
            var token = transformContext.HttpContext.RequestServices.GetService<ITokenAcquisition>();

            var accessToken = await token!.GetAccessTokenForUserAsync(scopes: scopes, user: user);
            if (accessToken != null)
            {
                transformContext.ProxyRequest.Headers.Authorization = new("Bearer", accessToken);
            }
        }
        catch (MicrosoftIdentityWebChallengeUserException ex)
        {
            transformBuilder.AddResponseTransform(transformContext =>
            {
                if (transformContext.ProxyResponse != null)
                {
                    transformContext.ProxyResponse.StatusCode = HttpStatusCode.Unauthorized;
                }
                return ValueTask.CompletedTask;
            });
        }
        catch (Exception ex)
        {
            throw;
        }
    });

}).RequireAuthorization();


app.Run();
    