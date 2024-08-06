using BIZNEWS_FREE.Areas.Admin.ViewModels;
using BIZNEWS_FREE.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BIZNEWS_FREE.Areas.Admin.Controllers
{
    [Area(nameof(Admin))]
    public class UsersController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public UsersController(UserManager<User> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task<IActionResult> Index(string searchUser)
        {
            var usersQuery = _userManager.Users.AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchUser))
            {
                usersQuery = usersQuery.Where(u => u.Firstname.Contains(searchUser) || u.Email.Contains(searchUser));
            }

            var users = await usersQuery.ToListAsync();
            return View(users);
        }

        [HttpGet]

        public async Task<IActionResult> AddRole(string userId)
        {
            var findUser = await _userManager.FindByIdAsync(userId);
            if (findUser == null) return NotFound();

            var userRoles = (await _userManager.GetRolesAsync(findUser)).ToList();
            var roles = _roleManager.Roles.Select(x => x.Name).ToList();

            UserVM userVM = new()
            {
                User = findUser,
                Roles = roles.Except(userRoles)
            };

            return View(userVM);
        }

        [HttpPost]
        public async Task<IActionResult> AddRole(string userId, string role)
        {
            if (string.IsNullOrWhiteSpace(role))
            {
                ModelState.AddModelError("", "Role cannot be empty.");
                return RedirectToAction(nameof(AddRole), new { userId });
            }

            var findUser = await _userManager.FindByIdAsync(userId);
            if (findUser == null) return NotFound();

            var result = await _userManager.AddToRoleAsync(findUser, role);
            if (result.Succeeded)
                return RedirectToAction(nameof(Index));

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return RedirectToAction(nameof(AddRole), new { userId });
        }

        [HttpGet]
        public async Task<IActionResult> EditRole(string userId)
        {
            var findUser = await _userManager.FindByIdAsync(userId);
            if (findUser == null) return NotFound();
            return View(findUser);

        }



        [HttpGet]
        public async Task<IActionResult> DeleteRole(string userId, string role)
        {
            var findUser = await _userManager.FindByIdAsync(userId);
            await _userManager.RemoveFromRoleAsync(findUser, role);
            return Redirect("/admin/users/index");


        }
    }
}
