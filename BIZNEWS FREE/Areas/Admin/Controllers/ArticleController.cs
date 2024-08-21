using BIZNEWS_FREE.Data;
using BIZNEWS_FREE.Helpers;
using BIZNEWS_FREE.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Security.Claims;
using WebUI.Helper;

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
                .Where(x => x.IsDeleted == false)
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
                    SeoUrl = article.Title.ReplaceInvalidChars(),
                    PhotoUrl = await file.SaveFileAsync(_env.WebRootPath, "/article-images/") // Сохранение загруженного файла
                };

                // Добавление новой статьи в базу данных
                await _context.Articles.AddAsync(newArticle);
                await _context.SaveChangesAsync();

                // Проверка существующих тегов и добавление их к статье
                for (int i = 0; i < tagIds.Count; i++)
                {
                    ArticleTag articleTag = new()
                    {
                        ArticleId = newArticle.Id,
                        TagId = tagIds[i]
                    };
                    await _context.ArticleTags.AddAsync(articleTag);
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
            var path = (_env.WebRootPath + article.PhotoUrl).ToLower();         //serverde silinmis fayllari saxlamasin  silsin

            if (System.IO.File.Exists(path))
                System.IO.File.Delete(path);

            _context.Articles.Remove(article);
            await _context.SaveChangesAsync();
            return Redirect("/Admin/Article/Index");

        }



        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            // Найти статью по ID
            var article = await _context.Articles.Include(X => X.ArticleTags).ThenInclude(X => X.Tag).FirstOrDefaultAsync(x => x.Id == id);

            // Если статья не найдена, вернуть 404
            if (article == null)
                return NotFound();

            // Загрузка категорий и тегов для передачи в представление
            var categories = _context.Categories.ToList();
            var tags = _context.Tags.ToList();

            // Передача данных в ViewBag и ViewData
            ViewBag.Tags = tags;
            ViewBag.Categories = new SelectList(categories, "Id", "CategoryName");

            // Передача модели статьи в представление
            return View(article);
        }

        //todo Seo, Comment
        [HttpPost]

        public async Task<IActionResult> Edit(Article article, IFormFile file, List<int> tagIds)             //если где-то произошла ошибка, что выходил список этот
        {
            var categories = _context.Categories.ToList();
            var tags = _context.Tags.ToList();

            ViewBag.Tags = tags;

            // Создаем SelectList для категорий и передаем его в представление
            ViewBag.Categories = new SelectList(categories, "Id", "CategoryName");

            var userId = _contextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            var user = await _userManager.FindByIdAsync(userId);


            if (file != null)
                article.PhotoUrl = await file.SaveFileAsync(_env.WebRootPath, "/article-images/");


            article.SeoUrl = article.Title.ReplaceInvalidChars();
            article.UpdatedBy = user.Firstname + "" + user.Lastname;
            article.UpdatedDate = DateTime.Now;


            var findTags = _context.ArticleTags.Where(x => x.ArticleId == article.Id).ToList();
            _context.ArticleTags.RemoveRange(findTags);
            await _context.SaveChangesAsync();

            for (int i = 0; i < tagIds.Count; i++)
            {
                ArticleTag articleTag = new()
                {
                    ArticleId = article.Id,
                    TagId = tagIds[i]
                };
                await _context.ArticleTags.AddAsync(articleTag);
            }
            await _context.SaveChangesAsync();

            _context.Articles.Update(article);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");

        }
    }
}

