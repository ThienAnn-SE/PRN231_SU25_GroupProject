using Microsoft.AspNetCore.Mvc.RazorPages;

namespace RazorFrontend.Pages.Tests
{
    public class ResultModel : PageModel
    {
        public string Result { get; set; } = string.Empty;

        public void OnGet()
        {
            if (TempData.ContainsKey("MbtiResult"))
            {
                Result = TempData["MbtiResult"]?.ToString() ?? "Unknown";
            }
            else
            {
                Result = "No result available.";
            }
        }
    }
}
