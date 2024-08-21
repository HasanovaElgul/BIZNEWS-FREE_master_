using BIZNEWS_FREE.Data;
using BIZNEWS_FREE.Models;
using BIZNEWS_FREE.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace BIZNEWS_FREE.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly AppDbContext _context;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            var featuredArticles = _context.Articles
                .Where(x => x.IsFeature == true && x.IsDeleted == false)
                .OrderByDescending(x => x.UpdatedDate)
                .Take(3).ToList();                           //ana sehifede 3 dene olan

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
