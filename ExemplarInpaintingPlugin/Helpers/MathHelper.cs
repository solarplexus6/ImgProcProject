using System;
using System.Collections.Generic;

namespace ExemplarInpaintingPlugin.Helpers
{
    public static class MathHelper
    {
        #region Public static methods

        public static int MaxIndex<T>(this IEnumerable<T> sequence)
            where T : IComparable<T>
        {
            var maxIndex = -1;
            var maxValue = default(T); // Immediately overwritten anyway

            var index = 0;
            foreach (var value in sequence)
            {
                if (value.CompareTo(maxValue) > 0 || maxIndex == -1)
                {
                    maxIndex = index;
                    maxValue = value;
                }
                index++;
            }
            return maxIndex;
        }

        public static T[,] Transposed<T>(this T[,] matrix)
        {
            int w = matrix.GetLength(0);
            int h = matrix.GetLength(1);
            var transposed = new T[w,h];
            for (int i = 0; i < w; i++)
            {
                for (int j = 0; j < h; j++)
                {
                    transposed[j, i] = transposed[i, j];
                }
            }
            return transposed;
        }

        #endregion
    }
}