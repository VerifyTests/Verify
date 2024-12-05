# Ordering


## Order properties alphabetically

Serialized properties can optionally be ordering alphabetically, ie ignoring the order they are defined when using reflection.

snippet: OrderProperties


## Dictionary order

Dictionaries are ordering by key.

To disable use:

snippet: DontOrderDictionaries


## Json/JObject ordered

Json and JObject are not ordered.

To enable ordering use:

snippet: OrderJsonObjects


## Ordering IEnumerable items

Items in an instance of an IEnumerable can be ordered.

This is helpful when verifying items that can have an inconsistent order, for example reading items from a database.


### OrderEnumerableBy


#### Globally

snippet: OrderEnumerableByGlobal


#### Instance

snippet: OrderEnumerableBy


#### Fluent

snippet: OrderEnumerableByFluent


#### Result

The resulting file will be:

snippet: OrderTests.EnumerableOrder.verified.txt


### OrderEnumerableByDescending


#### Globally

snippet: OrderEnumerableByDescendingGlobal


#### Instance

snippet: OrderEnumerableByDescending


#### Fluent

snippet: OrderEnumerableByDescendingFluent


#### Result

The resulting file will be:

snippet: OrderTests.OrderEnumerableByDescending.verified.txt