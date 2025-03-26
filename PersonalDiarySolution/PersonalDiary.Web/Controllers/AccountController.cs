using Microsoft.AspNetCore.Mvc;
using PersonalDiary.Web.Models.DTOs;
using PersonalDiary.Web.Services.Interface;

namespace PersonalDiary.Web.Controllers
{
    public class AccountController : Controller
    {
        private readonly IHttpClientService _httpClientService;
        public AccountController(IHttpClientService httpClientService)
        {
            _httpClientService = httpClientService;
        }

        [HttpGet]
        public IActionResult Login()
        {
            //if user is already logged in, redirect to home page
            if (!string.IsNullOrEmpty(HttpContext.Session.GetString("JWTToken")))
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginRequest loginRequest)
        {
            if(!ModelState.IsValid)
            {
                return View(loginRequest);
            }

            var result = await _httpClientService.LoginAsync(loginRequest);
            if (result.Success)
            {
                TempData["SuccessMessage"] = $"Chào mừng {result.Username} đã quay trở lại!";
                return RedirectToAction("Index", "Home");
            }

            ModelState.AddModelError("", result.Message);
            return View(loginRequest);
        }

        [HttpGet]
        public IActionResult Register()
        {
            //if user is already logged in, redirect to home page
            if (!string.IsNullOrEmpty(HttpContext.Session.GetString("JWTToken")))
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterRequest registerRequest)
        {
            if(!ModelState.IsValid)
            {
                return View(registerRequest);
            }

            var result = await _httpClientService.RegisterAsync(registerRequest);
            if (result.Success)
            {
                TempData["SuceessMessage"] = $"Chào mừng {result.Username}! Tài khoản của bạn đã được tạo thành công.";
                return RedirectToAction("Index", "Home");
            }

            ModelState.AddModelError("", result.Message);
            return View(registerRequest);
        }

        [HttpPost]
        public IActionResult Logout()
        {
            HttpContext.Session.Remove("JWTToken");
            HttpContext.Session.Remove("Username");
            return RedirectToAction("Index", "Home");
        }
    }
}
