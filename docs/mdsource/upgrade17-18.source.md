# Upgrading from Version 17 to 18


## Move to Argon

Verify needed some custom serialization APIs that do not exist in Newtonsoft.Json. [Argon](https://github.com/SimonCropp/Argon) is a hard fork of Newtonsoft.Json.

If verifying Newtonsoft.Json types (eg `JObject` or `JArray`), move over to using https://github.com/VerifyTests/Verify.NewtonsoftJson


## UseExtension() remove

The `UseExtension()` fluent API has been removed and replaced with an explicit override for the subset of `Verify()` methods that support using an extension. 

Custom overloads:

 * `Verify(string? target, string extension)`
 * `Verify(Stream? target, string extension)`
 * `Verify(byte[]? target, string extension)`

`Task<T>` based overrides also exist.


### VerifySettings case

Before:

```
var settings = new VerifySettings(classLevelSettings);
settings.UseExtension("xml");
//Some other settings
await Verify(@"<note></note>", settings);
```

After:

```
var settings = new VerifySettings(classLevelSettings);
//Some other settings
await Verify(@"<note></note>", "xml");
```


### Fluent case

Before:

```
await Verify(@"<note></note>")
    //Some other settings
    .UseExtension("xml");
```

After:

```
await Verify(@"<note></note>", "xml")
    //Some other settings
    ;
```


### FileStream

`Verify(FileStream? target)` has been added which derives the extension from the `FileStream` instance.


## VerifierSettings.DerivePathInfo moved

https://github.com/VerifyTests/Verify/pull/646

The implementation and API of `DerivePathInfo` differs by the testing framework. As such the API has been moved to be specific packages that support it:

 * `Verifier.DerivePathInfo` for Verify.XUnit, Verify.Expecto, and Verify.NUnit.
 * `VerifierBase.DerivePathInfo` for Verify.MSTest.


## Add received text to OnFirstVerify

https://github.com/VerifyTests/Verify/pull/609

Before:

```
VerifierSettings.OnFirstVerify(
    receivedFile =>
    {
        Debug.WriteLine(receivedFile);
        return Task.CompletedTask;
    });
```

After:

```
VerifierSettings.OnFirstVerify(
    (receivedFile, receivedText) =>
    {
        Debug.WriteLine(receivedFile);
        Debug.WriteLine(receivedText);
        return Task.CompletedTask;
    });
```


## Use `#` to delineate name/index

https://github.com/VerifyTests/Verify/pull/637

Previously index + name were appended to the prefix. This was often confusing.

Given the following:

 * `Test.Method.00.verified.txt`
 * `Test.Method.01.verified.png`
 * `Test.Method.02.name1.verified.png`
 * `Test.Method.03.name1.verified.html`
 * `Test.Method.04.name2.verified.png`
 * `Test.Method.05.name2.verified.html`

Will now become:

 * `Test.Method.verified.txt`
 * `Test.Method.verified.png`
 * `Test.Method#name1.verified.png`
 * `Test.Method#name1.verified.html`
 * `Test.Method#name2.verified.png`
 * `Test.Method#name2.verified.html`

Given the following:

 * `Test.Method.00.verified.txt`
 * `Test.Method.01.verified.txt`

Will now become:

 * `Test.Method#01.verified.txt`
 * `Test.Method#02.verified.txt`

This rename should be automatically handled when a pending change is accepted.


## Dictionary order

Previously no order was applied to `IDictionary` members. This proved problematic since the order is not guaranteed.

`IDictionary` members are now ordered based on key.


## Date changes

 * Trailing zeros in time will be ignored
 * DateTime offset will be replaced with either `Local` or `Utc`. `Unspecified` will not be included



### In Json


#### DateTime Before

```
2020-01-01T00:00:00+1
2020-01-01T01:01:00+0
```


#### DateTime After

```
2020-01-01T00:00:00 Local
2020-01-01 01:01 Utc
```


#### DateTimeOffset Before

```
2020-01-01T00:00:00+1
2020-01-01T01:01:00+0
```


#### DateTimeOffset After

```
2020-01-01 +1
2020-01-01 01:01 +0
```


### In Parameters

If affected by this change, the new verified file will need to be accpted and the old verified files will need to be manually cleaned up. 

#### DateTime Before

```
2020-01-01T00-00-00+11
2020-01-01T01-01-00+0
```


#### DateTime After

```
2020-01-01Local
2020-01-01T01-01Local
```


#### DateTimeOffset Before

```
2020-01-01T00-00-00+1
2020-01-01T01-01-00+0
```


#### DateTimeOffset After

```
2020-01-01+1
2020-01-01T01-01Utc
```
