using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;
using System;
using System.Collections.Generic;

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

    /// <summary>
    /// To use with linear models only
    /// Row majors matrices are expected
    /// </summary>
    public class KalmanFilter : AbstractKalmanFilter
    {
        /// <summary>
        /// State-transition model, also known as dynamic model
        /// </summary>
        private Matrix<double> Fk;

        /// <summary>
        /// Observation model
        /// </summary>
        private Matrix<double> Hk;

        public KalmanFilter(int dimStates, int dimObservation, double stdProcess, double stdMeasure, double[][] modelProcess, double[][] modelObservation)
            : base(dimStates, dimObservation)
        {
            q = stdProcess;
            r = stdMeasure;

            Q = Matrix.Build.Diagonal(L, L, q * q);
            R = Matrix.Build.Diagonal(M, M, r * r);

            Fk = Matrix.Build.DenseOfRowArrays(modelProcess);
            Hk = Matrix.Build.DenseOfRowArrays(modelObservation);
        }

        /// <summary>
        /// Produce a Kalman filter with observation of the same dimension than the states and with an identity relationship
        /// </summary>
        /// <param name="dimStates"></param>
        /// <param name="stdProcess"></param>
        /// <param name="stdMeasure"></param>
        /// <param name="modelProcess"></param>
        public KalmanFilter(int dimStates, double stdProcess, double stdMeasure, double[][] modelProcess)
            : this(dimStates, dimStates, stdProcess, stdMeasure, modelProcess, Matrix.Build.DenseIdentity(dimStates, dimStates).ToRowArrays())
        { }

        #region Init
        public override void Init(double[] estimateGuessed, double[][] covarianceInit)
        {
            x_est = Vector.Build.Dense(estimateGuessed);
            P = Matrix.Build.DenseOfRowArrays(covarianceInit);

            filterStepState = FilterState.Initialized;
        }
        #endregion

        #region Predict
        public override double[] Predict()
        {
            if (!(filterStepState == FilterState.JustUpdated || filterStepState == FilterState.Initialized))
                throw new Exception("Trying to predict without update or initialization");

            x_est_predicted = Fk * x_est;
            P_predicted = Fk * P * Fk.Transpose() + Q; //prediction from the model

            filterStepState = FilterState.JustPredicted;
            return x_est_predicted.ToArray();
        }

        public double[] Predict(double[][] modelProcess)
        {
            Fk = Matrix.Build.DenseOfRowArrays(modelProcess);
            return Predict();
        }

        public double[] Predict(Matrix<double> modelProcess)
        {
            Fk = modelProcess;
            return Predict();
        }
        #endregion

        #region Update
        public override double[] Update(double[] measure)
        {
            if (filterStepState != FilterState.JustPredicted)
                throw new Exception("Trying to update without a prediction");

            var S = Hk * P_predicted * Hk.Transpose();
            var K = P_predicted * Hk.Transpose() * S.Inverse(); // Kalman optimal gain

            var Z = Vector.Build.Dense(measure);
            var deviation = Z - Hk * x_est_predicted;

            x_est = x_est_predicted + K * deviation; //estimation update

            filterStepState = FilterState.JustUpdated;
            return x_est.ToArray();
        }

        public double[] Update(double[] measure, double[][] modelObservation)
        {
            Hk = Matrix.Build.DenseOfRowArrays(modelObservation);
            return Update(measure);
        }
        #endregion
    }

    /// <summary>
    /// UKF as described in Wan & van der Merwe 2000
    /// </summary>
    public class UnscentedKalmanFilter : AbstractKalmanFilter
    {
        /// <summary>
        /// The alpha coefficient, characterize sigma-points dispersion around mean
        /// </summary>
        private double alpha;

        /// <summary>
        /// The ki.
        /// </summary>
        private double kappa;

        /// <summary>
        /// The beta coefficient, characterize type of distribution (2 for normal one) 
        /// </summary>
        private double beta;

        /// <summary>
        /// Scale factor
        /// </summary>
        private double lambda;

        /// <summary>
        /// Mean weights
        /// </summary>
        private Vector<double> Wm;

        /// <summary>
        /// Covariance weights
        /// </summary>
        private Vector<double> Wc;

        /// <summary>
        /// Non-linear map of the process model
        /// </summary>
        private Func<Vector<double>, Vector<double>> f;

        /// <summary>
        /// Non-linear map of the observation model
        /// </summary>
        private Func<Vector<double>, Vector<double>> h;

        private UnscentedTransformResults ut_F;

        private class UnscentedTransformResults
        {
            /// <summary>
            /// propagated mean, y
            /// </summary>
            public Vector<double> mean;
            /// <summary>
            /// propagated sigma vectors, Y
            /// </summary>
            public List<Vector<double>> samplingPoints;
            /// <summary>
            /// covariance matrix, P
            /// </summary>
            public Matrix<double> covariance;
            /// <summary>
            /// deviation around mean, (Y_k-y)_k 
            /// </summary>
            public Matrix<double> deviations;
        }

        /// <summary>
        /// Constructor of Unscented Kalman Filter
        /// </summary>
        /// <param name="L">State dimension</param>
        /// <param name="M">Measurements dimension</param>
        /// <param name="q">std of process</param>
        /// <param name="r">std of measurement</param>
        public UnscentedKalmanFilter(int L, int M, double q, double r, Func<double[], double[]> processModel, Func<double[], double[]> observationModel)
            : base(L, M)
        {
            this.q = q;
            this.r = r;

            this.f = x => Vector.Build.Dense(processModel(x.ToArray()));
            this.h = x => Vector.Build.Dense(observationModel(x.ToArray()));
        }

        #region Init
        public override void Init(double[] initialStateGuessed, double[][] initialCovariance)
        {
            Q = Matrix.Build.Diagonal(L, L, q * q); //covariance of process
            R = Matrix.Build.Dense(M, M, r * r); //covariance of measurement  

            // unscented transform parameters
            alpha = 1e-3d;
            kappa = 0;
            beta = 2d;
            lambda = alpha * alpha * (L + kappa) - L;
            var c = L + lambda;

            //weights for means
            Wm = Vector.Build.Dense(2 * L + 1, 0.5 / c);
            Wm[0] = lambda / c;

            //weights for covariance
            Wc = Vector.Build.Dense(2 * L + 1);
            Wm.CopyTo(Wc);
            Wc[0] = Wm[0] + 1 - alpha * alpha + beta;

            filterStepState = FilterState.Initialized;
        }
        #endregion

        #region Predict
        public override double[] Predict()
        {
            if (!(filterStepState == FilterState.JustUpdated || filterStepState == FilterState.Initialized))
                throw new Exception("Trying to predict without update or initialization");

            //sigma points around x
            List<Vector<double>> X = GetSigmaPoints(x_est, P);

            //unscented transformation of process
            ut_F = UnscentedTransform(f, X, Wm, Wc, L, Q);

            //a priori estimated state
            x_est_predicted = ut_F.mean;

            filterStepState = FilterState.JustPredicted;
            return x_est_predicted.ToArray();
        }

        public double[] Predict(Func<Vector<double>, Vector<double>> processModel)
        {
            f = processModel;
            return Predict();
        }

        public double[] Predict(Func<double[], double[]> processModel)
        {
            f = x => Vector.Build.DenseOfArray(processModel(x.ToArray()));
            return Predict();
        }
        #endregion

        #region Update
        public override double[] Update(double[] measurements)
        {
            if (filterStepState != FilterState.JustPredicted)
                throw new Exception("Trying to update without prediction");

            // integating measures
            var z = Vector.Build.Dense(measurements);

            //unscented transformation of sampling points
            UnscentedTransformResults ut_H = UnscentedTransform(h, ut_F.samplingPoints, Wm, Wc, M, R);

            //a priori estimated output
            var z_est_apriori = ut_H.mean;

            //transformed cross-covariance
            Matrix<double> Pxy = ut_F.deviations * Matrix.Build.DiagonalOfDiagonalVector(Wc) * ut_H.deviations.Transpose();

            // Kalman gain matrix
            Matrix<double> K = Pxy * ut_H.covariance.Inverse();

            //state update a posteriori
            x_est = x_est_predicted + K * (z - z_est_apriori);

            //covariance update a posteriori
            P = ut_F.covariance - K * Pxy.Transpose();

            filterStepState = FilterState.JustUpdated;
            return x_est.ToArray();
        }

        public double[] Update(double[] measure, Func<double[], double[]> observationModel)
        {
            h = x => Vector.Build.DenseOfArray(observationModel(x.ToArray()));
            return Update(measure);
        }

        public double[] Update(double[] measure, Func<Vector<double>, Vector<double>> observationModel)
        {
            h = observationModel;
            return Update(measure);
        }
        #endregion

        #region UnscentedTransform
        /// <summary>
        /// Unscented Transformation
        /// </summary>
        /// <param name="f">nonlinear map</param>
        /// <param name="X">sigma vectors</param>
        /// <param name="Wm">Weights for means</param>
        /// <param name="Wc">Weights for covariance</param>
        /// <param name="n">numer of outputs of f</param>
        /// <param name="R">additive covariance</param>
        /// <returns>[transformed mean, transformed sampling points, transformed covariance, transformed deviations]</returns>
        private UnscentedTransformResults UnscentedTransform(Func<Vector<double>, Vector<double>> f, List<Vector<double>> X, Vector<double> Wm, Vector<double> Wc, int n, Matrix<double> R)
        {
            int numberOfSigmaPoints = X.Count;
            Vector<double> y = Vector.Build.Dense(n);
            List<Vector<double>> Y = new List<Vector<double>>();
            Matrix<double> Y1 = Matrix.Build.Dense(L, numberOfSigmaPoints, 0);

            Vector<double> Xk;
            for (int k = 0; k < numberOfSigmaPoints; k++)
            {
                Xk = X[k];
                var Yk = f(Xk);
                Y.Add(Yk);
                y += Yk * Wm[k];
                Y1.SetColumn(k, Yk - y);
            }

            Matrix<double> P = Y1 * Matrix.Build.DiagonalOfDiagonalVector(Wc) * Y1.Transpose() + R;

            var output = new UnscentedTransformResults()
            {
                mean = y,
                samplingPoints = Y,
                covariance = P,
                deviations = Y1
            };
            return output;
        }

        /// <summary>
        /// Sigma points around reference point
        /// </summary>
        /// <param name="x">reference point</param>
        /// <param name="P">covariance</param>
        /// <param name="c">coefficient sqrt(lambda+L)</param>
        /// <returns>Sigma points</returns>
        private List<Vector<double>> GetSigmaPoints(Vector<double> x, Matrix<double> P)
        {
            int L = x.Count;
            var scaleFactor = Math.Sqrt(L + lambda);

            Matrix<double> sqrtP = P.Cholesky().Factor;

            var A = sqrtP.Transpose() * scaleFactor;

            Matrix<double> Y = Matrix.Build.Dense(L, L, 1);
            for (int j = 0; j < L; j++)
            {
                Y.SetSubMatrix(0, L, j, 1, x.ToColumnMatrix());
            }

            var sigmaPoints = new List<Vector<double>> { x };

            var YplusA = Y + A;
            for (int k = 1; k < L + 1; k++)
            {
                sigmaPoints.Add(YplusA.Column(k));
            }

            var YminusA = Y - A;
            for (int k = L + 1; k < (2 * L + 1); k++)
            {
                sigmaPoints.Add(YminusA.Column(k));
            }

            return sigmaPoints;
        }
        #endregion
    }
}

