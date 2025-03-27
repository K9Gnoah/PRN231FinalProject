using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using PersonalDiary.Web.Models.DTOs;
using PersonalDiary.Web.Services.Interface;

namespace PersonalDiary.Web.Controllers
{
    public class CommentController : Controller
    {
        private readonly IHttpClientService _httpClient;
        private readonly ILogger<CommentController> _logger;

        public CommentController(IHttpClientService httpClient, ILogger<CommentController> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        [HttpPost]
        public async Task<IActionResult> Create(CommentCreateDTO model)
        {
            try
            {
                // Kiểm tra đăng nhập
                var token = HttpContext.Session.GetString("JWTToken");
                _logger.LogInformation("User has token: {HasToken}", !string.IsNullOrEmpty(token));

                // Debug dữ liệu trước khi gửi
                _logger.LogInformation("Creating comment with EntryId: {EntryId}, Content length: {ContentLength}, GuestName: {GuestName}",
                    model.entryId, model.content?.Length ?? 0, model.guestName);

                // Đảm bảo GuestName có giá trị nếu chưa đăng nhập
                if (string.IsNullOrEmpty(token) && string.IsNullOrEmpty(model.guestName))
                {
                    _logger.LogWarning("Guest user tried to comment without name");
                    TempData["Error"] = "Please enter your name to comment";
                    return RedirectToAction("Details", "Diary", new { id = model.entryId });
                }

                // Gọi API để tạo comment
                var createdComment = await _httpClient.CreateCommentAsync(model);
                TempData["Success"] = "Comment added successfully";

                return RedirectToAction("Details", "Diary", new { id = model.entryId });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating comment: {Message}", ex.Message);
                TempData["Error"] = "An error occurred while creating the comment";
                return RedirectToAction("Details", "Diary", new { id = model.entryId });
            }
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id, int entryId)
        {
            try
            {
                await _httpClient.DeleteCommentAsync(id);
                TempData["Success"] = "Comment deleted successfully";
            }
            catch (System.Exception ex)
            {
                TempData["Error"] = $"Failed to delete comment: {ex.Message}";
            }

            return RedirectToAction("Details", "Diary", new { id = entryId });
        }

        [HttpPost]
        public async Task<IActionResult> Update(int id, CommentUpdateDTO model)
        {
            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Please check your input and try again.";
                return RedirectToAction("Details", "Diary", new { id = model.EntryId });
            }

            try
            {
                var success = await _httpClient.UpdateCommentAsync(id, model);
                if(success)
                {
                    TempData["Success"] = "Comment updated succesfully";
                }
                else
                {
                    TempData["Error"] = "Failed to update comment";
                }
            }
            catch(System.Exception ex)
            {
                TempData["Error"] = $"Failed to update comment: {ex.Message}";
            }

            return RedirectToAction("Details", "Diary", new { id = model.EntryId });
        }

        //sp for ajax
        [HttpPost]
        public async Task<IActionResult> CreateAjax([FromBody] CommentCreateDTO model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var comment = await _httpClient.CreateCommentAsync(model);
                return Ok(comment);
            }
            catch (System.Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        
    }
}
