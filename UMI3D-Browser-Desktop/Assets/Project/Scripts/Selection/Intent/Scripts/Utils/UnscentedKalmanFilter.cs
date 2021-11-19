using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;
using MathNet.Numerics.LinearAlgebra.Factorization;
using System;

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

        /// <summary>
        /// Initialize the filter with the first guess of the state
        /// </summary>
        /// <param name="initialStateGuessed"></param>
        /// <param name="initialCovariance"></param>
        public abstract void Init(double[] initialStateGuessed, double[][] initialCovariance);

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

        public override void Init(double[] estimateGuessed, double[][] covarianceInit)
        {
            x_est = Vector.Build.Dense(estimateGuessed);
            P = Matrix.Build.DenseOfRowArrays(covarianceInit);

            isInitialized = true;
        }

        #region Predict
        public override double[] Predict()
        {
            x_est_predicted = Fk * x_est;
            P = Fk * P * Fk.Transpose() + Q; //estimation from the model

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
            var S = Hk * P * Hk.Transpose();
            var K = P * Hk.Transpose() * S.Inverse(); // Kalman optimal gain

            var Z = Vector.Build.Dense(measure);
            var deviation = Z - Hk * x_est_predicted;

            x_est = x_est_predicted.Add(K.Multiply(deviation)); //estimation update

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
        /// Scale factor
        /// </summary>
        private double c;

        /// <summary>
        /// Means weights
        /// </summary>
        private Matrix<double> Wm;

        /// <summary>
        /// Covariance weights
        /// </summary>
        private Matrix<double> Wc;

        /// <summary>
        /// Non-linear map
        /// </summary>
        private Func<Vector<double>, Vector<double>> f;

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
            public Matrix<double> samplingPoints;
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
        public UnscentedKalmanFilter(int L, double q, double r) // , Func<Matrix<double>, Matrix<double>> f, Func<Matrix<double>, Matrix<double>> h
            : base(L, L)
        {
            this.q = q;
            this.r = r;
        }

        public void Init()
        {
            var x_est = (q * Vector.Build.Random(L)).ToArray(); //initial state with noise
            var P = Matrix.Build.Diagonal(L, L, 1).ToRowArrays(); //initial state covraiance
            Init(x_est, P);
        }

        public override void Init(double[] initialStateGuessed, double[][] initialCovariance)
        {
            x_est = q * Vector.Build.Random(L); //initial state with noise
            P = Matrix.Build.Diagonal(L, L, 1);

            Q = Matrix.Build.Diagonal(L, L, q * q); //covariance of process
            R = Matrix.Build.Dense(M, M, r * r); //covariance of measurement  

            alpha = 1e-3d;
            kappa = 0;
            beta = 2d;
            lambda = alpha * alpha * (L + kappa) - L;
            c = L + lambda;

            //weights for means
            Wm = Matrix.Build.Dense(1, (2 * L + 1), 0.5 / c);
            Wm[0, 0] = lambda / c;

            //weights for covariance
            Wc = Matrix.Build.Dense(1, (2 * L + 1));
            Wm.CopyTo(Wc);
            Wc[0, 0] = Wm[0, 0] + 1 - alpha * alpha + beta;

            c = Math.Sqrt(c);

            isInitialized = true;
        }

        public override double[] Predict()
        {
            //sigma points around x
            Matrix<double> X = GetSigmaPoints(x_est, P, c);

            //unscented transformation of process
            // X1=sigmas(x1,P1,c) - sigma points around x1
            //X2=X1-x1(:,ones(1,size(X1,2))) - deviation of X1
            ut_F = UnscentedTransform(X, Wm, Wc, L, Q);

            //a priori estimated state
            x_est = ut_F.mean;

            return x_est.ToArray();
        }

        public override double[] Update(double[] measurements)
        {
            // integating measures
            var z = Vector.Build.Dense(measurements);

            //unscented transformation of measurments
            UnscentedTransformResults ut_H = UnscentedTransform(ut_F.samplingPoints, Wm, Wc, M, R);

            //a priori estimated state
            var x_est_apriori = ut_F.mean;

            //a priori estimated output
            var z_est_apriori = ut_H.mean;

            //transformed cross-covariance
            Matrix<double> Pxy = ut_F.deviations * Matrix.Build.Diagonal(Wc.Row(0).ToArray()) * ut_H.deviations.Transpose();

            // Kalman gain matrix
            Matrix<double> K = Pxy * ut_H.covariance.Inverse();

            //state update a posteriori
            x_est = x_est_apriori + K * (z - z_est_apriori);

            //covariance update a posteriori
            P = ut_F.covariance.Subtract(K.Multiply(Pxy.Transpose()));

            return x_est.ToArray();
        }

        public double[] getState()
        {
            return x_est.ToArray();
        }

        public double[,] getCovariance()
        {
            return P.ToArray();
        }

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
        private UnscentedTransformResults UnscentedTransform(Matrix<double> X, Matrix<double> Wm, Matrix<double> Wc, int n, Matrix<double> R)
        {
            int twoLPlusOne = X.ColumnCount;
            Vector<double> y = Vector.Build.Dense(n);
            Matrix<double> Y = Matrix.Build.Dense(n, twoLPlusOne, 0);

            Vector<double> Xk;
            for (int k = 0; k < twoLPlusOne; k++)
            {
                Xk = X.Column(k);
                Y.SetSubMatrix(0, Y.RowCount, k, 1, Xk.ToColumnMatrix()); // HERE, where is f ?
                y = y + Y.Column(k) * Wm[0, k];
            }

            Matrix<double> Y1 = Y - y.ToColumnMatrix() * Matrix.Build.Dense(1, twoLPlusOne, 1);
            Matrix<double> P = Y1 * Matrix.Build.Diagonal(Wc.Row(0).ToArray());
            P = P * Y1.Transpose() + R;

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
        private Matrix<double> GetSigmaPoints(Vector<double> x, Matrix<double> P, double c)
        {
            Matrix<double> sqrtP = P.Cholesky().Factor;

            var A = sqrtP.Transpose() * c;

            int L = x.Count;

            Matrix<double> Y = Matrix.Build.Dense(L, L, 1);
            for (int j = 0; j < L; j++)
            {
                Y.SetSubMatrix(0, L, j, 1, x.ToColumnMatrix());
            }

            Matrix<double> X = Matrix.Build.Dense(L, (2 * L + 1));
            X.SetSubMatrix(0, L, 0, 1, x.ToColumnMatrix());

            X.SetSubMatrix(0, L, 1, L, Y + A);

            X.SetSubMatrix(0, L, L + 1, L, Y - A);

            return X;
        }
    }
}

