using AppCore.BaseModel;
using AppCore.Dtos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using System.ComponentModel.DataAnnotations;
using System.Net.Http;
using System.Text;
using System.Text.Json;

namespace RazorFrontend.Pages.Universities
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
        public EditUniversityInput Input { get; set; } = new();

        [BindProperty(SupportsGet = true)]
        public Guid Id { get; set; }

        public string? ErrorMessage { get; set; }

        public class EditUniversityInput
        {
            [Required]
            public string Name { get; set; } = string.Empty;

            [Required]
            public string Location { get; set; } = string.Empty;

            [Required]
            public string PhoneNumber { get; set; } = string.Empty;

            [Required]
            [EmailAddress]
            public string Email { get; set; } = string.Empty;

            [Required]
            [Url]
            public string Website { get; set; } = string.Empty;

            [Required]
            public string Description { get; set; } = string.Empty;
        }

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
                    var university = apiResponse.Data;
                    Input = new EditUniversityInput
                    {
                        Name = university.Name,
                        Location = university.Location,
                        PhoneNumber = university.PhoneNumber,
                        Email = university.Email,
                        Website = university.Website,
                        Description = university.Description
                    };
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
            if (!ModelState.IsValid)
                return Page();

            var client = _clientFactory.CreateClient("ApiClient");
            var endpoint = string.Format(_config["ApiSettings:UniversityUpdateEndpoint"], Id);

            var dto = new CreateUpdateUniversityDto
            {
                Name = Input.Name,
                Location = Input.Location,
                PhoneNumber = Input.PhoneNumber,
                Email = Input.Email,
                Website = Input.Website,
                Description = Input.Description
            };

            try
            {
                var response = await client.PutAsJsonAsync(endpoint, dto);
                if (response.IsSuccessStatusCode)
                {
                    return RedirectToPage("Index");
                }

                var body = await response.Content.ReadAsStringAsync();
                ErrorMessage = $"Update failed: {body}";
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Error: {ex.Message}";
            }

            return Page();
        }
    }
}
