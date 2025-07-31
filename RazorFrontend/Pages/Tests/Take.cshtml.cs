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

        public TakeModel(IHttpClientFactory clientFactory)
        {
            _clientFactory = clientFactory;
        }

        public List<QuestionDto> Questions { get; set; } = new();
        public string? ErrorMessage { get; set; }

        [BindProperty]
        public List<Guid> AnswerIds { get; set; } = new();

        public Guid TestId { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            try
            {
                var client = _clientFactory.CreateClient("ApiClient");

                // Call API to get all tests
                var response = await client.GetFromJsonAsync<ApiResponses<TestDto>>("gateway/test");

                if (response?.Success == true && response.Data != null && response.Data.Any())
                {
                    var firstTest = response.Data.First();
                    TestId = firstTest.Id;
                    Questions = firstTest.Questions;
                    return Page();
                }

                ErrorMessage = "Failed to load test list.";
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
            if (AnswerIds == null || AnswerIds.Count != 50)
            {
                ErrorMessage = "You must answer all 50 questions.";
                return await OnGetAsync();
            }

            try
            {
                var client = _clientFactory.CreateClient("ApiClient");

                // Get userId từ JWT hoặc session — tạm thời để Guid.Empty nếu chưa có auth
                var submission = new TestSubmissionDto
                {
                    TestId = TestId,
                    Date = DateTime.UtcNow,
                    ExamineeId = Guid.Empty, // Cần sửa nếu đã có login
                    Answers = AnswerIds
                };

                var response = await client.PostAsJsonAsync("gateway/testsubmission", submission);

                if (response.IsSuccessStatusCode)
                {
                    TempData["MbtiResult"] = "Submitted successfully.";
                    return RedirectToPage("Result");
                }

                ErrorMessage = "Failed to submit test.";
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
