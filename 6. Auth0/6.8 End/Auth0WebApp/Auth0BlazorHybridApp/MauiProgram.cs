using Auth0.OidcClient;
using BlazorHybridApp;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.Logging;

namespace Auth0BlazorHybridApp;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();
		builder
			.UseMauiApp<App>()
			.ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
			});

		builder.Services.AddMauiBlazorWebView();

#if DEBUG
		builder.Services.AddBlazorWebViewDeveloperTools();
		builder.Logging.AddDebug();
#endif
        builder.Services.AddSingleton<MainPage>();


        builder.Services.AddSingleton(new Auth0Client(new()
        {
            Domain = "",
            ClientId = "",
            RedirectUri = "myapp://callback",
            PostLogoutRedirectUri = "myapp://callback",
            Scope = "openid profile email"
        }));
        builder.Services.AddAuthorizationCore();
        builder.Services.AddScoped<AuthenticationStateProvider, Auth0AuthenticationStateProvider>();
        return builder.Build();
	}
}
