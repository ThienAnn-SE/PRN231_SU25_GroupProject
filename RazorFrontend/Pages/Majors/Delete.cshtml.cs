using AppCore.BaseModel;
using AppCore.Dtos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net.Http;
using System.Net.Http.Json;

namespace RazorFrontend.Pages.Majors
{
    public class DeleteModel : PageModel
    {
        private readonly IHttpClientFactory _factory;
        private readonly IConfiguration _config;

        public DeleteModel(IHttpClientFactory factory, IConfiguration config)
        {
            _factory = factory;
            _config = config;
        }

        [BindProperty(SupportsGet = true)]
        public Guid Id { get; set; }

        public MajorDto? Major { get; set; }

        public string? ErrorMessage { get; set; }

        private Dictionary<Guid, string> _universityNames = new();

        public async Task<IActionResult> OnGetAsync()
        {
            await LoadUniversities();

            var client = _factory.CreateClient("ApiClient");
            var endpoint = string.Format(_config["ApiSettings:MajorGetByIdEndpoint"], Id);

            var response = await client.GetAsync(endpoint);

            if (!response.IsSuccessStatusCode)
            {
                ErrorMessage = "Unable to load major details.";
                return Page();
            }

            var json = await response.Content.ReadAsStringAsync();
            var apiResponse = System.Text.Json.JsonSerializer.Deserialize<ApiResponse<MajorDto>>(json, new System.Text.Json.JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            if (apiResponse?.Success == true && apiResponse.Data != null)
            {
                Major = apiResponse.Data;
            }
            else
            {
                ErrorMessage = apiResponse?.Message ?? "Major not found.";
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            try
            {
                var client = _factory.CreateClient("ApiClient");
                var endpoint = string.Format(_config["ApiSettings:MajorDeleteEndpoint"], Id);

                var response = await client.DeleteAsync(endpoint);

                if (response.IsSuccessStatusCode)
                {
                    return RedirectToPage("Index");
                }

                var errorContent = await response.Content.ReadAsStringAsync();
                ErrorMessage = $"Delete failed (Status: {response.StatusCode}): {errorContent}";
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Error: {ex.Message}";
            }

            return Page();
        }

        private async Task LoadUniversities()
        {
            var client = _factory.CreateClient("ApiClient");
            var endpoint = _config["ApiSettings:UniversityListEndpoint"];
            var response = await client.GetAsync(endpoint);
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var apiResponse = System.Text.Json.JsonSerializer.Deserialize<ApiResponses<UniversityDto>>(json, new System.Text.Json.JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                if (apiResponse?.Success == true && apiResponse.Data != null)
                {
                    _universityNames = apiResponse.Data.ToDictionary(u => u.Id, u => u.Name);
                }
            }
        }

        public string GetUniversityName(Guid? universityId)
        {
            if (universityId == null) return "Unknown";
            return _universityNames.TryGetValue(universityId.Value, out var name) ? name : "Unknown";
        }
    }
}
