using System;

namespace ExemplarInpaintingPlugin.DataTerm.Convolution
{
    public class ImgGradient
    {
        #region Properties

        public double Magnitude
        {
            get { return Math.Sqrt(Math.Pow(XMagnitude, 2) + Math.Pow(YMagnitude, 2)); }
        }        

        public double XMagnitude { get; set; }
        public double YMagnitude { get; set; }

        public double Angle
        {
            //reverse sign for top-left oriented coordinate system
            get { return - Math.Atan2(YMagnitude, XMagnitude); }
        }

        #endregion
    }
}