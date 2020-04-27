# Comparer

Comparers are used to compare non-text files.


## Custom Comparer

Using a custom comparer can be helpful when a result has changed, but not enough to fail verification. For example when rendering images/forms on different operating systems.

For samples purposes an image difference hash algorithm from the [ImageHash project](https://github.com/pgrho/phash) will be used:

snippet: ImageComparer

The returned `CompareResult.NotEqual` takes an optional message that will be rendered in the resulting text displayed to the user on test failure.


### Instance comparer

snippet: InstanceComparer


### Static comparer

snippet: StaticComparer


## Default Comparison

snippet: DefualtCompare