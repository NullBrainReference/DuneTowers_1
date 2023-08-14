using System;
using System.Collections.Generic;
using System.Linq;

namespace AI.A_Star
{
    internal interface IBinaryHeap<in TKey, T>
    {
        void Enqueue(T item);
        T Dequeue();
        void Clear();
        bool TryGet(TKey key, out T value);
        void Modify(T value);
        int Count { get; }
    }

    internal class BinaryHeap<TKey, T> : IBinaryHeap<TKey, T>
        where TKey : IEquatable<TKey>
        where T : IComparable<T>
    {
        private readonly IDictionary<TKey, int> map;
        private readonly IList<T> collection;
        private readonly Func<T, TKey> lookupFunc;

        public BinaryHeap(Func<T, TKey> lookupFunc, int capacity = 0)
        {
            this.lookupFunc = lookupFunc;
            collection = new List<T>(capacity);
            map = new Dictionary<TKey, int>(capacity);
        }

        public int Count => collection.Count;

        public void Enqueue(T item)
        {
            collection.Add(item);
            int i = collection.Count - 1;
            map[lookupFunc(item)] = i;
            while (i > 0)
            {
                int j = (i - 1) / 2;

                if (collection[i].CompareTo(collection[j]) > 0)
                    break;

                Swap(i, j);
                i = j;
            }
        }

        public T Dequeue()
        {
            if (collection.Count == 0) return default;

            T result = collection.First();
            RemoveRoot();
            map.Remove(lookupFunc(result));
            return result;
        }

        public void Clear()
        {
            collection.Clear();
            map.Clear();
        }

        public bool TryGet(TKey key, out T value)
        {
            if (!map.TryGetValue(key, out int index))
            {
                value = default;
                return false;
            }

            value = collection[index];
            return true;
        }

        public void Modify(T value)
        {
            if (!map.TryGetValue(lookupFunc(value), out int index))
                throw new KeyNotFoundException(nameof(value));

            collection[index] = value;
        }

        private void RemoveRoot()
        {
            collection[0] = collection.Last();
            map[lookupFunc(collection[0])] = 0;
            collection.RemoveAt(collection.Count - 1);

            var i = 0;
            while (true)
            {
                int largest = LargestIndex(i);
                if (largest == i)
                    return;

                Swap(i, largest);
                i = largest;
            }
        }

        private void Swap(int i, int j)
        {
            T temp = collection[i];
            collection[i] = collection[j];
            collection[j] = temp;
            map[lookupFunc(collection[i])] = i;
            map[lookupFunc(collection[j])] = j;
        }

        private int LargestIndex(int i)
        {
            int leftInd = 2 * i + 1;
            int rightInd = 2 * i + 2;
            int largest = i;

            if (leftInd < collection.Count && collection[leftInd].CompareTo(collection[largest]) < 0) largest = leftInd;

            if (rightInd < collection.Count && collection[rightInd].CompareTo(collection[largest]) < 0) largest = rightInd;

            return largest;
        }
    }
}