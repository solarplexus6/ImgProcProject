//
//  GIMP plugin
//  Region Filling and Object Removal by Exemplar-Based Image Inpainting
//  Rafal Lukaszewski, 2013
//

using System;
using System.Collections.Generic;
using System.Linq;
using ExemplarInpaintingPlugin.Consts;
using ExemplarInpaintingPlugin.DataTerm;
using ExemplarInpaintingPlugin.DataTerm.Convolution;
using ExemplarInpaintingPlugin.Helpers;
using ExemplarInpaintingPlugin.PixelDistance;
using ExemplarInpaintingPlugin.SelectionHandling;
using Gimp;

namespace ExemplarInpaintingPlugin
{
    internal class Renderer : BaseRenderer
    {
        readonly IPixelDistanceCalculator _pixelDistanceCalc = new RgbDistanceCalculator();
        private readonly int _iterations;
        private readonly int _windowSize;
        #region Ctors

        public Renderer(VariableSet variables) : base(variables)
        {
            _iterations = GetValue<int>(VariablesConsts.ITERATIONS);
            _windowSize = GetValue<int>(VariablesConsts.WINDOW_SIZE);
        }

        #endregion
        #region Public methods

        public void Render(Image image, Drawable drawable)
        {
            if (image.Selection.Empty)
            {
                return;
            }
            image.UndoGroupStart();
            Tile.CacheDefault(drawable);            

            var temp = image.Duplicate();
            var imageRect = image.Bounds;

            var layerPixels = LayerToArray(imageRect, drawable);
            
            //temp.Selection.Border(1);
            bool selectionNonEmpty;
            var selectionBounds = temp.Selection.Bounds(out selectionNonEmpty);
            var selectionMask = new SelectionMask(temp.Selection);

            //var progress = new Progress(_("Filling the region..."));
            var initArea = selectionMask.Area;

            //init confidance term
            //TODO throw out rgniterator from here
            var pxConfidenceTerm = RegionHelper.FilledMatrix(image.Width, image.Height, 1.0);
            var iter = new RgnIterator(image.Selection, RunMode.Noninteractive);
            iter.IterateSrc(pixel => pxConfidenceTerm[pixel.X, pixel.Y] = selectionMask[pixel.X, pixel.Y] ? 0 : 1);

            int iteration = 0;
            while (!selectionMask.Empty && iteration < _iterations)
            {
                //  initial selection border / target region 
                var selectionBorder =
                    RegionHelper.TraceBorder(selectionBounds,
                                             coord => selectionMask[coord.X, coord.Y])
                                .ToList();

                var borderConfidanceValues = NewConfidanceValue(selectionBorder, pxConfidenceTerm, _windowSize);
                var borderGradientValues = GradientValues(selectionBorder, layerPixels, selectionMask, _windowSize).ToList();
                var borderNormals = NormalVectors.CalculateNormals(selectionBorder);
                var borderDataTerms = borderGradientValues.Zip(borderNormals, ComputeDataTerm);
                var borderPriorities = borderConfidanceValues.Zip(borderDataTerms, (c, d) => c * d)
                                                             .ToArray();

                var currentIndex = borderPriorities.MaxIndex();
                var currentPoint = selectionBorder[currentIndex];
                var currentRect = currentPoint.PointCenteredRectangle(_windowSize);                
                var currentRegion = new PixelRgn(drawable, currentRect, true, true);

                var minRgnRect = _pixelDistanceCalc.MinDistanceRegion(currentRect, borderGradientValues[currentIndex], layerPixels,
                                                                      selectionMask);
                var minRgn = new PixelRgn(drawable, minRgnRect, false, false);

                var iterator = new RegionIterator(currentRegion, minRgn);
                iterator.ForEach((targetPixel, srcPixel) =>
                    {
                        if (!selectionMask[targetPixel.X, targetPixel.Y])
                        {
                            return;
                        }
                        layerPixels[targetPixel.X, targetPixel.Y] = srcPixel;
                        targetPixel.Set(srcPixel);
                    });
                drawable.Flush();
                drawable.MergeShadow(true);
                drawable.Update(temp.Selection.MaskBounds);                

                //update confidance values
                var currentCoords =
                    RegionHelper.Grid(currentRect).Where(coord => selectionMask[coord.X, coord.Y]).ToArray();
                var newConfidanceValues = NewConfidanceValue(currentCoords, pxConfidenceTerm, _windowSize).ToArray();
                foreach (var point in
                    currentCoords.Zip(newConfidanceValues,
                                      (coordinate, confidance) =>
                                      new {Coordinate = coordinate, Confidance = confidance}))
                {
                    pxConfidenceTerm[point.Coordinate.X, point.Coordinate.Y] = point.Confidance;
                }
                // exclude current region pixels from selection
                selectionMask.SetAreaToZero(currentRect);
                //progress.Update((initArea - selectionMask.Area)/(double) initArea);

                iteration++;
                if (iteration == _iterations)
                {
                    selectionBorder =
                    RegionHelper.TraceBorder(selectionBounds,
                                             coord => selectionMask[coord.X, coord.Y])
                                .ToList();
                    var rgn = new PixelRgn(drawable, selectionBounds, true, true);
                    for (int i = 0; i < selectionBorder.Count; i++)
                    {
                        var coord = selectionBorder[i];
                        rgn.SetPixel(new byte[] { (byte)(255), 0, 0, 255 }, coord.X, coord.Y);
                    }

                    drawable.Flush();
                    drawable.MergeShadow(false);
                    drawable.Update(temp.Selection.MaskBounds);
                }                
            }            
            temp.Delete();
            image.UndoGroupEnd();
        }

        private double ComputeDataTerm(ImgGradient gradient, Coordinate<double> normal)
        {
            var ux = gradient.YMagnitude;
            var uy = -gradient.XMagnitude;
            return Math.Abs(ux*normal.Y - uy*normal.X);
        }

        private Pixel[,] LayerToArray(Rectangle imageRect, Drawable drawable)
        {
            var pixels = new Pixel[imageRect.Width, imageRect.Height];
            var rgn = new PixelRgn(drawable, imageRect, false, false);
            var iterator = new RegionIterator(rgn);
            iterator.ForEach(pixel =>
                                pixels[pixel.X, pixel.Y] = pixel);
            return pixels;
        }

        #endregion
        #region Private methods

        /// <summary>
        /// Update C(p) values for given points
        /// </summary>
        /// <param name="pxConfidenceTerm"></param>
        /// <param name="coordinates"></param>
        /// <param name="windowSize"></param>
        private static IEnumerable<double> NewConfidanceValue(IEnumerable<IntCoordinate> coordinates,
                                          double[,] pxConfidenceTerm,
                                          int windowSize)
        {
            return coordinates.Select(coord => NewConfidanceValue(coord, pxConfidenceTerm, windowSize));
        }

        /// <summary>
        /// Calculate new confidance value of pixel at given coordinate
        /// </summary>
        /// <param name="coordinate"></param>
        /// <param name="pxConfidenceTerm"></param>
        /// <param name="windowSize"></param>
        private static double NewConfidanceValue(IntCoordinate coordinate, double[,] pxConfidenceTerm, 
                                          int windowSize)
        {
            var imageRect = new Rectangle(new IntCoordinate(0, 0), pxConfidenceTerm.GetLength(0),
                                          pxConfidenceTerm.GetLength(1));
            //TODO: handle boundary regions properly
            return RegionHelper.Grid(coordinate.PointCenteredRectangle(windowSize))
                               .Select(coordPrim => imageRect.IsInside(coordPrim)
                                                        ? pxConfidenceTerm[coordPrim.X, coordPrim.Y]
                                                        : 0)
                               .Sum()/(windowSize*windowSize);
        }

        /// <summary>
        /// Calculate data terms for specified coordinates
        /// </summary>
        /// <param name="selectionBorder"></param>
        /// <param name="layerPixels"></param>
        /// <param name="windowSize"></param>
        /// <returns></returns>
        private static IEnumerable<ImgGradient> GradientValues(List<IntCoordinate> selectionBorder, Pixel[,] layerPixels, SelectionMask selection, int windowSize)
        {
            var convolver = new Convolver(ScharrKernel.Instance);
            foreach (var coord in selectionBorder)
            {
                ImgGradient max = null;
                foreach (var sourceCoord in
                    RegionHelper.Grid(coord.PointCenteredRectangle(windowSize))
                                .Where(neigbour => !selection[neigbour.X, neigbour.Y]))
                {
                    var grad = convolver.Convolve(sourceCoord, layerPixels,
                                                  pixel => (pixel.Red + pixel.Green + pixel.Blue)/(1.0*255.0));
                    if (max == null)
                    {
                        max = grad;
                    }
                    max = grad.Magnitude > max.Magnitude ? grad : max;
                }
                yield return max;
            }            
        }

        #endregion
    }
}