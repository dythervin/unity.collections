using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using UnityEngine;
#if ODIN_INSPECTOR && UNITY_EDITOR
using Sirenix.OdinInspector;
#endif

namespace Dythervin.Collections
{
    [Serializable]
#if ODIN_INSPECTOR && UNITY_EDITOR
    [HideInPrefabAssets]
#endif
    public class SerializedHashSet<T> : ISerializationCallbackReceiver,
        ICollection<T>,
        IEnumerable<T>,
        IEnumerable,
        IReadOnlyCollection<T>,
        ISet<T>,
        IDeserializationCallback,
        ISerializable
    {
#region Fields

#if ODIN_INSPECTOR && UNITY_EDITOR
        [HideLabel]
#endif
        [SerializeField] protected T[] mValues = Array.Empty<T>();
        private readonly HashSet<T> _hashSet = new();

#endregion

#region Properties

        bool ICollection<T>.IsReadOnly => ((ICollection<T>)_hashSet).IsReadOnly;

        int ICollection<T>.Count => _hashSet.Count;

        int IReadOnlyCollection<T>.Count => _hashSet.Count;

#endregion

        public HashSet<T>.Enumerator GetEnumerator()
        {
            return _hashSet.GetEnumerator();
        }

#region ICollection<T>

        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            return _hashSet.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)_hashSet).GetEnumerator();
        }

        void ICollection<T>.Add(T item)
        {
            _hashSet.Add(item);
        }

        public void Clear()
        {
            _hashSet.Clear();
        }

        public bool Contains(T item)
        {
            return _hashSet.Contains(item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            _hashSet.CopyTo(array, arrayIndex);
        }

        public bool Remove(T item)
        {
            return _hashSet.Remove(item);
        }

#endregion

#region IDeserializationCallback

        public void OnDeserialization(object sender)
        {
            _hashSet.OnDeserialization(sender);
        }

#endregion

#region ISerializable

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            _hashSet.GetObjectData(info, context);
        }

#endregion

#region ISerializationCallbackReceiver

        void ISerializationCallbackReceiver.OnBeforeSerialize()
        {
            mValues = _hashSet.ToArray();
        }

        void ISerializationCallbackReceiver.OnAfterDeserialize()
        {
            _hashSet.Clear();
#if UNITY_2021_3_OR_NEWER
            _hashSet.EnsureCapacity(mValues.Length);
#endif
            foreach (T value in mValues)
            {
                _hashSet.Add(value);
            }
#if !UNITY_EDITOR
                mValues = null;
#endif
        }

#endregion

#region ISet<T>

        public void ExceptWith(IEnumerable<T> other)
        {
            _hashSet.ExceptWith(other);
        }

        public void IntersectWith(IEnumerable<T> other)
        {
            _hashSet.IntersectWith(other);
        }

        public bool IsProperSubsetOf(IEnumerable<T> other)
        {
            return _hashSet.IsProperSubsetOf(other);
        }

        public bool IsProperSupersetOf(IEnumerable<T> other)
        {
            return _hashSet.IsProperSupersetOf(other);
        }

        public bool IsSubsetOf(IEnumerable<T> other)
        {
            return _hashSet.IsSubsetOf(other);
        }

        public bool IsSupersetOf(IEnumerable<T> other)
        {
            return _hashSet.IsSupersetOf(other);
        }

        public bool Overlaps(IEnumerable<T> other)
        {
            return _hashSet.Overlaps(other);
        }

        public bool SetEquals(IEnumerable<T> other)
        {
            return _hashSet.SetEquals(other);
        }

        public void SymmetricExceptWith(IEnumerable<T> other)
        {
            _hashSet.SymmetricExceptWith(other);
        }

        public void UnionWith(IEnumerable<T> other)
        {
            _hashSet.UnionWith(other);
        }

        public bool Add(T item)
        {
            return _hashSet.Add(item);
        }

#endregion

#region Events handlers

        public static implicit operator HashSet<T>(SerializedHashSet<T> value)
        {
            return value._hashSet;
        }

#endregion
    }
}