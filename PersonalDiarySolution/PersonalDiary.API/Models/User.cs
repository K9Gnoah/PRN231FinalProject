using System;
using System.Collections.Generic;

namespace PersonalDiary.API.Models
{
    public partial class User
    {
        public User()
        {
            Comments = new HashSet<Comment>();
            DiaryEntries = new HashSet<DiaryEntry>();
        }

        public int UserId { get; set; }
        public string Username { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string PasswordHash { get; set; } = null!;
        public string? FullName { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? LastLoginDate { get; set; }
        public bool? IsActive { get; set; }

        public virtual ICollection<Comment> Comments { get; set; }
        public virtual ICollection<DiaryEntry> DiaryEntries { get; set; }
    }
}
