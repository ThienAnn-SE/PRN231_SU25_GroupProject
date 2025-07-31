using AppCore.BaseModel;
using AppCore.Dtos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net.Http;
using System.Net.Http.Json;

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
            var endpoint = string.Format(_config["ApiSettings:MajorGetByIdEndpoint"], Id);

            var response = await client.GetAsync(endpoint);
            if (!response.IsSuccessStatusCode)
            {
                ErrorMessage = "Major not found.";
                return Page();
            }

            var json = await response.Content.ReadAsStringAsync();
            var apiResponse = System.Text.Json.JsonSerializer.Deserialize<ApiResponse<MajorDto>>(json, new System.Text.Json.JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            if (apiResponse?.Success == true && apiResponse.Data != null)
            {
                var major = apiResponse.Data;
                Input = new CreateUpdateMajorDto
                {
                    Name = major.Name,
                    Description = major.Description,
                    RequiredSkills = major.RequiredSkills,
                    UniversityId = major.UniversityId
                };
            }
            else
            {
                ErrorMessage = apiResponse?.Message ?? "Unable to load major.";
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            await LoadUniversities();

            if (!ModelState.IsValid)
            {
                var errors = string.Join(", ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
                ErrorMessage = $"Model validation failed: {errors}";
                return Page();
            }

            try
            {
                var client = _factory.CreateClient("ApiClient");
                var endpoint = string.Format(_config["ApiSettings:MajorUpdateEndpoint"], Id);
                
                var result = await client.PutAsJsonAsync(endpoint, Input);

                if (result.IsSuccessStatusCode)
                {
                    return RedirectToPage("Index");
                }

                var errorContent = await result.Content.ReadAsStringAsync();
                ErrorMessage = $"Update failed (Status: {result.StatusCode}): {errorContent}";
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
                    UniversityList = apiResponse.Data;
                }
            }
        }
    }
}
