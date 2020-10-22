using System.Collections.Generic;
using System.Linq;

class DictionaryWrapper<TValue, TInner> : Dictionary<string, TValue>
    where TInner : IDictionary<string, TValue>
{
    public DictionaryWrapper(List<string> ignored, TInner inner) :
        base(inner.Where(x => !ignored.Contains(x.Key))
            .ToDictionary(x => x.Key, x => x.Value))
    {
    }
}