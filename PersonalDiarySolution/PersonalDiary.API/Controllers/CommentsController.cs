using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PersonalDiary.API.DTOs;
using PersonalDiary.API.Models;
using System.Security.Claims;

namespace PersonalDiary.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CommentsController : ControllerBase
    {
        private readonly PersonalDiaryDBContext _context;
        private readonly ILogger<CommentsController> _logger;
        public CommentsController(PersonalDiaryDBContext context, ILogger<CommentsController> logger)
        {
            _context = context;
            _logger = logger;
        }

        //get by entry id
        [HttpGet("entry/{entryId}")]
        public async Task<ActionResult<IEnumerable<CommentDTO>>> GetCommentsByEntry(int entryId)
        {
            try
            {
                var entry = await _context.DiaryEntries.FindAsync(entryId);
                if (entry == null)
                {
                    return NotFound("The post does not exist");
                }

                //check ability to view post if it is private
                if ((bool)!entry.IsPublic)
                {
                    var currentUserId = GetCurrentUserId();
                    if (currentUserId == null || entry.UserId != currentUserId.Value)
                    {
                        return Forbid();
                    }
                }

                var comments = await _context.Comments
                        .Where(c => c.EntryId == entryId)
                        .Include(c => c.User)
                        .OrderByDescending(c => c.CreatedDate)
                        .ToListAsync();

                var commentDtos = comments.Select(c => new CommentDTO
                {
                    CommentId = c.CommentId,
                    EntryId = c.EntryId,
                    UserId = c.UserId,
                    Username = c.User.Username,
                    Content = c.Content,
                    CreatedDate = (DateTime)c.CreatedDate,
                }).ToList();

                return Ok(commentDtos);
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "Error while getting comments for entry {EntryId}", entryId);
                return StatusCode(500, "Internal server error");
            }
        }

        //post api/comments
        [HttpPost]
        public async Task<ActionResult<CommentDTO>> CreateComment(CommentCreateDTO commentDto)
        {
            try
            {
                //check entry exist
                var entry = await _context.DiaryEntries.FindAsync(commentDto.EntryId);
                if(entry == null)
                {
                    return NotFound("Diary entry not found");
                }

                //check entry is public
                if ((bool)!entry.IsPublic)
                {
                    var userId = GetCurrentUserId();
                    if(userId == null)
                    {
                        return Forbid("Only logged-in user can comment on private post");
                    }

                    if(entry.UserId != userId.Value)
                    {
                        return Forbid("You can only comment on your own entries or public entries");
                    }
                }

                var currentUserId = GetCurrentUserId();

                var comment = new Comment
                {
                    EntryId = commentDto.EntryId,
                    UserId = currentUserId,
                    GuestName = currentUserId == null ?
                    (string.IsNullOrEmpty(commentDto.GuestName) ? "Guest" : commentDto.GuestName) : null,
                    Content = commentDto.Content,
                    CreatedDate = DateTime.Now
                };

                //validate guest must have guest name
                if(currentUserId == null && string.IsNullOrEmpty(commentDto.GuestName))
                {
                    ModelState.AddModelError("GuestName", "Guest name is required for non-logged-in users");
                    return BadRequest(ModelState);
                }

                _context.Comments.Add(comment);
                await _context.SaveChangesAsync();

                //load user information if available
                if (comment.UserId.HasValue)
                {
                    await _context.Entry(comment).Reference(c => c.User).LoadAsync();
                }

                var createdCommentDto = new CommentDTO
                {
                    CommentId = comment.CommentId,
                    EntryId = comment.EntryId,
                    UserId = comment.UserId,
                    Username = comment.UserId.HasValue ? comment.User?.Username : comment.GuestName,
                    IsGuest = !comment.UserId.HasValue,
                    Content = comment.Content,
                    CreatedDate = (DateTime)comment.CreatedDate,
                };

                return CreatedAtAction(nameof(GetCommentsByEntry), new { entryId = comment.EntryId }, createdCommentDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating comment");
                return StatusCode(500, "Internal server error");
            }
        }

        //Put: api/comments/{id}
        [Authorize]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateComment(int id, CommentUpdateDTO commentDto)
        {
            try
            {
                var userId = GetCurrentUserId();
                if (userId == null)
                {
                    return Unauthorized();
                }

                var comment = await _context.Comments.FindAsync(id);
                if (comment == null)
                {
                    return NotFound("Comment not found");
                }

                // Chỉ người tạo comment mới có thể sửa
                if (!comment.UserId.HasValue || comment.UserId.Value != userId.Value)
                {
                    return Forbid();
                }

                comment.Content = commentDto.Content;

                _context.Entry(comment).State = EntityState.Modified;
                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating comment {CommentId}", id);
                return StatusCode(500, "Internal server error");
            }
        }


        //Delete api/comments/{id}
        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteComment(int id)
        {
            try
            {
                var userId = GetCurrentUserId();
                if (userId == null)
                {
                    return Unauthorized();
                }

                var comment = await _context.Comments.FindAsync(id);
                if (comment == null)
                {
                    return NotFound("Comment not found");
                }

                // Kiểm tra xem entry có tồn tại không
                var entry = await _context.DiaryEntries.FindAsync(comment.EntryId);
                if (entry == null)
                {
                    return NotFound("Diary entry not found");
                }

                // Người tạo comment hoặc chủ nhân entry mới có thể xóa comment
                if ((!comment.UserId.HasValue || comment.UserId.Value != userId.Value) && entry.UserId != userId.Value)
                {
                    return Forbid();
                }

                _context.Comments.Remove(comment);
                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting comment {CommentId}", id);
                return StatusCode(500, "Internal server error");
            }
        }


        private int? GetCurrentUserId()
        {
            if (!User.Identity.IsAuthenticated)
            {
                return null;
            }

            var userIdClaim = User.FindFirst("userId") ??
                             User.FindFirst("nameid") ??
                             User.FindFirst(ClaimTypes.NameIdentifier) ??
                             User.FindFirst("sub");

            if (userIdClaim != null && int.TryParse(userIdClaim.Value, out int userId))
            {
                return userId;
            }
            return null;
        }
    }
}
