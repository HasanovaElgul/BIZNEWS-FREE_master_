using BIZNEWS_FREE.Data;
using BIZNEWS_FREE.Models;
using BIZNEWS_FREE.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace BIZNEWS_FREE.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly AppDbContext _context;

        public HomeController(ILogger<HomeController> logger, AppDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult Index()
{
    var featuredArticles = _context.Articles
        .Include(x => x.Category)
        .Where(x => x.IsActive == true && x.IsFeature == false)
        .OrderByDescending(x => x.ViewCount)
        .Take(7)
        .ToList();

    var articles = _context.Articles
        .Include(x => x.Category)
        .Where(x => x.IsDeleted == false)
        .OrderByDescending(x => x.UpdatedDate)
        .ToList();

    HomeVM homeVM = new()
    {
        FeaturedArticles = featuredArticles,
        Articles = articles
    };

    return View(homeVM);
}




        public IActionResult Privacy()
        {
            return View();
        }

        
    }
}