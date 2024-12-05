# Obsolete members

Members with an [ObsoleteAttribute](https://docs.microsoft.com/en-us/dotnet/api/system.obsoleteattribute) are ignored:

snippet: WithObsoleteProp

Result:

snippet: SerializationTests.WithObsoleteProp.verified.txt


### Including Obsolete members

Obsolete members can be included using `IncludeObsoletes`:

snippet: WithObsoletePropIncluded

Or globally:

snippet: WithObsoletePropIncludedGlobally

Result:

snippet: SerializationTests.WithObsoletePropIncluded.verified.txt