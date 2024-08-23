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
                .OrderByDescending(x => x.UpdatedDate)
                .Take(7).ToList();                           // Берем 7 статей: 3 для основной карусели и 4 для дополнительного блока

            HomeVM homeVM = new()
            {
                FeaturedArticles = featuredArticles
            };
            return View(homeVM);
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
}
