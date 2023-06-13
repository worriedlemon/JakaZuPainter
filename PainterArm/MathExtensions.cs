using JakaAPI.Types.Math;
using static System.Math;

namespace PainterArm.MathExtensions
{
    using DoubleTranslation = Func<double, double>;

    public class Matrix
    {
        private readonly double[,] data;
        public int NbRows { get; private set; }
        public int NbCols { get; private set; }
        public double this[int i, int j]
        {
            get
            {
                return data[i, j];
            }
            private set
            {
                data[i, j] = value;
            }
        }

        public Matrix(int rows, int cols)
        {
            if (rows < 1 || cols < 1)
            {
                throw new Exception($"Invalid matrix size");
            }

            NbRows = rows;
            NbCols = cols;

            data = new double[rows, cols];
            FillMatrix(0);
        }

        public Matrix(Matrix A) : this(A.data) { }

        public Matrix(double[,] data)
        {
            NbRows = data.GetLength(0);
            NbCols = data.GetLength(1);

            if (NbRows < 1 || NbCols < 1)
            {
                throw new Exception($"Invalid matrix size");
            }

            this.data = new double[NbRows, NbCols];

            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    this.data[i, j] = data[i, j];
                }
            }
        }

        public Matrix(Vector3 x, Vector3 y, Vector3 z)
        {
            NbCols = NbRows = 3;

            data = new double[3, 3];
            data[0, 0] = x.Dx;
            data[1, 0] = x.Dy;
            data[2, 0] = x.Dz;

            data[0, 1] = y.Dx;
            data[1, 1] = y.Dy;
            data[2, 1] = y.Dz;

            data[0, 2] = z.Dx;
            data[1, 2] = z.Dy;
            data[2, 2] = z.Dz;
        }

        //public Vector3 this[int i] => new(data[0, i], data[1, i], data[2, i]);

        public static Matrix operator *(Matrix A, Matrix B)
        {
            if (A.NbCols != B.NbRows)
            {
                throw new Exception($"Invalid matrix size");
            }

            Matrix result = new Matrix(A.NbRows, B.NbCols);

            for (int i = 0; i < result.NbRows; i++)
            {
                for (int j = 0; j < result.NbCols; j++)
                {
                    for (int k = 0; k < A.NbCols; k++)
                    {
                        result.data[i, j] += A.data[i, k] * B.data[k, j];
                    }
                }
            }

            return result;
        }

        public static Matrix operator *(Matrix A, double value)
        {
            Matrix result = new Matrix(A.NbRows, A.NbCols);

            for (int i = 0; i < A.NbRows; i++)
            {
                for (int j = 0; j < A.NbCols; j++)
                {
                    result.data[i, j] = A.data[i, j] * value;
                }
            }

            return result;
        }

        public static Matrix operator *(double value, Matrix A) => A * value;

        public static readonly Matrix Identity = new(new double[3, 3]
                {
                    { 1, 0, 0 },
                    { 0, 1, 0 },
                    { 0, 0, 1 }
                });

        public static Vector3 operator *(Matrix A, Vector3 v)
        {
            if (A.NbCols != 3 || A.NbRows != 3)
            {
                throw new Exception($"Invalid matrix size");
            }

            double[] vc = new double[3] { 0, 0, 0 };
            double[] coord = new double[3] { v.Dx, v.Dy, v.Dz };
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    vc[i] += A.data[i, j] * coord[j];
                }
            }
            return new Vector3(vc[0], vc[1], vc[2]);
        }

        public static Matrix RotationMatrix(double rx, double ry, double rz, bool degrees = true)
        {
            DoubleTranslation translate = degrees ? MathDefinitions.DegToRad : (double arg) => { return arg; };
            (double sin, double cos)
                phi = SinCos(translate(rx)),
                theta = SinCos(translate(ry)),
                psi = SinCos(translate(rz));

            return new Matrix(new double[3, 3]
            {
                { psi.cos * theta.cos, psi.cos * theta.sin * phi.sin - psi.sin * phi.cos,  psi.cos * theta.sin * phi.cos + psi.sin * phi.sin},
                { psi.sin * theta.cos, psi.sin * theta.sin * phi.sin + psi.cos * phi.cos, psi.sin * theta.sin * phi.cos - psi.cos * phi.sin },
                { -theta.sin, theta.cos * phi.sin, theta.cos * phi.cos }
            });
        }

        public (RPYRotation Main, RPYRotation Alt) ToRPY(bool degrees = true)
        {
            DoubleTranslation translate = degrees ? MathDefinitions.RadToDeg : (double arg) => { return arg; };

            Vector3 v = new(0, 0, 1);
            Vector3 u = new(data[0, 2], data[1, 2], data[2, 2]);

            double cos = Vector3.DotProduct(u, v);
            double sin = Vector3.VectorProduct(u, v).Length();

            Matrix R = new(new double[3, 3]
            {
                { cos, sin, 0},
                { -sin, cos, 0},
                { 0, 0, 1},
            });

            Vector3 w = u.Normalized();
            Vector3 k = (v - cos * u).Normalized();
            Vector3 r = Vector3.VectorProduct(u, v).Normalized();

            Matrix F = new Matrix(w, k, r).Transpose();
            Matrix U = F.Rounded(6).ReverseMatrix() * R * F;

            double ry1 = translate(Asin(-U[2, 0]));
            double ry2 = translate(180.0 - ry1);

            double rx1 = translate(Atan2(U[2, 1], U[2, 2]));
            double rx2 = translate(Atan2(-U[2, 1], -U[2, 2]));

            double rz1 = translate(Atan2(U[1, 0], U[0, 0]));
            double rz2 = translate(Atan2(-U[1, 0], -U[0, 0]));

            Console.WriteLine($"\nAlt solution: {rx1}, {ry1}, {rz1}");
            Console.WriteLine($"\nMain solution: {rx2}, {ry2}, {rz2}");

            return (new(rx1, ry1, rz1), new(rx2, ry2, rz2));
        }

        private void FillMatrix(double x)
        {
            for (int i = 0; i < NbRows; i++)
            {
                for (int j = 0; j < NbCols; j++)
                {
                    data[i, j] = x;
                }
            }
        }

        public static Matrix operator ~(Matrix A) => A.Transpose();

        public Matrix Rounded(int digits)
        {
            double[,] rounded_data = (double[,])data.Clone();

            for (int i = 0; i < NbRows; i++)
            {
                for (int j = 0; j < NbCols; j++)
                {
                    rounded_data[i, j] = Round(data[i, j], digits);
                }
            }

            return new Matrix(rounded_data);
        }

        public Matrix Transpose()
        {
            double[,] res = new double[NbRows, NbCols];

            for (int i = 0; i < NbRows; i++)
            {
                for (int j = 0; j < NbCols; j++)
                {
                    res[i, j] = data[j, i];
                }
            }

            return new Matrix(res);
        }

        private void SwapRows(int row1, int row2)
        {
            for (int j = 0; j < NbCols; j++)
            {
                double temp = this[row1, j];
                this[row1, j] = this[row2, j];
                this[row2, j] = temp;
            }
        }

        public Matrix ReverseMatrix()
        {
            int n = NbRows, m = NbCols;

            Matrix augmentedMatrix = new(n, 2 * m);
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < m; j++)
                {
                    augmentedMatrix[i, j] = this[i, j];
                }
                augmentedMatrix[i, m + i] = 1;
            }

            // Gauss-Jordan elimination algorithm
            for (int i = 0; i < n; i++)
            {
                if (augmentedMatrix[i, i] == 0)
                {
                    int swapRow = -1;
                    for (int k = i + 1; k < n; k++)
                    {
                        if (augmentedMatrix[k, i] != 0)
                        {
                            swapRow = k;
                            break;
                        }
                    }
                    if (swapRow == -1)
                    {
                        throw new InvalidOperationException("Матрица вырожденная, обратной матрицы не существует.");
                    }
                    augmentedMatrix.SwapRows(i, swapRow);
                }

                double scale = augmentedMatrix[i, i];
                for (int j = 0; j < 2 * m; j++)
                {
                    augmentedMatrix[i, j] /= scale;
                }

                for (int k = 0; k < n; k++)
                {
                    if (k != i)
                    {
                        double factor = augmentedMatrix[k, i];
                        for (int j = 0; j < 2 * m; j++)
                        {
                            augmentedMatrix[k, j] -= factor * augmentedMatrix[i, j];
                        }
                    }
                }
            }

            Matrix inverseMatrix = new(n, m);
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < m; j++)
                {
                    inverseMatrix[i, j] = augmentedMatrix[i, m + j];
                }
            }
            return inverseMatrix;
        }

        /// <summary>Returns result of <i>Ax = B</i> linear equation (is being solved with row reduction)</summary>
        /// <param name="A">Matrix variable</param>
        /// <param name="B">Vector variable</param>
        /// <returns>Resulting vector</returns>
        public static Matrix SolveLinearEquation(Matrix A, Matrix B)
        {
            Matrix identity = new Matrix(A);
            Matrix result = new Matrix(B);

            for (int i = 0; i < identity.NbRows; i++)
            {
                for (int k = 0; k < identity.NbRows; k++)
                {
                    double coef;
                    if (k == i)
                    {
                        coef = identity[i, i];
                        for (int j = 0; j < identity.NbCols; j++)
                        {
                            identity[k, j] /= coef;
                        }
                        result[k, 0] /= coef;
                    }
                    else
                    {
                        coef = identity[k, i] / identity[i, i];
                        for (int j = 0; j < identity.NbCols; j++)
                        {
                            identity[k, j] -= coef * identity[i, j];
                        }
                        result[k, 0] -= coef * B[i, 0];
                    }
                }
            }
            return result;
        }

        public override string ToString()
        {
            return $"([{data[0, 0]}, {data[0, 1]}, {data[0, 2]}],\n[{data[1, 0]}, {data[1, 1]}, {data[1, 2]}],\n[{data[2, 0]}, {data[2, 1]}, {data[2, 2]}])";
        }
    }

    public static class MathDefinitions
    {
        public static double DegToRad(double value) => value * PI / 180;

        public static double RadToDeg(double value) => value * 180 / PI;
    }
}
