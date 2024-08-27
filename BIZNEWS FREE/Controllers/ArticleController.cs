using BIZNEWS_FREE.Data;
using BIZNEWS_FREE.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;



namespace BIZNEWS_FREE.Controllers
{
    public class ArticleController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IHttpContextAccessor _contextAccessor;

        public ArticleController(AppDbContext context, IHttpContextAccessor contextAccessor)
        {
            _context = context;
           _contextAccessor = contextAccessor;
        }
        public IActionResult Detail(int id)
        {
            
            
            var article = _context.Articles
                .Include(x => x.Category)
                .Include(x => x.ArticleTags)
                .ThenInclude(x => x.Tag) // Предполагается, что Tag является свойством в ArticleTags
                .FirstOrDefault(x => x.Id == id);

            if (article == null)
            {
                return NotFound();
            }

            var cookie = _contextAccessor.HttpContext.Request.Cookies["Views"];
            string[] findCookie = { "" };
            
            if (cookie != null)
            {
                findCookie = cookie.Split('-').ToArray();
            }

            if (!findCookie.Contains(article.Id.ToString()))
            {
                Response.Cookies.Append("Views", $"{cookie}-{article.Id}");
            }

               return View(article);
        }


    }
}
