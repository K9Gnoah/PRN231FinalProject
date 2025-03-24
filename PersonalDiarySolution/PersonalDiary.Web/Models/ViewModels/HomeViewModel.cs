using PersonalDiary.Web.Models.DTOs;

namespace PersonalDiary.Web.Models.ViewModels
{
    public class HomeViewModel
    {
        public List<DiaryEntryDTO> PublicEntries { get; set; } = new List<DiaryEntryDTO>();
        public List<TagDTO> PopularTags { get; set; } = new List<TagDTO>();
    }
}
