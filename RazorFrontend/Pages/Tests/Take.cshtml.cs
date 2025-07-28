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
        public List<int> Answers { get; set; } = new();

        public async Task<IActionResult> OnGetAsync()
        {
            try
            {
                var client = _clientFactory.CreateClient("ApiClient");
                var testEndpoint = _config["ApiSettings:MBTITestId"]; // Get test by ID

                var response = await client.GetFromJsonAsync<ApiResponse<TestDto>>($"api/test/{testEndpoint}");

                if (response?.Success == true && response.Data != null)
                {
                    Questions = response.Data.Questions;
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
                return await OnGetAsync(); // Reload the test
            }

            try
            {
                // Gửi danh sách câu trả lời sang backend để đánh giá
                var result = WebApi.Extension.TestEvaluationExtensions.EvaluateMBTI(Answers);
                TempData["MbtiResult"] = result;
                return RedirectToPage("Result");
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
                return await OnGetAsync();
            }
        }
    }
}
