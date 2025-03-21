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
    public class DiaryEntriesController : ControllerBase
    {
        private readonly PersonalDiaryDBContext _context;

        public DiaryEntriesController(PersonalDiaryDBContext context)
        {
            _context = context;
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

        //get diary entry by id
        [HttpGet("{id}")]
        public async Task<ActionResult<DiaryEntryDTO>> GetDiaryEntry(int id)
        {
            var entry = await _context.DiaryEntries
                .Include(d => d.User)
                .Include(d => d.Tags)
                .FirstOrDefaultAsync(d => d.EntryId == id);

            if (entry == null)
            {
                return NotFound();
            }

            //check ability to view entry
            var currentUserId = GetCurrentUserId();
            if (entry.IsPublic != true && (currentUserId == null || entry.UserId != currentUserId.Value))
            {
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
        public async Task<ActionResult<DiaryEntryDTO>> CreateDiaryEntry(DiaryEntryDTO entryDTO)
        {
            var userId = GetCurrentUserId();
            if (userId == null)
            {
                return Unauthorized();
            }

            var entry = new DiaryEntry
            {
                UserId = userId.Value,
                Title = entryDTO.Title,
                Content = entryDTO.Content,
                CreatedDate = DateTime.Now,
                Mood = entryDTO.Mood,
                Weather = entryDTO.Weather,
                IsPublic = entryDTO.IsPublic
            };

            if (entryDTO.TagNames != null && entryDTO.TagNames.Any())
            {
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

            _context.DiaryEntries.Add(entry);
            await _context.SaveChangesAsync();

            //get full information of created entry
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

            return CreatedAtAction("GetDiaryEntry", new {id = result.EntryId }, result);
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
