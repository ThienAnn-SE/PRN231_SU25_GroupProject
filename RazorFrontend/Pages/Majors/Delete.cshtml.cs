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

        public async Task<IActionResult> OnGetAsync()
        {
            var client = _factory.CreateClient();
            var endpoint = $"{_config["ApiSettings:MajorListEndpoint"]}/{Id}";

            var response = await client.GetAsync(endpoint);

            if (!response.IsSuccessStatusCode)
            {
                ErrorMessage = "Unable to load major details.";
                return Page();
            }

            Major = await response.Content.ReadFromJsonAsync<MajorDto>();
            if (Major == null)
            {
                ErrorMessage = "Major not found.";
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var client = _factory.CreateClient();
            var endpoint = $"{_config["ApiSettings:MajorListEndpoint"]}/{Id}";

            var response = await client.DeleteAsync(endpoint);

            if (response.IsSuccessStatusCode)
            {
                return RedirectToPage("Index");
            }

            ErrorMessage = $"Delete failed: {response.ReasonPhrase}";
            return Page();
        }
    }
}
