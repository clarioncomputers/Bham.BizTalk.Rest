using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Bham.BizTalk.Rest
{
    /// <summary>
    /// Parses common Gallagher collection responses into the ids and href values
    /// needed by orchestration code and smoke tests.
    /// </summary>
    public static class GallagherApiResponseParser
    {
        /// <summary>
        /// Parses Gallagher JSON text into a dictionary-based object graph.
        /// </summary>
        public static IDictionary<string, object> DeserializeJsonObject(string json)
        {
            var parsed = ParseJson(json) as IDictionary<string, object>;
            if (parsed == null)
            {
                throw new InvalidOperationException("JSON root must be an object.");
            }

            return parsed;
        }

        /// <summary>
        /// Returns the id from the first entity in a Gallagher collection response.
        /// </summary>
        public static string GetFirstEntityId(string responseJson)
        {
            string id;
            if (!TryGetFirstEntityId(responseJson, out id))
            {
                throw new InvalidOperationException("No entity id was found in the Gallagher response.");
            }

            return id;
        }

        /// <summary>
        /// Tries to return the id from the first entity in a Gallagher collection response.
        /// </summary>
        public static bool TryGetFirstEntityId(string responseJson, out string id)
        {
            return TryGetFirstMatchingValue(responseJson, delegate(IDictionary<string, object> item)
            {
                string candidate;
                return TryGetString(item, "id", out candidate) ? candidate : null;
            }, out id);
        }

        /// <summary>
        /// Returns the id of the entity whose name matches the supplied value.
        /// </summary>
        public static string GetEntityIdByName(string responseJson, string name)
        {
            string id;
            if (!TryGetEntityIdByName(responseJson, name, out id))
            {
                throw new InvalidOperationException("No Gallagher entity with the requested name was found in the response.");
            }

            return id;
        }

        /// <summary>
        /// Tries to return the id of the entity whose name matches the supplied value.
        /// </summary>
        public static bool TryGetEntityIdByName(string responseJson, string name, out string id)
        {
            if (string.IsNullOrWhiteSpace(name)) throw new ArgumentNullException(nameof(name));

            var expectedName = name.Trim();
            return TryGetFirstMatchingValue(responseJson, delegate(IDictionary<string, object> item)
            {
                string itemName;
                string itemId;
                if (!TryGetString(item, "name", out itemName) || !TryGetString(item, "id", out itemId))
                {
                    return null;
                }

                return string.Equals(itemName, expectedName, StringComparison.OrdinalIgnoreCase) ? itemId : null;
            }, out id);
        }

        /// <summary>
        /// Returns the href of the entity whose name matches the supplied value.
        /// </summary>
        public static string GetEntityHrefByName(string responseJson, string name)
        {
            string href;
            if (!TryGetEntityHrefByName(responseJson, name, out href))
            {
                throw new InvalidOperationException("No Gallagher entity with the requested name was found in the response.");
            }

            return href;
        }

        /// <summary>
        /// Tries to return the href of the entity whose name matches the supplied value.
        /// </summary>
        public static bool TryGetEntityHrefByName(string responseJson, string name, out string href)
        {
            if (string.IsNullOrWhiteSpace(name)) throw new ArgumentNullException(nameof(name));

            var expectedName = name.Trim();
            return TryGetFirstMatchingValue(responseJson, delegate(IDictionary<string, object> item)
            {
                string itemName;
                string itemHref;
                if (!TryGetString(item, "name", out itemName) || !TryGetString(item, "href", out itemHref))
                {
                    return null;
                }

                return string.Equals(itemName, expectedName, StringComparison.OrdinalIgnoreCase) ? itemHref : null;
            }, out href);
        }

        /// <summary>
        /// Returns the membership href for the specified cardholder from an access-group membership response.
        /// </summary>
        public static string GetAccessGroupMembershipHrefForCardholder(string responseJson, string cardholderId)
        {
            string href;
            if (!TryGetAccessGroupMembershipHrefForCardholder(responseJson, cardholderId, out href))
            {
                throw new InvalidOperationException("No Gallagher access-group membership href was found for the requested cardholder.");
            }

            return href;
        }

        /// <summary>
        /// Tries to return the membership href for the specified cardholder from an access-group membership response.
        /// </summary>
        public static bool TryGetAccessGroupMembershipHrefForCardholder(string responseJson, string cardholderId, out string href)
        {
            if (string.IsNullOrWhiteSpace(cardholderId)) throw new ArgumentNullException(nameof(cardholderId));

            var expectedCardholderId = cardholderId.Trim();
            return TryGetFirstMatchingValue(responseJson, delegate(IDictionary<string, object> item)
            {
                string itemHref;
                if (!TryGetString(item, "href", out itemHref))
                {
                    return null;
                }

                if (itemHref.IndexOf("/cardholders/" + expectedCardholderId + "/access_groups/", StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    return itemHref;
                }

                object nestedCardholder;
                IDictionary<string, object> cardholderObject;
                string nestedId;
                if (item.TryGetValue("cardholder", out nestedCardholder) &&
                    (cardholderObject = nestedCardholder as IDictionary<string, object>) != null &&
                    TryGetString(cardholderObject, "id", out nestedId) &&
                    string.Equals(nestedId, expectedCardholderId, StringComparison.OrdinalIgnoreCase))
                {
                    return itemHref;
                }

                return null;
            }, out href);
        }

        /// <summary>
        /// Returns the membership href matching a cardholder name and optional date range.
        /// </summary>
        public static string GetAccessGroupMembershipHrefByNameAndDates(string responseJson, string cardholderName, string fromDate = null, string untilDate = null)
        {
            string href;
            if (!TryGetAccessGroupMembershipHrefByNameAndDates(responseJson, cardholderName, fromDate, untilDate, out href))
            {
                throw new InvalidOperationException("No Gallagher access-group membership href was found for the requested cardholder name/date criteria.");
            }

            return href;
        }

        /// <summary>
        /// Tries to return the membership href matching a cardholder name and optional date range.
        /// </summary>
        public static bool TryGetAccessGroupMembershipHrefByNameAndDates(string responseJson, string cardholderName, string fromDate, string untilDate, out string href)
        {
            if (string.IsNullOrWhiteSpace(cardholderName)) throw new ArgumentNullException(nameof(cardholderName));

            var expectedName = cardholderName.Trim();
            var expectedFrom = string.IsNullOrWhiteSpace(fromDate) ? null : fromDate.Trim();
            var expectedUntil = string.IsNullOrWhiteSpace(untilDate) ? null : untilDate.Trim();

            return TryGetFirstMatchingValue(responseJson, delegate(IDictionary<string, object> item)
            {
                string itemHref;
                if (!TryGetString(item, "href", out itemHref) ||
                    itemHref.IndexOf("/access_groups/", StringComparison.OrdinalIgnoreCase) < 0)
                {
                    return null;
                }

                string actualName;
                if (!TryGetMembershipCardholderName(item, out actualName) ||
                    !string.Equals(actualName, expectedName, StringComparison.OrdinalIgnoreCase))
                {
                    return null;
                }

                string actualFrom;
                if (expectedFrom != null && (!TryGetString(item, "from", out actualFrom) || !DatesMatch(actualFrom, expectedFrom)))
                {
                    return null;
                }

                string actualUntil;
                if (expectedUntil != null && (!TryGetString(item, "until", out actualUntil) || !DatesMatch(actualUntil, expectedUntil)))
                {
                    return null;
                }

                return itemHref;
            }, out href);
        }

        private static bool TryGetFirstMatchingValue(string responseJson, Func<IDictionary<string, object>, string> selector, out string value)
        {
            if (selector == null) throw new ArgumentNullException(nameof(selector));

            value = null;
            var root = ParseJson(responseJson);
            foreach (var item in EnumerateObjects(root))
            {
                var candidate = selector(item);
                if (!string.IsNullOrWhiteSpace(candidate))
                {
                    value = candidate.Trim();
                    return true;
                }
            }

            return false;
        }

        private static object ParseJson(string json)
        {
            if (string.IsNullOrWhiteSpace(json)) throw new ArgumentNullException(nameof(json));
            return new JsonParser(json).Parse();
        }

        private static IEnumerable<IDictionary<string, object>> EnumerateObjects(object root)
        {
            if (root == null)
            {
                yield break;
            }

            var stack = new Stack<object>();
            stack.Push(root);

            while (stack.Count > 0)
            {
                var current = stack.Pop();
                if (current == null)
                {
                    continue;
                }

                var dictionary = current as IDictionary<string, object>;
                if (dictionary != null)
                {
                    yield return dictionary;

                    foreach (var pair in dictionary)
                    {
                        if (pair.Value != null)
                        {
                            stack.Push(pair.Value);
                        }
                    }

                    continue;
                }

                var enumerable = current as IEnumerable;
                if (enumerable == null || current is string)
                {
                    continue;
                }

                foreach (var item in enumerable)
                {
                    if (item != null)
                    {
                        stack.Push(item);
                    }
                }
            }
        }

        private static bool TryGetString(IDictionary<string, object> item, string key, out string value)
        {
            value = null;
            if (item == null || string.IsNullOrWhiteSpace(key))
            {
                return false;
            }

            object rawValue;
            if (!item.TryGetValue(key, out rawValue) || rawValue == null)
            {
                return false;
            }

            value = Convert.ToString(rawValue);
            return !string.IsNullOrWhiteSpace(value);
        }

        private static bool TryGetMembershipCardholderName(IDictionary<string, object> item, out string name)
        {
            name = null;

            if (TryGetString(item, "name", out name))
            {
                return true;
            }

            object nestedCardholder;
            IDictionary<string, object> cardholderObject;
            if (item != null && item.TryGetValue("cardholder", out nestedCardholder) &&
                (cardholderObject = nestedCardholder as IDictionary<string, object>) != null)
            {
                return TryGetString(cardholderObject, "name", out name);
            }

            return false;
        }

        private static bool DatesMatch(string actual, string expected)
        {
            if (string.IsNullOrWhiteSpace(actual) || string.IsNullOrWhiteSpace(expected))
            {
                return false;
            }

            var actualTrimmed = actual.Trim();
            var expectedTrimmed = expected.Trim();

            if (string.Equals(actualTrimmed, expectedTrimmed, StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }

            DateTimeOffset actualDate;
            DateTimeOffset expectedDate;
            if (!DateTimeOffset.TryParse(actualTrimmed, CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal | DateTimeStyles.AllowWhiteSpaces, out actualDate) ||
                !DateTimeOffset.TryParse(expectedTrimmed, CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal | DateTimeStyles.AllowWhiteSpaces, out expectedDate))
            {
                return false;
            }

            if (expectedTrimmed.IndexOf("T", StringComparison.OrdinalIgnoreCase) >= 0 ||
                expectedTrimmed.IndexOf(":", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                return actualDate.ToUniversalTime() == expectedDate.ToUniversalTime();
            }

            return actualDate.UtcDateTime.Date == expectedDate.UtcDateTime.Date;
        }

        private sealed class JsonParser
        {
            private readonly string _json;
            private int _index;

            public JsonParser(string json)
            {
                _json = json;
            }

            public object Parse()
            {
                SkipWhitespace();
                var value = ParseValue();
                SkipWhitespace();
                return value;
            }

            private object ParseValue()
            {
                SkipWhitespace();
                if (_index >= _json.Length)
                {
                    throw new InvalidOperationException("Unexpected end of JSON input.");
                }

                switch (_json[_index])
                {
                    case '{':
                        return ParseObject();
                    case '[':
                        return ParseArray();
                    case '"':
                        return ParseString();
                    case 't':
                        ExpectKeyword("true");
                        return true;
                    case 'f':
                        ExpectKeyword("false");
                        return false;
                    case 'n':
                        ExpectKeyword("null");
                        return null;
                    default:
                        return ParseLiteral();
                }
            }

            private IDictionary<string, object> ParseObject()
            {
                var result = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
                _index++;
                SkipWhitespace();

                if (TryConsume('}'))
                {
                    return result;
                }

                while (true)
                {
                    SkipWhitespace();
                    var key = ParseString();
                    SkipWhitespace();
                    Expect(':');
                    var value = ParseValue();
                    result[key] = value;
                    SkipWhitespace();

                    if (TryConsume('}'))
                    {
                        return result;
                    }

                    Expect(',');
                }
            }

            private IList<object> ParseArray()
            {
                var result = new List<object>();
                _index++;
                SkipWhitespace();

                if (TryConsume(']'))
                {
                    return result;
                }

                while (true)
                {
                    result.Add(ParseValue());
                    SkipWhitespace();

                    if (TryConsume(']'))
                    {
                        return result;
                    }

                    Expect(',');
                }
            }

            private string ParseString()
            {
                Expect('"');
                var builder = new StringBuilder();

                while (_index < _json.Length)
                {
                    var current = _json[_index++];
                    if (current == '"')
                    {
                        return builder.ToString();
                    }

                    if (current != '\\')
                    {
                        builder.Append(current);
                        continue;
                    }

                    if (_index >= _json.Length)
                    {
                        throw new InvalidOperationException("Invalid JSON escape sequence.");
                    }

                    var escaped = _json[_index++];
                    switch (escaped)
                    {
                        case '"':
                        case '\\':
                        case '/':
                            builder.Append(escaped);
                            break;
                        case 'b':
                            builder.Append('\b');
                            break;
                        case 'f':
                            builder.Append('\f');
                            break;
                        case 'n':
                            builder.Append('\n');
                            break;
                        case 'r':
                            builder.Append('\r');
                            break;
                        case 't':
                            builder.Append('\t');
                            break;
                        case 'u':
                            builder.Append(ParseUnicodeEscape());
                            break;
                        default:
                            throw new InvalidOperationException("Invalid JSON escape character: " + escaped);
                    }
                }

                throw new InvalidOperationException("Unterminated JSON string.");
            }

            private string ParseLiteral()
            {
                var start = _index;
                while (_index < _json.Length)
                {
                    var current = _json[_index];
                    if (current == ',' || current == ']' || current == '}' || char.IsWhiteSpace(current))
                    {
                        break;
                    }

                    _index++;
                }

                return _json.Substring(start, _index - start);
            }

            private char ParseUnicodeEscape()
            {
                if (_index + 4 > _json.Length)
                {
                    throw new InvalidOperationException("Invalid JSON unicode escape sequence.");
                }

                var hex = _json.Substring(_index, 4);
                _index += 4;
                return (char)Convert.ToInt32(hex, 16);
            }

            private void SkipWhitespace()
            {
                while (_index < _json.Length && char.IsWhiteSpace(_json[_index]))
                {
                    _index++;
                }
            }

            private void Expect(char expected)
            {
                SkipWhitespace();
                if (_index >= _json.Length || _json[_index] != expected)
                {
                    throw new InvalidOperationException("Expected JSON token: " + expected);
                }

                _index++;
            }

            private void ExpectKeyword(string keyword)
            {
                if (_index + keyword.Length > _json.Length ||
                    string.Compare(_json, _index, keyword, 0, keyword.Length, StringComparison.Ordinal) != 0)
                {
                    throw new InvalidOperationException("Expected JSON keyword: " + keyword);
                }

                _index += keyword.Length;
            }

            private bool TryConsume(char token)
            {
                SkipWhitespace();
                if (_index < _json.Length && _json[_index] == token)
                {
                    _index++;
                    return true;
                }

                return false;
            }
        }
    }
}
