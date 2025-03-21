using MahnaMahna.Client.Models;
using MahnaMahna.Client.Pages;
using MahnaMahna.Client.Services;
using MahnaMahna.Components;
using MahnaMahna.Data;
using MahnaMahna.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Text.Json.Serialization;
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents()
    .AddInteractiveWebAssemblyComponents();

var connectionstring = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddPooledDbContextFactory<MahnaMahnaDbContext>(options =>
 options.UseSqlite(connectionstring));

//This is needed for running the migrations
builder.Services.AddDbContext<MahnaMahnaDbContext>(options => options.UseSqlite(connectionstring));

//Add the services
builder.Services.AddScoped<ITodoItemService, TodoItemService>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<ITodoApiService, TodoApiService>();
builder.Services.AddHttpClient();

//This is needed for the JsonSerializer to handle circular references when using Entity Framework, it is a better idea to convert the data to DTOs.
builder.Services.Configure<Microsoft.AspNetCore.Http.Json.JsonOptions>(options =>
{
    options.SerializerOptions.ReferenceHandler = ReferenceHandler.Preserve;
    options.SerializerOptions.WriteIndented = true;
});
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
app.UseStaticFiles();
app.UseAntiforgery();

//This is probably not the best idea, but it works for this project, this should probably be done in a migration while deploying
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<MahnaMahnaDbContext>();
    dbContext.Database.Migrate();
}
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




app.Run();
