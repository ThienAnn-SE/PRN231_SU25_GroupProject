using AppCore.BaseModel;
using AppCore.Dtos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Text.Json;

public class RegisterModel : PageModel
{
    private readonly HttpClient _client;
    private readonly IConfiguration _config;

    public RegisterModel(IHttpClientFactory factory, IConfiguration config)
    {
        _client = factory.CreateClient("ApiClient");
        _config = config;
    }

    [BindProperty]
    public RegisterInput Input { get; set; } = new();

    public string? ErrorMessage { get; set; }

    public class RegisterInput
    {
        [Required]
        [StringLength(50)]
        public string UserName { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        [StringLength(100, MinimumLength = 6)]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
            return Page();

        var registerDto = new RegisterDto
        {
            UserName = Input.UserName,
            Email = Input.Email,
            Password = Input.Password
        };

        var json = JsonSerializer.Serialize(registerDto);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var endpoint = _config["ApiSettings:RegisterEndpoint"] ?? "api/user/register";

        try
        {
            var response = await _client.PostAsync(endpoint, content);
            var body = await response.Content.ReadAsStringAsync();

            var result = JsonSerializer.Deserialize<ApiResponse>(body, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            if (response.IsSuccessStatusCode && result?.Success == true)
            {
                return RedirectToPage("Login");
            }

            ErrorMessage = result?.Message ?? "Registration failed.";
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Registration failed: {ex.Message}";
        }

        return Page();
    }
}
