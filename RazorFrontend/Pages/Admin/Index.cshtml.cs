using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using AppCore.Dtos;
using System.Net.Http.Json;

namespace RazorFrontend.Pages.Admin
{
    [Authorize(Roles = "Admin")]
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

        public async Task<IActionResult> OnGetAsync()
        {
            var client = _factory.CreateClient();
            
            // Get Tests
            var testsEndpoint = $"{_config["ApiSettings:TestListEndpoint"]}";
            var testsResponse = await client.GetAsync(testsEndpoint);
            if (testsResponse.IsSuccessStatusCode)
            {
                Tests = await testsResponse.Content.ReadFromJsonAsync<List<TestDto>>() ?? new();
            }
            else
            {
                ErrorMessage = "Unable to load tests.";
            }

            // Get Users
            var usersEndpoint = $"{_config["ApiSettings:UserListEndpoint"]}";
            var usersResponse = await client.GetAsync(usersEndpoint);
            if (usersResponse.IsSuccessStatusCode)
            {
                Users = await usersResponse.Content.ReadFromJsonAsync<List<UserDto>>() ?? new();
            }
            else
            {
                ErrorMessage = "Unable to load users.";
            }

            return Page();
        }
    }
}