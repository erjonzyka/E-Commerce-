using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
#pragma warning disable CS8618

namespace ProjectNet.Models;

public class Category
{
    [Key]
    public int CategoryId { get; set; }
    [Required(ErrorMessage = "Name is required")]
    [UniqueCategory]
    public string Name { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime UpdatedAt { get; set; } = DateTime.Now;
    public List<Association>? AllAssociations {get;set;}

}

public class UniqueCategoryAttribute : ValidationAttribute
{
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {

        if (value == null)
        {

            return new ValidationResult("Name is required!");
        }


        MyContext _context = (MyContext)validationContext.GetService(typeof(MyContext));

        if (_context.Categories.Any(e => e.Name == value.ToString()))
        {

            return new ValidationResult("This Category exists!");
        }
        else
        {

            return ValidationResult.Success;
        }
    }
}



