using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace BIZNEWS_FREE.Areas.Admin.Controllers
{
    [Area(nameof(Admin))]
    public class RoleController : Controller
    {
        private readonly RoleManager<IdentityRole> _roleManager;

        public RoleController(RoleManager<IdentityRole> roleManager)
        {
            _roleManager = roleManager;
        }


        public IActionResult Index()
        {
            var roles = _roleManager.Roles.ToList(); // Получение всех ролей в виде списка
            return View(roles);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(IdentityRole identityRole)
        {
            if (!ModelState.IsValid)
            {
                return View(identityRole);
            }

            var checkRole = await _roleManager.FindByNameAsync(identityRole.Name);
            if (checkRole != null)
            {
                ModelState.AddModelError("Name", "Role name already exists!");
                return View(identityRole); // Возвращаем модель, чтобы заполненные данные оставались 
            }

            var result = await _roleManager.CreateAsync(identityRole);
            if (result.Succeeded)
            {
                return RedirectToAction("Index");
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return View(identityRole);
        }
    }
}
