using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace PersonalDiary.Web.Models.DTOs
{
    public class CommentDTO
    {
        public int CommentId { get; set; }
        public int EntryId { get; set; }
        public int? UserId { get; set; }

        [JsonProperty("username")]
        public string AuthorName { get; set; } = null!;

        [JsonProperty("isGuest")]
        public bool IsGuest { get; set; }

        public string Content { get; set; } = null!;
        public DateTime? CreatedDate { get; set; }
        public bool IsOwner { get; set; }
    }

    public class CommentCreateDTO
    {
        public int entryId { get; set; }
        public string content { get; set; }
        public string guestName { get; set; }
    }

    public class CommentUpdateDTO
    {
        [Required]
        [StringLength(1000, MinimumLength = 1, ErrorMessage = "Comment content must be between 1 and 1000 characters")]
        public string Content { get; set; }
        public int EntryId { get; set; }
    }
}
