using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PersonalDiary.API.DTOs;
using PersonalDiary.API.Models;

namespace PersonalDiary.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CommentsController : ControllerBase
    {
        private readonly PersonalDiaryDBContext _context;
        public CommentsController(PersonalDiaryDBContext context)
        {
            _context = context;
        }

        //get by entry id
        [HttpGet("entr/{entryId}")]
        public async Task<ActionResult<IEnumerable<CommentDTO>>> GetCommentsByEntry(int entryId)
        {
            var currentUserId = GetCurrentUserId();
            var entry = await _context.DiaryEntries.FindAsync(entryId);
            if (entry == null)
            {
                return NotFound("The post does not exist");
            }

            //check ability to view post if it is private
            if (entry.IsPublic != true)
            {
                if (currentUserId == null || entry.UserId != currentUserId.Value)
                {
                    return Forbid();
                }
            }

            var comments = await _context.Comments
                    .Include(c => c.User)
                    .Where(c => c.EntryId == entryId)
                    .OrderByDescending(c => c.CreatedDate)
                    .ToListAsync();

            var result = comments.Select(c => new CommentDTO
            {
                CommentId = c.CommentId,
                EntryId = c.EntryId,
                UserId = c.UserId,
                AuthorName = c.UserId.HasValue ? c.User.Username : c.GuestName,
                Content = c.Content,
                CreatedDate = c.CreatedDate,
                IsOwner = c.UserId.HasValue && currentUserId.HasValue && c.UserId.Value == currentUserId.Value
            });

            return Ok(result);
        }

        //post api/comments
        [HttpPost]
        public async Task<ActionResult<CommentDTO>> CreateComment(CommentCreateDTO commentDto)
        {
            var entry = await _context.DiaryEntries.FindAsync(commentDto.EntryId);
            if (entry == null)
            {
                return NotFound("The post is not exist");
            }

            //check if post public or not
            if (entry.IsPublic != true)
            {
                return BadRequest("Cannot comment in private post");
            }

            var comment = new Comment
            {
                EntryId = commentDto.EntryId,
                Content = commentDto.Content,
                CreatedDate = DateTime.Now
            };

            //check if commenter is user or guest
            var currentUserId = GetCurrentUserId();
            if (currentUserId != null)
            {
                comment.UserId = currentUserId.Value;
            }
            else
            {
                //if guest, must provide guest name
                if (string.IsNullOrWhiteSpace(commentDto.GuestName))
                {
                    return BadRequest("Guest name is required for comment");
                }
                comment.GuestName = commentDto.GuestName;
            }

            _context.Comments.Add(comment);
            await _context.SaveChangesAsync();

            //get full information about cmt after create cmt
            var createdComment = await _context.Comments
                .Include(c => c.User)
                .FirstOrDefaultAsync(c => c.CommentId == comment.CommentId);

            var result = new CommentDTO
            {
                CommentId = createdComment.CommentId,
                EntryId = createdComment.EntryId,
                UserId = createdComment.UserId,
                AuthorName = createdComment.UserId.HasValue ? createdComment.User.Username : createdComment.GuestName,
                Content = createdComment.Content,
                CreatedDate = createdComment.CreatedDate,
                IsOwner = createdComment.UserId.HasValue && currentUserId.HasValue && createdComment.UserId.Value == currentUserId.Value
            };

            return CreatedAtAction("GetCommentsByEntry", new { entryId = result.EntryId }, result);
        }

        //Delete api/comments/{id}
        [Authorize]
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteComment(int id)
        {
            var comment =await _context.Comments.FindAsync(id);
            if(comment == null)
            {
                return NotFound();
            }

            //check login
            var currentUserId = GetCurrentUserId();
            if (currentUserId == null)
            {
                return Unauthorized();
            }

            //allow delete if user is owner of comment
            var entry = await _context.DiaryEntries.FindAsync(comment.EntryId);
            if((comment.UserId != currentUserId) && (entry.UserId != currentUserId))
            {
                return Forbid();
            }

            _context.Comments.Remove(comment);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private int? GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst("userId");
            if(userIdClaim != null && int.TryParse(userIdClaim.Value, out int userId))
            {
                return userId;
            }
            return null;
        }
    }
}
