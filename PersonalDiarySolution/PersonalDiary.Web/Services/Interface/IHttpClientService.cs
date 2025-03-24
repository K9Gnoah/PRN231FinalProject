using PersonalDiary.Web.Models.DTOs;

namespace PersonalDiary.Web.Services.Interface
{
    public interface IHttpClientService
    {
        //auth
        Task<AuthResponse> LoginAsync(LoginRequest loginRequest);
        Task<AuthResponse> RegisterAsync(RegisterRequest registerRequest);

        //diary entries
        Task<List<DiaryEntryDTO>> GetPublicEntriesAsync();
        Task<List<DiaryEntryDTO>> GetMyEntriesAsync();
        Task<DiaryEntryDTO> GetDiaryEntryAsync(int id);
        Task<DiaryEntryDTO> CreateDiaryEntryAsync(DiaryEntryCreateDTO entryDto);
        Task<DiaryEntryDTO> UpdateDiaryEntryAsync(int id, DiaryEntryUpdateDTO entryDto);
        Task DeleteDiaryEntryAsync(int id);

        //comments
        Task<List<CommentDTO>> GetCommentsByEntryAsync(int entryId);
        Task<CommentDTO> CreateCommentAsync(CommentCreateDTO commentDto);
        Task DeleteCommentAsync(int id);

        //tags
        Task<List<TagDTO>> GetTagsAsync();
        Task<List<TagDTO>> GetPopularTagsAsync(int count = 10);
        Task<List<DiaryEntryDTO>> GetEntriesByTagAsync(int tagId);
    }
}
