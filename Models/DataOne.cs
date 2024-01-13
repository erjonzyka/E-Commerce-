#pragma warning disable CS8618
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace ProjectNet.Models;

public class DataOne{
    public Product? Product {get;set;}

    public List<Association> Associations {get;set;}
    public List<Product> AllProducts {get;set;}
    public List<Category> AllCategories {get;set;}
    public Purchase? Purchase {get;set;}


    
}