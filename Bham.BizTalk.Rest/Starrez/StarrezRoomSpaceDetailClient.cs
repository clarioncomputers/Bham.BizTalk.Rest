using System.Net.Http;
using System.Threading.Tasks;

namespace Bham.BizTalk.Rest.Starrez
{
    public class StarrezRoomSpaceDetailClient
    {
        private readonly HttpClient _httpClient;
        public StarrezRoomSpaceDetailClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<string> GetRoomSpaceDetailAsync(int roomSpaceId)
        {
            var url = $"https://BIRMINGHAM.starrezhousing.com/StarRezRESTDev/services/select/RoomSpaceDetail.xml/?RoomSpaceID={roomSpaceId}";
            var response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
        }
    }
}
