## Overriding text used for parameters

`UseTextForParameters()` can be used to override the substitution text used for the `{Parameters}` part of the file convention.

```
{Directory}/{TestClassName}.{TestMethodName}_{Parameters}_{UniqueFor1}_{UniqueFor2}_{UniqueForX}.verified.{extension}
```

The below samples produce:

For the instance case:

 * TheTest.UseTextForParameters_Value1.verified.txt
 * TheTest.UseTextForParameters_Value2.verified.txt

For the fluent case:

 * TheTest.UseTextForParametersFluent_Value1.verified.txt
 * TheTest.UseTextForParametersFluent_Value2.verified.txt