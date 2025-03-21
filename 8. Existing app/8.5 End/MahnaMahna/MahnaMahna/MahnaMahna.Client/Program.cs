using MahnaMahna.Client.Requirements;
using MahnaMahna.Client.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.Services.AddScoped<ITodoApiService, TodoApiService>();

//Enable policies
builder.Services.AddAuthorizationCore(options =>
{
    options.AddPolicy("IsAssignedToUserPolicy", policy =>
    {
        policy.Requirements.Add(new IsAssignedToUserRequirement());
    });

    options.AddPolicy("RequireAdminRolePolicy", policy =>
    {
        policy.RequireRole("Admin");
    });
});
builder.Services.AddTransient<IAuthorizationHandler, IsAssignedToUserRequirementAuthorizationHandler>();

builder.Services.AddCascadingAuthenticationState();
builder.Services.AddAuthenticationStateDeserialization();

builder.Services.AddHttpClient("",(HttpClient client) => { client.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress); });
await builder.Build().RunAsync();
