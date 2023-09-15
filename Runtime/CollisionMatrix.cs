using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

namespace Dythervin.Collections
{
    public interface IReadOnlyCollisionMatrix
    {
#region Properties

        bool SelfIntersect { get; }

        int CappedKeyCount { get; }

        object this[int a, int b] { get; }

#endregion
    }

    public interface ICollisionMatrix : IReadOnlyCollisionMatrix
    {
#region Properties

        new object this[int a, int b] { get; set; }

#endregion
    }

    public interface IReadOnlyCollisionMatrix<out T> : IReadOnlyCollisionMatrix
    {
#region Properties

        IReadOnlyList<T> InternalArray { get; }

        int KeyCount { get; }

        new T this[int a, int b] { get; }

#endregion
    }

    public interface ICollisionMatrix<T> : IReadOnlyCollisionMatrix<T>, ICollisionMatrix
    {
#region Properties

        new T this[int a, int b] { get; set; }

#endregion
    }

    [Serializable]
    public class CollisionMatrix<T> : ICollisionMatrix<T>
    {
#region Fields

        private const bool SelfIntersectionDefault = false;
        [SerializeField] private T[] array;

        [SerializeField] [HideInInspector]
        private bool selfIntersect;

        private int _keyCount = -1;

#endregion

#region Properties

        public bool SelfIntersect
        {
            get => selfIntersect;
            set
            {
                if (selfIntersect == value)
                    return;

                selfIntersect = value;
                SetSize(CappedKeyCount, value);
            }
        }

        public int KeyCount
        {
            get
            {
                if (_keyCount != -1)
                    return _keyCount;

                if (array == null)
                    return 0;

                return _keyCount = CollisionMatrixHelper.Untriangulate(array.Length) + (selfIntersect ? 0 : 1);
            }
        }

        public int CappedKeyCount => KeyCount + (selfIntersect ? 0 : -1);

        public IReadOnlyList<T> InternalArray => array;

        public T this[int a, int b]
        {
            get => array[this.GetKeyIndex(a, b)];
            set => array[this.GetKeyIndex(a, b)] = value;
        }

        object IReadOnlyCollisionMatrix.this[int a, int b] => this[a, b];

        object ICollisionMatrix.this[int a, int b]
        {
            get => this[a, b];
            set => this[a, b] = (T)value;
        }

#endregion

#region Lifecycle

        public CollisionMatrix(bool selfIntersection = SelfIntersectionDefault)
        {
            selfIntersect = selfIntersection;
        }

        public CollisionMatrix([NotNull] IReadOnlyCollisionMatrix<T> matrix)
        {
            SetSize(matrix.KeyCount, matrix.SelfIntersect);
            CopyFrom(matrix);
        }

#endregion

        public void SetSize(int count)
        {
            SetSize(count, SelfIntersect);
        }

        public void SetSize(int count, bool selfIntersect)
        {
            Debug.Assert(count > 1);
            if (count == KeyCount && this.selfIntersect == selfIntersect)
                return;

            bool prevSelfIntersect = SelfIntersect;
            this.selfIntersect = selfIntersect;

            var prevArray = array;
            int prevCount = CappedKeyCount;
            _keyCount = count;

            array = new T[CollisionMatrixHelper.Triangulate(CappedKeyCount)];

            if (prevArray != null)
            {
                CopyFrom(prevArray, prevCount, prevSelfIntersect);
            }
        }

        public void CopyFrom(IReadOnlyCollisionMatrix<T> matrix)
        {
            CopyFrom(matrix.InternalArray, matrix.CappedKeyCount, matrix.SelfIntersect);
        }

        public void CopyFrom(IReadOnlyList<T> source, int sourceKeyCount, bool sourceSelfIntersect)
        {
            int max = Mathf.Min(sourceKeyCount, CappedKeyCount);
            int startIndex = SelfIntersect && sourceSelfIntersect ? 0 : 1;

            for (int a = 0; a < max; a++)
            {
                for (int b = a + startIndex; b < max + startIndex; b++)
                {
                    int rowIndex = CollisionMatrixHelper.GetKeyIndex(a, b, CappedKeyCount, SelfIntersect);
                    int sourceRowIndex = CollisionMatrixHelper.GetKeyIndex(a, b, sourceKeyCount, sourceSelfIntersect);

                    array[rowIndex] = source[sourceRowIndex];
                }
            }
        }
    }
}