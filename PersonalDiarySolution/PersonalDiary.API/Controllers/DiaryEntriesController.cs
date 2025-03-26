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
    public class DiaryEntriesController : ControllerBase
    {
        private readonly PersonalDiaryDBContext _context;
        private readonly ILogger<DiaryEntriesController> _logger;
        public DiaryEntriesController(PersonalDiaryDBContext context, ILogger<DiaryEntriesController> logger)
        {
            _context = context;
            _logger = logger;
        }

        //get all diary entries public
        [HttpGet("public")]
        public async Task<ActionResult<IEnumerable<DiaryEntryDTO>>> GetPublicEntries()
        {
            var publicEntries = await _context.DiaryEntries
                .Include(d => d.User)
                .Include(d => d.Comments)
                .Include(d => d.Tags)
                .Where(d => d.IsPublic == true)
                .OrderByDescending(d => d.CreatedDate)
                .Take(50)
                .ToListAsync();

            return Ok(publicEntries.Select(entry => new DiaryEntryDTO
            {
                EntryId = entry.EntryId,
                UserId = entry.UserId,
                Title = entry.Title,
                Content = entry.Content,
                CreatedDate = entry.CreatedDate,
                ModifiedDate = entry.ModifiedDate,
                Mood = entry.Mood,
                Weather = entry.Weather,
                IsPublic = entry.IsPublic,
                Username = entry.User.Username,
                TagNames = entry.Tags.Select(t => t.TagName).ToList(),
                CommentsCount = entry.Comments.Count
            }));
        }

        // GET: api/DiaryEntries/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<DiaryEntryDTO>> GetDiaryEntry(int id)
        {
            var entry = await _context.DiaryEntries
                .Include(d => d.User)
                .Include(d => d.Tags)
                .FirstOrDefaultAsync(d => d.EntryId == id);

            if (entry == null)
            {
                _logger.LogError("Diary entry with ID {id} not found", id);
                return NotFound();
            }

            // Kiểm tra quyền xem bài viết
            var currentUserId = GetCurrentUserId();
            if (entry.IsPublic != true && (currentUserId == null || entry.UserId != currentUserId.Value))
            {
                _logger.LogError("User {currentUserId} is not authorized to view diary entry with ID {id}", currentUserId, id);
                return Forbid();
            }

            var commentCount = await _context.Comments.CountAsync(c => c.EntryId == id);

            return new DiaryEntryDTO
            {
                EntryId = entry.EntryId,
                UserId = entry.UserId,
                Title = entry.Title,
                Content = entry.Content,
                CreatedDate = entry.CreatedDate,
                ModifiedDate = entry.ModifiedDate,
                Mood = entry.Mood,
                Weather = entry.Weather,
                IsPublic = entry.IsPublic,
                Username = entry.User.Username,
                TagNames = entry.Tags.Select(t => t.TagName).ToList(),
                CommentsCount = commentCount
            };
        }

        //get diary entries by user owner
        [Authorize]
        [HttpGet("my")]
        public async Task<ActionResult<IEnumerable<DiaryEntryDTO>>> GetMyEntries()
        {
            var userId = GetCurrentUserId();
            if (userId == null)
            {
                return Unauthorized();
            }

            var myEntries = await _context.DiaryEntries
                .Include(d => d.Tags)
                .Include(d => d.User)
                .Where(d => d.UserId == userId.Value)
                .OrderByDescending(d => d.CreatedDate)
                .ToListAsync();

            var result = myEntries.Select(entry => new DiaryEntryDTO
            {
                EntryId = entry.EntryId,
                UserId = entry.UserId,
                Title = entry.Title,
                Content = entry.Content,
                CreatedDate = entry.CreatedDate,
                ModifiedDate = entry.ModifiedDate,
                Mood = entry.Mood,
                Weather = entry.Weather,
                IsPublic = entry.IsPublic,
                Username = entry.User.Username,
                TagNames = entry.Tags.Select(t => t.TagName).ToList(),
                CommentsCount = _context.Comments.Count(c => c.EntryId == entry.EntryId)
            });

            return Ok(result);
        }

        //post create new diary entry
        [Authorize]
        [HttpPost]
        public async Task<ActionResult<DiaryEntryDTO>> CreateDiaryEntry(DiaryEntryCreateDTO entryDto)
        {
            _logger.LogInformation("Request headers:");
            foreach (var header in Request.Headers)
            {
                _logger.LogInformation("  {Key}: {Value}", header.Key, header.Value);
            }
            // Log tất cả các claims để kiểm tra
            _logger.LogInformation("User authenticated: {IsAuthenticated}", User.Identity.IsAuthenticated);
            if (User.Identity.IsAuthenticated)
            {
                _logger.LogInformation("User claims:");
                foreach (var claim in User.Claims)
                {
                    _logger.LogInformation("  {Type}: {Value}", claim.Type, claim.Value);
                }
            }

            if (entryDto == null)
            {
                _logger.LogError("Received null entryDto");
                return BadRequest("Entry data is null");
            }

            if (!ModelState.IsValid)
            {
                _logger.LogError("Invalid model state: {ModelState}", ModelState);
                return BadRequest(ModelState);
            }

            try
            {
                // Lấy UserId từ claim
                var userId = GetUserId();
                if (userId == null)
                {
                    _logger.LogError("User ID not found in claims");
                    return Unauthorized();
                }

                _logger.LogInformation("User ID found: {UserId}", userId);

                var entry = new DiaryEntry
                {
                    UserId = userId.Value, // Sử dụng UserId từ claim
                    Title = entryDto.Title,
                    Content = entryDto.Content,
                    CreatedDate = DateTime.Now,
                    Mood = entryDto.Mood,
                    Weather = entryDto.Weather,
                    IsPublic = entryDto.IsPublic,
                    Tags = new List<Tag>()
                };

                if (entryDto.TagNames != null && entryDto.TagNames.Any())
                {
                    foreach (var tagName in entryDto.TagNames)
                    {
                        var tag = await _context.Tags.FirstOrDefaultAsync(t => t.TagName == tagName);
                        if (tag == null)
                        {
                            tag = new Tag { TagName = tagName };
                            _context.Tags.Add(tag);
                            await _context.SaveChangesAsync();
                        }

                        entry.Tags.Add(tag);
                    }
                }

                _context.DiaryEntries.Add(entry);
                await _context.SaveChangesAsync();

                var createdEntry = await _context.DiaryEntries
                    .Include(d => d.Tags)
                    .Include(d => d.User)
                    .FirstOrDefaultAsync(d => d.EntryId == entry.EntryId);

                var result = new DiaryEntryDTO
                {
                    EntryId = createdEntry.EntryId,
                    UserId = createdEntry.UserId,
                    Title = createdEntry.Title,
                    Content = createdEntry.Content,
                    CreatedDate = createdEntry.CreatedDate,
                    ModifiedDate = createdEntry.ModifiedDate,
                    Mood = createdEntry.Mood,
                    Weather = createdEntry.Weather,
                    IsPublic = createdEntry.IsPublic,
                    Username = createdEntry.User.Username,
                    TagNames = createdEntry.Tags.Select(t => t.TagName).ToList(),
                    CommentsCount = 0
                };

                return CreatedAtAction("GetDiaryEntry", new { id = result.EntryId }, result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating diary entry");
                return StatusCode(500, "Internal server error");
            }
        }

        private int? GetUserId()
        {
            var userIdClaim = User.FindFirst("nameid") ??
                              User.FindFirst("userId") ??
                              User.FindFirst(ClaimTypes.NameIdentifier) ??
                              User.FindFirst("sub");

            if (userIdClaim != null)
            {
                _logger.LogInformation("Found userId claim: {Type}={Value}", userIdClaim.Type, userIdClaim.Value);
                if (int.TryParse(userIdClaim.Value, out int userId))
                {
                    return userId;
                }
            }

            _logger.LogWarning("No valid userId claim found");
            return null;
        }

        //update api PUT
        [Authorize]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateDiaryEntry(int id, DiaryEntryUpdateDTO entryDTO)
        {
            var userId = GetCurrentUserId();
            if (userId == null)
            {
                return Unauthorized();
            }

            var entry = await _context.DiaryEntries
                .Include(d => d.Tags)
                .FirstOrDefaultAsync(d => d.EntryId == id);

            if (entry == null)
            {
                return NotFound();
            }

            if (entry.UserId != userId)
            {
                return Forbid();
            }

            //update entry
            entry.Title = entryDTO.Title;
            entry.Content = entryDTO.Content;
            entry.Mood = entryDTO.Mood;
            entry.Weather = entryDTO.Weather;
            entry.IsPublic = entryDTO.IsPublic;
            entry.ModifiedDate = DateTime.Now;

            //update tags
            if(entryDTO.TagNames != null)
            {
                //remove all tags
                entry.Tags.Clear();

                //add new tags
                foreach (var tagName in entryDTO.TagNames)
                {
                    var tag = await _context.Tags.FirstOrDefaultAsync(t => t.TagName == tagName);
                    if (tag == null)
                    {
                        tag = new Tag { TagName = tagName };
                        _context.Tags.Add(tag);
                        await _context.SaveChangesAsync();
                    }

                    entry.Tags.Add(tag);
                }
            }

            await _context.SaveChangesAsync();
            return NoContent();
        }

        //delete api
        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDiaryEntry(int id)
        {
            var userId = GetCurrentUserId();
            if (userId == null)
            {
                return Unauthorized();
            }

            var entry = await _context.DiaryEntries.FindAsync(id);


            if (entry == null)
            {
                return NotFound();
            }

            if (entry.UserId != userId)
            {
                return Forbid();
            }

            //delete comments releted to entry
            var comments = await _context.Comments.Where(c => c.EntryId == id).ToListAsync();
            _context.Comments.RemoveRange(comments);
            _context.DiaryEntries.Remove(entry);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private int? GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst("userId");
            if (userIdClaim != null && int.TryParse(userIdClaim.Value, out int userId))
            {
                return userId;
            }
            return null;
        }
    }
}
