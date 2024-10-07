## File name suffix

Every parameterised case should have a unique [file name](/docs/naming.md) with the parameters appended to the file name. This happens automatically for NUnit; xUnit and MSTest require the use of `UseParameters()` (see below).

The appending format is `_ParamName=ParamValue` repeated for each parameter. 

A test with two parameters `param1` + `param2`, and called twice with the values `value1a` + `value2a` and `value1b` + `value2b` would have the following file names:

  * `MyTest.MyMethod_param1=value1a_param2=value2a.verified.txt`
  * `MyTest.MyMethod_param1=value1b_param2=value2b.verified.txt`


### Invalid characters

Characters that cannot be used for a file name are replaced with a dash (`-`).
