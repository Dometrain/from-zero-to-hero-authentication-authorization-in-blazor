namespace MahnaMahna.Services;
using System.Collections.Generic;
using System.Threading.Tasks;
using MahnaMahna.Client.Models;
using MahnaMahna.Data;

using Microsoft.EntityFrameworkCore;

public interface ITodoItemService
{
    Task<IEnumerable<TodoItem>> GetAllAsync();
    Task<TodoItem> GetByIdAsync(int id);
    Task<TodoItem> CreateAsync(TodoItem item);
    Task<TodoItem> UpdateAsync(TodoItem item);
    Task DeleteAsync(int id);
}

public class TodoItemService : ITodoItemService
{
    private readonly IDbContextFactory<MahnaMahnaDbContext> _contextFactory;

    public TodoItemService(IDbContextFactory<MahnaMahnaDbContext> contextFactory)
    {
        _contextFactory = contextFactory;
    }

    public async Task<IEnumerable<TodoItem>> GetAllAsync()
    {
        using var context = _contextFactory.CreateDbContext();
        var list = await context.TodoItems.Include(t => t.Categories).ToListAsync();
        return list;
    }

    public async Task<TodoItem> GetByIdAsync(int id)
    {
        using var context = _contextFactory.CreateDbContext();
        return await context.TodoItems.Include(t => t.Categories).FirstOrDefaultAsync(t => t.Id == id);
    }

    public async Task<TodoItem> CreateAsync(TodoItem item)
    { 
        using var context = _contextFactory.CreateDbContext();
        context.TodoItems.Add(item);
        await context.SaveChangesAsync();
        return item;
    }

    public async Task<TodoItem> UpdateAsync(TodoItem item)
    {
        using var context = _contextFactory.CreateDbContext();

        // Fetch existing TodoItem with related Categories
        var existingItem = await context.TodoItems
            .Include(t => t.Categories)
            .FirstOrDefaultAsync(t => t.Id == item.Id);

        if (existingItem == null)
        {
            // Handle item not found scenario
            throw new KeyNotFoundException("TodoItem not found");
        }

        // Update scalar properties
        existingItem.Text = item.Text;
        existingItem.State = item.State;
        existingItem.AssignedTo = item.AssignedTo;
        // Update Categories
        existingItem.Categories.Clear();
        if (item.Categories!=null)
        {
            foreach (var category in item.Categories)
            {
                var existingCategory = await context.Categories.FindAsync(category.Id);
                if (existingCategory != null)
                {
                    existingItem.Categories.Add(existingCategory);
                }
            }
        }

        await context.SaveChangesAsync();
        return existingItem;
    }

    public async Task DeleteAsync(int id)
    {
        using var context = _contextFactory.CreateDbContext();
        var item = await context.TodoItems.FindAsync(id);
        if (item != null)
        {
            context.TodoItems.Remove(item);
            await context.SaveChangesAsync();
        }
    }
}
