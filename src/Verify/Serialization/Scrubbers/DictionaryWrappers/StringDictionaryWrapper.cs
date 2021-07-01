﻿using System.Collections.Generic;
using System.Linq;

class StringDictionaryWrapper<TValue, TInner> :
    Dictionary<string, TValue>,
    IDictionaryWrapper
    where TInner : IDictionary<string, TValue>
{
    public StringDictionaryWrapper(List<string> ignored, TInner inner) :
        base(inner.Where(x => !ignored.Contains(x.Key))
            .ToDictionary(x => x.Key, x => x.Value))
    {
    }
}