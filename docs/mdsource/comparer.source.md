# Comparer

Comparers are used to compare non-text files.


## Custom Comparer

Using a custom comparer can be helpful when a result has changed, but not enough to fail verification. For example when rendering images/forms on different operating systems.

For samples purposes an [image difference hash algorithm](https://github.com/coenm/ImageHash#hash-algorithms) from the [ImageHash project](https://github.com/coenm/ImageHash) will be used:

snippet: ImageComparer


### Instance comparer

snippet: InstanceComparer


### Static comparer

snippet: StaticComparer


## Default Comparison

snippet: DefualtCompare