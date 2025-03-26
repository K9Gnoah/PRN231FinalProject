using Newtonsoft.Json;
using PersonalDiary.Web.Models.DTOs;
using PersonalDiary.Web.Services.Interface;
using System.Net.Http.Headers;
using System.Text;

namespace PersonalDiary.Web.Services
{
    public class HttpClientService : IHttpClientService
    {
        private readonly HttpClient _httpClient;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly string _baseUrl;

        public HttpClientService(IHttpClientFactory httpClientFactory, IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
        {
            _httpClient = httpClientFactory.CreateClient();
            _baseUrl = configuration["ApiSettings:BaseUrl"];
            _httpContextAccessor = httpContextAccessor;

            //add token to header
            var token = _httpContextAccessor.HttpContext?.Session.GetString("JWTToken");
            if(!string.IsNullOrEmpty(token))
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }
        }



        public async Task<CommentDTO> CreateCommentAsync(CommentCreateDTO commentDto)
        {
            //add token to header
            UpdateAuthorizationHeader();

            var content = new StringContent(JsonConvert.SerializeObject(commentDto), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync($"{_baseUrl}/Comments", content);
            response.EnsureSuccessStatusCode();

            var responseContent = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<CommentDTO>(responseContent);
        }

        public async Task<DiaryEntryDTO> CreateDiaryEntryAsync(DiaryEntryCreateDTO entryDto)
        {
            UpdateAuthorizationHeader();

            var content = new StringContent(JsonConvert.SerializeObject(entryDto), Encoding.UTF8, "application/json");

            // Log dữ liệu gửi lên
            Console.WriteLine("Sending data: " + await content.ReadAsStringAsync());

            var response = await _httpClient.PostAsync($"{_baseUrl}/DiaryEntries", content);
            response.EnsureSuccessStatusCode();

            var responseContent = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<DiaryEntryDTO>(responseContent);
        }

        public async Task DeleteCommentAsync(int id)
        {
            //add token to header
            UpdateAuthorizationHeader();

            var response = await _httpClient.DeleteAsync($"{_baseUrl}/Comments/{id}");
            response.EnsureSuccessStatusCode();
        }

        public async Task DeleteDiaryEntryAsync(int id)
        {
            //add token to header
            UpdateAuthorizationHeader();

            var response = await _httpClient.DeleteAsync($"{_baseUrl}/DiaryEntries/{id}");
            response.EnsureSuccessStatusCode();
        }

        public async Task<List<CommentDTO>> GetCommentsByEntryAsync(int entryId)
        {
            var response = await _httpClient.GetAsync($"{_baseUrl}/Comments/entry/{entryId}");
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"GetCommentsByEntryAsync response: {content}"); 
            return JsonConvert.DeserializeObject<List<CommentDTO>>(content);
        }

        public async Task<bool> UpdateCommentAsync(int id, CommentUpdateDTO commentDto)
        {
            UpdateAuthorizationHeader();

            var content = new StringContent(JsonConvert.SerializeObject(commentDto), Encoding.UTF8, "application/json");
            var response = await _httpClient.PutAsync($"{_baseUrl}/Comments/{id}", content);

            try
            {
                response.EnsureSuccessStatusCode();
                return true;
            }
            catch (HttpRequestException)
            {
                return false;
            }
        }

        public async Task<DiaryEntryDTO> GetDiaryEntryAsync(int id)
        {

            UpdateAuthorizationHeader();

            var response = await _httpClient.GetAsync($"{_baseUrl}/DiaryEntries/{id}");
            response.EnsureSuccessStatusCode(); 

            var content = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<DiaryEntryDTO>(content);
        }

        public async Task<List<DiaryEntryDTO>> GetEntriesByTagAsync(int tagId)
        {
            var response = await _httpClient.GetAsync($"{_baseUrl}/Tags/{tagId}/entries");
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<List<DiaryEntryDTO>>(content);
        }

        public async Task<List<DiaryEntryDTO>> GetMyEntriesAsync()
        {
            //add token to header
            UpdateAuthorizationHeader();

            var response = await _httpClient.GetAsync($"{_baseUrl}/DiaryEntries/my");
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<List<DiaryEntryDTO>>(content);
        }        

        public async Task<List<TagDTO>> GetPopularTagsAsync(int count = 10)
        {
            var response = await _httpClient.GetAsync($"{_baseUrl}/Tags/popular?count={count}");
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<List<TagDTO>>(content);
        }

        public async Task<List<DiaryEntryDTO>> GetPublicEntriesAsync()
        {
            var response = await _httpClient.GetAsync($"{_baseUrl}/DiaryEntries/public");
            response.EnsureSuccessStatusCode(); 
            var content = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<List<DiaryEntryDTO>>(content);
        }

        public async Task<List<TagDTO>> GetTagsAsync()
        {
            var response = await _httpClient.GetAsync($"{_baseUrl}/Tags");
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<List<TagDTO>>(content);
        }       

        public async Task<AuthResponse> LoginAsync(LoginRequest loginRequest)
        {
            var content = new StringContent(JsonConvert.SerializeObject(loginRequest), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync($"{_baseUrl}/Auth/login", content);

            var responseContent = await response.Content.ReadAsStringAsync();
            if (response.IsSuccessStatusCode)
            {
                var authResponse= JsonConvert.DeserializeObject<AuthResponse>(responseContent);

                //add token to session if login is successful
                if(authResponse.Success && !string.IsNullOrEmpty(authResponse.Token))
                {
                    _httpContextAccessor.HttpContext.Session.SetString("JWTToken", authResponse.Token);
                    _httpContextAccessor.HttpContext.Session.SetString("Username", authResponse.Username);
                }

                return authResponse;
            }

            return new AuthResponse { Success = false, Message = "Login failed!" + responseContent };
        }

        public Task<AuthResponse> RegisterAsync(RegisterRequest registerRequest)
        {
            throw new NotImplementedException();
        }


        public async Task UpdateDiaryEntryAsync(int id, DiaryEntryUpdateDTO entryDto)
        {
            //add token to header
            UpdateAuthorizationHeader();

            var content = new StringContent(JsonConvert.SerializeObject(entryDto), Encoding.UTF8, "application/json");
            var response = await _httpClient.PutAsync($"{_baseUrl}/DiaryEntries/{id}", content);
            response.EnsureSuccessStatusCode();
        }

        private void UpdateAuthorizationHeader()
        {
            var token = _httpContextAccessor.HttpContext?.Session.GetString("JWTToken");
            if (!string.IsNullOrEmpty(token))
            {
                if(_httpClient.DefaultRequestHeaders.Contains("Authorization"))
                {
                    _httpClient.DefaultRequestHeaders.Remove("Authorization");
                }
                _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
            }
        }
    }
}
