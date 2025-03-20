using System;
using System.Collections.Generic;

namespace PersonalDiary.API.Models
{
    public partial class Comment
    {
        public int CommentId { get; set; }
        public int EntryId { get; set; }
        public int? UserId { get; set; }
        public string? GuestName { get; set; }
        public string Content { get; set; } = null!;
        public DateTime? CreatedDate { get; set; }

        public virtual DiaryEntry Entry { get; set; } = null!;
        public virtual User? User { get; set; }
    }
}
