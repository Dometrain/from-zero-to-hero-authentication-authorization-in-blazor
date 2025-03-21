using MahnaMahna.Client.Services;

using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.Services.AddScoped<ITodoApiService, TodoApiService>();

builder.Services.AddHttpClient("",(HttpClient client) => { client.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress); });
await builder.Build().RunAsync();
