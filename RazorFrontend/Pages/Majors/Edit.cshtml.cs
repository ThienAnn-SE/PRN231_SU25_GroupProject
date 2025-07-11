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

            var client = _factory.CreateClient();
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
                var client = _factory.CreateClient();
                var endpoint = $"{_config["ApiSettings:MajorListEndpoint"]}/{Id}";
                var result = await client.PutAsJsonAsync(endpoint, Input);

                if (result.IsSuccessStatusCode)
                    return RedirectToPage("Index");

                ErrorMessage = $"Update failed: {await result.Content.ReadAsStringAsync()}";
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Error: {ex.Message}";
            }

            return Page();
        }

        private async Task LoadUniversities()
        {
            var client = _factory.CreateClient();
            var endpoint = _config["ApiSettings:UniversityListEndpoint"];
            var response = await client.GetAsync(endpoint);
            if (response.IsSuccessStatusCode)
            {
                var data = await response.Content.ReadFromJsonAsync<List<UniversityDto>>();
                if (data != null)
                    UniversityList = data;
            }
        }
    }
}
