using AppCore.BaseModel;
using AppCore.Dtos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using System.ComponentModel.DataAnnotations;
using System.Net.Http;
using System.Net.Http.Json;

namespace RazorFrontend.Pages.Majors
{
    public class CreateModel : PageModel
    {
        private readonly IHttpClientFactory _factory;
        private readonly IConfiguration _config;

        public CreateModel(IHttpClientFactory factory, IConfiguration config)
        {
            _factory = factory;
            _config = config;
        }

        [BindProperty]
        public CreateUpdateMajorDto Input { get; set; } = new();

        public List<UniversityDto> UniversityList { get; set; } = new();

        public string? ErrorMessage { get; set; }

        public async Task OnGetAsync()
        {
            await LoadUniversities();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            await LoadUniversities();

            if (!ModelState.IsValid)
                return Page();

            try
            {
                var client = _factory.CreateClient("ApiClient");
                var endpoint = _config["ApiSettings:MajorCreateEndpoint"];
                var response = await client.PostAsJsonAsync(endpoint, Input);

                if (response.IsSuccessStatusCode)
                {
                    return RedirectToPage("Index");
                }

                var body = await response.Content.ReadAsStringAsync();
                ErrorMessage = $"Failed to create: {body}";
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Error: {ex.Message}";
            }

            return Page();
        }

        private async Task LoadUniversities()
        {
            try
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
            catch
            {
                UniversityList = new(); // fallback
            }
        }
    }
}
