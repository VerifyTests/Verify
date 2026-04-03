# Numeric Ids


## ScrubNumericIds

Opt in scrubbing of numeric properties ending in `Id` or `ID`. Each unique numeric value gets a stable counter based replacement, similar to [Guid](guids.md) and [Date](dates.md) scrubbing.

The counter is scoped per property name. For properties named `Id`, the declaring type name is used as the scope (e.g. `Customer_1`). For properties like `CustomerId` or `OrderId`, the full property name is the scope (e.g. `CustomerId_1`, `OrderId_1`). This ensures stable output regardless of the actual numeric values, which is particularly useful when working with auto-incrementing database ids.


### Fluent

snippet: ScrubNumericIdsFluent

Results in the following:

snippet: SerializationTests.ScrubNumericIdsFluent.verified.txt


### Instance

snippet: ScrubNumericIdsInstance


### Globally

snippet: ScrubNumericIdsGlobal


### Parent-child relationships

When verifying object graphs with parent-child relationships, each id property gets its own counter scope. Properties named `Id` use the declaring type as the scope, while foreign key properties like `CustomerId` and `OrderId` use the property name.

snippet: ScrubNumericIdsRelationships

Results in the following:

snippet: SerializationTests.ScrubNumericIdsNamedType.verified.txt

Note:

 * `Id` on `Customer` produces `Customer_1`, `Customer_2`
 * `Id` on `Order` produces `Order_1`, `Order_2`
 * `Id` on `OrderItem` produces `OrderItem_1`, `OrderItem_2`, `OrderItem_3`
 * `ProductId` is scoped independently, so the same product (id 7) is `ProductId_1` in both orders
 * `Quantity` is not scrubbed since it does not end in `Id`


## ScrubMembers approach

For more targeted control, `ScrubMembers` can be used to check the DeclaringType and the name of the member.

snippet: NumericIdSample

Produces

snippet: NumericIdSample.Test.verified.txt
