﻿using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using Newtonsoft.Json;

class SharedScrubber
{
    internal static List<string> datetimeFormats = new List<string>();
    internal static List<string> datetimeOffsetFormats = new List<string>();
    bool scrubGuids;
    bool scrubDateTimes;
    JsonSerializerSettings settings;
    static Func<Guid, int> intOrNextGuid = null!;
    static Func<DateTime, int> intOrNextDateTime = null!;
    static Func<DateTimeOffset, int> intOrNextDateTimeOffset = null!;

    public static void SetIntOrNext(
        Func<Guid, int> guid,
        Func<DateTime, int> dateTime,
        Func<DateTimeOffset, int> dateTimeOffset
    )
    {
        intOrNextGuid = guid;
        intOrNextDateTime = dateTime;
        intOrNextDateTimeOffset = dateTimeOffset;
    }

    public SharedScrubber(bool scrubGuids, bool scrubDateTimes, JsonSerializerSettings settings)
    {
        this.scrubGuids = scrubGuids;
        this.scrubDateTimes = scrubDateTimes;
        this.settings = settings;
    }

    public bool TryConvert(Guid value, [NotNullWhen(true)] out string? result)
    {
        if (!scrubGuids)
        {
            result = null;
            return false;
        }

        result = Convert(value);
        return true;
    }

    public bool TryConvert(DateTime value, [NotNullWhen(true)] out string? result)
    {
        if (!scrubDateTimes)
        {
            result = null;
            return false;
        }

        result = Convert(value);
        return true;
    }

    public bool TryConvert(DateTimeOffset value, [NotNullWhen(true)] out string? result)
    {
        if (!scrubDateTimes)
        {
            result = null;
            return false;
        }

        result = Convert(value);
        return true;
    }

    static string Convert(Guid guid)
    {
        var next = intOrNextGuid(guid);
        return $"Guid_{next}";
    }

    static string Convert(DateTime dateTime)
    {
        var next = intOrNextDateTime(dateTime);
        return $"DateTime_{next}";
    }

    static string Convert(DateTimeOffset dateTime)
    {
        var next = intOrNextDateTimeOffset(dateTime);
        return $"DateTimeOffset_{next}";
    }

    public bool TryConvertString(string value, [NotNullWhen(true)] out string? result)
    {
        if (TryParseConvertGuid(value, out result))
        {
            return true;
        }

        if (TryParseConvertDateTimeOffset(value, out result))
        {
            return true;
        }

        if (TryParseConvertDateTime(value, out result))
        {
            return true;
        }

        result = null;
        return false;
    }

    public bool TryParseConvertDateTime(string value, [NotNullWhen(true)] out string? result)
    {
        if (scrubDateTimes)
        {
            if (DateTime.TryParseExact(value, settings.DateFormatString, null, DateTimeStyles.None, out var dateTime))
            {
                result = Convert(dateTime);
                return true;
            }

            foreach (var format in datetimeFormats)
            {
                if (DateTime.TryParseExact(value, format, null, DateTimeStyles.None, out dateTime))
                {
                    result = Convert(dateTime);
                    return true;
                }
            }
        }

        result = null;
        return false;
    }

    public bool TryParseConvertDateTimeOffset(string value, [NotNullWhen(true)] out string? result)
    {
        if (scrubDateTimes)
        {
            if (DateTimeOffset.TryParseExact(value, settings.DateFormatString, null, DateTimeStyles.None, out var dateTimeOffset))
            {
                result = Convert(dateTimeOffset);
                return true;
            }

            foreach (var format in datetimeOffsetFormats)
            {
                if (DateTimeOffset.TryParseExact(value, format, null, DateTimeStyles.None, out dateTimeOffset))
                {
                    result = Convert(dateTimeOffset);
                    return true;
                }
            }
        }
        result = null;

        return false;
    }

    public bool TryParseConvertGuid(string value, [NotNullWhen(true)] out string? result)
    {
        if (scrubGuids)
        {
            if (Guid.TryParse(value, out var guid))
            {
                result = Convert(guid);
                return true;
            }
        }

        result = null;
        return false;
    }
}