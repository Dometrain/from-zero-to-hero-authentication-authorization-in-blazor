using System.ComponentModel.DataAnnotations;

namespace BlazorWebAppBuilInAuthentication.Client.SuperHeroDemo;

public class Superhero
{
    public int Id { get; set; }
    [Required, MinLength(5)]
    public string Name { get; set; }
    [Required, MinLength(5)]
    public string RealName { get; set; }
    [Required, MinLength(5)]
    public string Powers { get; set; }
}
