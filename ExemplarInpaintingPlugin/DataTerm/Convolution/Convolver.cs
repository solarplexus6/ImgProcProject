using System;
using ExemplarInpaintingPlugin.Helpers;
using Gimp;

namespace ExemplarInpaintingPlugin.DataTerm.Convolution
{    
    public class Convolver
    {
        private SeparableKernel _kernel;

        public Convolver(SeparableKernel kernel)
        {
            _kernel = kernel;
        }

        public ImgGradient Convolve<T>(IntCoordinate coord, T[,] data, Func<T, double> converter)
        {
            //http://www.songho.ca/dsp/convolution/convolution2d_separable.html            

            var rect = coord.PointCenteredRectangle(_kernel.Size);
            var dataWidth = data.GetLength(0);
            var dataHeight = data.GetLength(1);

            var gradient = new ImgGradient();

            for (int i = 0; i < _kernel.Size; i++)
            {
                var xTmp = 0.0;
                var yTmp = 0.0;
                for (int j = 0; j < _kernel.Size; j++)
                {
                    if (rect.X1 + i >= dataWidth || rect.Y1 + j >= dataHeight)
                    {
                        continue;                        
                    }
                    var value = converter(data[rect.X1 + i, rect.Y1 + j]);
                    xTmp += _kernel.XColumnVector[j]*value;
                    yTmp += _kernel.YColumnVector[j]*value;
                }

                gradient.XMagnitude += xTmp * _kernel.XRowVector[i];
                gradient.YMagnitude += yTmp * _kernel.YRowVector[i];
            }
            gradient.XMagnitude = gradient.XMagnitude * _kernel.ScaleFactor;
            gradient.YMagnitude = gradient.YMagnitude * _kernel.ScaleFactor;

            return gradient;
        }
    }
}