using System.ComponentModel.DataAnnotations;

namespace PersonalDiary.Web.Models.DTOs
{
    public class CommentDTO
    {
        public int CommentId { get; set; }
        public int EntryId { get; set; }
        public int? UserId { get; set; }
        public string AuthorName { get; set; } = null!;
        public string Content { get; set; } = null!;
        public DateTime? CreatedDate { get; set; }
        public bool IsOwner { get; set; }
    }

    public class CommentCreateDTO
    {
        public int EntryId { get; set; }
        public string? GuestName { get; set; }
        public string Content { get; set; } = null!;
    }

    public class CommentUpdateDTO
    {
        [Required]
        [StringLength(1000, MinimumLength = 1, ErrorMessage = "Comment content must be between 1 and 1000 characters")]
        public string Content { get; set; }
        public int EntryId { get; set; }
    }
}
