using Microsoft.AspNetCore.Mvc;

namespace BIZNEWS_FREE.Controllers
{
    public class ArticleController : Controller
    {
        public IActionResult Detail(int id)
        {
            return View();
        }

    }
}
