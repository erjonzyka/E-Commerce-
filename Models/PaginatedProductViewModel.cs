#pragma warning disable CS8618
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace ProjectNet.Models;

public class PaginatedProductViewModel
{
    public List<Product> Products { get; set; }
    public List<UserReg> Users {get;set;}
    public List<Purchase> Purchases {get;set;}
    public int PageNumber { get; set; }
    public int TotalPages { get; set; }
}