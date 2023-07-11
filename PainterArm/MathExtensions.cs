using JakaAPI.Types.Math;
using System.Globalization;
using static System.Math;

namespace PainterArm.MathExtensions
{
    using DoubleTranslation = Func<double, double>;

    /// <summary>
    /// A structure, which represents a geometric vector in 3 dimentions
    /// </summary>
    public class Vector3
    {
        public double Dx { get; private set; }
        public double Dy { get; private set; }
        public double Dz { get; private set; }

        public Vector3() { }

        public Vector3(double x, double y, double z)
        {
            Dx = x;
            Dy = y;
            Dz = z;
        }

        public static Vector3 operator +(Vector3 first, Vector3 second) => new(first.Dx + second.Dx, first.Dy + second.Dy, first.Dz + second.Dz);

        public static Vector3 operator -(Vector3 vector) => new(-vector.Dx, -vector.Dy, -vector.Dz);

        public static Vector3 operator -(Vector3 first, Vector3 second) => first + (-second);

        public static Vector3 operator *(Vector3 vector, double multiplier) => new(vector.Dx * multiplier, vector.Dy * multiplier, vector.Dz * multiplier);

        public static Vector3 operator *(double multiplier, Vector3 vector) => vector * multiplier;

        public static Vector3 operator /(Vector3 vector, double divider) => vector * (1.0 / divider);

        public static explicit operator Vector3(Point point) => new(point.X, point.Y, point.Z);


        public static explicit operator Point(Vector3 vector) => new(vector.Dx, vector.Dy, vector.Dz);

        /// <returns>The length of this <see cref="Vector3"> instance</returns>
        public double Length() => Sqrt(Dx * Dx + Dy * Dy + Dz * Dz);

        /// <returns>A <see cref="Vector3"/> with the unit length and the same direction as the base</returns>
        /// <exception cref="DivideByZeroException"></exception>
        public Vector3 Normalized()
        {
            double length = Length();
            if (length == 0)
            {
                throw new DivideByZeroException("Length of vector is zero: cannot be normalized");
            }
            return this / length;
        }

        /// <param name="a">First vector</param>
        /// <param name="b">Second vector</param>
        /// <returns>A new <see cref="Vector3"/>, which is directed perpendicular to <i>a</i> and <i>b</i></returns>
        public static Vector3 VectorProduct(Vector3 a, Vector3 b) => new(a.Dy * b.Dz - a.Dz * b.Dy, a.Dz * b.Dx - a.Dx * b.Dz, a.Dx * b.Dy - a.Dy * b.Dx);

        /// <param name="a">First vector</param>
        /// <param name="b">Second vector</param>
        /// <returns>A <see cref="double"/> value, representing dot product of two vectors</returns>
        public static double DotProduct(Vector3 a, Vector3 b) => a.Dx * b.Dx + a.Dy * b.Dy + a.Dz * b.Dz;

        /// <param name="direction">Direction vector</param>
        /// <returns>A <see cref="Vector3[]"/> array, representing an arbitrary orthogonal basis for a direction vector</returns>
        public static Vector3[] GetOrthogonalBasis(Vector3 direction)
        {
            Vector3 axis1 = direction.Normalized();
            Vector3 upVect = new Vector3(0, 0, 1);
            Vector3 axis2 = VectorProduct(axis1, upVect);
            Vector3 axis3 = VectorProduct(axis1, axis2);
            return new Vector3[] { axis1, axis2, axis3 };
        }

        public (RPYRotation MainSolution, RPYRotation AltSolution) ToRPY(bool degrees = true)
        {
            DoubleTranslation translate = degrees ? MathDefinitions.RadToDeg : (double arg) => { return arg; };

            Vector3 v = new(0, 0, 1);
            Vector3 u = new(Dx, Dy, Dz);

            double cos = DotProduct(u, v);
            double sin = VectorProduct(u, v).Length();

            Matrix R = new(new double[3, 3]
            {
                { cos, sin, 0},
                { -sin, cos, 0},
                { 0, 0, 1},
            });

            Vector3 w = u.Normalized();
            Vector3 k = (v - cos * u).Normalized();
            Vector3 r = VectorProduct(u, v).Normalized();

            Matrix F = new Matrix(w, k, r).Transpose();
            Matrix U = F.Rounded(6).ReverseMatrix() * R * F;

            double ry1 = translate(Asin(-U[2, 0]));
            double ry2 = translate(PI - ry1);

            double rx1 = translate(Atan2(U[2, 1], U[2, 2]));
            double rx2 = translate(Atan2(-U[2, 1], -U[2, 2]));

            double rz1 = translate(Atan2(U[1, 0], U[0, 0]));
            double rz2 = translate(Atan2(-U[1, 0], -U[0, 0]));

            Console.WriteLine($"\nMain solution: {rx1}, {ry1}, {rz1}");
            Console.WriteLine($"\nAlt solution: {rx2}, {ry2}, {rz2}");

            return (new(rx1, ry1, rz1), new(rx2, ry2, rz2));
        }

        public override string ToString()
        {
            return $"[{Dx.ToString(CultureInfo.InvariantCulture)}," +
                $"{Dy.ToString(CultureInfo.InvariantCulture)}," +
                $"{Dz.ToString(CultureInfo.InvariantCulture)}]";
        }
    }

    /// <summary>
    /// A structure, which represents an arbitary 2D matrix
    /// </summary>
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

    /// <summary>
    /// Common math formulas
    /// </summary>
    public static class MathDefinitions
    {
        public static double DegToRad(double value) => value * PI / 180;

        public static double RadToDeg(double value) => value * 180 / PI;
    }
}
