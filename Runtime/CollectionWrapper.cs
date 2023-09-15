using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Dythervin.Collections
{
    [HideLabel]
    [Serializable]
    public class CollectionWrapper<TCollection, T> : ISerializationCallbackReceiver
        where TCollection : class, ICollection<T>, new()
    {
        // ReSharper disable once InconsistentNaming
        [SerializeReference] private T[] array;

        [NonSerialized]
        private TCollection collection;

        public CollectionWrapper()
        {
            collection = new TCollection();
        }

        public CollectionWrapper(TCollection collection)
        {
            this.collection = collection;
        }

        void ISerializationCallbackReceiver.OnBeforeSerialize()
        {
        }

        void ISerializationCallbackReceiver.OnAfterDeserialize()
        {
            if (array == null)
                return;

            collection.Clear();
            foreach (T t in array)
            {
                collection.Add(t);
            }
        }
    }
}