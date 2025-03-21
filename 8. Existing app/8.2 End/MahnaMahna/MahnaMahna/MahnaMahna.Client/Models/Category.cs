namespace MahnaMahna.Client.Models;

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

public class Category
{
    [Key]
    public int Id { get; set; }

    [Required]
    public string Name { get; set; }

    [JsonIgnore]
    public ICollection<TodoItem> TodoItems { get; set; }
}

