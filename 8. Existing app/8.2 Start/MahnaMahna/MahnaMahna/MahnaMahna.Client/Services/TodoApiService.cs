namespace MahnaMahna.Client.Services;

using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json.Serialization;
using System.Text.Json;
using System.Threading.Tasks;
using MahnaMahna.Client.Models;

public interface ITodoApiService
{
    Task<Category?> CreateCategoryAsync(Category category);
    Task<TodoItem?> CreateTodoItemAsync(TodoItem todoItem);
    Task DeleteCategoryAsync(int id);
    Task DeleteTodoItemAsync(int id);
    Task<List<Category>> GetCategoriesAsync();
    Task<Category?> GetCategoryByIdAsync(int id);
    Task<TodoItem?> GetTodoItemByIdAsync(int id);
    Task<List<TodoItem>> GetTodoItemsAsync();
    Task UpdateCategoryAsync(Category category);
    Task UpdateTodoItemAsync(TodoItem todoItem);
}

public class TodoApiService : ITodoApiService
{
    private readonly HttpClient _httpClient;
    public TodoApiService(HttpClient httpClient,NavigationManager manager)
    {
        _httpClient = httpClient;
        _httpClient.BaseAddress = new Uri(manager.BaseUri);
    }

    JsonSerializerOptions options = new()
    {
        ReferenceHandler = ReferenceHandler.Preserve,
        PropertyNameCaseInsensitive = true
    };

    // TodoItem methods
    public async Task<List<TodoItem>> GetTodoItemsAsync()
    {
        return (await _httpClient.GetFromJsonAsync<IEnumerable<TodoItem>>("/api/todos", options))?.ToList() ?? new List<TodoItem>();
    }

    public async Task<TodoItem?> GetTodoItemByIdAsync(int id)
    {
        return await _httpClient.GetFromJsonAsync<TodoItem>($"/api/todos/{id}", options);
    }

    public async Task<TodoItem?> CreateTodoItemAsync(TodoItem todoItem)
    {
        var response = await _httpClient.PostAsJsonAsync("/api/todos", todoItem);
        response.EnsureSuccessStatusCode();

        var item = await response.Content.ReadFromJsonAsync<TodoItem>();
        return item;
    }

    public async Task UpdateTodoItemAsync(TodoItem todoItem)
    {
        var response = await _httpClient.PutAsJsonAsync($"/api/todos/{todoItem.Id}", todoItem, options);
        response.EnsureSuccessStatusCode();
    }

    public async Task DeleteTodoItemAsync(int id)
    {
        var response = await _httpClient.DeleteAsync($"/api/todos/{id}");
        response.EnsureSuccessStatusCode();
    }

    // Category methods
    public async Task<List<Category>> GetCategoriesAsync()
    {
        return (await _httpClient.GetFromJsonAsync<IEnumerable<Category>>("/api/categories", options))?.ToList() ?? new List<Category>();
    }

    public async Task<Category?> GetCategoryByIdAsync(int id)
    {
        return await _httpClient.GetFromJsonAsync<Category>($"/api/categories/{id}", options);
    }

    public async Task<Category?> CreateCategoryAsync(Category category)
    {
        var response = await _httpClient.PostAsJsonAsync("/api/categories", category, options);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<Category>();
    }

    public async Task UpdateCategoryAsync(Category category)
    {
        var response = await _httpClient.PutAsJsonAsync($"/api/categories/{category.Id}", category);
        response.EnsureSuccessStatusCode();
    }

    public async Task DeleteCategoryAsync(int id)
    {
        var response = await _httpClient.DeleteAsync($"/api/categories/{id}");
        response.EnsureSuccessStatusCode();
    }
}
