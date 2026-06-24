# Starrez API Usage Guide

This document describes how to use the Starrez API client and facade classes, including authentication, environment configuration, and logging.

## 1. Overview

The Starrez API client classes provide methods to interact with the Starrez REST API for Entry, Booking, RoomSpaceDetail, and Booking updates. All requests use Basic Authentication and support environment-specific base URLs (e.g., development or production).

## 2. Authentication

All API calls require a username and password for Basic Authentication. These credentials are passed to each method and are used to set the HTTP Authorization header.

## 3. Environment Configuration

The base URL for the Starrez API should be set according to the environment:

- **Development:**
  `https://BIRMINGHAM.starrezhousing.com/StarRezRESTDev/services/select/`
- **Production:**
  `https://BIRMINGHAM.starrezhousing.com/StarRezREST/services/select/`

## 4. Client Method Signatures

```
Task<string> GetEntryAsync(string baseUrl, int entryId, string username, string password)
Task<string> GetBookingAsync(string baseUrl, int bookingId, string username, string password)
Task<string> GetRoomSpaceDetailAsync(string baseUrl, int roomSpaceId, string username, string password)
Task<string> UpdateBookingAsync(string baseUrl, int bookingId, string xmlBody, string username, string password)
```

## 5. Facade Usage Example

The `StarrezFacade` class wraps the client classes and manages the API calls and logging. It is constructed with the base URL, username, and password:

```
var httpClient = new HttpClient();
var baseUrl = "https://BIRMINGHAM.starrezhousing.com/StarRezRESTDev/services/select/";
var username = "your-username";
var password = "your-password";
var facade = new StarrezFacade(httpClient, baseUrl, username, password);

// Example: Process a student entry
string result = await facade.ProcessStudentEntryAsync(entryId);
```

## 6. Logging

All API calls and key steps are logged using NLog. The `StarrezFacade` logs:
- Start and completion of each process
- API responses
- Extracted IDs (BookingID, RoomSpaceID)
- Errors and warnings during XML parsing


## 7. Extracting IDs from API Responses

### Extracting BookingID from Entry Response
After calling the Entry API, you can extract the Booking ID from the XML response using the `ExtractBookingId` method in the `StarrezFacade`:

```
string entryResponse = await facade._entryClient.GetEntryAsync(baseUrl, entryId, username, password);
int bookingId = facade.ExtractBookingId(entryResponse);
```

If you use `ProcessStudentEntryAsync`, this extraction is handled automatically.

### Extracting RoomSpaceID from Booking Response
After calling the Booking API, extract the RoomSpaceID from the XML response using the `ExtractRoomSpaceId` method:

```
string bookingResponse = await facade._bookingClient.GetBookingAsync(baseUrl, bookingId, username, password);
int roomSpaceId = facade.ExtractRoomSpaceId(bookingResponse);
```

This is also handled automatically in `ProcessStudentEntryAsync`.


## 8. Updating a Booking

To update a booking, construct the XML body and call:

```
string xmlBody = @"<Booking>
  <CustomDate1>2024-09-21T00:00:00</CustomDate1>
  <CustomString10>2024-09-21T00:00:00</CustomString10>
  <Start_BookingReasonID>1</Start_BookingReasonID>
  <EntryStatusEnum>Reserved</EntryStatusEnum>
</Booking>";
string updateResult = await facade._updateBookingClient.UpdateBookingAsync(baseUrl, bookingId, xmlBody, username, password);
```

The XML body must match the schema expected by your Starrez instance. The example above demonstrates how to update fields such as `CustomDate1`, `CustomString10`, `Start_BookingReasonID`, and `EntryStatusEnum` for a booking.

The API call sends the XML as the request body with the content type `application/xml`.

## 9. Notes
- Always use the correct base URL for your environment.
- Ensure credentials are kept secure and not hard-coded in source files.
- Review logs for troubleshooting and auditing API interactions.

---

For further details, see the implementation in the `Bham.BizTalk.Client.Starrez` and `Bham.BizTalk.Client.Facades` namespaces.
