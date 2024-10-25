using System.Text;
using Newtonsoft.Json;

namespace ToDo_Api.IntegrationTests.Helpers;

/// <summary>
/// Provides helper methods for working with <see cref="HttpContent"/>.
/// </summary>
public static class HttpContentHelper
{
    /// <summary>
    /// Converts the specified object into JSON format and wraps it in an <see cref="HttpContent"/> object
    /// </summary>
    /// <param name="obj">The object to be converted to JSON.</param>
    /// <returns>An <see cref="HttpContent"/> object containing the JSON representation of the specified object,
    /// encoded with UTF-8 and having a media type of "application/json".</returns>
    public static HttpContent ToJsonHttpContent(this object obj)
    {
        var json = JsonConvert.SerializeObject(obj);

        var content = new StringContent(json, Encoding.UTF8, "application/json");

        return content;
    }
}
