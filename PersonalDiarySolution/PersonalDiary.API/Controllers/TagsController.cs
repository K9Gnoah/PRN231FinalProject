using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PersonalDiary.API.DTOs;
using PersonalDiary.API.Models;

namespace PersonalDiary.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TagsController : ControllerBase
    {
        private readonly PersonalDiaryDBContext _context;
        public TagsController(PersonalDiaryDBContext context)
        {
            _context = context;
        }

        //get all tags
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TagDTO>>> GetTags()
        {
            var tags = await _context.Tags
                .OrderBy(t => t.TagName)
                .Select(t => new TagDTO
                {
                    TagId = t.TagId,
                    TagName = t.TagName
                })
                .ToListAsync();
            return Ok(tags);
        }

        //get tags popular 
        [HttpGet("popular")]
        public async Task<ActionResult<IEnumerable<TagDTO>>> GetPopularTags(int count = 10)
        {
            var popularTags = await _context.Tags
                .Select(t => new
                {
                    Tag = t,
                    Count = t.Entries.Count
                })
                .OrderByDescending(t => t.Count)
                .Take(count)
                .Select(t => new TagDTO
                {
                    TagId = t.Tag.TagId,
                    TagName = t.Tag.TagName
                })
                .ToListAsync();

            return Ok(popularTags);

        }

        //get entry by tag
        [HttpGet("{tagId}/entries")]
        public async Task<ActionResult<IEnumerable<DiaryEntryDTO>>> GetEntriesByTag(int tagId)
        {
            var tag = await _context.Tags.FindAsync(tagId);
            if (tag == null)
            {
                return NotFound();
            }

            var entries = await _context.DiaryEntries
                .Include(e => e.User)
                .Include(e => e.Tags)
                .Include(e => e.Comments)
                .Where(e => e.Tags.Any(t => t.TagId == tagId) && e.IsPublic == true)
                .OrderByDescending(e => e.CreatedDate)
                .ToListAsync();

            var result = entries.Select(entry => new DiaryEntryDTO
            {
                EntryId = entry.EntryId,
                UserId = entry.UserId,
                Title = entry.Title,
                Content = entry.Content,
                CreatedDate = (DateTime)entry.CreatedDate,
                ModifiedDate = entry.ModifiedDate,
                Mood = entry.Mood,
                Weather = entry.Weather,
                IsPublic = (bool)entry.IsPublic,
                Username = entry.User.Username,
                TagNames = entry.Tags.Select(t => t.TagName).ToList(),
                CommentsCount = entry.Comments.Count
            });

            return Ok(result);
        }

        //get api tag search
        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<TagDTO>>> SearchTags(string query)
        {
            if (string.IsNullOrWhiteSpace(query))
            {
                return BadRequest("Please input key tag to search");
            }

            var matchingTags = await _context.Tags
                .Where(t => t.TagName.Contains(query))
                .OrderBy(t => t.TagName)
                .Select(t => new TagDTO
                {
                    TagId = t.TagId,
                    TagName = t.TagName
                })
                .ToListAsync();

            return Ok(matchingTags);
        }


    }
}
