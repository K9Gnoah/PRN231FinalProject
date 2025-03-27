using System.ComponentModel.DataAnnotations;

namespace PersonalDiary.API.DTOs
{
    public class CommentDTO
    {
        public int CommentId { get; set; }
        public int EntryId { get; set; }
        public int? UserId { get; set; }
        public string Username { get; set; } 
        public bool IsGuest { get; set; }
        public string Content { get; set; }
        public DateTime CreatedDate { get; set; }
        public bool IsOwner { get; set; }
    }

    public class CommentCreateDTO
    {
        [Required]
        public int EntryId { get; set; }

        [Required]
        [StringLength(1000, MinimumLength = 1)]
        public string Content { get; set; }

        [StringLength(50)]
        public string GuestName { get; set; }
    }

    public class CommentUpdateDTO
    {
        [Required]
        [StringLength(1000, MinimumLength = 1)]
        public string Content { get; set; }
    }

}
