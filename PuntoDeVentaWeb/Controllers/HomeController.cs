using System.Diagnostics;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PuntoDeVentaWeb.Models;

namespace PuntoDeVentaWeb.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly SignInManager<User> _signInManager;
    private readonly UserManager<User> _userManager;

    public HomeController(ILogger<HomeController> logger, SignInManager<User> signInManager, UserManager<User> userManager)
    {
        _logger = logger;
        _signInManager = signInManager;
        _userManager = userManager;
    }

    public IActionResult Index()
    {
        // Check if the user is authenticated
        if (_signInManager.IsSignedIn(User))
        {
            var user = _userManager.GetUserAsync(User).Result;
            ViewData["User"] = user?.Name + " " + user?.LastName;
        }
        else
        {
            ViewData["User"] = "Stranger";
        }
        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
