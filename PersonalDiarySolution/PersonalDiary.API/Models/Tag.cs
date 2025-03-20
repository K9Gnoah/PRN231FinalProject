using System;
using System.Collections.Generic;

namespace PersonalDiary.API.Models
{
    public partial class Tag
    {
        public Tag()
        {
            Entries = new HashSet<DiaryEntry>();
        }

        public int TagId { get; set; }
        public string TagName { get; set; } = null!;

        public virtual ICollection<DiaryEntry> Entries { get; set; }
    }
}
