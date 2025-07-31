using AppCore.BaseModel;
using AppCore.Dtos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using System.Net.Http.Json;
using System.Text.Json;

namespace RazorFrontend.Pages.Tests
{
    public class TakeModel : PageModel
    {
        private readonly IHttpClientFactory _clientFactory;
        private readonly IConfiguration _config;

        public TakeModel(IHttpClientFactory clientFactory, IConfiguration config)
        {
            _clientFactory = clientFactory;
            _config = config;
        }

        public List<QuestionDto> Questions { get; set; } = new();
        public string? ErrorMessage { get; set; }

        [BindProperty]
        public List<Guid> Answers { get; set; } = new();

        private Guid CurrentTestId { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            try
            {
                var client = _clientFactory.CreateClient("ApiClient");
                var response = await client.GetFromJsonAsync<ApiResponse<TestDto>>("api/test/by-type/MBTI");

                if (response?.Success == true && response.Data != null)
                {
                    Questions = response.Data.Questions;
                    CurrentTestId = response.Data.Id;
                    TempData["CurrentTestId"] = CurrentTestId;
                    return Page();
                }

                ErrorMessage = response?.Message ?? "Failed to load test.";
                return Page();
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
                return Page();
            }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (Answers == null || Answers.Count != 50)
            {
                ErrorMessage = "You must answer all 50 questions.";
                return await OnGetAsync();
            }

            try
            {
                var testId = Guid.Parse(TempData["CurrentTestId"]?.ToString() ?? Guid.Empty.ToString());

                var client = _clientFactory.CreateClient("ApiClient");
                var examineeId = new Guid("11111111-1111-1111-1111-111111111111");

                var submission = new TestSubmissionDto
                {
                    TestId = testId,
                    ExamineeId = examineeId,
                    Date = DateTime.UtcNow,
                    Answers = Answers
                };

                var response = await client.PostAsJsonAsync("api/testsubmission", submission);

                if (response.IsSuccessStatusCode)
                {
                    var apiResult = await response.Content.ReadFromJsonAsync<ApiResponse>();
                    TempData["SubmissionMessage"] = apiResult?.Message ?? "Submitted!";
                    return RedirectToPage("Result");
                }

                var error = await response.Content.ReadAsStringAsync();
                ErrorMessage = $"Submission failed: {error}";
                return await OnGetAsync();
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
                return await OnGetAsync();
            }
        }
    }
}
