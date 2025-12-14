namespace VerifyTests;

static class XmlScrubber
{
    public static string Scrub(XContainer target, VerifySettings settings)
    {
        var counter = Counter.Current;
        var serialization = settings.serialization;
        var elements = target
            .Descendants()
            .ToList();

        foreach (var element in elements)
        {
            if (serialization.TryGetScrubOrIgnoreByName(element.Name.LocalName, out var scrubOrIgnore))
            {
                if (scrubOrIgnore == ScrubOrIgnore.Ignore)
                {
                    element.Remove();
                }
                else
                {
                    element.Value = "Scrubbed";
                }

                continue;
            }

            ScrubAttributes(element, serialization, settings, counter);
        }

        foreach (var node in target.DescendantNodes())
        {
            switch (node)
            {
                case XText text:
                    text.Value = ConvertValue(text.Value, settings, counter);
                    continue;
                case XComment comment:
                    comment.Value = ConvertValue(comment.Value, settings, counter);
                    continue;
            }
        }

        return target.ToString();
    }

    static string ConvertValue(string value, VerifySettings settings, Counter counter)
    {
        var span = value.AsSpan();
        if (counter.TryConvert(span, out var result))
        {
            return result;
        }

        return ApplyScrubbers.ApplyForPropertyValue(span, settings, counter).ToString();
    }

    static void ScrubAttributes(XElement node, SerializationSettings serialization, VerifySettings settings, Counter counter)
    {
        foreach (var attribute in node
                     .Attributes()
                     .ToList())
        {
            if (serialization.TryGetScrubOrIgnoreByName(attribute.Name.LocalName, out var scrubOrIgnore))
            {
                if (scrubOrIgnore == ScrubOrIgnore.Ignore)
                {
                    attribute.Remove();
                }
                else
                {
                    attribute.Value = "Scrubbed";
                }

                continue;
            }

            var span = attribute.Value.AsSpan();
            if (counter.TryConvert(span, out var result))
            {
                attribute.Value = result;
            }
            else
            {
                attribute.Value = ApplyScrubbers.ApplyForPropertyValue(span, settings, counter).ToString();
            }
        }
    }
}
