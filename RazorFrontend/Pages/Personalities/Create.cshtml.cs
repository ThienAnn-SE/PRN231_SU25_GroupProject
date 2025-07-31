using AppCore.Dtos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using System.ComponentModel.DataAnnotations;
using System.Net.Http;
using System.Net.Http.Json;
using System.Collections.Generic;
using System.Threading.Tasks;

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
        public List<PersonalityTypeDto> PersonalityTypeList { get; set; } = new();

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
                var client = _factory.CreateClient();
                var endpoint = _config["ApiSettings:PersonalityCreateEndpoint"] ?? "api/personality";
                var response = await client.PostAsJsonAsync(endpoint, Input);

                if (response.IsSuccessStatusCode)
                {
                    return RedirectToPage("Index");
                }

                var body = await response.Content.ReadAsStringAsync();
                ErrorMessage = $"Failed to create: {body}";
            }
            catch (System.Exception ex)
            {
                ErrorMessage = $"Error: {ex.Message}";
            }

            return Page();
        }

        private async Task LoadPersonalityTypes()
        {
            try
            {
                var client = _factory.CreateClient();
                var endpoint = _config["ApiSettings:PersonalityTypeListEndpoint"] ?? "api/personalitytype";
                var response = await client.GetAsync(endpoint);

                if (response.IsSuccessStatusCode)
                {
                    var data = await response.Content.ReadFromJsonAsync<List<PersonalityTypeDto>>();
                    if (data != null) PersonalityTypeList = data;
                }
            }
            catch
            {
                PersonalityTypeList = new();
            }
        }
    }
}
