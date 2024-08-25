using BIZNEWS_FREE.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BIZNEWS_FREE.Controllers
{
    public class ArticleController : Controller
    {
        private readonly AppDbContext _context;

        public ArticleController(AppDbContext context)
        {
            _context = context;
        }
        public IActionResult Detail(int id)
        {
            var article = _context.Articles
                .Include(x => x.Category)
                .Include(x => x.ArticleTags)
                .ThenInclude(x => x.Tag) // Предполагается, что Tag является свойством в ArticleTags
                .FirstOrDefault(x => x.Id == id);
            return View(article);

        }

    }
}
