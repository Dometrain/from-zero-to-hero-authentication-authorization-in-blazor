using BlazorWebAppBuilInAuthentication.Client.Requirements;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.Services.AddAuthorizationCore(options =>
{
    // Simple role-based policy
    options.AddPolicy("RequireAdminRole", policy =>
    {
        policy.RequireRole("Admin");
    });

    options.AddPolicy("IsAssignedToUserPolicy",
        policy => policy.Requirements.Add(new IsAssignedToUserRequirement()));
});
builder.Services.AddTransient<IAuthorizationHandler, IsAssignedToUserRequirementAuthorizationHandler>();
builder.Services.AddCascadingAuthenticationState();
builder.Services.AddAuthenticationStateDeserialization();

await builder.Build().RunAsync();
