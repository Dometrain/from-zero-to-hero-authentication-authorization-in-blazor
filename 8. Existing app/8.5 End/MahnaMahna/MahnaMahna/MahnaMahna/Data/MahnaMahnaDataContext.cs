namespace MahnaMahna.Data;

using MahnaMahna.Client.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Reflection.Emit;

public class MahnaMahnaDbContext : DbContext
{
    public DbSet<TodoItem> TodoItems { get; set; }
    public DbSet<Category> Categories { get; set; }

    public MahnaMahnaDbContext(DbContextOptions<MahnaMahnaDbContext> options)
    : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<TodoItem>()
            .HasMany(t => t.Categories)
            .WithMany(c => c.TodoItems)
            .UsingEntity<Dictionary<string, object>>(
                "TodoItemCategory",
                j => j.HasOne<Category>().WithMany().HasForeignKey("CategoryId"),
                j => j.HasOne<TodoItem>().WithMany().HasForeignKey("TodoItemId")
            );
    }
}
