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

    
    [HttpGet("shop")]
    public IActionResult Shop()
    {
        DataOne data = new DataOne();
        List<Product>AllProducts = _context.Products.Include(e=> e.AllAssociations).ThenInclude(e=> e.category).ToList();
        List<Category> AllCategories = _context.Categories.Include(e=> e.AllAssociations).Where(e=> e.AllAssociations.Count!=0).ToList();
        data.AllProducts = AllProducts;
        data.AllCategories = AllCategories;
        return View(data);
    }

    [HttpGet("shop/{id}")]
    public IActionResult ShopByCategory(int id)
    {
        DataOne data = new DataOne();
        List<Product>AllProducts = _context.Products.Include(e=> e.AllAssociations).Where(e => e.AllAssociations.Any(e => e.CategoryId == id)).ToList();
        List<Category> AllCategories = _context.Categories.Include(e=> e.AllAssociations).Where(e=> e.AllAssociations.Count!=0).ToList();
        data.AllProducts = AllProducts;
        data.AllCategories = AllCategories;
        return View("Shop",data);
    }

    public IActionResult Privacy()
    {
        ViewBag.AllCategories = _context.Categories.ToList();
        return View();
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

    [SessionCheck]
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
        return View("ItemDetails",data);
    }
    [SessionCheck]
    [HttpGet("myprofile")]
    public IActionResult MyProfile(){
        UserReg user = _context.Users.Include(e=> e.Purchases).ThenInclude(e=> e.Product).FirstOrDefault(e=> e.id == HttpContext.Session.GetInt32("UserId"));
        return View(user);
    }
    [SessionCheck]
    [HttpPost("addtocart/{id}")]
    public IActionResult AddToCart(DataOne data,int id)
    {
        Product? product = _context.Products.FirstOrDefault(e=> e.ProductId == id);
        Cart newCart = data.Cart;
        product.Quantity -= newCart.Quantity;
        newCart.ProductId = id;
        newCart.UserId =  HttpContext.Session.GetInt32("UserId");
        Console.WriteLine($"Sasia vjen sa : {newCart.Quantity}");
        int? currentCartNo = HttpContext.Session.GetInt32("CartNo");

    if (currentCartNo.HasValue)
    {
    int newCartNo = currentCartNo.Value + 1;
    HttpContext.Session.SetInt32("CartNo", newCartNo);
    _context.Add(newCart);
        _context.SaveChanges();
        return RedirectToAction("ItemDetails", new{id=id});
    }
    else
    {
    HttpContext.Session.SetInt32("CartNo", 1);
    }
        _context.Add(newCart);
        _context.SaveChanges();
        return RedirectToAction("ItemDetails", new{id=id});
    }

    [SessionCheck]
    [HttpGet("mycart")]
    public IActionResult MyCart(){
        List <Cart> MyCarts = _context.Carts.Include(e=> e.User).Include(e=> e.Product).Where(e=> e.UserId == HttpContext.Session.GetInt32("UserId")).OrderByDescending(e=> e.CartId).ToList();
        return View(MyCarts);
    }


    [SessionCheck]
    [HttpGet("cart/remove/{id}")]
    public IActionResult RemoveFromCart(int id)
    {
        Cart? cart = _context.Carts.FirstOrDefault(e=> e.CartId == id);
        Product? product = _context.Products.FirstOrDefault(e=> e.ProductId == cart.ProductId);
        product.Quantity += cart.Quantity;
        _context.Remove(cart);
        _context.SaveChanges();
        int? currentCartNo = HttpContext.Session.GetInt32("CartNo");
        if (currentCartNo.HasValue && currentCartNo !=0)
        {
        int newCartNo = currentCartNo.Value - 1;
        HttpContext.Session.SetInt32("CartNo", newCartNo);
        }else{
            HttpContext.Session.SetInt32("CartNo", 0);
        }
    
        return RedirectToAction("MyCart");
    }

    [SessionCheck]
    [HttpGet("cart/purchase")]
    public IActionResult BuyFromCart()
    {
        List <Cart>? AllCarts = _context.Carts.Include(e=> e.Product).Include(e=> e.User).Where(e=> e.UserId == HttpContext.Session.GetInt32("UserId")).ToList();
        foreach(Cart item in AllCarts){
            item.User.Points += (item.Product.Price * item.Quantity)/10;
            Purchase purchase = new Purchase ();
            purchase.Quantity = item.Quantity;
            purchase.Total = item.Product.Price * purchase.Quantity;
            purchase.UserId = HttpContext.Session.GetInt32("UserId");
            purchase.ProductId = item.ProductId;
            _context.Purchases.Add(purchase);
            _context.Carts.Remove(item);
        }
        _context.SaveChanges();
        HttpContext.Session.SetInt32("CartNo", 0);
        return RedirectToAction("MyCart");
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













