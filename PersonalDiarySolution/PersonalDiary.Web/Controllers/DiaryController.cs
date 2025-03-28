using Microsoft.AspNetCore.Mvc;
using PersonalDiary.Web.Models.DTOs;
using PersonalDiary.Web.Models.ViewModels;
using PersonalDiary.Web.Services.Interface;

namespace PersonalDiary.Web.Controllers
{
    public class DiaryController : Controller
    {
        private readonly IHttpClientService _httpClientService;

        public DiaryController(IHttpClientService httpClientService)
        {
            _httpClientService = httpClientService;
        }

        // GET: /Diary
        public async Task<IActionResult> Index()
        {
            // Kiểm tra đăng nhập
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("JWTToken")))
            {
                return RedirectToAction("Login", "Account");
            }

            try
            {
                var entries = await _httpClientService.GetMyEntriesAsync();
                return View(entries);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Không thể lấy danh sách nhật ký: " + ex.Message;
                return RedirectToAction("Index", "Home");
            }
        }

        // GET: /Diary/Details/5
        public async Task<IActionResult> Details(int id)
        {
            try
            {
                var entry = await _httpClientService.GetDiaryEntryAsync(id);
                var comments = await _httpClientService.GetCommentsByEntryAsync(id);
               
                var (publicCount, otherEntries) = await _httpClientService.GetAuthorInfoAsync(
                    entry.Username,
                    entry.EntryId
                );

                var viewModel = new DiaryEntryViewModel
                {
                    Entry = entry,
                    Comments = comments,
                    NewComment = new CommentCreateDTO { entryId = id },
                    IsAuthenticated = !string.IsNullOrEmpty(HttpContext.Session.GetString("JWTToken")),
                    IsOwner = entry.Username == HttpContext.Session.GetString("Username"),
                    PublicPostCount = publicCount,
                    OtherEntries = otherEntries
                };

                return View(viewModel);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Không thể xem bài viết: " + ex.Message;
                return RedirectToAction("Index", "Home");
            }
        }

        // GET: /Diary/Create
        public async Task<IActionResult> Create()
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("JWTToken")))
            {
                return RedirectToAction("Login", "Account");
            }

            // Lấy danh sách tag để hiển thị gợi ý
            ViewBag.Tags = await _httpClientService.GetTagsAsync();

            // Lấy danh sách tag phổ biến
            ViewBag.PopularTags = await _httpClientService.GetPopularTagsAsync(15);

            return View();
        }

        // POST: /Diary/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(DiaryEntryCreateDTO entryDto)
        {
            // Kiểm tra đăng nhập
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("JWTToken")))
            {
                return RedirectToAction("Login", "Account");
            }

            if (!ModelState.IsValid)
            {
                ViewBag.Tags = await _httpClientService.GetTagsAsync();
                return View(entryDto);
            }

            try
            {
                var createdEntry = await _httpClientService.CreateDiaryEntryAsync(entryDto);
                TempData["SuccessMessage"] = "Bài viết đã được tạo thành công!";
                return RedirectToAction(nameof(Details), new { id = createdEntry.EntryId });
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Không thể tạo bài viết: " + ex.Message);
                ViewBag.Tags = await _httpClientService.GetTagsAsync();
                return View(entryDto);
            }
        }

        // GET: /Diary/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            // Kiểm tra đăng nhập
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("JWTToken")))
            {
                return RedirectToAction("Login", "Account");
            }

            try
            {
                var entry = await _httpClientService.GetDiaryEntryAsync(id);

                // Kiểm tra quyền chỉnh sửa
                if (entry.Username != HttpContext.Session.GetString("Username"))
                {
                    TempData["ErrorMessage"] = "Bạn không có quyền chỉnh sửa bài viết này.";
                    return RedirectToAction("Index", "Home");
                }

                var editDto = new DiaryEntryUpdateDTO
                {
                    Title = entry.Title,
                    Content = entry.Content,
                    Mood = entry.Mood,
                    Weather = entry.Weather,
                    IsPublic = entry.IsPublic ?? false,
                    TagNames = entry.TagNames
                };

                ViewBag.Tags = await _httpClientService.GetTagsAsync();
                ViewBag.PopularTags = await _httpClientService.GetPopularTagsAsync(15);
                ViewBag.EntryId = id;
                return View(editDto);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Không thể tải bài viết để chỉnh sửa: " + ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: /Diary/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, DiaryEntryUpdateDTO entryDto)
        {
            // Kiểm tra đăng nhập
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("JWTToken")))
            {
                return RedirectToAction("Login", "Account");
            }

            if (!ModelState.IsValid)
            {
                ViewBag.Tags = await _httpClientService.GetTagsAsync();
                ViewBag.PopularTags = await _httpClientService.GetPopularTagsAsync(15);
                ViewBag.EntryId = id;
                return View(entryDto);
            }

            try
            {
                // Đảm bảo TagNames không null khi gửi đến API
                entryDto.TagNames ??= new List<string>();

                await _httpClientService.UpdateDiaryEntryAsync(id, entryDto);
                TempData["SuccessMessage"] = "Bài viết đã được cập nhật thành công!";
                return RedirectToAction(nameof(Details), new { id });
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Không thể cập nhật bài viết: " + ex.Message);
                ViewBag.Tags = await _httpClientService.GetTagsAsync();
                ViewBag.PopularTags = await _httpClientService.GetPopularTagsAsync(15);
                ViewBag.EntryId = id;
                return View(entryDto);
            }
        }

        // POST: /Diary/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            // Kiểm tra đăng nhập
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("JWTToken")))
            {
                return RedirectToAction("Login", "Account");
            }

            try
            {
                await _httpClientService.DeleteDiaryEntryAsync(id);
                TempData["SuccessMessage"] = "Bài viết đã được xóa thành công!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Không thể xóa bài viết: " + ex.Message;
                return RedirectToAction(nameof(Details), new { id });
            }
        }


    }
}