using Microsoft.AspNetCore.Mvc;

namespace TickTacToeWeb.Controllers;
[Route("[controller]")]
public class GameController : Controller
{
    [HttpGet("/Enter")]
    public IActionResult Enter()
    {
        return View();
    }

    [HttpGet("/Play")]
    public IActionResult Play()
    {
        return View();
    }
    [HttpGet("/Play/{id}")]
    public IActionResult Play(Guid id)
    {
        return View(id);
    }

}
