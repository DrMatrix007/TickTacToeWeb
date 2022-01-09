using Microsoft.AspNetCore.Mvc;

namespace TickTacToeWeb.Controllers;
public class HomeController : Controller
{
    public IActionResult Index()
    {
        return View();
    }
}
