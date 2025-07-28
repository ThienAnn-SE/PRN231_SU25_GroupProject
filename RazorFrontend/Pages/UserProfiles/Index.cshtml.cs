using AppCore.BaseModel;
using AppCore.Dtos;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using System.Net.Http;
using System.Net.Http.Json;

namespace RazorFrontend.Pages.UserProfiles
{
    public class IndexModel : PageModel
    {
        private readonly IHttpClientFactory _clientFactory;
        private readonly IConfiguration _config;

        public IndexModel(IHttpClientFactory clientFactory, IConfiguration config)
        {
            _clientFactory = clientFactory;
            _config = config;
        }

        public List<UserProfileDto> Profiles { get; set; } = new();
        public string? ErrorMessage { get; set; }

        public async Task OnGetAsync()
        {
            var client = _clientFactory.CreateClient("ApiClient");

            var endpoint = _config["ApiSettings:UserProfileListEndpoint"];

            try
            {
                var response = await client.GetAsync(endpoint);

                if (response.IsSuccessStatusCode)
                {
                    var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponses<UserProfileDto>>();

                    if (apiResponse?.Success == true && apiResponse.Data != null)
                    {
                        Profiles = apiResponse.Data;
                    }
                    else
                    {
                        ErrorMessage = apiResponse?.Message ?? "Failed to load user profiles.";
                    }
                }
                else
                {
                    ErrorMessage = "Failed to fetch user profiles from API.";
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Error: {ex.Message}";
            }
        }
    }
}
