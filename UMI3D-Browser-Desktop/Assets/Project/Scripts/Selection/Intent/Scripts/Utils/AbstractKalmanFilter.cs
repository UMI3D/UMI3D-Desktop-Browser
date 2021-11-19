using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;

namespace KalmanFilter
{
    /// <summary>
    /// To use with linear models only
    /// </summary>
    public abstract class AbstractKalmanFilter
    {
        /// <summary>
        /// State dimension
        /// </summary>
        protected readonly int L;

        /// <summary>
        /// Measurement dimension
        /// </summary>
        protected readonly int M;

        /// <summary>
		/// State estimation a priori
		/// </summary>
        protected Vector<double> x_est_predicted;

        /// <summary>
        /// State estimation a posteriori
        /// </summary>
        protected Vector<double> x_est;

        /// <summary>
        /// Covariance predicted
        /// </summary>
        protected Matrix<double> P_predicted;

        /// <summary>
		/// Covariance
		/// </summary>
        protected Matrix<double> P;

        /// <summary>
		/// Std of process 
		/// </summary>
        protected double q;

        /// <summary>
		/// Std of measurement 
		/// </summary>
        protected double r;

        /// <summary>
		/// Covariance of process
		/// </summary>
        protected Matrix<double> Q;

        /// <summary>
		/// Covariance of measurement 
		/// </summary>
        protected Matrix<double> R;

        /// <summary>
        /// True when a first guess has been provided
        /// </summary>
        protected bool isInitialized = false;

        protected enum FilterState
        {
            Unitialized,
            Initialized,
            JustPredicted,
            JustUpdated
        }
        protected FilterState filterStepState;

        #region Getters
        /// <summary>
        /// Covariance of the process last predicted
        /// </summary>
        public double[][] CovariancePredicted
        { get => P_predicted.ToRowArrays(); }

        /// <summary>
        /// Covariance of the process from last update
        /// </summary>
        public double[][] Covariance
        { get => P.ToRowArrays(); }

        /// <summary>
        /// Next estimated state from last prediction
        /// </summary>
        public double[] StateEstimationPredicted
        { get => x_est_predicted.ToArray();  }

        /// <summary>
        /// Next estimated state from last prediction
        /// </summary>
        public double[] StateEstimation
        { get => x_est.ToArray(); }
        #endregion

        /// <summary>
        /// Initialize the filter with the first guess of the state
        /// </summary>
        /// <param name="initialStateGuessed"></param>
        /// <param name="initialCovariance"></param>
        public abstract void Init(double[] initialStateGuessed, double[][] initialCovariance);

        /// <summary>
        /// Initialize the filter with a random guess perfectly known (i.e. cov = Id)
        /// </summary>
        public void Init()
        {
            var x_est = (q * Vector.Build.Random(L)).ToArray(); //initial state with noise
            var P = Matrix.Build.Diagonal(L, L, 1).ToRowArrays(); //initial state covraiance
            Init(x_est, P);
        }

        /// <summary>
        /// Predict the next state of the system
        /// </summary>
        /// <returns>Next state prediction</returns>
        public abstract double[] Predict();

        /// <summary>
        /// Update the prediction to an estimation according to the measurements
        /// </summary>
        /// <param name="measurements"></param>
        /// <returns>State estimation</returns>
        public abstract double[] Update(double[] measurements);

        protected AbstractKalmanFilter(int dimStates, int dimObservation)
        {
            L = dimStates;
            M = dimObservation;

            filterStepState = FilterState.Unitialized;
        }
    }

}

