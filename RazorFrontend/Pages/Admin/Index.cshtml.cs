using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using AppCore.Dtos;
using System.Net.Http.Json;
using AppCore.BaseModel;
using System.Text.Json;

namespace RazorFrontend.Pages.Admin
{
    //[Authorize(Roles = "Admin")]
    public class IndexModel : PageModel
    {
        private readonly IHttpClientFactory _factory;
        private readonly IConfiguration _config;

        public IndexModel(IHttpClientFactory factory, IConfiguration config)
        {
            _factory = factory;
            _config = config;
        }

        public List<TestDto> Tests { get; set; } = new();
        public List<UserDto> Users { get; set; } = new();
        public string? ErrorMessage { get; set; }

        public async Task OnGetAsync()
        {
            var client = _factory.CreateClient();
            try
            {
                // Get Tests
                var testsEndpoint = $"{_config["ApiSettings:TestListEndpoint"]}" ?? "api/test";
                var testsResponse = await client.GetAsync(testsEndpoint);

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
                        return;
                    }
                }
                else
                {
                    ErrorMessage = $"Test API call failed: {testsResponse.StatusCode}";
                    return;
                }

                // Get Users
                var usersEndpoint = $"{_config["ApiSettings:UserListEndpoint"]}" ?? "api/user";
                var usersResponse = await client.GetAsync(usersEndpoint);

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
                        return;
                    }
                }
                else
                {
                    ErrorMessage = $"User API call failed: {usersResponse.StatusCode}";
                    return;
                }

            }
            catch (Exception ex)
            {
                ErrorMessage = $"Unexpected error: {ex.Message}";
            }
        }
    }
}