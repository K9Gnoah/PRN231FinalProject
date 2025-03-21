namespace PersonalDiary.API.DTOs
{
    public class AuthDTO
    {
        public class LoginRequest
        {
            public string Username { get; set; } = null!;
            public string Password { get; set; } = null!;
        }

        public class RegisterRequest
        {
            public string Username { get; set; } = null!;
            public string Email { get; set; } = null!;
            public string Password { get; set; } = null!;
            public string? FullName { get; set; }
        }

        public class AuthResponse
        {
            public bool Success { get; set; }
            public string Message { get; set; } = null!;
            public string? Token { get; set; }
            public string? Username { get; set; }
        }
    }
}
