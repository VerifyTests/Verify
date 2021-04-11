using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using VerifyTests;

class JTokenConverter :
    WriteOnlyJsonConverter<JToken>
{
    List<string> ignoredByNameMembers;

    public JTokenConverter(List<string> ignoredByNameMembers)
    {
        this.ignoredByNameMembers = ignoredByNameMembers;
    }

    public override void WriteJson(JsonWriter writer, JToken? value, JsonSerializer serializer, IReadOnlyDictionary<string, object> context)
    {
        if (value is null)
        {
            return;
        }

        var dictionary = value.ToObject<Dictionary<string, object>>()!;
        var wrapper = new DictionaryWrapper<object, IDictionary<string, object>>(ignoredByNameMembers, dictionary);
        serializer.Serialize(writer, wrapper);
    }
}