using PersonalDiary.Web.Models.DTOs;

namespace PersonalDiary.Web.Models.ViewModels
{
    public class DiaryEntryViewModel
    {
        public DiaryEntryDTO Entry { get; set; } = null!;
        public List<CommentDTO> Comments { get; set; } = new List<CommentDTO>();
        public CommentCreateDTO NewComment { get; set; } = new CommentCreateDTO();
        public bool IsAuthenticated { get; set; }
        public bool IsOwner { get; set; }

        public int PublicPostCount { get; set; }
        public List<DiaryEntryDTO> OtherEntries { get; set; } = new List<DiaryEntryDTO>();
    }
}
