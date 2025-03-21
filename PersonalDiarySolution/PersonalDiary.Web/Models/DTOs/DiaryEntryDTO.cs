namespace PersonalDiary.Web.Models.DTOs
{
    public class DiaryEntryDTO
    {
        public int EntryId { get; set; }
        public int UserId { get; set; }
        public string Title { get; set; } = null!;
        public string? Content { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public string? Mood { get; set; }
        public string? Weather { get; set; }
        public bool? IsPublic { get; set; }
        public List<string> TagNames { get; set; } = new List<string>();
        public string Username { get; set; } = null!;
        public int CommentsCount { get; set; }
    }

    public class DiaryEntryCreateDTO
    {
        public string Title { get; set; } = null!;
        public string? Content { get; set; }
        public string? Mood { get; set; }
        public string? Weather { get; set; }
        public bool IsPublic { get; set; }
        public List<string>? TagNames { get; set; }
    }

    public class DiaryEntryUpdateDTO
    {
        public string Title { get; set; } = null!;
        public string? Content { get; set; }
        public string? Mood { get; set; }
        public string? Weather { get; set; }
        public bool IsPublic { get; set; }
        public List<string>? TagNames { get; set; }
    }
}
