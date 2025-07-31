using AppCore.BaseModel;
using AppCore.Dtos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace RazorFrontend.Pages.Tests
{
    public class ResultModel : PageModel
    {
        private readonly IHttpClientFactory _clientFactory;

        public ResultModel(IHttpClientFactory clientFactory)
        {
            _clientFactory = clientFactory;
        }

        public string PersonalityResult { get; set; } = "No result available.";

        public async Task<IActionResult> OnGetAsync()
        {
            if (!TempData.TryGetValue("SubmissionId", out var submissionIdObj) || submissionIdObj is null)
            {
                return Page();
            }

            var submissionId = Guid.Parse(submissionIdObj.ToString()!);
            var client = _clientFactory.CreateClient("ApiClient");

            var response = await client.GetFromJsonAsync<ApiResponse<TestSubmissionDto>>($"api/testsubmission/{submissionId}");
            if (response?.Success == true && response.Data != null)
            {
                var personalityId = response.Data.PersonalityId;
                if (personalityId != null)
                {
                    var personality = await client.GetFromJsonAsync<ApiResponse<PersonalityDto>>($"api/personality/{personalityId}");
                    if (personality?.Success == true && personality.Data != null)
                    {
                        PersonalityResult = personality.Data.Name;
                    }
                }
            }

            return Page();
        }
    }
}
