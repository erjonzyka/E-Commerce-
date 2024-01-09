#pragma warning disable CS8618
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace ProjectNet.Models;

public class UserLogin
{

    [Required(ErrorMessage ="Username is required")]
    [MinLength(3, ErrorMessage ="Username must be at least 3 characters")]
    [MaxLength(15, ErrorMessage ="Username cant be more than 15 characters")]
    [ExistingUsername]

    public string LUsername { get; set; }
    [Required]
    [MinLength(8, ErrorMessage ="Password must be at least 8 characters")]
    public string LPassword { get; set; }


}



public class ExistingUsernameAttribute : ValidationAttribute
{
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {

        if (value == null)
        {

            return new ValidationResult("Username is required!");
        }


        MyContext _context = (MyContext)validationContext.GetService(typeof(MyContext));

        if (!_context.Users.Any(e => e.Username == value.ToString()))
        {

            return new ValidationResult("User not registered");
        }
        else
        {

            return ValidationResult.Success;
        }
    }
}




