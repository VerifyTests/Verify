[UsesVerify]
public class JsonTests
{
    [Fact]
    public Task JObjectOrdering1()
    {
        var obj = new JObject(
            new JProperty("@xmlns", 2),
            new JProperty("#text", 1)
        );

        return Verify(obj);
    }

    [Fact]
    public Task JTokenIgnore()
    {
        var jToken = JToken.Parse(
            """
            {
              Include: 1,
              Ignore: 2,
              "Memory Info": {
                      fragmentedBytes: 208,
                      heapSizeBytes: 2479536,
                      highMemoryLoadThresholdBytes: 30821986713,
                      memoryLoadBytes: 14041127280,
                      totalAvailableMemoryBytes: 34246651904
                    }
            }
            """);
        return Verify(jToken)
            .IgnoreMembers("Ignore", "Memory Info");
    }

    [Fact]
    public Task JTokenScrub()
    {
        var jToken = JToken.Parse(
            """
            {
              Include: 1,
              Scrub: 2,
              "Memory Info": {
                      fragmentedBytes: 208,
                      heapSizeBytes: 2479536,
                      highMemoryLoadThresholdBytes: 30821986713,
                      memoryLoadBytes: 14041127280,
                      totalAvailableMemoryBytes: 34246651904
                    }
            }
            """);
        return Verify(jToken)
            .ScrubMembers("Scrub", "Memory Info");
    }

    [Fact]
    public Task JObjectIgnore()
    {
        var obj = new JObject(
            new JProperty("Include", 2),
            new JProperty("Ignore", 1)
        );

        return Verify(obj)
            .IgnoreMember("Ignore");
    }

    [Fact]
    public Task JObjectOrdering2()
    {
        var obj = new JObject(
            new JProperty("#text", 1),
            new JProperty("@xmlns", 2)
        );

        return Verify(obj);
    }

    [Fact]
    public Task IgnoreJTokenByName()
    {
        var json = """
                   {
                     'short': {
                       'key': {
                         'code': 0,
                         'msg': 'No action taken'
                       },
                       'Ignore1': {
                         'code': 2,
                         'msg': 'ignore this'
                       }
                     }
                   }
                   """;
        var target = JToken.Parse(json);
        return Verify(target)
            .IgnoreMember("Ignore1");
    }

    [Fact]
    public Task ScrubJTokenByName()
    {
        var json = """
                   {
                     'short': {
                       'key': {
                         'code': 0,
                         'msg': 'No action taken'
                       },
                       'Scrub': {
                         'code': 2,
                         'msg': 'ignore this'
                       }
                     }
                   }
                   """;
        var target = JToken.Parse(json);
        return Verify(target)
            .ScrubMember("Scrub");
    }

    [Fact]
    public Task VerifyJsonGuid() =>
        VerifyJson("{'key': 'c572ff75-e1a2-49bd-99b9-4550697946c3'}");

    [Fact]
    public Task VerifyJsonEmpty() =>
        VerifyJson("{}");

    [Fact]
    public Task VerifyJsonRefRespectSerializerSettings() =>
        VerifyJson("{'$ref': '#/no/ref'}")
            .AddExtraSettings(s => s.MetadataPropertyHandling = MetadataPropertyHandling.Ignore);

    [Fact]
    public Task VerifyJsonTypeRespectSerializerSettings() =>
        VerifyJson("{ '$type': 'MyNamespace.User, MyAssembly'}")
            .AddExtraSettings(s => s.MetadataPropertyHandling = MetadataPropertyHandling.Ignore);

    [Fact]
    public Task VerifyJsonOnlyIgnoredMember() =>
        VerifyJson("{'key': 'value'}")
            .IgnoreMember("key");

    [Fact]
    public Task VerifyJsonDateTime()
    {
        var json = $"{{'key': '{DateTime.Now:yyyy-MM-ddTHH:mm:ss}'}}";
        return VerifyJson(json);
    }

    [Fact]
    public Task VerifyJsonWithArray()
    {
        var json = """
                   {
                       commitments: [
                         {
                           id: '9585dadf-551a-43eb-960c-18b935993cc3',
                           title: 'Commitment1'
                         }
                       ]
                   }
                   """;
        return VerifyJson(json);
    }

    [Fact]
    public Task VerifyJsonWithArrayAtRoot()
    {
        var json = """
                   [
                       {
                           id: '9585dadf-551a-43eb-960c-18b935993cc3',
                           title: 'Commitment1'
                       }
                   ]
                   """;
        return VerifyJson(json);
    }

    #region VerifyJson

    [Fact]
    public Task VerifyJsonString()
    {
        var json = "{'key': {'msg': 'No action taken'}}";
        return VerifyJson(json);
    }

    [Fact]
    public Task VerifyJsonStream()
    {
        var json = "{'key': {'msg': 'No action taken'}}";
        var stream = new MemoryStream(Encoding.UTF8.GetBytes(json));
        return VerifyJson(stream);
    }

    [Fact]
    public Task VerifyJsonJToken()
    {
        var json = "{'key': {'msg': 'No action taken'}}";
        var target = JToken.Parse(json);
        return Verify(target);
    }

    #endregion
}