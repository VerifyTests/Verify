Verify uses the [PureAttribute](https://learn.microsoft.com/en-us/dotnet/api/system.diagnostics.contracts.pureattribute) to mark methods where the result of the method is expected to be used. For example awaiting the call to `Verify()`.
Rider and ReSharper can be configured to treat the return value of these methods as an error.
Add the following to the `.editorconfig` file:

```
[*.cs]
resharper_return_value_of_pure_method_is_not_used_highlighting = error
```