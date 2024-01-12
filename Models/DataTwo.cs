#pragma warning disable CS8618
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace ProjectNet.Models;

public class DataTwo{
    public Product? Product {get;set;}

    public List<Category>? Categories {get;set;}

    public Category? Category {get;set;}

    public Association? Association {get;set;}


    
}