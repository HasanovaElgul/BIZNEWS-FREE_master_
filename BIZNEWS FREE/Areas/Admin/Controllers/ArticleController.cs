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
    [Authorize] // Требует аутентификации для доступа к методам контроллера
    public class ArticleController : Controller
    {
        private readonly AppDbContext _context; // Контекст базы данных
        private readonly IWebHostEnvironment _env; // Среда веб-хостинга
        private readonly IHttpContextAccessor _contextAccessor; // Доступ к текущему контексту HTTP
        private readonly UserManager<User> _userManager; // Управление пользователями
        private readonly ILogger<ArticleController> _logger; // Логирование

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
            // Получение списка статей из базы данных с включением связанных категорий и тегов
            var articles = _context.Articles
                .Include(x => x.Category) // Включаем связанные категории
                .Include(x => x.ArticleTags) // Включаем связанные теги
                .ThenInclude(x => x.Tag) // Включаем сами теги
                .ToList();

            // Передаем список статей в представление
            return View(articles);
        }

        // Метод для отображения формы создания новой статьи
        [HttpGet]
        [Authorize]
        public IActionResult Create()
        {
            // Получаем список категорий и тегов из базы данных
            var categories = _context.Categories.ToList();
            var tags = _context.Tags.ToList();

            // Передаем список тегов в представление
            ViewData["tags"] = tags;

            // Создаем SelectList для категорий и передаем его в представление
            ViewBag.Categories = new SelectList(categories, "Id", "CategoryName");

            return View();
        }

        // Метод для обработки отправки формы создания новой статьи
        [HttpPost]
        [ValidateAntiForgeryToken] // Проверяет наличие и правильность токена для предотвращения CSRF атак
        public async Task<IActionResult> Create(Article article, IFormFile file, List<int> tagIds)
        {
            try
            {
                // Загрузка данных категорий и тегов для использования в представлении
                LoadCategoriesAndTags();

                // Получение идентификатора текущего пользователя
                var userId = _contextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (userId == null)
                {
                    ModelState.AddModelError("", "Пользователь не найден");
                    return View();
                }

                // Поиск пользователя по его идентификатору
                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                {
                    ModelState.AddModelError("", "Пользователь не найден");
                    return View();
                }

                // Проверка, был ли выбран файл
                if (file == null || file.Length == 0)
                {
                    ModelState.AddModelError("", "Файл не выбран");
                    return View();
                }

                // Создание нового объекта статьи
                var newArticle = new Article
                {
                    Title = article.Title,
                    Content = article.Content,
                    CreatedDate = DateTime.Now,
                    CategoryId = article.CategoryId,
                    IsActive = article.IsActive,
                    IsFeature = article.IsFeature,
                    CreatedBy = $"{user.Firstname} {user.Lastname}",
                    SeoUrl = "", // Замените на логику генерации SEO URL
                    PhotoUrl = await file.SaveFileAsync(_env.WebRootPath, "article-images") // Сохранение загруженного файла
                };

                // Добавление новой статьи в базу данных
                await _context.Articles.AddAsync(newArticle);
                await _context.SaveChangesAsync();

                // Проверка существующих тегов и добавление их к статье
                var validTagIds = _context.Tags.Where(t => tagIds.Contains(t.Id)).Select(t => t.Id).ToList();
                foreach (var tagId in validTagIds)
                {
                    var articleTag = new ArticleTag
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
                // Логирование ошибки и отображение сообщения об ошибке
                _logger.LogError(ex, "Ошибка при создании статьи: {Message}", ex.Message);
                ModelState.AddModelError("", "Произошла ошибка при создании статьи");
                return View();
            }
        }

        // Метод для загрузки категорий и тегов
        private void LoadCategoriesAndTags()
        {
            var categories = _context.Categories.ToList();
            var tags = _context.Tags.ToList();
            ViewData["tags"] = tags;
            ViewBag.Categories = new SelectList(categories, "Id", "CategoryName");
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var article = _context.Articles.FirstOrDefault(x => x.Id == id);
            _context.Articles.Remove(article);
            await _context.SaveChangesAsync();
            return Redirect("/Admin/Article/Index");
        }
    }
}
