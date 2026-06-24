using System.Net.Http;
using System.Threading.Tasks;

namespace Bham.BizTalk.Rest.Starrez
{
    public class StarrezBookingClient
    {
        private readonly HttpClient _httpClient;
        public StarrezBookingClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<string> GetBookingAsync(int bookingId)
        {
            var url = $"https://BIRMINGHAM.starrezhousing.com/StarRezRESTDev/services/select/Booking.xml/?BookingID={bookingId}";
            var response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
        }
    }
}
