using System;
using System.Collections.Generic;
using System.Collections;

namespace FocusFounder.UI
{
    /// <summary>
    /// Observable collection for UI list binding
    /// </summary>
    public class ObservableCollection<T> : IEnumerable<T>
    {
        private readonly List<T> _items = new();

        public event Action<T> OnItemAdded;
        public event Action<T> OnItemRemoved;
        public event Action OnCollectionChanged;

        public int Count => _items.Count;
        public T this[int index] => _items[index];

        public void Add(T item)
        {
            _items.Add(item);
            OnItemAdded?.Invoke(item);
            OnCollectionChanged?.Invoke();
        }

        public bool Remove(T item)
        {
            if (_items.Remove(item))
            {
                OnItemRemoved?.Invoke(item);
                OnCollectionChanged?.Invoke();
                return true;
            }
            return false;
        }

        public void Clear()
        {
            var itemsToRemove = new List<T>(_items);
            _items.Clear();

            foreach (var item in itemsToRemove)
                OnItemRemoved?.Invoke(item);

            OnCollectionChanged?.Invoke();
        }

        public IEnumerator<T> GetEnumerator() => _items.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}