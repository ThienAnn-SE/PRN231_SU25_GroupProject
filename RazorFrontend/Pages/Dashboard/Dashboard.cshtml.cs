using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace RazorFrontend.Pages.Dashboard
{
    public class DashboardModel : PageModel
    {
        private readonly HttpClient _httpClient;

        public int UserCount { get; set; }
        public int MajorCount { get; set; }
        public int TestCount { get; set; }

        public DashboardModel(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("MyApiClient");
        }

        public async Task OnGetAsync()
        {
            try
            {
                // Gọi API để lấy dữ liệu
                UserCount = await _httpClient.GetFromJsonAsync<int>("https://localhost:5001/api/userprofiles/count");
                MajorCount = await _httpClient.GetFromJsonAsync<int>("https://localhost:5001/api/majors/count");
                TestCount = await _httpClient.GetFromJsonAsync<int>("https://localhost:5001/api/tests/count");
            }
            catch
            {
                // Nếu lỗi thì gán giá trị mặc định
                UserCount = 0;
                MajorCount = 0;
                TestCount = 0;
            }
        }
    }
}
