using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using WebApplication1.Models;

public class HomeController : Controller
{
    private readonly MyDbContext _context;

    public HomeController(MyDbContext context)
    {
        _context = context;
    }

    // Публичные страницы - доступ без авторизации
    [AllowAnonymous]
    public async Task<IActionResult> Index()
    {
        return View();
    }

    [AllowAnonymous]
    public async Task<IActionResult> Contacts()
    {
        return View();
    }
}