using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using ProjectNet.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
namespace ProjectNet.Controllers;

public class LoginController : Controller
{    
    private readonly ILogger<LoginController> _logger;
    // Add a private variable of type MyContext (or whatever you named your context file)
    private MyContext _context;         
    // Here we can "inject" our context service into the constructor 
    // The "logger" was something that was already in our code, we're just adding around it   
    public LoginController(ILogger<LoginController> logger, MyContext context)    
    {        
        _logger = logger;
        // When our HomeController is instantiated, it will fill in _context with context
        // Remember that when context is initialized, it brings in everything we need from DbContext
        // which comes from Entity Framework Core
        _context = context;    
    } 

    public IActionResult Index()
    {
        return View();
    }

   [HttpPost("login")]
    public IActionResult Login(UserLogin user){
        if(ModelState.IsValid){
            UserReg? CurrentUser = _context.Users.Include(e=> e.Carts).FirstOrDefault(e => e.Email == user.LEmail);
            if(CurrentUser == null){
                ModelState.AddModelError("LEmail", "Invalid Username/Password");
                return View("Index");
            }
            PasswordHasher<UserLogin> hasher = new PasswordHasher<UserLogin> ();
            var result = hasher.VerifyHashedPassword(user, CurrentUser.Password, user.LPassword);
            if(result == 0){
                ModelState.AddModelError("LPassword", "Password invalid");
                return View("Index");
            }
            if(CurrentUser.Role == 1){
                HttpContext.Session.SetInt32("AdminId", CurrentUser.id);
                HttpContext.Session.SetInt32("UserId", CurrentUser.id);
                HttpContext.Session.SetString("UserName", CurrentUser.FirstName);
                return  RedirectToAction("Index", "Admin");
            }
            HttpContext.Session.SetInt32("UserId", CurrentUser.id);
            HttpContext.Session.SetString("UserName", CurrentUser.FirstName);
            HttpContext.Session.SetInt32("CartNo", CurrentUser.Carts.Count);
            return  RedirectToAction("Index", "Home");
        }
        else{
            return View("Index");
        }
    }

    [HttpGet("reg")]
    public IActionResult PreRegister(){
        return View();
    }

        [HttpPost("register")]
    public IActionResult Register(UserReg user){
        if(ModelState.IsValid){
            PasswordHasher<UserReg> Hasher = new PasswordHasher<UserReg>();
            user.Password = Hasher.HashPassword(user, user.Password);  
            _context.Add(user);
            _context.SaveChanges();
            HttpContext.Session.SetInt32("UserId", user.id);
            HttpContext.Session.SetInt32("CartNo", 0);
            HttpContext.Session.SetString("UserName", user.FirstName);
            return RedirectToAction("Index", "Home");
        }
        else{
            return View("PreRegister");
        }
    }

    
}