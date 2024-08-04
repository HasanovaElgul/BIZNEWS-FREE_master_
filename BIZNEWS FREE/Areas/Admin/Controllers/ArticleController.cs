using BIZNEWS_FREE.Data;
using BIZNEWS_FREE.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Security.Claims;

namespace BIZNEWS_FREE.Areas.Admin.Controllers
{
    [Area(nameof(Admin))]
    public class ArticleController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly UserManager<User> _userManager;

        public ArticleController(AppDbContext context, IWebHostEnvironment env, IHttpContextAccessor contextAccessor, UserManager<User> userManager)
        {
            _context = context;
            _env = env;
            _contextAccessor = contextAccessor;
            _userManager = userManager;
        }

        // GET: /Admin/Article/Index
        public IActionResult Index()
        {
            var articles = _context.Articles.ToList(); // Получение списка статей из базы данных
            return View(articles);
        }

        // GET: /Admin/Article/Create
        [HttpGet]
        public IActionResult Create()
        {
            var categories = _context.Categories.ToList();
            var tags = _context.Tags.ToList();
            ViewData["tags"] = tags; // Передаем список тегов в представление
            ViewBag.Categories = new SelectList(categories, "Id", "CategoryName"); // Создаем SelectList для категорий

            return View();
        }

        // POST: /Admin/Article/Create
        [HttpPost]
        [ValidateAntiForgeryToken] // Проверяет наличие и правильность токена
        public async Task<IActionResult> Create(Article article, IFormFile file, List<int> tagIds)
        {
            var categories = _context.Categories.ToList();
            var tags = _context.Tags.ToList();
            ViewData["tags"] = tags; // Передаем список тегов в представление
            ViewBag.Categories = new SelectList(categories, "Id", "CategoryName"); // Создаем SelectList для категорий

            if (file == null || file.Length == 0)
            {
                ModelState.AddModelError("", "Выберите файл для загрузки.");
                return View(article);
            }

            // Проверка контекста и пользователя
            var user = _contextAccessor.HttpContext?.User;
            if (user == null)
            {
                return Unauthorized();
            }

            var userId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
            {
                return Unauthorized();
            }

            var currentUser = await _userManager.FindByIdAsync(userId);
            if (currentUser == null)
            {
                return Unauthorized();
            }

            var path = Path.Combine("uploads", Guid.NewGuid() + Path.GetFileName(file.FileName));
            var fullPath = Path.Combine(_env.WebRootPath, path);

            try
            {
                var directory = Path.GetDirectoryName(fullPath);
                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                using (var fileStream = new FileStream(fullPath, FileMode.Create))
                {
                    await file.CopyToAsync(fileStream);
                }
            }
            catch (Exception ex)
            {
                // Логирование ошибки загрузки файла
                // Предполагается, что у вас есть настроенный ILogger
                // _logger.LogError(ex, "Ошибка при загрузке файла.");
                ModelState.AddModelError("", "Произошла ошибка при загрузке файла.");
                return View(article);
            }

            var newArticle = new Article
            {
                PhotoUrl = path,
                Title = article.Title,
                Content = article.Content,
                CreatedDate = DateTime.Now,
                CategoryId = article.CategoryId,
                IsActive = article.IsActive,
                IsFeature = article.IsFeature,
                CreatedBy = $"{currentUser.Firstname} {currentUser.Lastname}"
            };

            try
            {
                await _context.Articles.AddAsync(newArticle);
                await _context.SaveChangesAsync();

                foreach (var tagId in tagIds)
                {
                    var articleTag = new ArticleTag
                    {
                        ArticleId = newArticle.Id,
                        TagId = tagId
                    };
                    await _context.ArticleTags.AddAsync(articleTag);
                }
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                // Логирование ошибки сохранения статьи
                // _logger.LogError(ex, "Ошибка при сохранении статьи.");
                ModelState.AddModelError("", "Произошла ошибка при сохранении статьи.");
                return View(article);
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
