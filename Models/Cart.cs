using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
#pragma warning disable CS8618

namespace ProjectNet.Models;

public class Cart
{
    [Key]
    public int CartId { get; set; }
    [Required]
    [Range(1,10000000,ErrorMessage ="Value cannot be smaller than 1")]
    public int Quantity {get;set;}
    public int? ProductId { get; set; }

    public int? UserId { get; set; }

    public Product? Product { get; set; }
    public UserReg? User { get; set; }
}






