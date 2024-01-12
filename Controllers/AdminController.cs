using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjectNet.Models;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Filters;
namespace ProjectNet.Controllers;

public class AdminController : Controller
{    
    private readonly ILogger<AdminController> _logger;
    // Add a private variable of type MyContext (or whatever you named your context file)
    private MyContext _context;         
    // Here we can "inject" our context service into the constructor 
    // The "logger" was something that was already in our code, we're just adding around it   
    private readonly IWebHostEnvironment _environment;
    // Here we can "inject" our context service into the constructor 
    // The "logger" was something that was already in our code, we're just adding around it   
    public AdminController(ILogger<AdminController> logger, MyContext context, IWebHostEnvironment environment)
    {
        _logger = logger;
        // When our HomeController is instantiated, it will fill in _context with context
        // Remember that when context is initialized, it brings in everything we need from DbContext
        // which comes from Entity Framework Core
        _context = context;
        _environment = environment;

    }

    [SessionCheck]
    [AdminCheck]
    public IActionResult Index()
    {
        List<Product>AllProducts = _context.Products.Include(e=> e.AllAssociations).ThenInclude(e=> e.category).ToList();
        return View(AllProducts);
    }


    [AdminCheck]
    [HttpGet("newproduct")]
    public IActionResult NewProduct()
    {
        DataTwo DataTwo = new DataTwo();
        DataTwo.Categories= _context.Categories.ToList();
        return View(DataTwo);
    }
    
    [AdminCheck]
    [HttpPost("registerproduct")]
    public async Task<IActionResult> RegisterProduct(Product product)
{
    if (ModelState.IsValid)
    {
        if (product.ImageFile != null && product.ImageFile.Length > 0)
        {
            Console.WriteLine("U ekzekutua");
            // Process the uploaded file
            var uploadsFolder = Path.Combine(_environment.WebRootPath, "uploads");
            var uniqueFileName = Guid.NewGuid().ToString() + "_" + product.ImageFile.FileName;
            var filePath = Path.Combine(uploadsFolder, uniqueFileName);

            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await product.ImageFile.CopyToAsync(fileStream);
            }

            // Update the model properties with the file details
            product.ImageFileName = uniqueFileName;
            product.ImageData = System.IO.File.ReadAllBytes(filePath);
        }
            _context.Products.Add(product);
            _context.SaveChanges();
            Association association = new Association();
             association.ProductId = product.ProductId;
            association.CategoryId = product.CategoryId;
            _context.Associations.Add(association);
            _context.SaveChanges();
            return RedirectToAction("Index");
           }
   
         ViewBag.AllProducts = _context.Products.ToList();
        DataTwo DataTwo = new DataTwo();
        DataTwo.Categories= _context.Categories.ToList();
        return View("NewProduct", DataTwo);

}

    [SessionCheck]
    [AdminCheck]
    [HttpGet("delete/{id}")]
    public IActionResult DeleteItem(int id){
        Product product = _context.Products.Include(e=> e.AllAssociations).FirstOrDefault(e=>e.ProductId == id);
        List<Association> association = _context.Associations.Where(e => e.ProductId == id).ToList();
        _context.Remove(product);
        _context.SaveChanges();
        return RedirectToAction("Index");
    }

    [SessionCheck]
    [AdminCheck]
    [HttpGet("item/edit/{id}")]
    public IActionResult EditItem(int id)
    {
        Product? product = _context.Products.FirstOrDefault(e => e.ProductId == id);
        return View(product);
    }


    [SessionCheck]
    [HttpPost("item/post/edit/{id}")]
    public IActionResult EditProduct(Product productt, int id)
{
    if (ModelState.IsValid)
    {
        Product productFromDb = _context.Products.FirstOrDefault(e => e.ProductId == id);
        productFromDb.Name = productt.Name;
        productFromDb.Price = productt.Price;
        productFromDb.Quantity = productt.Quantity;
        productFromDb.Description = productt.Description;
        productFromDb.UpdatedAt = DateTime.Now;
        _context.SaveChanges();
        return RedirectToAction("Shop");
    }
    else
    {
        Product? product = _context.Products.FirstOrDefault(e => e.ProductId == id);
        return View("EditItem", product);
    }
}

[AdminCheck]
    [HttpPost("registercategory")]
    public IActionResult RegisterCategory(Category category)
    {
        if (ModelState.IsValid)
        {
            _context.Add(category);
            _context.SaveChanges();
            return RedirectToAction("NewProduct");
        }
        ViewBag.AllCategories = _context.Categories.ToList();
        DataTwo DataTwo = new DataTwo();
        DataTwo.Categories= _context.Categories.ToList();
        return View("NewProduct",DataTwo);
    }




}


