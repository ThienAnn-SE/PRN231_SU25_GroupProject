using AppCore.BaseModel;
using AppCore.Dtos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;

namespace RazorFrontend.Pages.UserProfiles
{
    public class EditModel : PageModel
    {
        private readonly IHttpClientFactory _clientFactory;
        private readonly IConfiguration _config;

        public EditModel(IHttpClientFactory clientFactory, IConfiguration config)
        {
            _clientFactory = clientFactory;
            _config = config;
        }

        [BindProperty]
        public UserProfileDto Input { get; set; } = new();

        public string? ErrorMessage { get; set; }

        public async Task<IActionResult> OnGetAsync(Guid id)
        {
            if (id == Guid.Empty)
                return RedirectToPage("Index");

            var client = _clientFactory.CreateClient("ApiClient");
            var endpoint = string.Format(_config["ApiSettings:UserProfileGetByIdEndpoint"], id);

            try
            {
                var response = await client.GetAsync(endpoint);
                if (response.IsSuccessStatusCode)
                {
                    var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponse<UserProfileDto>>();
                    if (apiResponse != null && apiResponse.Success && apiResponse.Data != null)
                    {
                        Input = apiResponse.Data;
                    }
                    else
                    {
                        ErrorMessage = apiResponse?.Message ?? "Failed to load profile.";
                    }
                }
                else
                {
                    ErrorMessage = "Profile not found.";
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Exception: {ex.Message}";
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
                return Page();

            var client = _clientFactory.CreateClient("ApiClient");
            var endpoint = string.Format(_config["ApiSettings:UserProfileUpdateEndpoint"], Input.Id);

            var content = new StringContent(JsonSerializer.Serialize(Input), Encoding.UTF8, "application/json");

            try
            {
                var response = await client.PutAsync(endpoint, content);
                if (response.IsSuccessStatusCode)
                {
                    return RedirectToPage("Index");
                }
                else
                {
                    var body = await response.Content.ReadAsStringAsync();
                    ErrorMessage = $"Update failed: {body}";
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
