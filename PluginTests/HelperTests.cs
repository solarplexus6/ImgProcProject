//
//  GIMP plugin
//  Region Filling and Object Removal by Exemplar-Based Image Inpainting
//  Rafal Lukaszewski, 2013
//

using System.Collections.Generic;
using System.Linq;
using ExemplarInpaintingPlugin.Helpers;
using Gimp;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace PluginTests
{
    [TestClass]
    public class HelperTests
    {
        #region Public methods

        [TestMethod]
        public void TestGrid()
        {
            const int WIDTH = 3;
            const int HEIGHT = 3;
            var grid = RegionHelper.Grid(WIDTH, HEIGHT).ToList();

            var expected = new List<IntCoordinate>
                {
                    new IntCoordinate(0, 0),
                    new IntCoordinate(0, 1),
                    new IntCoordinate(0, 2),
                    new IntCoordinate(1, 0),
                    new IntCoordinate(1, 1),
                    new IntCoordinate(1, 2),
                    new IntCoordinate(2, 0),
                    new IntCoordinate(2, 1),
                    new IntCoordinate(2, 2),
                };

            CollectionAssert.AreEqual(expected, grid);
        }

        [TestMethod]
        public void TestGridRegion()
        {
            const int WIDTH_START = 1;
            const int HEIGHT_START = 1;
            const int WIDTH_END = 3;
            const int HEIGHT_END = 3;
            var grid = RegionHelper.Grid(WIDTH_START, WIDTH_END, HEIGHT_START, HEIGHT_END).ToList();

            var expected = new List<IntCoordinate>
                {
                    new IntCoordinate(1, 1),
                    new IntCoordinate(1, 2),
                    new IntCoordinate(2, 1),
                    new IntCoordinate(2, 2),
                };

            CollectionAssert.AreEqual(expected, grid.ToList());

            grid = RegionHelper.Grid(new Rectangle(WIDTH_START, HEIGHT_START, WIDTH_END, HEIGHT_END)).ToList();
            CollectionAssert.AreEqual(expected, grid.ToList());
        }

        [TestMethod]
        public void TraceBorder()
        {
            var testImage = new[,]
                {
                    {0, 0, 1, 0, 0},
                    {0, 0, 1, 1, 0},
                    {0, 1, 1, 1, 1},
                    {0, 1, 1, 1, 0},
                    {0, 0, 1, 0, 0},
                };
            var border = RegionHelper.TraceBorder(new Rectangle(0, 0, 4, 4),
                                                  coordinate => testImage[coordinate.Y, coordinate.X] != 0);
            var expected = RegionHelper.Grid(5, 5)
                                       .Where(e => testImage[e.Y, e.X] != 0)
                                       .Except(new[]
                                           {new IntCoordinate(2, 2), new IntCoordinate(3, 2), new IntCoordinate(2, 3),})
                                       .ToList();
            CollectionAssert.AreEquivalent(expected, border);
        }

        #endregion
    }
}