using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ApiEcommerce1.Models;

public class Product
{
    [Key]
    public int ProductId { get; set; }
    [Required]
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    [Range(0, double.MaxValue)]
    [Column(TypeName = "decimal(18,2)")]
    public decimal Price { get; set; }

    public string ImgUrl { get; set; } = string.Empty;

    [Required]
    public string SKU { get; set; } = string.Empty;

    [Range(0, int.MaxValue)]
    public int Stock { get; set; }

    public DateTime CreationDate { get; set; } = DateTime.Now;
    public DateTime? UpdateDate { get; set; } = null;

    // relation with acategory
    public int CategoryId { get; set; }
    [ForeignKey("CategoryId")]
    public required Category Category { get; set; }
}