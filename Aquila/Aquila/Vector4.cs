namespace Aquila
{
    public struct Vector4
    {
        private double e0;
        private double e1;
        private double e2;
        private double e3;

        public Vector4(double e0, double e1, double e2, double e3)
        {
            this.e0 = e0;
            this.e1 = e1;
            this.e2 = e2;
            this.e3 = e3;
        }

        public Vector4(Vector4 vector)
        {
            this.e0 = vector.e0;
            this.e1 = vector.e1;
            this.e2 = vector.e2;
            this.e3 = vector.e3;
        }

        public double X
        {
            get { return this.e0; }
            set { this.e0 = value; }
        }

        public double Y
        {
            get { return this.e1; }
            set { this.e1 = value; }
        }

        public double Z
        {
            get { return this.e2; }
            set { this.e2 = value; }
        }

        public double W
        {
            get { return this.e3; }
            set { this.e3 = value; }
        }

        public double R
        {
            get { return this.e0; }
            set { this.e0 = value; }
        }

        public double G
        {
            get { return this.e1; }
            set { this.e1 = value; }
        }

        public double B
        {
            get { return this.e2; }
            set { this.e2 = value; }
        }

        public double A
        {
            get { return this.e3; }
            set { this.e3 = value; }
        }

        public double S
        {
            get { return this.e0; }
            set { this.e0 = value; }
        }

        public double T
        {
            get { return this.e1; }
            set { this.e1 = value; }
        }

        public double P
        {
            get { return this.e2; }
            set { this.e2 = value; }
        }

        public double Q
        {
            get { return this.e3; }
            set { this.e3 = value; }
        }

        // TODO should we implement here 4*4*4*4 + 4*4*4 + 4*4 + 4 = 340 swizzling possibilities?

        public string Pretty()
        {
            return string.Format("{0}({1}, {2}, {3}, {4})",
                this.GetType().Name, this.e0, this.e1, this.e2, this.e3);
        }

        public void Add(double value)
        {
            this.e0 += value;
            this.e1 += value;
            this.e2 += value;
            this.e3 += value;
        }

        public void Subtract(double value)
        {
            this.e0 -= value;
            this.e1 -= value;
            this.e2 -= value;
            this.e3 -= value;
        }

        public void Multiply(double value)
        {
            this.e0 *= value;
            this.e1 *= value;
            this.e2 *= value;
            this.e3 *= value;
        }

        /// <summary>
        /// The method is faster if we internally use the reciprocal from value.
        /// On the other hand we loose precision (See Test.cs). Therefore, if a
        /// faster division is needed, the Multiply method should be used.
        /// </summary>
        public void Divide(double value)
        {
            this.e0 /= value;
            this.e1 /= value;
            this.e2 /= value;
            this.e3 /= value;
        }

        public void Transform(Matrix4x4 matrix)
        {
            double e0 = this.e0;
            double e1 = this.e1;
            double e2 = this.e2;
            double e3 = this.e3;

            this.e0 = matrix.E00 * e0 + matrix.E01 * e1 + matrix.E02 * e2 + matrix.E03 * e3;
            this.e1 = matrix.E10 * e0 + matrix.E11 * e1 + matrix.E12 * e2 + matrix.E13 * e3;
            this.e2 = matrix.E20 * e0 + matrix.E21 * e1 + matrix.E22 * e2 + matrix.E23 * e3;
            this.e3 = matrix.E30 * e0 + matrix.E31 * e1 + matrix.E32 * e2 + matrix.E33 * e3;
        }

        public double this[int index]
        {
            get
            {
                switch (index)
                {
                    case 0: return this.e0;
                    case 1: return this.e1;
                    case 2: return this.e2;
                    case 3: return this.e3;
                    default: throw new System.IndexOutOfRangeException();
                }
            }
            set
            {
                switch (index)
                {
                    case 0: this.e0 = value; break;
                    case 1: this.e1 = value; break;
                    case 2: this.e2 = value; break;
                    case 3: this.e3 = value; break;
                    default: throw new System.IndexOutOfRangeException();
                }
            }
        }

        public static Vector4 operator +(Vector4 vector1, Vector4 vector2)
        {
            Vector4 result;
            result.e0 = vector1.e0 + vector2.e0;
            result.e1 = vector1.e1 + vector2.e1;
            result.e2 = vector1.e2 + vector2.e2;
            result.e3 = vector1.e3 + vector2.e3;
            return result;
        }

        public static Vector4 operator -(Vector4 vector1, Vector4 vector2)
        {
            Vector4 result;
            result.e0 = vector1.e0 - vector2.e0;
            result.e1 = vector1.e1 - vector2.e1;
            result.e2 = vector1.e2 - vector2.e2;
            result.e3 = vector1.e3 - vector2.e3;
            return result;
        }

        public static Vector4 operator *(Vector4 vector, Matrix4x4 matrix)
        {
            vector.Transform(matrix);
            return vector;
        }

        public static Vector4 operator *(Vector4 vector, double value)
        {
            vector.Multiply(value);
            return vector;
        }

        public static Vector4 operator /(Vector4 vector, double value)
        {
            vector.Divide(value);
            return vector;
        }
    }
}
