using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Bham.BizTalk.Rest.Starrez
{
    public class StarrezUpdateBookingClient
    {
        private readonly HttpClient _httpClient;
        public StarrezUpdateBookingClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<string> UpdateBookingAsync(int bookingId, string xmlBody)
        {
            var url = $"https://BIRMINGHAM.starrezhousing.com/StarRezRESTDev/services/update/Booking/{bookingId}";
            var content = new StringContent(xmlBody, Encoding.UTF8, "application/xml");
            var response = await _httpClient.PostAsync(url, content);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
        }
    }
}
