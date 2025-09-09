using System.ComponentModel.DataAnnotations;

namespace ApiEcommerce1.Models.Dtos;

public class CreateCategoryDto
{
    [Required(ErrorMessage = "Name is required")]
    [MaxLength(50, ErrorMessage = "Name cant have more than 50 characters")]
    [MinLength(3, ErrorMessage = "Name cant have less than 3 characters")]
    public string Name { get; set; } = string.Empty;
}