using Microsoft.AspNetCore.Mvc;
using PersonalDiary.Web.Models;
using PersonalDiary.Web.Models.ViewModels;
using PersonalDiary.Web.Services.Interface;
using System.Diagnostics;

namespace PersonalDiary.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly IHttpClientService _httpClientService;
        private readonly ILogger<HomeController> _logger;

        public HomeController(IHttpClientService httpClientService,ILogger<HomeController> logger)
        {
            _httpClientService = httpClientService;
            _logger = logger;
        }

        public async Task<IActionResult> Index()    
        {
            var viewModel = new HomeViewModel();
            try
            {
                //get diary entries public
                viewModel.PublicEntries = await _httpClientService.GetPublicEntriesAsync();

                //get popular tags
                viewModel.PopularTags = await _httpClientService.GetPopularTagsAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while getting data for homepage");
                TempData["ErrorMessage"] = "Error while getting data. Please try again later";
            }
            return View(viewModel);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
