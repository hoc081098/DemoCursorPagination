using System.Text.Json;
using Microsoft.AspNetCore.Authentication; // for Base64UrlTextEncoder
using System.Text;

namespace DemoCursorPagination;

public sealed record Cursor(DateOnly Date, string LastId)
{
  public static string Encode(Cursor cursor)
  {
    var jsonString = JsonSerializer.Serialize(cursor);
    return Base64UrlTextEncoder.Encode(Encoding.UTF8.GetBytes(jsonString));
  }

  public static Cursor? Decode(string? encodedCursor)
  {
    if (string.IsNullOrEmpty(encodedCursor))
    {
      return null;
    }

    try
    {
      var jsonString = Encoding.UTF8.GetString(Base64UrlTextEncoder.Decode(encodedCursor));
      return JsonSerializer.Deserialize<Cursor>(jsonString);
    }
    catch
    {
      return null;
    }
  }
}