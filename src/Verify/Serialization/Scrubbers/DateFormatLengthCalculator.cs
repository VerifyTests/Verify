static class DateFormatLengthCalculator
{
    const int maxSecondsFractionDigits = 7;

    public static (int max, int min) GetLength(scoped CharSpan format, Culture culture)
    {
        var cultureDates = DateScrubber.GetCultureDates(culture);

        var index = 0;

        var minLength = 0;
        var maxLength = 0;
        while (index < format.Length)
        {
            var ch = format[index];
            int nextChar;
            int tokenLen;
            switch (ch)
            {
                case 'g':
                    tokenLen = ParseRepeatPattern(format, index, ch);
                    minLength += cultureDates.EraShort;
                    maxLength += cultureDates.EraLong;
                    break;

                case 'h':
                case 'H':
                case 'm':
                case 's':
                    tokenLen = ParseRepeatPattern(format, index, ch);
                    minLength += Math.Min(tokenLen, 2);
                    maxLength += 2;
                    break;

                case 'f':
                case 'F':
                    tokenLen = ParseRepeatPattern(format, index, ch);
                    if (tokenLen > maxSecondsFractionDigits)
                    {
                        throw new FormatException("Too many second fraction digits");
                    }

                    minLength += tokenLen;
                    maxLength += tokenLen;

                    break;
                case 't':
                    tokenLen = ParseRepeatPattern(format, index, ch);
                    if (tokenLen == 1)
                    {
                        maxLength += 1;
                        minLength += 1;
                    }
                    else
                    {
                        maxLength += 2;
                        minLength += 2;
                    }

                    break;
                case 'd':
                    // tokenLen == 1 : Day of month as digits with no leading zero.
                    // tokenLen == 2 : Day of month as digits with leading zero for single-digit months.
                    // tokenLen == 3 : Day of week as a three-letter abbreviation.
                    // tokenLen >= 4 : Day of week as its full name.
                    tokenLen = ParseRepeatPattern(format, index, ch);
                    if (tokenLen == 1)
                    {
                        minLength += 1;
                        maxLength += 2;
                    }
                    else if (tokenLen == 2)
                    {
                        minLength += 2;
                        maxLength += 2;
                    }
                    else if (tokenLen == 3)
                    {
                        minLength += cultureDates.AbbreviatedDayNameShort;
                        maxLength += cultureDates.AbbreviatedDayNameLong;
                    }
                    else
                    {
                        minLength += cultureDates.DayNameShort;
                        maxLength += cultureDates.DayNameLong;
                    }

                    break;
                case 'M':
                    // tokenLen == 1 : Month as digits with no leading zero.
                    // tokenLen == 2 : Month as digits with leading zero for single-digit months.
                    // tokenLen == 3 : Month as a three-letter abbreviation.
                    // tokenLen >= 4 : Month as its full name.
                    tokenLen = ParseRepeatPattern(format, index, ch);
                    if (tokenLen == 1)
                    {
                        minLength += 1;
                        maxLength += 2;
                    }
                    else if (tokenLen == 2)
                    {
                        minLength += 2;
                        maxLength += 2;
                    }
                    else if (tokenLen == 3)
                    {
                        minLength += cultureDates.AbbreviatedMonthNameShort;
                        maxLength += cultureDates.AbbreviatedMonthNameLong;
                    }
                    else
                    {
                        minLength += cultureDates.MonthNameShort;
                        maxLength += cultureDates.MonthNameLong;
                    }

                    break;
                case 'y':
                    // Notes about OS behavior:
                    // y: Always print (year % 100). No leading zero.
                    // yy: Always print (year % 100) with leading zero.
                    // yyy/yyyy/yyyyy/... : Print year value.  With leading zeros.
                    tokenLen = ParseRepeatPattern(format, index, ch);
                    if (tokenLen == 1)
                    {
                        minLength += 1;
                        maxLength += 4;
                    }
                    else if (tokenLen == 2)
                    {
                        minLength += 2;
                        maxLength += 4;
                    }
                    else
                    {
                        minLength += tokenLen;
                        maxLength += Math.Max(tokenLen, 4);
                    }

                    break;
                case 'z':
                    tokenLen = ParseRepeatPattern(format, index, ch);
                    if (tokenLen == 1)
                    {
                        minLength += 2;
                        maxLength += 3;
                    }
                    else if (tokenLen == 2)
                    {
                        minLength += 3;
                        maxLength += 3;
                    }
                    else
                    {
                        minLength += 6;
                        maxLength += 6;
                    }

                    break;
                case 'K':
                    tokenLen = 1;
                    minLength += 6;
                    maxLength += 6;
                    break;
                case ':':
                    minLength += cultureDates.TimeSeparator;
                    maxLength += cultureDates.TimeSeparator;
                    tokenLen = 1;
                    break;
                case '/':
                    minLength += cultureDates.DateSeparator;
                    maxLength += cultureDates.DateSeparator;
                    tokenLen = 1;
                    break;
                case '\'':
                case '\"':
                    tokenLen = ParseQuoteString(format, index);
                    var unwrapped = tokenLen - 2;
                    minLength += unwrapped;
                    maxLength += unwrapped;
                    break;
                case '%':
                    // Optional format character.
                    // For example, format string "%d" will print day of month
                    // without leading zero.  Most of the cases, "%" can be ignored.
                    nextChar = ParseNextChar(format, index);
                    // nextChar will be -1 if we have already reached the end of the format string.
                    // Besides, we will not allow "%%" to appear in the pattern.
                    if (nextChar is < 0 or '%')
                    {
                        throw new FormatException("Detected '%' at the end of the format string or '%%' appears in the format string");
                    }

                    var nextCharChar = (char) nextChar;
                    var innerLength = GetLength([nextCharChar], culture);
                    maxLength += innerLength.max;
                    minLength += innerLength.min;
                    tokenLen = 2;

                    break;
                case '\\':
                    // Escaped character.  Can be used to insert a character into the format string.
                    // For example, "\d" will insert the character 'd' into the string.
                    //
                    // NOTE: we can remove this format character if we enforce the enforced quote
                    // character rule.
                    // That is, we ask everyone to use single quote or double quote to insert characters,
                    // then we can remove this character.
                    nextChar = ParseNextChar(format, index);
                    if (nextChar < 0)
                    {
                        // This means that '\' is at the end of the formatting string.
                        throw new FormatException("Detected a '\\' at the end of the formatting string");
                    }

                    minLength++;
                    maxLength++;
                    tokenLen = 2;

                    break;
                default:
                    minLength++;
                    maxLength++;
                    tokenLen = 1;
                    break;
            }

            index += tokenLen;
        }
        return (maxLength, minLength);
    }

    // Get the next character at the index of 'pos' in the 'format' string.
    // Return value of -1 means 'pos' is already at the end of the 'format' string.
    // Otherwise, return value is the int value of the next character.
    static int ParseNextChar(CharSpan format, int pos)
    {
        if (pos + 1 >= format.Length)
        {
            return -1;
        }

        return format[pos + 1];
    }

    static int ParseRepeatPattern(CharSpan format, int pos, char patternChar)
    {
        var index = pos + 1;
        while (index < format.Length && format[index] == patternChar)
        {
            index++;
        }

        return index - pos;
    }

    // The pos should point to a quote character. This method will
    // append to the result StringBuilder the string enclosed by the quote character.
    static int ParseQuoteString(scoped CharSpan format, int pos)
    {
        // pos will be the index of the quote character in the 'format' string.
        var formatLen = format.Length;
        var beginPos = pos;
        // Get the character used to quote the following string.
        var quoteChar = format[pos++];

        var foundQuote = false;
        while (pos < formatLen)
        {
            var ch = format[pos++];
            if (ch == quoteChar)
            {
                foundQuote = true;
                break;
            }

            if (ch != '\\')
            {
                continue;
            }

            if (pos >= formatLen)
            {
                throw new FormatException("Invalid that '\\' is at the end of the formatting string.");
            }

            // The following are used to support escaped character.
            // Escaped character is also supported in the quoted string.
            // Therefore, someone can use a format like "'minute:' mm\"" to display:
            //  minute: 45"
            // because the second double quote is escaped.
            pos++;
        }

        if (foundQuote)
        {
            // Return the character count including the begin/end quote characters and enclosed string.
            return pos - beginPos;
        }

        throw new FormatException($"we can't find the matching quote: {quoteChar}");
    }
}