using System;
using System.Collections.Generic;
using System.Linq;
using ExemplarInpaintingPlugin.Helpers;
using Gimp;

namespace ExemplarInpaintingPlugin
{
    internal class Renderer : BaseRenderer
    {
        #region Ctors

        public Renderer(VariableSet variables) : base(variables)
        {
        }

        #endregion
        #region Public methods

        public void Render(Image image, Drawable drawable)
        {
            const int WINDOW_SIZE = 9;

            var red = new Pixel(255, 0, 0, 255);

            Tile.CacheDefault(drawable);
            var temp = image.Duplicate();

            temp.Selection.Border(1);

            bool selectionNonEmpty;
            var selectionBounds = temp.Selection.Bounds(out selectionNonEmpty);
            var selectionBorder =
                BorderHelper.TraceBorder(selectionBounds, coord => temp.Selection[coord.X, coord.Y] != 0)
                            .ToList();
            //var rgn = new PixelRgn(drawable, selectionBounds, true, true);

            //selectionBorder.ForEach(coord => rgn.SetPixel(red.Bytes, coord.X, coord.Y));
            //drawable.Flush(); 
            //drawable.MergeShadow(true);
            //drawable.Update(temp.Selection.MaskBounds);

            //var testBorder = new Pixel[300,320];

            //var iter = new RgnIterator(image.ActiveDrawable, _("Hello"));
            //iter.IterateSrc(pixel => testBorder[pixel.X, pixel.Y] = pixel);

            var pxConfidenceTerm = BorderHelper.FilledMatrix(image.Width, image.Height, 1.0);
            var pxDataTerm = BorderHelper.FilledMatrix(image.Width, image.Height, 1.0);

            //init confidance term
            var iter = new RgnIterator(image.Selection, RunMode.Noninteractive);
            iter.IterateSrc(pixel => pxConfidenceTerm[pixel.X, pixel.Y] = pixel.Bytes[0] != 0 ? 0 : 1);

            UpdateConfidanceTerm(pxConfidenceTerm, selectionBorder);

            //var selectionBorderPxPriorities = selectionBorder.Select((coordinate, i) =>
            //    {
            //        var windowIter = new PixelRgn(drawable, new Rectangle(), )
            //    });

            //var testRgn = new PixelRgn(drawable, true, true);
            //var testIterator = new RegionIterator(testRgn);
            //testIterator.ForEach(pixel => testBorder[pixel.X, pixel.Y] = pixel);
            //testIterator.ForEach(pixel => readRgn.SetPixel(red.Bytes, pixel.X, pixel.Y));
            //drawable.Flush(); 
            //drawable.MergeShadow(true);
            //drawable.Update(drawable.Bounds);

            //      _indexedColorsMap = new IndexedColorsMap();

            //var rectangle = sourceDrawable.MaskBounds;
            //var srcPR = new PixelRgn(sourceDrawable, rectangle, true, true);
            //var destPR = new PixelRgn(toDiffDrawable, rectangle, false, false);

            //var iterator = new RegionIterator(srcPR, destPR);
            //iterator.ForEach((src, dest) => src.Set(MakeAbsDiff(dest, src)));

            //sourceDrawable.Flush();
            //sourceDrawable.MergeShadow(false);
            //sourceDrawable.Update(rectangle);

            //iter.IterateSrcDest(src => selectionBorder.Any(tuple => tuple.Item1 == src.X && tuple.Item2 == src.Y) ? red : src);
            temp.Delete();
        }

        #endregion
        #region Private methods

        private void UpdateConfidanceTerm(double[,] pxConfidenceTerm, List<IntCoordinate> selectionBorder)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}