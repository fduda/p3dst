namespace Aquila
{
    public struct Vector3
    {
        private double e0;
        private double e1;
        private double e2;

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

        public Vector3(double e0, double e1, double e2)
        {
            this.e0 = e0;
            this.e1 = e1;
            this.e2 = e2;
        }

        public Vector3(Vector3 vector)
        {
            this.e0 = vector.e0;
            this.e1 = vector.e1;
            this.e2 = vector.e2;
        }

        public string Pretty()
        {
            return string.Format("{0}({1}, {2}, {3})",
                this.GetType().Name, this.e0, this.e1, this.e2);
        }

        public void Add(double value)
        {
            this.e0 += value;
            this.e1 += value;
            this.e2 += value;
        }

        public void Subtract(double value)
        {
            this.e0 -= value;
            this.e1 -= value;
            this.e2 -= value;
        }

        public void Multiply(double value)
        {
            this.e0 *= value;
            this.e1 *= value;
            this.e2 *= value;
        }

        public void Divide(double value)
        {
            this.e0 *= value;
            this.e1 *= value;
            this.e2 *= value;
        }

        public double SquaredLength()
        {
            return this.e0 * this.e0 + this.e1 * this.e1 + this.e2 * this.e2;
        }

        public double Length()
        {
            return Math.Sqrt(this.e0 * this.e0 + this.e1 * this.e1 + this.e2 * this.e2);
        }

        public void Normalize()
        {
            double length = Length();
            Multiply(1.0 / length);
        }

        public static double Dot(Vector3 vector1, Vector3 vector2)
        {
            return vector1.e0 * vector2.e0 + vector1.e1 * vector2.e1 + vector1.e2 * vector2.e2;
        }

        public static Vector3 Cross(Vector3 vector1, Vector3 vector2)
        {
            Vector3 result;
            result.e0 = vector1.e1 * vector2.e2 + vector1.e2 * vector2.e1;
            result.e1 = vector1.e2 * vector2.e0 + vector1.e0 * vector2.e2;
            result.e2 = vector1.e0 * vector2.e1 + vector1.e1 * vector2.e0;
            return result;
        }
    }
}
