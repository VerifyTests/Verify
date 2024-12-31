static class DateFormatLengthCalculator
{

    private static void FormatCustomized<TChar>(
        DateTime dateTime, scoped ReadOnlySpan<char> format, DateTimeFormatInfo dtfi, TimeSpan offset, ref ValueListBuilder<TChar> result) where TChar : unmanaged, IUtfChar<TChar>
    {
        Calendar cal = dtfi.Calendar;

        // This is a flag to indicate if we are formatting the dates using Hebrew calendar.
        bool isHebrewCalendar = (cal.ID == CalendarId.HEBREW);
        bool isJapaneseCalendar = (cal.ID == CalendarId.JAPAN);
        // This is a flag to indicate if we are formatting hour/minute/second only.
        bool bTimeOnly = true;

        int i = 0;
        int tokenLen, hour12;

        int length = 0;
        while (i < format.Length)
        {
            char ch = format[i];
            int nextChar;
            switch (ch)
            {
                case 'g':
                    tokenLen = ParseRepeatPattern(format, i, ch);
                    AppendString(ref result, dtfi.GetEraName(cal.GetEra(dateTime)));
                    break;

                case 'h':
                    tokenLen = ParseRepeatPattern(format, i, ch);
                    hour12 = dateTime.Hour;
                    if (hour12 > 12)
                    {
                        hour12 -= 12;
                    }
                    else if (hour12 == 0)
                    {
                        hour12 = 12;
                    }
                    FormatDigits(ref result, hour12, Math.Min(tokenLen, 2));
                    break;

                case 'H':
                    tokenLen = ParseRepeatPattern(format, i, ch);
                    FormatDigits(ref result, dateTime.Hour, Math.Min(tokenLen, 2));
                    break;

                case 'm':
                    tokenLen = ParseRepeatPattern(format, i, ch);
                    FormatDigits(ref result, dateTime.Minute, Math.Min(tokenLen, 2));
                    break;

                case 's':
                    tokenLen = ParseRepeatPattern(format, i, ch);
                    FormatDigits(ref result, dateTime.Second, Math.Min(tokenLen, 2));
                    break;

                case 'f':
                case 'F':
                    tokenLen = ParseRepeatPattern(format, i, ch);
                    if (tokenLen <= MaxSecondsFractionDigits)
                    {
                        int fraction = (int)(dateTime.Ticks % TimeSpan.TicksPerSecond);
                        fraction /= TimeSpanParse.Pow10UpToMaxFractionDigits(MaxSecondsFractionDigits - tokenLen);
                        if (ch == 'f')
                        {
                            FormatFraction(ref result, fraction, fixedNumberFormats[tokenLen - 1]);
                        }
                        else
                        {
                            int effectiveDigits = tokenLen;
                            while (effectiveDigits > 0)
                            {
                                if (fraction % 10 == 0)
                                {
                                    fraction /= 10;
                                    effectiveDigits--;
                                }
                                else
                                {
                                    break;
                                }
                            }
                            if (effectiveDigits > 0)
                            {
                                FormatFraction(ref result, fraction, fixedNumberFormats[effectiveDigits - 1]);
                            }
                            else
                            {
                                // No fraction to emit, so see if we should remove decimal also.
                                if (result.Length > 0 && result[^1] == TChar.CastFrom('.'))
                                {
                                    result.Length--;
                                }
                            }
                        }
                    }
                    else
                    {
                        throw new FormatException(SR.Format_InvalidString);
                    }
                    break;

                case 't':
                    tokenLen = ParseRepeatPattern(format, i, ch);
                    if (tokenLen == 1)
                    {
                        string designator = dateTime.Hour < 12 ? dtfi.AMDesignator : dtfi.PMDesignator;
                        if (designator.Length >= 1)
                        {
                            AppendChar(ref result, designator[0]);
                        }
                    }
                    else
                    {
                        result.Append(dateTime.Hour < 12 ? dtfi.AMDesignatorTChar<TChar>() : dtfi.PMDesignatorTChar<TChar>());
                    }
                    break;

                case 'd':
                    //
                    // tokenLen == 1 : Day of month as digits with no leading zero.
                    // tokenLen == 2 : Day of month as digits with leading zero for single-digit months.
                    // tokenLen == 3 : Day of week as a three-letter abbreviation.
                    // tokenLen >= 4 : Day of week as its full name.
                    //
                    tokenLen = ParseRepeatPattern(format, i, ch);
                    if (tokenLen <= 2)
                    {
                        int day = cal.GetDayOfMonth(dateTime);
                        if (isHebrewCalendar)
                        {
                            // For Hebrew calendar, we need to convert numbers to Hebrew text for yyyy, MM, and dd values.
                            HebrewNumber.Append(ref result, day);
                        }
                        else
                        {
                            FormatDigits(ref result, day, tokenLen);
                        }
                    }
                    else
                    {
                        int dayOfWeek = (int)cal.GetDayOfWeek(dateTime);
                        AppendString(ref result, FormatDayOfWeek(dayOfWeek, tokenLen, dtfi));
                    }
                    bTimeOnly = false;
                    break;

                case 'M':
                    // tokenLen == 1 : Month as digits with no leading zero.
                    // tokenLen == 2 : Month as digits with leading zero for single-digit months.
                    // tokenLen == 3 : Month as a three-letter abbreviation.
                    // tokenLen >= 4 : Month as its full name.
                    tokenLen = ParseRepeatPattern(format, i, ch);
                    int month = cal.GetMonth(dateTime);
                    if (tokenLen <= 2)
                    {
                        if (isHebrewCalendar)
                        {
                            // For Hebrew calendar, we need to convert numbers to Hebrew text for yyyy, MM, and dd values.
                            HebrewNumber.Append(ref result, month);
                        }
                        else
                        {
                            FormatDigits(ref result, month, tokenLen);
                        }
                    }
                    else
                    {
                        if (isHebrewCalendar)
                        {
                            AppendString(ref result, FormatHebrewMonthName(dateTime, month, tokenLen, dtfi));
                        }
                        else
                        {
                            if ((dtfi.FormatFlags & DateTimeFormatFlags.UseGenitiveMonth) != 0)
                            {
                                AppendString(ref result,
                                    dtfi.InternalGetMonthName(
                                        month,
                                        IsUseGenitiveForm(format, i, tokenLen, 'd') ? MonthNameStyles.Genitive : MonthNameStyles.Regular,
                                        tokenLen == 3));
                            }
                            else
                            {
                                AppendString(ref result, FormatMonth(month, tokenLen, dtfi));
                            }
                        }
                    }
                    bTimeOnly = false;
                    break;

                case 'y':
                    // Notes about OS behavior:
                    // y: Always print (year % 100). No leading zero.
                    // yy: Always print (year % 100) with leading zero.
                    // yyy/yyyy/yyyyy/... : Print year value.  With leading zeros.

                    int year = cal.GetYear(dateTime);
                    tokenLen = ParseRepeatPattern(format, i, ch);
                    if (isJapaneseCalendar &&
                        !LocalAppContextSwitches.FormatJapaneseFirstYearAsANumber &&
                        year == 1 &&
                        ((i + tokenLen < format.Length && format[i + tokenLen] == DateTimeFormatInfoScanner.CJKYearSuff) ||
                         (i + tokenLen < format.Length - 1 && format[i + tokenLen] == '\'' && format[i + tokenLen + 1] == DateTimeFormatInfoScanner.CJKYearSuff)))
                    {
                        // We are formatting a Japanese date with year equals 1 and the year number is followed by the year sign \u5e74
                        // In Japanese dates, the first year in the era is not formatted as a number 1 instead it is formatted as \u5143 which means
                        // first or beginning of the era.
                        AppendChar(ref result, DateTimeFormatInfo.JapaneseEraStart[0]);
                    }
                    else if (dtfi.HasForceTwoDigitYears)
                    {
                        FormatDigits(ref result, year, Math.Min(tokenLen, 2));
                    }
                    else if (cal.ID == CalendarId.HEBREW)
                    {
                        HebrewNumber.Append(ref result, year);
                    }
                    else
                    {
                        if (tokenLen <= 2)
                        {
                            FormatDigits(ref result, year % 100, tokenLen);
                        }
                        else if (tokenLen <= 16) // FormatDigits has an implicit 16-digit limit
                        {
                            FormatDigits(ref result, year, tokenLen);
                        }
                        else
                        {
                            AppendString(ref result, year.ToString("D" + tokenLen.ToString(CultureInfo.InvariantCulture), CultureInfo.InvariantCulture));
                        }
                    }
                    bTimeOnly = false;
                    break;

                case 'z':
                    tokenLen = ParseRepeatPattern(format, i, ch);
                    FormatCustomizedTimeZone(dateTime, offset, tokenLen, bTimeOnly, ref result);
                    break;

                case 'K':
                    tokenLen = 1;
                    FormatCustomizedRoundripTimeZone(dateTime, offset, ref result);
                    break;

                case ':':
                    result.Append(dtfi.TimeSeparatorTChar<TChar>());
                    tokenLen = 1;
                    break;

                case '/':
                    result.Append(dtfi.DateSeparatorTChar<TChar>());
                    tokenLen = 1;
                    break;

                case '\'':
                case '\"':
                    tokenLen = ParseQuoteString(format, i, ref result);
                    break;

                case '%':
                    // Optional format character.
                    // For example, format string "%d" will print day of month
                    // without leading zero.  Most of the cases, "%" can be ignored.
                    nextChar = ParseNextChar(format, i);
                    // nextChar will be -1 if we have already reached the end of the format string.
                    // Besides, we will not allow "%%" to appear in the pattern.
                    if (nextChar >= 0 && nextChar != '%')
                    {
                        char nextCharChar = (char)nextChar;
                        FormatCustomized(dateTime, new ReadOnlySpan<char>(in nextCharChar), dtfi, offset, ref result);
                        tokenLen = 2;
                    }
                    else
                    {
                        //
                        // This means that '%' is at the end of the format string or
                        // "%%" appears in the format string.
                        //
                        throw new FormatException(SR.Format_InvalidString);
                    }
                    break;

                case '\\':
                    // Escaped character.  Can be used to insert a character into the format string.
                    // For example, "\d" will insert the character 'd' into the string.
                    //
                    // NOTENOTE : we can remove this format character if we enforce the enforced quote
                    // character rule.
                    // That is, we ask everyone to use single quote or double quote to insert characters,
                    // then we can remove this character.
                    //
                    nextChar = ParseNextChar(format, i);
                    if (nextChar >= 0)
                    {
                        result.Append(TChar.CastFrom(nextChar));
                        tokenLen = 2;
                    }
                    else
                    {
                        //
                        // This means that '\' is at the end of the formatting string.
                        //
                        throw new FormatException(SR.Format_InvalidString);
                    }
                    break;

                default:
                    // NOTENOTE : we can remove this rule if we enforce the enforced quote character rule.
                    // That is, if we ask everyone to use single quote or double quote to insert characters,
                    // then we can remove this default block.
                    AppendChar(ref result, ch);
                    tokenLen = 1;
                    break;
            }
            i += tokenLen;
        }
    }
    internal static int ParseRepeatPattern(ReadOnlySpan<char> format, int pos, char patternChar)
    {
        int index = pos + 1;
        while ((uint)index < (uint)format.Length && format[index] == patternChar)
        {
            index++;
        }
        return index - pos;
    }
}