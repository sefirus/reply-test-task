using System.Text.Json;
using Reply.ContactManagement.Core.Entities;
using Reply.ContactManagement.Core.Enums;

namespace Reply.ContactManagement.Application.Services;

internal static class CustomFieldValueConverter
{
    public static bool TryMapToStorage(
        CustomFieldDataType dataType,
        JsonElement value,
        out string? stringValue,
        out int? integerValue,
        out bool? booleanValue)
    {
        stringValue = null;
        integerValue = null;
        booleanValue = null;

        if (value.ValueKind == JsonValueKind.Null)
        {
            return true;
        }

        return dataType switch
        {
            CustomFieldDataType.String => TryMapString(value, out stringValue),
            CustomFieldDataType.Integer => TryMapInteger(value, out integerValue),
            CustomFieldDataType.Boolean => TryMapBoolean(value, out booleanValue),
            _ => false
        };
    }

    public static JsonElement ToJsonElement(ContactCustomFieldValue value, CustomFieldDataType dataType)
    {
        return dataType switch
        {
            CustomFieldDataType.String => JsonSerializer.SerializeToElement(value.StringValue),
            CustomFieldDataType.Integer => JsonSerializer.SerializeToElement(value.IntegerValue),
            CustomFieldDataType.Boolean => JsonSerializer.SerializeToElement(value.BooleanValue),
            _ => JsonSerializer.SerializeToElement<object?>(null)
        };
    }

    private static bool TryMapString(JsonElement value, out string? stringValue)
    {
        stringValue = null;

        if (value.ValueKind != JsonValueKind.String)
        {
            return false;
        }

        stringValue = value.GetString();

        return stringValue is null || stringValue.Length <= 16000;
    }

    private static bool TryMapInteger(JsonElement value, out int? integerValue)
    {
        integerValue = null;

        if (value.ValueKind != JsonValueKind.Number || !value.TryGetInt32(out var parsedValue))
        {
            return false;
        }

        integerValue = parsedValue;

        return true;
    }

    private static bool TryMapBoolean(JsonElement value, out bool? booleanValue)
    {
        booleanValue = value.ValueKind switch
        {
            JsonValueKind.True => true,
            JsonValueKind.False => false,
            _ => null
        };

        return value.ValueKind is JsonValueKind.True or JsonValueKind.False;
    }
}
