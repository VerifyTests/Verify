﻿using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using VerifyTests;

static class JsonFormatter
{
    internal static char QuoteChar = '\'';

    public static StringBuilder AsJson(object? input, JsonSerializerSettings settings, List<ToAppend> appends, VerifySettings verifySettings)
    {
        if (appends.Any())
        {
            var dictionary = new Dictionary<string, object>();
            if (input == null)
            {
                dictionary.Add("target", "null");
            }
            else
            {
                dictionary.Add("target", input);
            }

            input = dictionary;
            foreach (var append in appends)
            {
                dictionary[append.Name] = append.Data;
            }
        }

        var serializer = JsonSerializer.Create(settings);

        var builder = new StringBuilder();
        using var stringWriter = new StringWriter(builder)
        {
            NewLine = "\n"
        };
        using var writer = new JsonTextWriterEx(stringWriter, verifySettings.Context)
        {
            QuoteChar = QuoteChar,
            QuoteName = false
        };
        serializer.Serialize(writer, input);
        return builder;
    }
}