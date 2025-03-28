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
            if (!string.IsNullOrEmpty(HttpContext.Session.GetString("JWTToken")))
            {
                return RedirectToAction("Index", "Home");
            }

            return View();
        }

        // POST: /Account/Register
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterDTO model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View(model);
                }

                if (model.Password != model.ConfirmPassword)
                {
                    ModelState.AddModelError("ConfirmPassword", "Mật khẩu xác nhận không khớp");
                    return View(model);
                }

                // Gọi API để đăng ký
                var result = await _httpClientService.RegisterAsync(model);

                if (result.Success)
                {
                    TempData["SuccessMessage"] = "Đăng ký tài khoản thành công! Vui lòng đăng nhập để tiếp tục.";
                    return RedirectToAction("Login");
                }
                else
                {
                    // Hiển thị thông báo lỗi chi tiết từ API
                    ModelState.AddModelError("", result.Message);

                    // Log lỗi để debug
                    Console.WriteLine($"Registration failed: {result.Message}");

                    return View(model);
                }
            }
            catch (Exception ex)
            {
                // Log exception
                Console.WriteLine($"Exception during registration: {ex.Message}");
                ModelState.AddModelError("", $"Lỗi đăng ký: {ex.Message}");
                return View(model);
            }
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
