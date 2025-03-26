using Microsoft.AspNetCore.Mvc;
using PersonalDiary.Web.Services.Interface;

namespace PersonalDiary.Web.Controllers
{
    public class TagController : Controller
    {
        private readonly IHttpClientService _httpClientService;

        public TagController(IHttpClientService httpClientService)
        {
            _httpClientService = httpClientService;
        }

        // GET: /Tag/Entries/5
        public async Task<IActionResult> Entries(int id)
        {
            try
            {
                var entries = await _httpClientService.GetEntriesByTagAsync(id);
                var tags = await _httpClientService.GetTagsAsync();
                var currentTag = tags.FirstOrDefault(t => t.TagId == id);

                ViewBag.TagName = currentTag?.TagName ?? "Unknown Tag";
                ViewBag.TagId = id;

                return View(entries);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Không thể tải bài viết cho tag này: " + ex.Message;
                return RedirectToAction("Index", "Home");
            }
        }
    }
}