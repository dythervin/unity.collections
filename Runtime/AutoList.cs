using System;
using System.Collections.Generic;
using Dythervin.Core.Extensions;
using UnityEngine;

namespace Dythervin.Collections
{
    [Serializable]
    public class AutoList<T> : MonoBehaviour, IIndexer<int, T>
        where T : Component
    {
        [SerializeField]
        private T prefab;

        private readonly List<T> _list;

        private AutoList() { }

        public AutoList(T prefab, int capacity)
        {
            this.prefab = prefab;
            _list = new List<T>(capacity);
        }

        public IReadOnlyList<T> List => _list;

        public T this[int index]
        {
            get
            {
                PopulateTo(index);
                return _list[index];
            }
            set
            {
                PopulateTo(index);
                _list[index] = value;
            }
        }

        public event Action<T> OnSpawn;


        public void DisableFrom(int index)
        {
            for (int i = index; i < _list.Count; i++)
            {
                _list[i].gameObject.SetActive(false);
            }
        }

        public void SetEnabled(int count, int startIndex = 0)
        {
            count += startIndex;
            for (int i = 0; i < _list.Count; i++)
            {
                _list[i].gameObject.SetActive(i >= startIndex && i < count);
            }
        }

        public void Trim(int startIndex)
        {
            while (_list.Count >= startIndex)
                Destroy(_list.PopLast());
        }

        public void PopulateTo(int index)
        {
            if (_list.Count > index)
                return;


            while (_list.Count <= index)
            {
                T newObj = Instantiate(prefab, transform);
                OnSpawn?.Invoke(newObj);
                _list.Add(newObj);
            }
        }
    }
}