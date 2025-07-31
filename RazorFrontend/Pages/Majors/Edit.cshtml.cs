using AppCore.Dtos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net.Http;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace RazorFrontend.Pages.Majors
{
    public class EditModel : PageModel
    {
        private readonly IHttpClientFactory _factory;
        private readonly IConfiguration _config;

        public EditModel(IHttpClientFactory factory, IConfiguration config)
        {
            _factory = factory;
            _config = config;
        }

        [BindProperty]
        public CreateUpdateMajorDto Input { get; set; } = new();

        [BindProperty(SupportsGet = true)]
        public Guid Id { get; set; }

        public List<UniversityDto> UniversityList { get; set; } = new();

        public string? ErrorMessage { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            await LoadUniversities();

            var client = _factory.CreateClient("ApiClient");
            var endpoint = $"{_config["ApiSettings:MajorListEndpoint"]}/{Id}";

            var response = await client.GetAsync(endpoint);
            if (!response.IsSuccessStatusCode)
            {
                ErrorMessage = "Major not found.";
                return Page();
            }

            var major = await response.Content.ReadFromJsonAsync<MajorDto>();
            if (major is null)
            {
                ErrorMessage = "Unable to load major.";
                return Page();
            }

            Input = new CreateUpdateMajorDto
            {
                Name = major.Name,
                Description = major.Description,
                RequiredSkills = major.RequiredSkills,
                UniversityId = major.UniversityId
            };

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            await LoadUniversities();

            if (!ModelState.IsValid)
                return Page();

            try
            {
                var client = _factory.CreateClient("ApiClient");
                var endpoint = $"{_config["ApiSettings:MajorListEndpoint"]}/{Id}";
                var result = await client.PutAsJsonAsync(endpoint, Input);

                if (result.IsSuccessStatusCode)
                    return RedirectToPage("Index");

                // Try to parse API validation errors
                var content = await result.Content.ReadAsStringAsync();
                try
                {
                    var problemDetails = System.Text.Json.JsonSerializer.Deserialize<ValidationProblemDetails>(content);
                    if (problemDetails?.Errors != null)
                    {
                        foreach (var err in problemDetails.Errors)
                        {
                            foreach (var msg in err.Value)
                            {
                                ModelState.AddModelError(err.Key, msg);
                            }
                        }
                    }
                    else
                    {
                        ErrorMessage = $"Update failed: {content}";
                    }
                }
                catch
                {
                    ErrorMessage = $"Update failed: {content}";
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Error: {ex.Message}";
            }

            return Page();
        }

        private class UniversityApiResponse
        {
            public List<UniversityDto> Data { get; set; } = new();
        }

        private async Task LoadUniversities()
        {
            var client = _factory.CreateClient("ApiClient");
            var endpoint = _config["ApiSettings:UniversityListEndpoint"];
            var response = await client.GetAsync(endpoint);
            if (response.IsSuccessStatusCode)
            {
                try
                {
                    var json = await response.Content.ReadAsStringAsync();
                    if (json.Trim().StartsWith("["))
                    {
                        UniversityList = await response.Content.ReadFromJsonAsync<List<UniversityDto>>() ?? new();
                    }
                    else
                    {
                        var result = await response.Content.ReadFromJsonAsync<UniversityApiResponse>();
                        UniversityList = result?.Data ?? new();
                    }
                }
                catch (Exception ex)
                {
                    ErrorMessage = $"Failed to read university list: {ex.Message}";
                }
            }
            else
            {
                ErrorMessage = $"Failed to load university list (HTTP {response.StatusCode})";
            }
        }
    }
}
