using BIZNEWS_FREE.Data;
using BIZNEWS_FREE.Helpers;
using BIZNEWS_FREE.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace BIZNEWS_FREE.Areas.Admin.Controllers
{
    [Area(nameof(Admin))]
    [Authorize]
    public class ArticleController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly UserManager<User> _userManager;
        private readonly ILogger<ArticleController> _logger;

        // Конструктор контроллера. Зависимости внедряются через конструктор.
        public ArticleController(AppDbContext context, IWebHostEnvironment env, IHttpContextAccessor contextAccessor, UserManager<User> userManager, ILogger<ArticleController> logger)
        {
            _context = context;
            _env = env;
            _contextAccessor = contextAccessor;
            _userManager = userManager;
            _logger = logger;
        }

        // Метод для отображения списка статей
        public IActionResult Index()
        {
            var articles = _context.Articles
                .Include(x => x.Category)
                .Include(x => x.ArticleTags)
                .ThenInclude(x => x.Tag).ToList(); // Получение списка статей из базы данных
            return View(articles);
        }

        // Метод для отображения формы создания новой статьи
        [HttpGet]
        [Authorize]
        public IActionResult Create()
        {
            var categories = _context.Categories.ToList();
            var tags = _context.Tags.ToList();
            ViewData["tags"] = tags; // Передаем список тегов в представление
            ViewBag.Categories = new SelectList(categories, "Id", "CategoryName"); // Создаем SelectList для категорий

            return View();
        }

        // Метод для обработки отправки формы создания новой статьи
        [HttpPost]
        [ValidateAntiForgeryToken] // Проверяет наличие и правильность токена
        public async Task<IActionResult> Create(Article article, IFormFile file, List<int> tagIds)
      {
            try
            {
                var categories = _context.Categories.ToList();
                var tags = _context.Tags.ToList();
                ViewData["tags"] = tags; // Передаем список тегов в представление
                ViewBag.Categories = new SelectList(categories, "Id", "CategoryName"); // Создаем SelectList для категорий

                var userId = _contextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                {
                    // Обработка случая, когда пользователь не найден
                    ModelState.AddModelError("", "Пользователь не найден");
                    return View();
                }

                Article newArticle = new();


                // Формирование пути для сохранения файла
                if (file != null)
                {

                    newArticle.PhotoUrl = await file.SaveFileAsync(_env.WebRootPath, "article-images");        //helpers была создана папка информцию получаем оттуда
                }
                else
                {
                    ModelState.AddModelError("", "Файл не выбран");
                    return View();
                }

                // Создание новой статьи        
                newArticle.Title = article.Title;
                newArticle.Content = article.Content;
                newArticle.CreatedDate = DateTime.Now;
                newArticle.CategoryId = article.CategoryId;
                newArticle.IsActive = article.IsActive;
                newArticle.IsFeature = article.IsFeature;
                newArticle.CreatedBy = $"{user.Firstname} {user.Lastname}";
                newArticle.SeoUrl = "";

                // Сохранение статьи в базе данных
                await _context.Articles.AddAsync(newArticle);
                await _context.SaveChangesAsync();

                // Добавление тегов к статье
                foreach (var tagId in tagIds)
                {
                    ArticleTag articleTag = new()
                    {
                        ArticleId = newArticle.Id,
                        TagId = tagId
                    };
                    _context.ArticleTags.Add(articleTag);
                }
                await _context.SaveChangesAsync();

                // Логирование успешного создания статьи
                _logger.LogInformation("Статья {Title} успешно создана пользователем {User}", newArticle.Title, user.Email);

                // Перенаправление на страницу списка статей после успешного создания
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                // Логирование ошибки
                _logger.LogError(ex, "Ошибка при создании статьи");

                // Обработка ошибки и возврат представления с сообщением об ошибке
                ModelState.AddModelError("", "Произошла ошибка при создании статьи");
                return View();
            }
        }
    }
}
