using AppCore.Dtos;
using AppCore.BaseModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Text.Json;

public class LoginModel : PageModel
{
    private readonly HttpClient _client;
    private readonly IConfiguration _config;

    public LoginModel(IHttpClientFactory factory, IConfiguration config)
    {
        _client = factory.CreateClient("ApiClient");
        _config = config;
    }

    [BindProperty]
    public LoginInput Input { get; set; } = new();

    public string? ErrorMessage { get; set; }

    public class LoginInput
    {
        [Required]
        public string UserName { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
            return Page();

        var loginDto = new LoginDto
        {
            UserName = Input.UserName,
            Password = Input.Password
        };

        var json = JsonSerializer.Serialize(loginDto);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var endpoint = _config["ApiSettings:LoginEndpoint"];

        try
        {
            var response = await _client.PostAsync(endpoint, content);
            var body = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<ApiResponse<RefreshTokenDto>>(body, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            if (result?.Success == true && result.Data != null)
            {
                return RedirectToPage("/Index");
            }

            ErrorMessage = result?.Message ?? "Login failed.";
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Login failed: {ex.Message}";
        }

        return Page();
    }
}
