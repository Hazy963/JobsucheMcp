using System.Text.Json;

namespace JobsucheMcp.Services;

/// <summary>Small helpers that make reading JSON safe: missing fields give "" instead of errors.</summary>
public static class JsonHelper
{
    /// <summary>Reads a direct property as text. Returns "" if missing, null or not a simple value.</summary>
    public static string GetText(JsonElement element, string propertyName)
    {
        if (element.ValueKind != JsonValueKind.Object)
        {
            return "";
        }

        if (!element.TryGetProperty(propertyName, out JsonElement value))
        {
            return "";
        }

        if (value.ValueKind == JsonValueKind.String)
        {
            return value.GetString() ?? "";
        }

        if (value.ValueKind == JsonValueKind.Number || value.ValueKind == JsonValueKind.True || value.ValueKind == JsonValueKind.False)
        {
            return value.ToString();
        }

        return "";
    }

    /// <summary>Reads a direct property as whole number. Returns 0 if missing.</summary>
    public static int GetNumber(JsonElement element, string propertyName)
    {
        if (element.ValueKind == JsonValueKind.Object
            && element.TryGetProperty(propertyName, out JsonElement value)
            && value.ValueKind == JsonValueKind.Number
            && value.TryGetInt32(out int number))
        {
            return number;
        }

        return 0;
    }

    /// <summary>Reads a text property inside a child object, for example parent "eintrittszeitraum" and child "von".</summary>
    public static string GetNestedText(JsonElement element, string parentName, string childName)
    {
        if (element.ValueKind == JsonValueKind.Object
            && element.TryGetProperty(parentName, out JsonElement parent))
        {
            return GetText(parent, childName);
        }

        return "";
    }

    /// <summary>Searches the first text property with this name anywhere inside the element.</summary>
    public static string FindText(JsonElement element, string propertyName)
    {
        if (element.ValueKind == JsonValueKind.Object)
        {
            foreach (JsonProperty property in element.EnumerateObject())
            {
                if (property.Name == propertyName && property.Value.ValueKind == JsonValueKind.String)
                {
                    return property.Value.GetString() ?? "";
                }

                string found = FindText(property.Value, propertyName);
                if (found != "")
                {
                    return found;
                }
            }
        }
        else if (element.ValueKind == JsonValueKind.Array)
        {
            foreach (JsonElement item in element.EnumerateArray())
            {
                string found = FindText(item, propertyName);
                if (found != "")
                {
                    return found;
                }
            }
        }

        return "";
    }
}
