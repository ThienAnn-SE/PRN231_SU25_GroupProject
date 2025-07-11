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
    public class CreateModel : PageModel
    {
        private readonly IHttpClientFactory _clientFactory;
        private readonly IConfiguration _config;

        public CreateModel(IHttpClientFactory clientFactory, IConfiguration config)
        {
            _clientFactory = clientFactory;
            _config = config;
        }

        [BindProperty]
        public CreateUniversityInput Input { get; set; } = new();

        public string? ErrorMessage { get; set; }

        public class CreateUniversityInput
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

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
                return Page();

            var client = _clientFactory.CreateClient();
            var baseUrl = _config["ApiSettings:BaseUrl"];
            var endpoint = $"{baseUrl}api/university";

            var dto = new CreateUpdateUniversityDto
            {
                Name = Input.Name,
                Location = Input.Location,
                PhoneNumber = Input.PhoneNumber,
                Email = Input.Email,
                Website = Input.Website,
                Description = Input.Description
            };

            var content = new StringContent(JsonSerializer.Serialize(dto), Encoding.UTF8, "application/json");

            try
            {
                var response = await client.PostAsync(endpoint, content);
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
    }
}
