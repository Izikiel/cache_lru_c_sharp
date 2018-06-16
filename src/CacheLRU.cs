using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace CacheLRU
{
    public class CacheLRU<Tkey, Tvalue> : IDictionary<Tkey, Tvalue>
    {
        readonly int capacity;
        int count = 0;
        readonly LinkedList<KeyValuePair<Tkey, Tvalue>> lru_order = new LinkedList<KeyValuePair<Tkey, Tvalue>>();
        readonly Dictionary<Tkey, LinkedListNode<KeyValuePair<Tkey, Tvalue>>> dict = new Dictionary<Tkey, LinkedListNode<KeyValuePair<Tkey, Tvalue>>>();

        public CacheLRU(int capacity)
        {
            if (capacity <= 0)
            {
                throw new ArgumentOutOfRangeException("capacity cannot be 0 or less");
            }

            this.capacity = capacity;
        }

        public Tvalue this[Tkey key]
        {
            get => Dict[key].Value.Value;
            set => Update(key, value);
        }

        public void Update(Tkey key, Tvalue value)
        {
            LinkedListNode<KeyValuePair<Tkey, Tvalue>> node;
            var kv = new KeyValuePair<Tkey, Tvalue>(key, value);

            if (Remove(key, out node))
            {
                node.Value = kv;
            }
            else
            {
                node = new LinkedListNode<KeyValuePair<Tkey, Tvalue>>(kv);
            }
            Add(node);
        }

        public ICollection<Tkey> Keys => this.Dict.Keys;

        public ICollection<Tvalue> Values => (ICollection<Tvalue>)from node in Dict.Values
                                                                  select node.Value.Value;

        public int Count => count;
        public bool Contains(KeyValuePair<Tkey, Tvalue> item) => Dict.ContainsKey(item.Key) &&
                                                                 Dict[item.Key].Value.Value.Equals(item.Value);

        public bool IsReadOnly => false;

        private Dictionary<Tkey, LinkedListNode<KeyValuePair<Tkey, Tvalue>>> Dict { get => dict; }
        private LinkedList<KeyValuePair<Tkey, Tvalue>> Lru_order { get => lru_order; }

        public void Add(Tkey key, Tvalue value)
        {
            if (Dict.ContainsKey(key))
            {
                throw new ArgumentException("Element already in cache");
            }
            var pair = new KeyValuePair<Tkey, Tvalue>(key, value);
            var node = new LinkedListNode<KeyValuePair<Tkey, Tvalue>>(pair);
            Add(node);

        }

        public void Add(KeyValuePair<Tkey, Tvalue> item)
        {
            if (Contains(item))
            {
                throw new ArgumentException("Element already in cache");
            }
            Update(item.Key, item.Value);
        }

        private void Add(LinkedListNode<KeyValuePair<Tkey, Tvalue>> node)
        {
            if (count == capacity)
            {
                RemoveLRU();
            }

            Dict.Add(node.Value.Key, node);
            Lru_order.AddLast(node);
            count++;
        }

        public void Clear()
        {
            count = 0;
            Lru_order.Clear();
            Dict.Clear();
        }


        public bool ContainsKey(Tkey key) => Dict.ContainsKey(key);


        public bool Remove(Tkey key)
        {
            LinkedListNode<KeyValuePair<Tkey, Tvalue>> node;
            return Remove(key, out node);
        }

        private bool Remove(Tkey key, out LinkedListNode<KeyValuePair<Tkey, Tvalue>> value)
        {
            var removed = false;
            LinkedListNode<KeyValuePair<Tkey, Tvalue>> node = default;
            if (Dict.Remove(key, out node))
            {
                Lru_order.Remove(node);
                count--;
                removed = true;
            }
            value = node;
            return removed;
        }

        public bool Remove(Tkey key, out Tvalue value)
        {
            LinkedListNode<KeyValuePair<Tkey, Tvalue>> node;
            if (Remove(key, out node))
            {
                value = node.Value.Value;
                return true;
            }

            value = default;
            return false;
        }

        public bool Remove(KeyValuePair<Tkey, Tvalue> item)
        {
            if (Contains(item))
            {
                return Remove(item.Key);
            }
            return false;
        }

        private void RemoveLRU()
        {
            if (count == 0)
            {
                return;
            }

            Dict.Remove(Lru_order.First.Value.Key);
            Lru_order.RemoveFirst();
            count--;
        }

        public bool TryGetValue(Tkey key, out Tvalue value)
        {
            LinkedListNode<KeyValuePair<Tkey, Tvalue>> node;
            value = default;
            if (Dict.TryGetValue(key, out node))
            {
                value = node.Value.Value;
                return true;
            }
            value = default;
            return false;
        }

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

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public IEnumerator<KeyValuePair<Tkey, Tvalue>> GetEnumerator() => new LRUEnumerator(Dict.GetEnumerator());

        public void CopyTo(KeyValuePair<Tkey, Tvalue>[] array, int arrayIndex) {
            var i = 0;

            foreach (var key in Keys)
            {
                array[i+arrayIndex] = new KeyValuePair<Tkey, Tvalue>(key, Dict[key].Value.Value);
                i++;
            }
        }

    }

}
