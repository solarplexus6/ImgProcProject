//
//  GIMP plugin
//  Region Filling and Object Removal by Exemplar-Based Image Inpainting
//  Rafal Lukaszewski, 2013
//

using System;
using System.Diagnostics;
using ExemplarInpaintingPlugin.DataTerm.Convolution;
using ExemplarInpaintingPlugin.Helpers;
using ExemplarInpaintingPlugin.SelectionHandling;
using Gimp;

namespace ExemplarInpaintingPlugin.PixelDistance
{
    public class RgbDistanceCalculator : IPixelDistanceCalculator
    {
        #region IPixelDistanceCalculator Members

        public Rectangle MinDistanceRegion(Rectangle sourceRect, Drawable drawable, SelectionMask selectionMask)
        {
            Debug.Assert(sourceRect.Width == sourceRect.Height);

            var windowSize = sourceRect.Width;
            var windowDelta = windowSize/2;
            Rectangle minRegionRect = null;

            var minDistance = 0.0;

            var sourceRgn = new PixelRgn(drawable, sourceRect.AdjustedToImageEdges(drawable.Bounds), false, false);
            var sourceArray = sourceRgn.ToArrayWithoutSelection(selectionMask);

            var currentCoordinate = new IntCoordinate(0, 0);
            for (int i = windowDelta; i < drawable.Width - windowDelta; i++)
            {
                for (int j = windowDelta; j < drawable.Height - windowDelta; j++)
                {
                    if (j == sourceRect.Y1 + windowDelta && i == sourceRect.X1 + windowDelta)
                    {
                        continue;
                    }
                    currentCoordinate.X = i;
                    currentCoordinate.Y = j;
                    var currentRgnRect = currentCoordinate.PointCenteredRectangle(windowSize);
                    var dist = RegionDistance(sourceArray, currentRgnRect, drawable, selectionMask);
                    if (dist == null)
                    {
                        continue;
                    }
                    if (dist == 0)
                    {
                        return currentRgnRect;
                    }
                    if ((minDistance > dist || minRegionRect == null))
                    {
                        minDistance = (double) dist;
                        minRegionRect = currentRgnRect;
                    }
                }
            }

            return minRegionRect;
        }

        public Rectangle MinDistanceRegion(Rectangle sourceRect, ImgGradient gradient, Pixel[,] layerPixels, SelectionMask selectionMask)
        {
            Debug.Assert(sourceRect.Width == sourceRect.Height);

            var windowSize = sourceRect.Width;
            var windowDelta = windowSize/2;
            var layerWidth = layerPixels.GetLength(0);
            var layerHeight = layerPixels.GetLength(1);

            Rectangle minRegionRect = null;
            var minDistance = 0.0;
            var adjustedSourceRect = sourceRect.AdjustedToImageEdges(new Rectangle(new IntCoordinate(0, 0),
                                                                                   layerWidth, layerHeight));
            var currentCoordinate = new IntCoordinate(0, 0);
            var orthoGradient = new Coordinate<double>(-gradient.YMagnitude*windowDelta,
                                                       gradient.XMagnitude*windowDelta);
            for (int i = windowDelta; i < layerWidth - windowDelta; i++)
            {
                for (int j = windowDelta; j < layerHeight - windowDelta; j++)
                {
                    if (j == sourceRect.Y1 + windowDelta && i == sourceRect.X1 + windowDelta)
                    {
                        continue;
                    }
                    currentCoordinate.X = i;
                    currentCoordinate.Y = j;
                    var currentRgnRect = currentCoordinate.PointCenteredRectangle(windowSize);
                    var dist = RegionDistance(adjustedSourceRect, currentRgnRect, layerPixels, selectionMask);
                    if (dist == null)
                    {
                        continue;
                    }
                    if (dist == 0)
                    {
                        return currentRgnRect;
                    }
                    if ((minDistance > dist || minRegionRect == null))
                    {                        
                        minDistance = (double) dist;
                        minRegionRect = currentRgnRect;
                    }
                }
            }

            return minRegionRect;
        }

        public double? RegionDistance(Rectangle sourceRect, Rectangle rgnRect, Pixel[,] layerPixels,
                                      SelectionMask selectionMask)
        {
            var sum = 0.0;
            var sourceWidth = sourceRect.Width;
            var sourceHeight = sourceRect.Height;

            for (int y1 = sourceRect.Y1, y2 = rgnRect.Y1;
                 y1 < sourceRect.Y1 + sourceHeight;
                 y1++, y2++)
            {
                for (int x1 = sourceRect.X1, x2 = rgnRect.X1;
                     x1 < sourceRect.X1 + sourceWidth;
                     x1++, x2++)
                {
                    //if there's selection pixel inside of examined region mark region as useless
                    if (selectionMask[x2, y2])
                    {
                        return null;
                    }
                    //if it's pixel from target region don't examine it
                    if (selectionMask[x1, y1])
                    {
                        continue;
                    }

                    sum += DistanceSqr(layerPixels[x1, y1], layerPixels[x2, y2]);
                }
            }

            return sum;
        }

        public double DistanceSqr(Pixel p1, Pixel p2)
        {
            return Math.Pow(p1.Red - p2.Red, 2) +
                   Math.Pow(p1.Green - p2.Green, 2) +
                   Math.Pow(p1.Blue - p2.Blue, 2);
        }

        public double? RegionDistance(Pixel[,] source, Rectangle rgnRect, Drawable drawable, SelectionMask selectionMask)
        {
            var sum = 0.0;
            var sourceWidth = source.GetLength(0);
            var sourceHeight = source.GetLength(1);
            var rgn = new PixelRgn(drawable, rgnRect, false, false);
            for (IntPtr pr = PixelRgn.Register(rgn);
                 pr != IntPtr.Zero;
                 pr = PixelRgn.Process(pr))
            {
                for (int y1 = rgn.Y - rgnRect.UpperLeft.Y, y2 = rgn.Y;
                     y2 < rgn.Y + rgn.H;
                     y1++, y2++)
                {
                    for (int x1 = rgn.X - rgnRect.UpperLeft.X, x2 = rgn.X;
                         x2 < rgn.X + rgn.W;
                         x1++, x2++)
                    {
                        if (selectionMask[x2, y2])
                        {
                            return null;
                        }
                        if (x1 >= sourceWidth ||
                            x2 >= sourceHeight ||
                            source[x1, y1] == null)
                        {
                            continue;
                        }

                        sum += DistanceSqr(source[x1, y1], rgn[y2, x2]);
                    }
                }
            }

            return sum;
        }

        #endregion
    }
}