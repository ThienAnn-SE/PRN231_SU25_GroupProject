using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using AppCore.Dtos;
using System.Net.Http.Json;
using AppCore.BaseModel;
using System.Text.Json;
using Microsoft.Extensions.Options;

namespace RazorFrontend.Pages.Admin
{
    //[Authorize(Roles = "Admin")]
    public class IndexModel : PageModel
    {
        private readonly IHttpClientFactory _factory;
        private readonly ApiSettings _apiSettings;

        public IndexModel(IHttpClientFactory factory, IOptions<ApiSettings> options)
        {
            _factory = factory;
            _apiSettings = options.Value;
        }

        public List<TestDto> Tests { get; set; } = new();
        public List<UserDto> Users { get; set; } = new();
        public string? ErrorMessage { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            if(string.IsNullOrWhiteSpace(_apiSettings.TestListEndpoint) ||
               string.IsNullOrWhiteSpace(_apiSettings.UserListEndpoint))
            {
                ErrorMessage = "API settings are not configured properly.";
                return Page();
            }
            var client = _factory.CreateClient();
            try
            {
                // Get Tests
                var testsResponse = await client.GetAsync(_apiSettings.TestListEndpoint);
                if (!testsResponse.IsSuccessStatusCode)
                {
                    ErrorMessage = $"Test API call failed: {testsResponse.StatusCode}";
                    return Page();
                }

                    if (testsResponse.IsSuccessStatusCode)
                {
                    var testJson = await testsResponse.Content.ReadAsStringAsync();
                    var testResult = JsonSerializer.Deserialize<ApiResponses<TestDto>>(testJson, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

                    if (testResult?.Success == true && testResult.Data != null)
                    {
                        Tests = testResult.Data;
                    }
                    else
                    {
                        ErrorMessage = testResult?.Message ?? "Failed to load tests.";
                        return Page();
                    }
                }
                else
                {
                    ErrorMessage = $"Test API call failed: {testsResponse.StatusCode}";
                    return Page();
                }

                // Get Users
                var usersResponse = await client.GetAsync(_apiSettings.UserListEndpoint);
                if (!usersResponse.IsSuccessStatusCode)
                {
                    ErrorMessage = $"User API call failed: {usersResponse.StatusCode}";
                    return Page();
                }

                if (usersResponse.IsSuccessStatusCode)
                {
                    var userJson = await usersResponse.Content.ReadAsStringAsync();
                    var userResult = JsonSerializer.Deserialize<ApiResponses<UserDto>>(userJson, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

                    if (userResult?.Success == true && userResult.Data != null)
                    {
                        Users = userResult.Data;
                    }
                    else
                    {
                        ErrorMessage = userResult?.Message ?? "Failed to load users.";
                        return Page();
                    }
                }
                else
                {
                    ErrorMessage = $"User API call failed: {usersResponse.StatusCode}";
                    return Page();
                }

            }
            catch (Exception ex)
            {
                ErrorMessage = $"Unexpected error: {ex.Message}";
            }
            return Page();
        }
    }
}