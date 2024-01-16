using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
#pragma warning disable CS8618

namespace ProjectNet.Models;

public class Cart
{
    [Key]
    public int CartId { get; set; }

    public int? ProductId { get; set; }

    public int? UserId { get; set; }

    public Product? Product { get; set; }
    public UserReg? User { get; set; }
}






