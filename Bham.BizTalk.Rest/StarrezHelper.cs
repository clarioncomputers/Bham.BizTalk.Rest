using System;
using System.Net;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using NLog;

namespace Bham.BizTalk.Rest
{
    public static class StarrezHelper
    {
        /// <summary>
        /// Extracts RoomSpaceID from Starrez Booking XML using StarrezFacade logic.
        /// </summary>
        public static int ExtractRoomSpaceId(string bookingXml)
        {
            var facade = new Bham.BizTalk.Rest.Facades.StarrezFacade(new System.Net.Http.HttpClient());
            return facade.ExtractRoomSpaceId(bookingXml);
        }

        /// <summary>
        /// Extracts BookingID from Starrez Entry XML using StarrezFacade logic.
        /// </summary>
        public static int ExtractBookingId(string entryXml)
        {
            var facade = new Bham.BizTalk.Rest.Facades.StarrezFacade(new System.Net.Http.HttpClient());
            return facade.ExtractBookingId(entryXml);
        }

        /// <summary>
        /// Extracts EntryID text value from Starrez Entry XML.
        /// </summary>
        public static string ExtractEntryId(string entryXml)
        {
            if (string.IsNullOrWhiteSpace(entryXml))
            {
                return string.Empty;
            }

            try
            {
                var doc = XDocument.Parse(entryXml);
                var entryIdElement = doc.Descendants("EntryID").FirstOrDefault();
                if (entryIdElement != null)
                {
                    var entryId = (entryIdElement.Value ?? string.Empty).Trim();
                    logger.Info($"Parsed EntryID: {entryId}");
                    return entryId;
                }

                logger.Warn("EntryID element not found in Entry XML.");
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Error parsing EntryID from Entry XML.");
            }

            return string.Empty;
        }
        
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Calls the Starrez Booking API and logs the request/response.
        /// </summary>
        /// <param name="baseUrl">Base URL for Starrez API (e.g., https://BIRMINGHAM.starrezhousing.com/StarRezRESTDev/services/select)</param>
        /// <param name="username">API username</param>
        /// <param name="password">API password</param>
        /// <param name="bookingId">Booking ID to query</param>
        /// <returns>API response as string</returns>
        public static string GetBookingByIdWithNLog(string baseUrl, string username, string password, string bookingId)
        {
            string url = $"{baseUrl}/Booking.xml/?BookingID={bookingId}";
            try
            {
                logger.Info($"Calling Starrez Booking API: {url}");
                var request = (HttpWebRequest)WebRequest.Create(url);
                request.Method = "GET";
                request.Credentials = new NetworkCredential(username, password);
                using (var response = (HttpWebResponse)request.GetResponse())
                using (var reader = new StreamReader(response.GetResponseStream()))
                {
                    string result = reader.ReadToEnd();
                    logger.Info($"Starrez Booking API response: {result.Substring(0, Math.Min(result.Length, 500))}");
                    return result;
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex, $"Error calling Starrez Booking API: {url}");
                throw;
            }
        }

        /// <summary>
        /// Calls the Starrez Entry API and logs the request/response.
        /// </summary>
        public static string GetEntryByIdWithNLog(string baseUrl, string username, string password, string entryId)
        {
            string url = $"{baseUrl}/Entry.xml/?ID1={entryId}";
            try
            {
                logger.Info($"Calling Starrez Entry API: {url}");
                var request = (HttpWebRequest)WebRequest.Create(url);
                request.Method = "GET";
                request.Credentials = new NetworkCredential(username, password);
                using (var response = (HttpWebResponse)request.GetResponse())
                using (var reader = new StreamReader(response.GetResponseStream()))
                {
                    string result = reader.ReadToEnd();
                    logger.Info($"Starrez Entry API response: {result.Substring(0, Math.Min(result.Length, 500))}");
                    return result;
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex, $"Error calling Starrez Entry API: {url}");
                throw;
            }
        }

        /// <summary>
        /// Calls the Starrez RoomSpaceDetail API and logs the request/response.
        /// </summary>
        public static string GetRoomSpaceDetailByIdWithNLog(string baseUrl, string username, string password, string roomSpaceDetailId)
        {
            string url = $"{baseUrl}/RoomSpaceDetail.xml/?RoomSpaceDetailID={roomSpaceDetailId}";
            try
            {
                logger.Info($"Calling Starrez RoomSpaceDetail API: {url}");
                var request = (HttpWebRequest)WebRequest.Create(url);
                request.Method = "GET";
                request.Credentials = new NetworkCredential(username, password);
                using (var response = (HttpWebResponse)request.GetResponse())
                using (var reader = new StreamReader(response.GetResponseStream()))
                {
                    string result = reader.ReadToEnd();
                    logger.Info($"Starrez RoomSpaceDetail API response: {result.Substring(0, Math.Min(result.Length, 500))}");
                    return result;
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex, $"Error calling Starrez RoomSpaceDetail API: {url}");
                throw;
            }
        }

        /// <summary>
        /// Generic method for Starrez update (POST/PUT) operations with logging.
        /// </summary>
        public static string UpdateStarrezWithNLog(string url, string username, string password, string payload, string method = "POST")
        {
            try
            {
                logger.Info($"Calling Starrez Update API: {url} with method {method}");
                var request = (HttpWebRequest)WebRequest.Create(url);
                request.Method = method;
                request.Credentials = new NetworkCredential(username, password);
                request.ContentType = "application/xml";
                using (var streamWriter = new StreamWriter(request.GetRequestStream()))
                {
                    streamWriter.Write(payload);
                }
                using (var response = (HttpWebResponse)request.GetResponse())
                using (var reader = new StreamReader(response.GetResponseStream()))
                {
                    string result = reader.ReadToEnd();
                    logger.Info($"Starrez Update API response: {result.Substring(0, Math.Min(result.Length, 500))}");
                    return result;
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex, $"Error calling Starrez Update API: {url}");
                throw;
            }
        }
    }
}
