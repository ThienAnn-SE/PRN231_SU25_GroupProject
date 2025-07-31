using AppCore.Dtos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net.Http;
using System.Net.Http.Json;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RazorFrontend.Pages.Personalities
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
        public PersonalityDto Input { get; set; } = new();

        [BindProperty(SupportsGet = true)]
        public Guid Id { get; set; }

        public List<PersonalityTypeDto> PersonalityTypeList { get; set; } = new();

        public string? ErrorMessage { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            await LoadPersonalityTypes();

            var client = _factory.CreateClient();
            var endpoint = $"{_config["ApiSettings:PersonalityListEndpoint"] ?? "api/personality"}/{Id}";

            var response = await client.GetAsync(endpoint);
            if (!response.IsSuccessStatusCode)
            {
                ErrorMessage = "Personality not found.";
                return Page();
            }

            var personality = await response.Content.ReadFromJsonAsync<PersonalityDetailDto>();
            if (personality is null)
            {
                ErrorMessage = "Unable to load personality.";
                return Page();
            }

            Input = new PersonalityDto
            {
                Name = personality.Name,
                Description = personality.Description,
                PersonalityTypeId = personality.PersonalityType.Id,
            };

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            await LoadPersonalityTypes();

            if (!ModelState.IsValid)
                return Page();

            try
            {
                var client = _factory.CreateClient();
                var endpoint = $"{_config["ApiSettings:PersonalityListEndpoint"] ?? "api/personality"}/{Id}";
                var result = await client.PutAsJsonAsync(endpoint, Input);

                if (result.IsSuccessStatusCode)
                    return RedirectToPage("Index");

                ErrorMessage = $"Update failed: {await result.Content.ReadAsStringAsync()}";
            }
            catch (System.Exception ex)
            {
                ErrorMessage = $"Error: {ex.Message}";
            }

            return Page();
        }

        private async Task LoadPersonalityTypes()
        {
            var client = _factory.CreateClient();
            var endpoint = _config["ApiSettings:PersonalityTypeListEndpoint"] ?? "api/personalitytype";
            var response = await client.GetAsync(endpoint);
            if (response.IsSuccessStatusCode)
            {
                var data = await response.Content.ReadFromJsonAsync<List<PersonalityTypeDto>>();
                if (data != null)
                    PersonalityTypeList = data;
            }
        }
    }
} 