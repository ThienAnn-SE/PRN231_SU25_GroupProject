using AppCore.Dtos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using System.Net.Http;
using System.Text;
using System.Text.Json;

namespace RazorFrontend.Pages.UserProfiles
{
    public class CreateModel : PageModel
    {
        private readonly IHttpClientFactory _clientFactory;
        private readonly IConfiguration _config;

        public CreateModel(IHttpClientFactory clientFactory, IConfiguration config)
        {
            _clientFactory = clientFactory;
            _config = config;
        }

        [BindProperty]
        public UserProfileDto Input { get; set; } = new();

        public string? ErrorMessage { get; set; }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
                return Page();

            var client = _clientFactory.CreateClient("ApiClient");

            var endpoint = _config["ApiSettings:UserProfileCreateEndpoint"];
            var content = new StringContent(JsonSerializer.Serialize(Input), Encoding.UTF8, "application/json");

            try
            {
                var response = await client.PostAsync(endpoint, content);
                if (response.IsSuccessStatusCode)
                {
                    return RedirectToPage("Index");
                }
                else
                {
                    var result = await response.Content.ReadAsStringAsync();
                    ErrorMessage = $"Failed to create profile: {result}";
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Exception: {ex.Message}";
            }

            return Page();
        }
    }
}
