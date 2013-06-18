using Gimp;

namespace ExemplarInpaintingPlugin.Helpers
{
    public static class RectangleHelper
    {
        #region Public static methods

        public static Rectangle AdjustedToImageEdges(this Rectangle originRect, Rectangle imageRect)
        {
            var rect = new Rectangle(originRect.UpperLeft, originRect.LowerRight);
            if (!imageRect.IsInside(rect.UpperLeft))
            {
                if (rect.UpperLeft.X < imageRect.UpperLeft.X)
                {
                    rect.UpperLeft.X = imageRect.UpperLeft.X;
                }
                if (rect.UpperLeft.Y < imageRect.UpperLeft.Y)
                {
                    rect.UpperLeft.Y = imageRect.UpperLeft.Y;
                }
            }
            if (!imageRect.IsInside(rect.LowerRight))
            {
                if (rect.LowerRight.X > imageRect.LowerRight.X)
                {
                    rect.LowerRight.X = imageRect.LowerRight.X;
                }
                if (rect.LowerRight.Y > imageRect.LowerRight.Y)
                {
                    rect.LowerRight.Y = imageRect.LowerRight.Y;
                }
            }
            return rect;
        }

        public static bool IsInside(this Rectangle rect, IntCoordinate coordinate)
        {
            return rect.IsInside(coordinate.X, coordinate.Y);
        }

        public static bool IsInside(this Rectangle rect, int x, int y)
        {
            return rect.X1 <= x && x < rect.X2 &&
                   rect.Y1 <= y && y < rect.Y2;
        }

        #endregion
    }
}