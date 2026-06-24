using System.Net.Http;
using System.Threading.Tasks;

namespace Bham.BizTalk.Rest.Starrez
{
    public class StarrezEntryClient
    {
        private readonly HttpClient _httpClient;
        public StarrezEntryClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<string> GetEntryAsync(int entryId)
        {
            var url = $"https://BIRMINGHAM.starrezhousing.com/StarRezRESTDev/services/select/Entry.xml/?ID1={entryId}";
            var response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
        }
    }
}
