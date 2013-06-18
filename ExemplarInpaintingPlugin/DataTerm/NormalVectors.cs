using System;
using System.Collections.Generic;
using Gimp;

namespace ExemplarInpaintingPlugin.DataTerm
{
    public class NormalVectors
    {
        #region Public static methods

        public static IEnumerable<Coordinate<double>> CalculateNormals(List<IntCoordinate> selectionBorder)
        {
            if (selectionBorder.Count == 1)
            {
                yield return new Coordinate<double>(1, 0);
                yield break;
            }
            if (selectionBorder.Count == 2)
            {
                var normal = Normal(selectionBorder[0], selectionBorder[1]);
                yield return normal;
                yield return normal;
                yield break;
            }

            //calculate normal based on one preceding and one successive point in the list
            yield return Normal(selectionBorder[0], selectionBorder[1]);
            for (int i = 1; i < selectionBorder.Count - 1; i++)
            {
                yield return Normal(selectionBorder[i - 1], selectionBorder[i + 1]);
            }
            yield return Normal(selectionBorder[selectionBorder.Count - 2], selectionBorder[selectionBorder.Count - 1]);
        }

        /// <summary>
        ///     Left hand normal
        /// </summary>
        /// <param name="c1"></param>
        /// <param name="c2"></param>
        /// <returns></returns>
        public static Coordinate<double> Normal(IntCoordinate c1, IntCoordinate c2)
        {
            var normal = new Coordinate<double>(c1.Y - c2.Y, -(c1.X - c2.X));
            var mag = Math.Sqrt(normal.X*normal.X + normal.Y*normal.Y);
            normal.X = normal.X/mag;
            normal.Y = normal.Y/mag;

            return normal;
        }

        #endregion
    }
}