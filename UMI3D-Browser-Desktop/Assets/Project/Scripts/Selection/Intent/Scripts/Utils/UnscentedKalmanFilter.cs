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
        protected Matrix<double> x_est_predicted;

        /// <summary>
        /// State estimation a posteriori
        /// </summary>
        protected Matrix<double> x_est;

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
            : this(dimStates, dimStates, stdProcess, stdMeasure, modelProcess, Matrix.Build.SparseIdentity(dimStates, dimStates).ToRowArrays())
        { }

        public void Init(double[] estimateGuessed, double[][] covarianceInit)
        {
            x_est = Matrix.Build.Dense(1, L, estimateGuessed);
            P = Matrix.Build.DenseOfRowArrays(covarianceInit);

            isInitialized = true;
        }

        #region Predict
        public override double[] Predict()
        {
            x_est_predicted = Fk.Multiply(x_est);
            P = Fk.Multiply(P).Multiply(Fk.Transpose()) + Q; //estimation from the model

            return x_est_predicted.ToColumnMajorArray();
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
            var S = Hk.Multiply(P).Multiply(Hk.Transpose());
            var K = P.Multiply(Hk.Transpose()).Multiply(S.Inverse()); // Kalman optimal gain

            var Z = Matrix.Build.DenseOfRowArrays(measure);
            var deviation = Z.Subtract(Hk.Multiply(x_est_predicted));

            x_est = x_est_predicted.Add(K.Multiply(deviation)); //estimation update

            return x_est.ToColumnMajorArray();
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
            public Matrix<double> mean;
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

        private void Init()
        {
            x_est = q * Matrix.Build.Random(L, 1); //initial state with noise
            P = Matrix.Build.Diagonal(L, L, 1); //initial state covraiance

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

            return x_est.ToColumnMajorArray();
        }

        public override double[] Update(double[] measurements)
        {
            // integating measures
            var z = Matrix.Build.Dense(M, 1, 0);
            z.SetColumn(0, measurements);

            //unscented transformation of measurments
            UnscentedTransformResults ut_H = UnscentedTransform(ut_F.samplingPoints, Wm, Wc, M, R);

            //a priori estimated state
            var x_est_apriori = ut_F.mean;

            //a priori estimated output
            var z_est_apriori = ut_H.mean;

            //transformed cross-covariance
            Matrix<double> Pxy = ut_F.deviations.Multiply(Matrix.Build.Diagonal(Wc.Row(0).ToArray())).Multiply(ut_H.deviations.Transpose());

            // Kalman gain matrix
            Matrix<double> K = Pxy.Multiply(ut_H.covariance.Inverse());

            //state update a posteriori
            x_est = x_est_apriori.Add(K.Multiply(z.Subtract(z_est_apriori)));

            //covariance update a posteriori
            P = ut_F.covariance.Subtract(K.Multiply(Pxy.Transpose()));

            return x_est.ToColumnMajorArray();
        }

        public double[] getState()
        {
            return x_est.ToColumnArrays()[0];
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
            Matrix<double> y = Matrix.Build.Dense(n, 1, 0);
            Matrix<double> Y = Matrix.Build.Dense(n, twoLPlusOne, 0);

            Matrix<double> Xk;
            for (int k = 0; k < twoLPlusOne; k++)
            {
                Xk = X.SubMatrix(0, X.RowCount, k, 1);
                Y.SetSubMatrix(0, Y.RowCount, k, 1, Xk); // HERE, where is f ?
                y = y.Add(Y.SubMatrix(0, Y.RowCount, k, 1).Multiply(Wm[0, k]));
            }

            Matrix<double> Y1 = Y.Subtract(y.Multiply(Matrix.Build.Dense(1, twoLPlusOne, 1)));
            Matrix<double> P = Y1.Multiply(Matrix.Build.Diagonal(Wc.Row(0).ToArray()));
            P = P.Multiply(Y1.Transpose());
            P = P.Add(R);

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
        private Matrix<double> GetSigmaPoints(Matrix<double> x, Matrix<double> P, double c)
        {
            Matrix<double> sqrtP = P.Cholesky().Factor;

            var A = sqrtP.Transpose().Multiply(c);

            int L = x.RowCount;

            Matrix<double> Y = Matrix.Build.Dense(L, L, 1);
            for (int j = 0; j < L; j++)
            {
                Y.SetSubMatrix(0, L, j, 1, x);
            }

            Matrix<double> X = Matrix.Build.Dense(L, (2 * L + 1));
            X.SetSubMatrix(0, L, 0, 1, x);

            Matrix<double> Y_plus_A = Y.Add(A);
            X.SetSubMatrix(0, L, 1, L, Y_plus_A);

            Matrix<double> Y_minus_A = Y.Subtract(A);
            X.SetSubMatrix(0, L, L + 1, L, Y_minus_A);

            return X;
        }
    }
}

