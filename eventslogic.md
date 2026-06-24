# Gallagher API Events Logic

## 1. How to get all card events for the last hour for all cardholders, if it's the first time they used their card?

- Remove the cardholder filter from your API call to get events for all cardholders.
- Set the `after` parameter to one hour ago.
- Retrieve all events of type 20001 (card access).
- Post-process the results to keep only the first event per cardholder within the last hour.

Example API call (replace the date/time with the correct value for one hour ago):

    https://its-d-cdx-01.adf.bham.ac.uk:8904/api/events?after=16/04/2026%2013:00&type=20001&fields=cardholder,entryAccessZone,time,message

The API does not directly support filtering for "first use in the hour". You must fetch all events for the last hour and, in your application, group events by cardholder and select the earliest event for each cardholder.

---

## 2. Explanation of API response with no events

- "events": []
    - No card access events were found for the specified time range and filters.
- "previous", "next", "updates":
    - Pagination and update links for navigating results or checking for new events.

Summary: No card access events of type 20001 were found for any cardholder in the last hour. Double-check your date/time, time zone, and parameters if you expected results.

---

## 3. Explanation of API response with events

- "events": [ ... ]
    - Contains card access events matching your query.
    - Each event includes cardholder details, access zone details, time, and a message.
- "previous", "next", "updates":
    - Pagination and update links.

Summary: You received access events for cardholders in the specified zone and time. To filter for only the first event per cardholder, process the "events" array in your application to keep only the earliest event for each cardholder.

---

## 4. C# code sample to process events and return the first event per cardholder

```
using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;

// Assume 'eventsJson' is a JArray of your event objects from the API response
public List<JObject> GetFirstEventPerCardholder(JArray eventsJson)
{
    // Parse events into a list of objects with cardholder ID and time
    var events = eventsJson
        .Select(e => new
        {
            CardholderId = (string)e["cardholder"]["id"],
            Time = DateTime.Parse((string)e["time"]),
            Event = (JObject)e
        })
        .ToList();

    // Group by cardholder and select the earliest event for each
    var firstEvents = events
        .GroupBy(e => e.CardholderId)
        .Select(g => g.OrderBy(e => e.Time).First().Event)
        .ToList();

    return firstEvents;
}
```

Usage:
- Deserialize your API response to a JObject.
- Pass the "events" JArray to this method.
- The result is a list of event objects, one per cardholder, containing only their first event in the time window.
