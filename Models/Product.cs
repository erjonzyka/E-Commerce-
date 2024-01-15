using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations.Schema;
#pragma warning disable CS8618

namespace ProjectNet.Models;

public class Product
{
    [Key]
    public int ProductId { get; set; }
    [Required(ErrorMessage = "Name is required")]
    public string Name { get; set; }

    [Required(ErrorMessage = "Name is required")]
    public string Brand {get;set; }

    [Required(ErrorMessage = "Price is required")]
    public int Price { get; set; }

    [Required(ErrorMessage = "Quantity is required")]
    [Range(1, int.MaxValue, ErrorMessage = "Value must be greater than or equal to 1")]
    public int Quantity {get;set;}
    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime UpdatedAt { get; set; } = DateTime.Now;

    public int? CategoryId { get; set; }

    public Category? category {get;set;}

    public string? ImageFileName { get; set; } 
        public byte[]? ImageData { get; set; } 

        [Display(Name = "Upload Image")] 
        [NotMapped]
        public IFormFile? ImageFile { get; set; } 
  
    
    public List<Association>? AllAssociations {get;set;}
    public List<Purchase>? Purchases {get;set;}
    public Category? Category;
}


public class UniqueProductAttribute : ValidationAttribute
{
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {

        if (value == null)
        {

            return new ValidationResult("Name is required!");
        }


        MyContext _context = (MyContext)validationContext.GetService(typeof(MyContext));

        if (_context.Products.Any(e => e.Name == value.ToString()))
        {

            return new ValidationResult("This Product exists!");
        }
        else
        {

            return ValidationResult.Success;
        }
    }
}




