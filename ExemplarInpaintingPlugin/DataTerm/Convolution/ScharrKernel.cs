namespace ExemplarInpaintingPlugin.DataTerm.Convolution
{
    public class ScharrKernel : SeparableKernel
    {
        #region Constants

        private const double SCALE_FACTOR = 1.0/32.0;
        private const int SIZE = 3;

        #endregion
        #region Private fields

        private static ScharrKernel _instance;

        //http://www.hlevkin.com/articles/SobelScharrGradients5x5.pdf
        private readonly double[] _xColumnVector = new[] {3.0, 10.0, 3.0};
        private readonly double[] _xRowVector = new[] {-1.0, .0, 1.0};
        private readonly double[] _yColumnVector = new[] {-1.0, .0, 1.0};
        private readonly double[] _yRowVector = new[] {3.0, 10.0, 3.0};

        #endregion
        #region Properties

        public static ScharrKernel Instance
        {
            get { return _instance ?? (_instance = new ScharrKernel()); }
        }

        public override double ScaleFactor
        {
            get { return SCALE_FACTOR; }
        }

        public override int Size
        {
            get { return SIZE; }
        }

        public override double[] XColumnVector
        {
            get { return _xColumnVector; }
        }

        public override double[] XRowVector
        {
            get { return _xRowVector; }
        }

        public override double[] YColumnVector
        {
            get { return _yColumnVector; }
        }

        public override double[] YRowVector
        {
            get { return _yRowVector; }
        }

        #endregion
        #region Ctors

        private ScharrKernel()
        {
        }

        #endregion
    }
}