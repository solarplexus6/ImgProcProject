//
//  GIMP plugin
//  Region Filling and Object Removal by Exemplar-Based Image Inpainting
//  Rafal Lukaszewski, 2013
//

using Gimp;

namespace ExemplarInpaintingPlugin.Helpers
{
    public static class CoordinateHelper
    {
        #region Public static methods

        public static Rectangle PointCenteredRectangle(this IntCoordinate coord, int size)
        {
            var delta = size/2;
            return new Rectangle(
                new IntCoordinate(coord.X - delta, coord.Y - delta),
                size, size);
        }

        #endregion
    }
}