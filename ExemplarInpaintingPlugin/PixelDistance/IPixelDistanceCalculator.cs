using ExemplarInpaintingPlugin.DataTerm.Convolution;
using ExemplarInpaintingPlugin.SelectionHandling;
using Gimp;

namespace ExemplarInpaintingPlugin.PixelDistance
{
    public interface IPixelDistanceCalculator
    {
        #region Public methods

        double DistanceSqr(Pixel p1, Pixel p2);
        Rectangle MinDistanceRegion(Rectangle sourceRect, Drawable drawable, SelectionMask selectionMask);
        Rectangle MinDistanceRegion(Rectangle sourceRect, ImgGradient gradient, Pixel[,] layerPixels, SelectionMask selectionMask);
        double? RegionDistance(Pixel[,] source, Rectangle rgnRect, Drawable drawable, SelectionMask selectionMask);

        double? RegionDistance(Rectangle sourceRect, Rectangle rgnRect, Pixel[,] layerPixels,
                               SelectionMask selectionMask);

        #endregion
    }
}