using System;
using System.Collections.Generic;

namespace PersonalDiary.API.Models
{
    public partial class DiaryEntry
    {
        public DiaryEntry()
        {
            Comments = new HashSet<Comment>();
            Tags = new HashSet<Tag>();
        }

        public int EntryId { get; set; }
        public int UserId { get; set; }
        public string Title { get; set; } = null!;
        public string? Content { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public string? Mood { get; set; }
        public string? Weather { get; set; }
        public bool? IsPublic { get; set; }

        public virtual User User { get; set; } = null!;
        public virtual ICollection<Comment> Comments { get; set; }

        public virtual ICollection<Tag> Tags { get; set; }
    }
}
