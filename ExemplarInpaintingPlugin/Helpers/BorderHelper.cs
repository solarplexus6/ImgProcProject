using System;
using System.Collections.Generic;
using System.Linq;
using Gimp;

namespace ExemplarInpaintingPlugin.Helpers
{
    public class BorderHelper
    {
        #region Public static methods

        public static T[,] FilledMatrix<T>(int width, int height, T value)
        {
            var matrix = new T[width,height];
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    matrix[i, j] = value;
                }
            }
            return matrix;
        }

        public static IEnumerable<IntCoordinate> Grid(int width, int height)
        {
            return Enumerable.Range(0, width)
                             .SelectMany(
                                 w =>
                                 Enumerable.Repeat(w, height)
                                           .Zip(Enumerable.Range(0, height), (i, j) => new IntCoordinate(i, j)));
        }

        public static IEnumerable<IntCoordinate> Grid(int widthStart, int widthEnd, int heightStart, int heightEnd)
        {
            //TODO: reconsider arguments meaning
            return Enumerable.Range(widthStart, widthEnd - widthStart)
                             .SelectMany(
                                 w =>
                                 Enumerable.Repeat(w, heightEnd - heightStart)
                                           .Zip(Enumerable.Range(heightStart, heightEnd - heightStart), (i, j) => new IntCoordinate(i, j)));
        }

        public static IEnumerable<IntCoordinate> Grid(Rectangle bounds)
        {            
            //TODO: reconsider arguments meaning
            return Grid(bounds.X1, bounds.X2, bounds.Y1 + 1, bounds.Y2 + 1);
        }

        private static readonly IntCoordinate[] _mooreNeighborhood = new[]
            {
                new IntCoordinate(1, 0),
                new IntCoordinate(1, 1),
                new IntCoordinate(0, 1),
                new IntCoordinate(-1, 1),
                new IntCoordinate(-1, 0),
                new IntCoordinate(-1, -1),
                new IntCoordinate(0, -1),
                new IntCoordinate(1, -1),
            };

        /// <summary>
        /// Calculate border of binary image using Radial Sweep algorithm
        /// </summary>
        /// <param name="isBlack">Function by which algorithm checks if given point is in the pattern</param>
        /// <param name="bounds">Bounding box for border tracing</param>
        /// <returns>List of boundary pixels</returns>     
        public static List<IntCoordinate> TraceBorder(Rectangle bounds, Func<IntCoordinate, bool> isBlack)
        {
            //http://www.imageprocessingplace.com/downloads_V3/root_downloads/tutorials/contour_tracing_Abeer_George_Ghuneim/ray.html            

            const int N_DIRS = 8;
            var border = new List<IntCoordinate>();

            var starting = Grid(bounds).FirstOrDefault(isBlack);
            if (starting == null)
            {
                return border;
            }

            border.Add(starting);            

            Func<int, IntCoordinate, IntCoordinate> neighbor = (i, coordinate) =>
                {
                    var delta = _mooreNeighborhood[i%N_DIRS];
                    return coordinate + delta;
                };
            Func<int, IntCoordinate, Tuple<int, IntCoordinate>> findNextNeighbor = (lastDir, coord) =>
                Enumerable.Range(lastDir % 2 == 0 ? (lastDir + 7) % N_DIRS : (lastDir + 6) % N_DIRS, N_DIRS)
                          .Select(i => new Tuple<int, IntCoordinate>(i % N_DIRS, neighbor(i, coord)))
                          .FirstOrDefault(tuple =>
                              bounds.X1 <= tuple.Item2.X && tuple.Item2.X <= bounds.X2
                              && bounds.Y1 <= tuple.Item2.Y && tuple.Item2.Y <= bounds.Y2
                              && isBlack(tuple.Item2));
            var step = findNextNeighbor(0, starting);
            if (step == null)
            {
                return border;
            }
            var dir = step.Item1;
            var boundary = step.Item2;            
            //TODO: Stopping Criterion 2 
            while (boundary != starting)
            {
                border.Add(boundary);
                step = findNextNeighbor(dir, boundary);
                if (step == null)
                {
                    break;
                }
                dir = step.Item1;
                boundary = step.Item2;
            }

            return border;
        }

        #endregion
    }
}