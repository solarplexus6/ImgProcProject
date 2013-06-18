namespace ExemplarInpaintingPlugin.DataTerm.Convolution
{
    public abstract class SeparableKernel
    {
        #region Properties

        public virtual double ScaleFactor
        {
            get { return 1; }
        }

        public abstract int Size { get; }

        public abstract double[] XColumnVector { get; }
        public abstract double[] XRowVector { get; }

        public abstract double[] YColumnVector { get; }
        public abstract double[] YRowVector { get; }

        #endregion
    }
}