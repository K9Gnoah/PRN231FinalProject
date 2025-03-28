using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
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
            try
            {
                // Add token to header
                UpdateAuthorizationHeader();

                // Debug: log token
                var token = _httpContextAccessor.HttpContext?.Session.GetString("JWTToken");
                Console.WriteLine($"Token exists: {!string.IsNullOrEmpty(token)}");
                Console.WriteLine($"Authorization header: {_httpClient.DefaultRequestHeaders.Authorization}");

                Console.WriteLine($"CommentDTO: EntryId={commentDto.entryId}, Content={commentDto.content}, GuestName={commentDto.guestName ?? "null"}");

                if (string.IsNullOrEmpty(commentDto.guestName) && !string.IsNullOrEmpty(token))
                {
                    commentDto.guestName = "LoggedInUser"; 
                }

                var jsonContent = JsonConvert.SerializeObject(commentDto);
                Console.WriteLine($"Sending JSON: {jsonContent}");

                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync($"{_baseUrl}/Comments", content);

                var responseContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Response status: {(int)response.StatusCode} {response.StatusCode}");
                Console.WriteLine($"Response content: {responseContent}");

                if (!response.IsSuccessStatusCode)
                {
                    throw new HttpRequestException($"API Error: {responseContent}");
                }

                return JsonConvert.DeserializeObject<CommentDTO>(responseContent);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception in CreateCommentAsync: {ex.Message}");
                throw;
            }
        }

        public async Task<DiaryEntryDTO> CreateDiaryEntryAsync(DiaryEntryCreateDTO entryDto)
        {
            UpdateAuthorizationHeader();

            var content = new StringContent(JsonConvert.SerializeObject(entryDto), Encoding.UTF8, "application/json");

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
            try
            {
                var response = await _httpClient.GetAsync($"{_baseUrl}/Comments/entry/{entryId}");
                response.EnsureSuccessStatusCode();

                var content = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"GetCommentsByEntryAsync response: {content}");

                // Deserialize với setting để debug
                var settings = new JsonSerializerSettings
                {
                    MissingMemberHandling = MissingMemberHandling.Ignore,
                    NullValueHandling = NullValueHandling.Include
                };


                // Deserialize thành dynamic trước để xem cấu trúc
                var dynamicResponse = JsonConvert.DeserializeObject<dynamic>(content);
                foreach (var item in dynamicResponse)
                {
                    Console.WriteLine("JSON Properties:");
                    foreach (var prop in ((JObject)item).Properties())
                    {
                        Console.WriteLine($"  {prop.Name}: {prop.Value}");
                    }
                }

                var comments = JsonConvert.DeserializeObject<List<CommentDTO>>(content, settings);
                return comments;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetCommentsByEntryAsync: {ex.Message}");
                throw;
            }
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
            try
            {
                var response = await _httpClient.GetAsync($"{_baseUrl}/Tags/popular?count={count}");
                response.EnsureSuccessStatusCode();

                var content = await response.Content.ReadAsStringAsync();
                var popularTags = JsonConvert.DeserializeObject<List<TagDTO>>(content);

                return popularTags ?? new List<TagDTO>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetPopularTagsAsync: {ex.Message}");
                return new List<TagDTO>();
            }
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
            Console.WriteLine($"Updating auth header with token: {!string.IsNullOrEmpty(token)}");

            if (!string.IsNullOrEmpty(token))
            {
                // remove old header before add new header
                if (_httpClient.DefaultRequestHeaders.Contains("Authorization"))
                {
                    _httpClient.DefaultRequestHeaders.Remove("Authorization");
                }

                _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");

                // Kiểm tra lại để xác nhận header đã được thêm đúng
                Console.WriteLine($"Header after update: {_httpClient.DefaultRequestHeaders.Authorization}");
            }
            else
            {
                Console.WriteLine("No token found in session, request will be sent without authentication");
            }
        }

        public async Task<int> GetPublicEntryCountByUsernameAsync(string username)
        {
            try
            {
                var response = await _httpClient.GetAsync($"{_baseUrl}/DiaryEntries/count/public/{username}");
                response.EnsureSuccessStatusCode();

                var content = await response.Content.ReadAsStringAsync();
                return int.Parse(content);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetPublicEntryCountByUsernameAsync: {ex.Message}");
                return 0; 
            }
        }

        public async Task<List<DiaryEntryDTO>> GetOtherPublicEntriesByUsernameAsync(string username, int currentEntryId, int limit = 3)
        {
            try
            {
                var response = await _httpClient.GetAsync($"{_baseUrl}/DiaryEntries/public/{username}?excludeId={currentEntryId}&limit={limit}");
                response.EnsureSuccessStatusCode();

                var content = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<List<DiaryEntryDTO>>(content) ?? new List<DiaryEntryDTO>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetOtherPublicEntriesByUsernameAsync: {ex.Message}");
                return new List<DiaryEntryDTO>(); 
            }
        }

        public async Task<(int publicCount, List<DiaryEntryDTO> otherEntries)> GetAuthorInfoAsync(string username, int currentEntryId, int limit = 3)
        {
            try
            {
                var countTask = GetPublicEntryCountByUsernameAsync(username);
                var entriesTask = GetOtherPublicEntriesByUsernameAsync(username, currentEntryId, limit);

                await Task.WhenAll(countTask, entriesTask);

                return (countTask.Result, entriesTask.Result);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetAuthorInfoAsync: {ex.Message}");
                return (0, new List<DiaryEntryDTO>());
            }
        }

        public async Task<AuthResponseDTO> RegisterAsync(RegisterDTO model)
        {
            try
            {
                Console.WriteLine($"Sending register request to: {_baseUrl}/Users/register");
                Console.WriteLine($"User data: {JsonConvert.SerializeObject(model)}");

                var json = JsonConvert.SerializeObject(model);
                var content = new StringContent(json, Encoding.UTF8, "application/json");


                var response = await _httpClient.PostAsync($"{_baseUrl}/Auth/register", content);


                var responseContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Register API response: {response.StatusCode}, Content: {responseContent}");

                // Phần còn lại giữ nguyên
                if (response.IsSuccessStatusCode)
                {
                    return new AuthResponseDTO
                    {
                        Success = true,
                        Message = "Đăng ký thành công"
                    };
                }
                else
                {
                    // Hiển thị thông tin chi tiết hơn về lỗi
                    return new AuthResponseDTO
                    {
                        Success = false,
                        Message = $"Đăng ký thất bại: {response.StatusCode}. Chi tiết: {responseContent}"
                    };
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception in RegisterAsync: {ex.Message}");
                return new AuthResponseDTO
                {
                    Success = false,
                    Message = $"Lỗi kết nối đến API: {ex.Message}"
                };
            }
        }
    }
}
