using System;
using System.Collections.Generic;
using Dythervin.Core;
using UnityEngine.Assertions;

namespace Dythervin.Collections
{
    public static class CollisionMatrixHelper
    {
        private static readonly Dictionary<Type, IReadOnlyList<string>> CustomNames =
            new Dictionary<Type, IReadOnlyList<string>>();

        public static void RegisterType(Type type, IReadOnlyList<string> readOnlyList)
        {
            CustomNames.Add(type, readOnlyList);
        }

        public static bool TryGetNames(Type type, out IReadOnlyList<string> readOnlyList)
        {
            return CustomNames.TryGetValue(type, out readOnlyList);
        }

        public static int GetRowLength(int length, int arrayIndex)
        {
            return length - arrayIndex;
        }

        public static int GetKeyIndex(this ICollisionMatrix matrix, int a, int b)
        {
            return GetKeyIndex(a, b, matrix.CappedKeyCount, matrix.SelfIntersect);
        }

        public static int GetRowLength(this ICollisionMatrix matrix, int arrayIndex)
        {
            return GetRowLength(matrix.CappedKeyCount, arrayIndex);
        }

        public static int GetKeyIndex(int a, int b, int length, bool selfIntersection)
        {
            Reorder(ref a, ref b, length);
            if (!selfIntersection)
            {
                Assert.IsTrue(a != b);
                b--;
            }

            return b + length * a - Triangulate(a);
        }

        public static int Triangulate(int i)
        {
            return (i * i + i) / 2;
        }

        public static int Untriangulate(int i)
        {
            Assert.IsTrue(i >= 0);
            int d = (int)Math.Sqrt(1 * 1 - 4 * 1 * -2 * i);
            return (-1 + d) / (2 * 1);
        }

        public static void Reorder(ref int a, ref int b, int length)
        {
            if (a > b)
            {
                (a, b) = (b, a);
            }

            Assert.IsTrue(a >= 0 && a < length);
            Assert.IsTrue(length >= b);
        }
    }
}