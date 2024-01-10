using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjectNet.Models;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Filters;
namespace ProjectNet.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    // Add a private variable of type MyContext (or whatever you named your context file)
    private MyContext _context;
    private readonly IWebHostEnvironment _environment;
    // Here we can "inject" our context service into the constructor 
    // The "logger" was something that was already in our code, we're just adding around it   
    public HomeController(ILogger<HomeController> logger, MyContext context, IWebHostEnvironment environment)
    {
        _logger = logger;
        // When our HomeController is instantiated, it will fill in _context with context
        // Remember that when context is initialized, it brings in everything we need from DbContext
        // which comes from Entity Framework Core
        _context = context;
        _environment = environment;
    }

    public IActionResult Index()
    {
        List<Product>AllProducts = _context.Products.Include(e=> e.AllAssociations).ThenInclude(e=> e.category).ToList();
        return View(AllProducts);
    }

    [SessionCheck]
    [HttpGet("logout")]
    public IActionResult LogOut()
    {
        HttpContext.Session.Clear();
        return RedirectToAction("Index", "Login");
    }

    [SessionCheck]
    [HttpGet("shop")]
    public IActionResult Shop()
    {
        List<Product>AllProducts = _context.Products.Include(e=> e.AllAssociations).ThenInclude(e=> e.category).ToList();
        return View(AllProducts);
    }

    public IActionResult Privacy()
    {
        ViewBag.AllCategories = _context.Categories.ToList();
        return View();
    }

    [AdminCheck]
    [HttpGet("newproduct")]
    public IActionResult NewProduct()
    {
        return View();
    }

    [AdminCheck]
    [HttpPost("registerproduct")]
public async Task<IActionResult> RegisterProduct(Product product)
{
    if (ModelState.IsValid)
    {
        if (product.ImageFile != null && product.ImageFile.Length > 0)
        {
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
        _context.Add(product);
        _context.SaveChanges();
        return RedirectToAction("Index");
    }
    ViewBag.AllProducts = _context.Products.ToList();
    return View("Index", product);
}


    [AdminCheck]
    [HttpPost("registercategory")]
    public IActionResult RegisterCategory(Category category)
    {
        if (ModelState.IsValid)
        {
            _context.Add(category);
            _context.SaveChanges();
            return RedirectToAction("Privacy");
        }
        ViewBag.AllCategories = _context.Categories.ToList();
        return View("Privacy");
    }




    [HttpPost("product/addcategory/{id}")]
    public IActionResult AddProductCategory(Association a, int id)
    {
        a.ProductId = id;
        if (ModelState.IsValid)
        {

            bool associationExists = _context.Associations
                .Any(assoc => assoc.ProductId == a.ProductId && assoc.CategoryId == a.CategoryId);

            if (!associationExists)
            {
                _context.Add(a);
                _context.SaveChanges();
                return RedirectToAction("ProductDetails" ,new{id=id});
            }
            else
            {
                ModelState.AddModelError(nameof(a.CategoryId), "Category already added!");
                ViewBag.product = _context.Products.Include(e => e.AllAssociations).ThenInclude(f => f.category).FirstOrDefault(s => s.ProductId == id);
                ViewBag.Categories = _context.Categories.ToList();
                return View("ProductDetails", a);
            }
        }
        return View("ProductDetails", a);
    }


    [HttpGet("category/details/{id}")]
    public IActionResult CategoryDetails(int id)
    {
        ViewBag.category = _context.Categories.Include(e => e.AllAssociations).ThenInclude(f => f.product).FirstOrDefault(s => s.CategoryId == id);
        ViewBag.Products = _context.Products.ToList();

        return View();
    }

    [HttpPost("category/addproduct/{id}")]
    public IActionResult AddCategoryProduct(Association a, int id)
    {
        a.CategoryId = id;
        if (ModelState.IsValid)
        {

            bool associationExists = _context.Associations
                .Any(assoc => assoc.ProductId == a.ProductId && assoc.CategoryId == a.CategoryId);

            if (!associationExists)
            {
                _context.Add(a);
                _context.SaveChanges();
                return RedirectToAction("CategoryDetails" ,new{id=id});
            }
            else
            {
                ModelState.AddModelError(nameof(a.ProductId), "Product already added!");
                ViewBag.category = _context.Categories.Include(e => e.AllAssociations).ThenInclude(f => f.product).FirstOrDefault(s => s.CategoryId == id);
                ViewBag.Products = _context.Products.ToList();
                return View("CategoryDetails", a);
            }

        }
        return View("CategoryDetails", a);
    }


    [HttpPost("Home/DestroyAssocP/{id}")]
    public IActionResult DeleteAssocP(Association association, int id){
        Association? ToDelete = _context.Associations.FirstOrDefault(e => e.ProductId==association.ProductId && e.CategoryId == association.CategoryId);
        _context.Remove(ToDelete);
        _context.SaveChanges();
        return RedirectToAction("ProductDetails", new{id = id});
    }


       [HttpPost("Home/DestroyAssocC/{id}")]
    public IActionResult DeleteAssocC(Association association, int id){
        Association? ToDelete = _context.Associations.FirstOrDefault(e => e.ProductId==association.ProductId && e.CategoryId == association.CategoryId);
        _context.Remove(ToDelete);
        _context.SaveChanges();
        return RedirectToAction("CategoryDetails", new{id = id});
    }

    [SessionCheck]
    [HttpGet("item/details/{id}")]
    public IActionResult ItemDetails(int id)
    {
        Product product = _context.Products.FirstOrDefault(e=> e.ProductId ==id);
        DataOne data = new DataOne();
        data.Product = product;
        return View(data);
    }


    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }

    [HttpPost("Home/Purchase/{id}")]
    public IActionResult Purchase(Purchase purchase,int id){
        Product product = _context.Products.FirstOrDefault(e=> e.ProductId == id);
        DataOne data = new DataOne();
        data.Product = product;
        if(ModelState.IsValid){
        if(product.Quantity < purchase.Quantity || purchase.Quantity<0){
            ModelState.AddModelError("Quantity", "Not Enough Stock!");
        
        return View("ItemDetails",data);
        }
        else{
            product.Quantity -= purchase.Quantity;
            UserReg user = _context.Users.FirstOrDefault(e=> e.id == HttpContext.Session.GetInt32("UserId"));
            user.Points += (product.Price * purchase.Quantity)/10;
            purchase.Total = product.Price * purchase.Quantity;
            _context.Add(purchase);
            _context.SaveChanges();
            return RedirectToAction("ItemDetails", new{id = id});
        }
        }
        return View();
    }

    [HttpGet("myprofile")]
    public IActionResult MyProfile(){
        UserReg user = _context.Users.Include(e=> e.Purchases).ThenInclude(e=> e.Product).FirstOrDefault(e=> e.id == HttpContext.Session.GetInt32("UserId"));
        return View(user);
    }

    [SessionCheck]
    [AdminCheck]
    [HttpGet("delete/{id}")]
    public IActionResult DeleteItem(int id){
        Product product = _context.Products.Include(e=> e.AllAssociations).FirstOrDefault(e=>e.ProductId == id);
        List<Association> association = _context.Associations.Where(e => e.ProductId == id).ToList();
        _context.RemoveRange(association);
        _context.Remove(product);
        _context.SaveChanges();
        return RedirectToAction("Shop");
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




}






public class SessionCheckAttribute : ActionFilterAttribute
{
    public override void OnActionExecuting(ActionExecutingContext context)
    {
        int? userId = context.HttpContext.Session.GetInt32("UserId");
        if (userId == null)
        {
            context.Result = new RedirectToActionResult("Index", "Login", null);
        }
    }
}

public class AdminCheckAttribute : ActionFilterAttribute
{
    public override void OnActionExecuting(ActionExecutingContext context)
    {
        int? userId = context.HttpContext.Session.GetInt32("AdminId");
        if (userId == null)
        {
            context.Result = new RedirectToActionResult("Index", "Home", null);
        }
    }
}













