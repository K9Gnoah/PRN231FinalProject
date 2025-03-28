using PersonalDiary.Web.Models.DTOs;

namespace PersonalDiary.Web.Services.Interface
{
    public interface IHttpClientService
    {
        //auth
        Task<AuthResponse> LoginAsync(LoginRequest loginRequest);
        Task<AuthResponseDTO> RegisterAsync(RegisterDTO model);

        //diary entries
        Task<List<DiaryEntryDTO>> GetPublicEntriesAsync();
        Task<List<DiaryEntryDTO>> GetMyEntriesAsync();
        Task<DiaryEntryDTO> GetDiaryEntryAsync(int id);
        Task<DiaryEntryDTO> CreateDiaryEntryAsync(DiaryEntryCreateDTO entryDto);
        Task UpdateDiaryEntryAsync(int id, DiaryEntryUpdateDTO entryDto);
        Task DeleteDiaryEntryAsync(int id);

        //comments
        Task<List<CommentDTO>> GetCommentsByEntryAsync(int entryId);
        Task<CommentDTO> CreateCommentAsync(CommentCreateDTO commentDto);
        Task DeleteCommentAsync(int id);
        Task<bool> UpdateCommentAsync(int id, CommentUpdateDTO commentDto);

        //tags
        Task<List<TagDTO>> GetTagsAsync();
        Task<List<TagDTO>> GetPopularTagsAsync(int count = 10);
        Task<List<DiaryEntryDTO>> GetEntriesByTagAsync(int tagId);

        Task<int> GetPublicEntryCountByUsernameAsync(string username);
        Task<List<DiaryEntryDTO>> GetOtherPublicEntriesByUsernameAsync(string username, int currentEntryId, int limit = 3);
        Task<(int publicCount, List<DiaryEntryDTO> otherEntries)> GetAuthorInfoAsync(string username, int currentEntryId, int limit = 3);
    }
}
