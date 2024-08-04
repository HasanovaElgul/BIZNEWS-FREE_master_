using BIZNEWS_FREE.DTOs;
using BIZNEWS_FREE.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace BIZNEWS_FREE.Controllers
{
    // DTO = Data Transfer Object
    public class AuthController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager; // daxil olmaq üçün lazımdır

        // Dependency Injection
        public AuthController(UserManager<User> userManager, SignInManager<User> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager; // SignInManager-in düzgün şəkildə inisializasiyası
        }



        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterDTO registerDTO)
        {
            if (!ModelState.IsValid) // əgər məcburi xanaları göstərmək üçün
            {
                return View();
            }

            User newUser = new()
            {
                Firstname = registerDTO.Firstname,
                Lastname = registerDTO.Lastname,
                Email = registerDTO.Email,
                PhotoUrl = "/",
                UserName = registerDTO.Email
            };

            IdentityResult identityResult = await _userManager.CreateAsync(newUser, registerDTO.Password);
            if (identityResult.Succeeded)
            {
                return RedirectToAction("Index", "Home"); // əgər uğurludursa index səhifəsinə qayıtmaq üçün
            }
            else
            {
                foreach (var error in identityResult.Errors)
                {
                    ModelState.AddModelError("error", error.Description);
                }
                return View();
            }
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginDTO loginDTO)
        {
            if (!ModelState.IsValid)
            {
                return View(loginDTO);
            }

            var findUser = await _userManager.FindByEmailAsync(loginDTO.Email);

            if (findUser == null)
            {
                ModelState.AddModelError("error", "İstifadəçi tapılmadı!");
                return View();
            }

            var signInResult = await _signInManager.PasswordSignInAsync(findUser, loginDTO.Password, false, lockoutOnFailure: false);

            if (signInResult.Succeeded)
            {
                return RedirectToAction("Index", "Home");
            }
            else
            {
                ModelState.AddModelError("error", "İstifadəçi adı və ya şifrə yanlışdır!");
                return View();
            }
        }

        [Authorize]
        public async Task<IActionResult> LogOut()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }
    }
}
