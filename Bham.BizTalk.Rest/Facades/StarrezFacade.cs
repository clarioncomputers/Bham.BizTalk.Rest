using System.Net.Http;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Linq;
using NLog;
using Bham.BizTalk.Rest.Starrez;

namespace Bham.BizTalk.Rest.Facades
{
    public class StarrezFacade
    {
        private static readonly ILogger Logger = LogManager.GetLogger("StarrezFacade");

        private readonly StarrezEntryClient _entryClient;
        private readonly StarrezBookingClient _bookingClient;
        private readonly StarrezRoomSpaceDetailClient _roomSpaceDetailClient;
        private readonly StarrezUpdateBookingClient _updateBookingClient;

        public StarrezFacade(HttpClient httpClient)
        {
            _entryClient = new StarrezEntryClient(httpClient);
            _bookingClient = new StarrezBookingClient(httpClient);
            _roomSpaceDetailClient = new StarrezRoomSpaceDetailClient(httpClient);
            _updateBookingClient = new StarrezUpdateBookingClient(httpClient);
        }

        /// <summary>
        /// Logs an orchestration transcript or any string to NLog (Info level).
        /// Call this from BizTalk Expression Shapes.
        /// </summary>
        public static void LogTranscript(string transcript)
        {
            Logger.Info("Orchestration Transcript: {0}", transcript);
        }

        public async Task<string> ProcessStudentEntryAsync(int entryId)
        {
            Logger.Info($"Starting student entry process for EntryID: {entryId}");
            // 1. Call Entry API
            var entryResponse = await _entryClient.GetEntryAsync(entryId);
            Logger.Debug($"Entry API response: {entryResponse}");
            int bookingId = ExtractBookingId(entryResponse);
            Logger.Info($"Extracted BookingID: {bookingId}");

            // 2. Call Booking API
            var bookingResponse = await _bookingClient.GetBookingAsync(bookingId);
            Logger.Debug($"Booking API response: {bookingResponse}");
            int roomSpaceId = ExtractRoomSpaceId(bookingResponse);
            Logger.Info($"Extracted RoomSpaceID: {roomSpaceId}");

            // 3. Call RoomSpaceDetail API
            var roomSpaceDetailResponse = await _roomSpaceDetailClient.GetRoomSpaceDetailAsync(roomSpaceId);
            Logger.Debug($"RoomSpaceDetail API response: {roomSpaceDetailResponse}");

            // 4. Update Booking if needed (XML parsing and logic needed)
            // string xmlBody = ...
            // var updateResponse = await _updateBookingClient.UpdateBookingAsync(bookingId, xmlBody);

            Logger.Info($"Completed student entry process for EntryID: {entryId}");
            return roomSpaceDetailResponse;
        }

        public int ExtractBookingId(string entryXml)
        {
            if (string.IsNullOrWhiteSpace(entryXml)) return 0;
            try
            {
                var doc = XDocument.Parse(entryXml);
                var bookingIdElement = doc.Descendants("BookingID").FirstOrDefault();
                int bookingId;
                if (bookingIdElement != null && int.TryParse(bookingIdElement.Value, out bookingId))
                {
                    Logger.Info($"Parsed BookingID: {bookingId}");
                    return bookingId;
                }
                else
                {
                    Logger.Warn("BookingID element not found or invalid in Entry XML.");
                }
            }
            catch (System.Exception ex)
            {
                Logger.Error(ex, "Error parsing BookingID from Entry XML.");
            }
            return 0;
        }

        public int ExtractRoomSpaceId(string bookingXml)
        {
            if (string.IsNullOrWhiteSpace(bookingXml)) return 0;
            try
            {
                var doc = XDocument.Parse(bookingXml);
                var roomSpaceIdElement = doc.Descendants("RoomSpaceID").FirstOrDefault();
                int roomSpaceId;
                if (roomSpaceIdElement != null && int.TryParse(roomSpaceIdElement.Value, out roomSpaceId))
                {
                    Logger.Info($"Parsed RoomSpaceID: {roomSpaceId}");
                    return roomSpaceId;
                }
                else
                {
                    Logger.Warn("RoomSpaceID element not found or invalid in Booking XML.");
                }
            }
            catch (System.Exception ex)
            {
                Logger.Error(ex, "Error parsing RoomSpaceID from Booking XML.");
            }
            return 0;
        }
    }
}
