using Auth0.AspNetCore.Authentication;
using MahnaMahna.Client.Models;
using MahnaMahna.Client.Services;
using MahnaMahna.Components;
using MahnaMahna.Data;
using MahnaMahna.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;
using Auth0WebApp;

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
});

var connectionstring = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddPooledDbContextFactory<MahnaMahnaDbContext>(options =>
options.UseSqlite(connectionstring));

//Add the services
builder.Services.AddScoped<ITodoItemService, TodoItemService>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<ITodoApiService, TodoApiService>();

//Needed for local API calls
builder.Services.AddHttpContextAccessor();
builder.Services.AddTransient<LocalApiCallsHttpHandler>();

//Needed for API calls
builder.Services.AddHttpClient();
builder.Services.AddSingleton<IAuth0Service, Auth0Service>();
builder.Services.AddHttpClient<IAuth0Service, Auth0Service>(httpClient =>
{
    httpClient.BaseAddress = new("https://localhost:7286");
}).ConfigurePrimaryHttpMessageHandler<LocalApiCallsHttpHandler>();

//This is needed for the JsonSerializer to handle circular references when using Entity Framework, it is a better idea to convert the data to DTOs.
builder.Services.Configure<Microsoft.AspNetCore.Http.Json.JsonOptions>(options =>
{
    options.SerializerOptions.ReferenceHandler = ReferenceHandler.Preserve;
    options.SerializerOptions.WriteIndented = true;
});
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
app.MapStaticAssets();
app.UseAntiforgery();

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
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode()
    .AddInteractiveWebAssemblyRenderMode()
    .AddAdditionalAssemblies(typeof(MahnaMahna.Client._Imports).Assembly);

//Minimal API for the TodoItems and Categories
app.MapGet("/api/todos", async (ITodoItemService todoItemService) =>
{
    return Results.Ok(await todoItemService.GetAllAsync());
});

app.MapGet("/api/todos/{id}", async (int id, ITodoItemService todoItemService) =>
{
    var todoItem = await todoItemService.GetByIdAsync(id);
    return todoItem == null ? Results.NotFound() : Results.Ok(todoItem);
});

app.MapPost("/api/todos", async (TodoItem todoItem, ITodoItemService todoItemService) =>
{
    var createdTodoItem = await todoItemService.CreateAsync(todoItem);
    return Results.Created($"/api/todos/{createdTodoItem.Id}", createdTodoItem);
});

app.MapPut("/api/todos/{id}", async (int id, TodoItem todoItem, ITodoItemService todoItemService) =>
{
    if (id != todoItem.Id)
    {
        return Results.BadRequest();
    }

    await todoItemService.UpdateAsync(todoItem);
    return Results.NoContent();
});

app.MapDelete("/api/todos/{id}", async (int id, ITodoItemService todoItemService) =>
{
    await todoItemService.DeleteAsync(id);
    return Results.NoContent();
});

app.MapGet("/api/categories", async (ICategoryService categoryService) =>
{
    return Results.Ok(await categoryService.GetAllAsync());
});

app.MapGet("/api/categories/{id}", async (int id, ICategoryService categoryService) =>
{
    var category = await categoryService.GetByIdAsync(id);
    return category == null ? Results.NotFound() : Results.Ok(category);
});

app.MapPost("/api/categories", async (Category category, ICategoryService categoryService) =>
{
    var createdCategory = await categoryService.CreateAsync(category);
    return Results.Created($"/api/categories/{createdCategory.Id}", createdCategory);
});

app.MapPut("/api/categories/{id}", async (int id, Category category, ICategoryService categoryService) =>
{
    if (id != category.Id)
    {
        return Results.BadRequest();
    }

    await categoryService.UpdateAsync(category);
    return Results.NoContent();
});

app.MapDelete("/api/categories/{id}", async (int id, ICategoryService categoryService) =>
{
    await categoryService.DeleteAsync(id);
    return Results.NoContent();
});


app.MapGet("/api/users", async (IAuth0Service service) =>
{
    return await service.GetUsersAsync();
});

app.Run();
