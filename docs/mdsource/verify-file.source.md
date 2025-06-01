# VerifyFile

Verifies the contents of a file.

snippet: VerifyFile


## Optional Info

An optional `info` parameter can be supplied to add more context to the test. The instance passed will be json serialized.

snippet: VerifyFileWithInfo


## Using a custom extension

snippet: VerifyFileExtension


## Verify a file without using a unit test

Use the functionality of VerifyTests outside of a unit test.

snippet: VerifyFileWithoutUnitTest

Result:

```
{targetDirectory}/sample.verified.txt
```


## Verify Files

Verify multiple files using file name as the name for the verified file:

snippet: VerifyFiles


### With info

snippet: VerifyFilesWithInfo


### With File Scrubber

snippet: VerifyFilesWithFileScrubber