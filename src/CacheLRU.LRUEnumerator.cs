using System;
using System.Collections;
using System.Collections.Generic;

namespace CacheLRU
{
    public partial class CacheLRU<Tkey, Tvalue>
    {
        struct LRUEnumerator : IEnumerator<KeyValuePair<Tkey, Tvalue>>
        {
            Dictionary<Tkey, LinkedListNode<KeyValuePair<Tkey, Tvalue>>>.Enumerator enumerator;

            public LRUEnumerator(Dictionary<Tkey, LinkedListNode<KeyValuePair<Tkey, Tvalue>>>.Enumerator to_wrap)
            {
                enumerator = to_wrap;
            }

            public KeyValuePair<Tkey, Tvalue> Current => enumerator.Current.Value.Value;

            object IEnumerator.Current => Current;

            public void Dispose()
            {
                enumerator.Dispose();
            }

            public bool MoveNext()
            {
                return enumerator.MoveNext();
            }

            public void Reset()
            {
                throw new NotSupportedException();
            }
        }

    }

}
