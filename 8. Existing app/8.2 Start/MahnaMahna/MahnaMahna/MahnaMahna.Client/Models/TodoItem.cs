namespace MahnaMahna.Client.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;

public class TodoItem
{
    [Key]
    public int Id { get; set; }

    [Required]
    public string Text { get; set; }

    [Required]
    public TodoItemState State { get; set; }

    public ICollection<Category> Categories { get; set; }

    [NotMapped]
    public bool IsCompleted
    {
        get { return State == TodoItemState.Completed; }
        set { State = value ? TodoItemState.Completed : TodoItemState.Pending; }
    }
}
