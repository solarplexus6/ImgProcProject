//
//  GIMP plugin
//  Region Filling and Object Removal by Exemplar-Based Image Inpainting
//  Rafal Lukaszewski, 2013
//

using ExemplarInpaintingPlugin.Helpers;
using Gimp;

namespace ExemplarInpaintingPlugin.SelectionHandling
{
    public class SelectionMask
    {
        #region Private fields

        private int _area;
        private readonly bool[,] _mask;
        private readonly Rectangle _originBounds;

        #endregion
        #region Properties

        public int Area
        {
            get { return _area; }
        }

        public bool Empty
        {
            get { return _area == 0; }
        }

        public bool this[int x, int y]
        {
            get
            {
                return _originBounds.IsInside(x, y) &&
                       _mask[x - _originBounds.X1, y - _originBounds.Y1];
            }
        }

        #endregion
        #region Ctors

        public SelectionMask(Selection selection)
        {
            bool selectionNonEmpty;
            _originBounds = selection.Bounds(out selectionNonEmpty);
            if (!selectionNonEmpty)
            {
                _mask = null;
                return;
            }

            _mask = new bool[_originBounds.Width,_originBounds.Height];
            var selRgn = new PixelRgn(selection, _originBounds, false, false);
            var iterator = new RegionIterator(selRgn);

            iterator.ForEach(pixel =>
                {
                    if (pixel.Red > 128)
                    {
                        _area++;
                        _mask[pixel.X - _originBounds.X1, pixel.Y - _originBounds.Y1] = true;
                    }
                });
        }

        #endregion
        #region Public methods

        public void SetAreaToZero(Rectangle rect)
        {
            for (int i = rect.X1; i < rect.X1 + rect.Width; i++)
            {
                for (int j = rect.Y1; j < rect.Y1 + rect.Height; j++)
                {
                    if (!_originBounds.IsInside(i, j) || 
                        !_mask[i - _originBounds.X1, j - _originBounds.Y1])
                    {
                        continue;
                    }                    
                    _mask[i - _originBounds.X1, j - _originBounds.Y1] = false;
                    _area--;
                }
            }
        }

        #endregion
    }
}