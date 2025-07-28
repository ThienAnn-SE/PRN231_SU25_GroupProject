using AppCore.BaseModel;
using AppCore.Dtos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using System.Net.Http;
using System.Text.Json;

namespace RazorFrontend.Pages.Universities
{
    public class DeleteModel : PageModel
    {
        private readonly IHttpClientFactory _clientFactory;
        private readonly IConfiguration _config;

        public DeleteModel(IHttpClientFactory clientFactory, IConfiguration config)
        {
            _clientFactory = clientFactory;
            _config = config;
        }

        [BindProperty(SupportsGet = true)]
        public Guid Id { get; set; }

        public UniversityDto? University { get; set; }

        public string? ErrorMessage { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            var client = _clientFactory.CreateClient("ApiClient");
            var endpoint = string.Format(_config["ApiSettings:UniversityByIdEndpoint"], Id);

            try
            {
                var response = await client.GetAsync(endpoint);
                if (!response.IsSuccessStatusCode)
                {
                    ErrorMessage = $"Unable to load university data. Status: {response.StatusCode}";
                    return Page();
                }

                var json = await response.Content.ReadAsStringAsync();
                ErrorMessage = $"Debug - API Response: {json}";
                
                var apiResponse = JsonSerializer.Deserialize<ApiResponse<UniversityDto>>(json, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                if (apiResponse?.Success == true && apiResponse.Data != null)
                {
                    University = apiResponse.Data;
                    ErrorMessage = null; // Clear debug message
                }
                else
                {
                    ErrorMessage = apiResponse?.Message ?? "University not found.";
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Error: {ex.Message}";
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var client = _clientFactory.CreateClient("ApiClient");
            var endpoint = string.Format(_config["ApiSettings:UniversityDeleteEndpoint"], Id);

            try
            {
                var response = await client.DeleteAsync(endpoint);
                if (response.IsSuccessStatusCode)
                {
                    return RedirectToPage("Index");
                }

                var body = await response.Content.ReadAsStringAsync();
                ErrorMessage = $"Failed to delete: {body}";
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Error: {ex.Message}";
            }

            return Page();
        }
    }
}
