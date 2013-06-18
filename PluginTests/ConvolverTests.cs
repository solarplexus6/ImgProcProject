//
//  GIMP plugin
//  Region Filling and Object Removal by Exemplar-Based Image Inpainting
//  Rafal Lukaszewski, 2013
//

using System;
using ExemplarInpaintingPlugin.DataTerm.Convolution;
using Gimp;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace PluginTests
{
    [TestClass]
    public class ConvolverTests
    {
        [TestMethod]
        public void ConvolveWithScharr()
        {
            var matrix = new double[3,3];
            var convolver = new Convolver(ScharrKernel.Instance);          

            matrix[0, 0] = 0; matrix[1, 0] = 1; matrix[2, 0] = 1;
            matrix[0, 1] = 0; matrix[1, 1] = 0; matrix[2, 1] = 1;
            matrix[0, 2] = 0; matrix[1, 2] = 0; matrix[2, 2] = 0;
                        
            var gradient = convolver.Convolve(new IntCoordinate(1,1), matrix, elem => elem);
            Assert.AreEqual(gradient.Angle, Math.PI/4);

            matrix[0, 0] = 1; matrix[1, 0] = 1; matrix[2, 0] = 0;
            matrix[0, 1] = 1; matrix[1, 1] = 0; matrix[2, 1] = 0;
            matrix[0, 2] = 0; matrix[1, 2] = 0; matrix[2, 2] = 0;

            gradient = convolver.Convolve(new IntCoordinate(1, 1), matrix, elem => elem);
            Assert.AreEqual(gradient.Angle, Math.PI * 3/4);

            matrix[0, 0] = 1; matrix[1, 0] = 0; matrix[2, 0] = 0;
            matrix[0, 1] = 1; matrix[1, 1] = 0; matrix[2, 1] = 0;
            matrix[0, 2] = 1; matrix[1, 2] = 0; matrix[2, 2] = 0;

            gradient = convolver.Convolve(new IntCoordinate(1, 1), matrix, elem => elem);
            Assert.AreEqual(gradient.Angle, -Math.PI);

            matrix[0, 0] = 0; matrix[1, 0] = 0; matrix[2, 0] = 0;
            matrix[0, 1] = 0; matrix[1, 1] = 0; matrix[2, 1] = 0;
            matrix[0, 2] = 1; matrix[1, 2] = 1; matrix[2, 2] = 1;

            gradient = convolver.Convolve(new IntCoordinate(1, 1), matrix, elem => elem);
            Assert.AreEqual(gradient.Angle, -Math.PI/2);
        }
    }
}
