using AppCore.BaseModel;
using AppCore.Dtos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using System.Net.Http.Json;
using System.Text.Json;

namespace RazorFrontend.Pages.Personalities
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
        public PersonalityDto Input { get; set; } = new();

        public List<CreateUpdatePersonalityDto> PersonalityTypes { get; set; } = new();

        public string? ErrorMessage { get; set; }

        public async Task OnGetAsync()
        {
            await LoadPersonalityTypes();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            await LoadPersonalityTypes();

            if (!ModelState.IsValid)
                return Page();

            try
            {
                var client = _factory.CreateClient("ApiClient");
                var endpoint = _config["ApiSettings:PersonalityCreateEndpoint"] ?? "api/personality";

                var response = await client.PostAsJsonAsync(endpoint, Input);

                if (response.IsSuccessStatusCode)
                {
                    return RedirectToPage("Index");
                }

                var body = await response.Content.ReadAsStringAsync();
                ErrorMessage = $"Failed to create personality: {body}";
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Error: {ex.Message}";
            }

            return Page();
        }

        private async Task LoadPersonalityTypes()
        {
            try
            {
                var client = _factory.CreateClient("ApiClient");
                var endpoint = _config["ApiSettings:PersonalityTypeListEndpoint"] ?? "api/personality/type";

                var response = await client.GetAsync(endpoint);

                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    var result = JsonSerializer.Deserialize<ApiResponses<CreateUpdatePersonalityDto>>(json, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

                    if (result?.Success == true && result.Data != null)
                    {
                        PersonalityTypes = result.Data;
                    }
                }
            }
            catch
            {
                PersonalityTypes = new();
            }
        }
    }
}
