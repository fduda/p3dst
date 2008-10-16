namespace Aquila
{
    public struct Vector4
    {
        private float e0;
        private float e1;
        private float e2;
        private float e3;

        public Vector4(float e0, float e1, float e2, float e3)
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

        public float X
        {
            get { return this.e0; }
            set { this.e0 = value; }
        }

        public float Y
        {
            get { return this.e1; }
            set { this.e1 = value; }
        }

        public float Z
        {
            get { return this.e2; }
            set { this.e2 = value; }
        }

        public float W
        {
            get { return this.e3; }
            set { this.e3 = value; }
        }

        public float R
        {
            get { return this.e0; }
            set { this.e0 = value; }
        }

        public float G
        {
            get { return this.e1; }
            set { this.e1 = value; }
        }

        public float B
        {
            get { return this.e2; }
            set { this.e2 = value; }
        }

        public float A
        {
            get { return this.e3; }
            set { this.e3 = value; }
        }

        public float S
        {
            get { return this.e0; }
            set { this.e0 = value; }
        }

        public float T
        {
            get { return this.e1; }
            set { this.e1 = value; }
        }

        public float P
        {
            get { return this.e2; }
            set { this.e2 = value; }
        }

        public float Q
        {
            get { return this.e3; }
            set { this.e3 = value; }
        }

        // TODO should we implement here all 3 * (4**4 + 4**3 + 4**2 + 4**1) = 1020 swizzling possibilities?

        public string Pretty()
        {
            return string.Format("{0}({1}, {2}, {3}, {4})",
                this.GetType().Name, this.e0, this.e1, this.e2, this.e3);
        }

        public void Add(float value)
        {
            this.e0 += value;
            this.e1 += value;
            this.e2 += value;
            this.e3 += value;
        }

        public void Subtract(float value)
        {
            this.e0 -= value;
            this.e1 -= value;
            this.e2 -= value;
            this.e3 -= value;
        }

        public void Multiply(float value)
        {
            this.e0 *= value;
            this.e1 *= value;
            this.e2 *= value;
            this.e3 *= value;
        }

        /// <summary>
        /// The method is faster if we internally use the reciprocal from value.
        /// On the other hand we loose precision (See Test.cs). Therefore, if a
        /// faster division is needed, the multiply method should be used by
        /// the user of this class.
        /// </summary>
        public void Divide(float value)
        {
            this.e0 /= value;
            this.e1 /= value;
            this.e2 /= value;
            this.e3 /= value;
        }

        /// <summary>
        /// The transform method is here because we like to modify the vector
        /// and not the matrix. Beside the operators most (all?) methods of the
        /// two classes modify themselves.
        /// </summary>
        public void Transform(Matrix4 matrix)
        {
            float e0 = this.e0;
            float e1 = this.e1;
            float e2 = this.e2;
            float e3 = this.e3;

            this.e0 = matrix.E00 * e0 + matrix.E01 * e1 + matrix.E02 * e2 + matrix.E03 * e3;
            this.e1 = matrix.E10 * e0 + matrix.E11 * e1 + matrix.E12 * e2 + matrix.E13 * e3;
            this.e2 = matrix.E20 * e0 + matrix.E21 * e1 + matrix.E22 * e2 + matrix.E23 * e3;
            this.e3 = matrix.E30 * e0 + matrix.E31 * e1 + matrix.E32 * e2 + matrix.E33 * e3;
        }

        // TODO test how fast and or slow this indexer is. If it is to slow then remove it. If it is fast enough add one to Matrix4 and Vector3.
        public float this[int index]
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

        public static Vector4 operator *(Matrix4 matrix, Vector4 vector)
        {
            vector.Transform(matrix);
            return vector;
        }

        public static Vector4 operator *(Vector4 vector, float value)
        {
            vector.Multiply(value);
            return vector;
        }

        public static Vector4 operator /(Vector4 vector, float value)
        {
            vector.Divide(value);
            return vector;
        }

        /// <summary>
        /// Do not know if there is a standardized way to assign w. We choose
        /// 1.0 / w here. Has the advantage that we later only have can use a
        /// mulitiplication instead a division.
        /// </summary>
        internal void HomogenousDivide()
        {
            float w = 1.0f / this.W;
            this.X *= w;
            this.Y *= w;
            this.Z *= w;
            this.W = w;
        }
    }
}
