Parameters can be hashed as an alternative to being stringified. This is useful when the parameters are large and could potentially generate file names that exceed allowances of the OS.

Hashing parameter is achieved by using `HashParameters`.

[Overriding text used for parameters](#overriding-text-used-for-parameters) is respected when generating the hash.

[XxHash64](https://learn.microsoft.com/en-us/dotnet/api/system.io.hashing.xxhash64) is used to perform the hash.