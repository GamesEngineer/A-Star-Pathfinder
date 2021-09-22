//#define ENABLE_VALIDATION
using System;
using System.Collections;
using System.Collections.Generic;

namespace GameU
{
    public interface IHeapItem
    {
        public int HeapPosition { get; set; }
    }

    public class MinHeap<Item> : ICollection<Item>
        where Item : IEquatable<Item>, IComparable<Item>, IHeapItem
    {
        private static int ParentIndex(int nodeIndex) => nodeIndex == 0 ? -1 : (nodeIndex - 1) / 2;
        private static int LeftChildIndex(int nodeIndex) => nodeIndex * 2 + 1;
        private static int RightChildIndex(int nodeIndex) => nodeIndex * 2 + 2;

        private readonly List<Item> items = new List<Item>();

        public int Count => items.Count;

        public bool IsReadOnly => false;

        public bool IsValid
        {
            get
            {
                for (int itemIndex = 1; itemIndex < items.Count; itemIndex++)
                {
                    int parentIndex = ParentIndex(itemIndex);
                    if (items[parentIndex].CompareTo(items[itemIndex]) > 0)
                        return false;
                }
                return true;
            }
        }

        public void Add(Item item) => Enqueue(item);

        public void Enqueue(Item item)
        {
            item.HeapPosition = items.Count;
            items.Add(item);
            SiftUp(item.HeapPosition);
        }

        public void Clear() => items.Clear();

        public bool Contains(Item item) => item.Equals(items[item.HeapPosition]);

        public int IndexOf(Item item) => item.HeapPosition;

        public void CopyTo(Item[] array, int arrayIndex) => items.CopyTo(array, arrayIndex);

        public IEnumerator<Item> GetEnumerator() => items.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => items.GetEnumerator();

        public Item Peek() => items.Count > 0 ? items[0] : default;

        public Item Dequeue()
        {
            if (items.Count <= 0) return default;
            Item top = Peek();
            Remove(top);
            return top;
        }

        public bool Remove(Item item) => RemoveAt(item.HeapPosition);

        public bool RemoveAt(int index)
        {
            if (index < 0 || index >= items.Count) return false;

            int lastIndex = items.Count - 1;
            Item last = items[lastIndex];
            items.RemoveAt(lastIndex);

            if (items.Count == 0) return true;

            last.HeapPosition = index;
            items[index] = last;
            SiftDown(index);

            return true;
        }

        public int Reprioritize(Item item)
        {
            int oldIndex = item.HeapPosition;
            int newIndex = SiftUp(oldIndex);
            if (newIndex == oldIndex)
            {
                newIndex = SiftDown(oldIndex);
            }
            return newIndex;
        }

        private void Swap(int indexA, int indexB)
        {
            Item a = items[indexA];
            Item b = items[indexB];
            a.HeapPosition = indexB;
            b.HeapPosition = indexA;
            items[indexA] = b;
            items[indexB] = a;
        }

        private int SiftUp(int itemIndex)
        {
            if (itemIndex < 0 || itemIndex >= items.Count)
                throw new IndexOutOfRangeException();

            int parentIndex = ParentIndex(itemIndex);
            while (parentIndex >= 0)
            {
                if (items[parentIndex].CompareTo(items[itemIndex]) <= 0) break;
                // move up
                Swap(parentIndex, itemIndex);
                itemIndex = parentIndex;
                parentIndex = ParentIndex(itemIndex);
            }

#if ENABLE_VALIDATION
            if (!IsValid) throw new Exception("Invalid Heap");
#endif

            return itemIndex;
        }

        private int SiftDown(int itemIndex)
        {
            if (itemIndex < 0 || itemIndex >= items.Count)
                throw new IndexOutOfRangeException();

            while (itemIndex < items.Count)
            {
                int leftChildIndex = LeftChildIndex(itemIndex);
                int rightChildIndex = RightChildIndex(itemIndex);
                if (leftChildIndex >= items.Count) break;

                // Select the child with the "lowest" comparison value
                int swapIndex = leftChildIndex;
                if (rightChildIndex < items.Count && items[leftChildIndex].CompareTo(items[rightChildIndex]) > 0)
                {
                    swapIndex = rightChildIndex;
                }

                if (items[itemIndex].CompareTo(items[swapIndex]) <= 0) break;
                Swap(itemIndex, swapIndex);
                itemIndex = swapIndex;
            }

#if ENABLE_VALIDATION
            if (!IsValid) throw new Exception("Invalid Heap");
#endif

            return itemIndex;
        }
    }
}
