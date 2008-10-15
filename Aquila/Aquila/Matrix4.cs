namespace Aquila
{
    public struct Matrix4
    {
        private double e00;
        private double e01;
        private double e02;
        private double e03;
        private double e10;
        private double e11;
        private double e12;
        private double e13;
        private double e20;
        private double e21;
        private double e22;
        private double e23;
        private double e30;
        private double e31;
        private double e32;
        private double e33;

        public double E00
        {
            get { return this.e00; }
            set { this.e00 = value; }
        }

        public double E01
        {
            get { return this.e01; }
            set { this.e01 = value; }
        }

        public double E02
        {
            get { return this.e02; }
            set { this.e02 = value; }
        }

        public double E03
        {
            get { return this.e03; }
            set { this.e03 = value; }
        }

        public double E10
        {
            get { return this.e10; }
            set { this.e10 = value; }
        }

        public double E11
        {
            get { return this.e11; }
            set { this.e11 = value; }
        }

        public double E12
        {
            get { return this.e12; }
            set { this.e12 = value; }
        }

        public double E13
        {
            get { return this.e13; }
            set { this.e13 = value; }
        }

        public double E20
        {
            get { return this.e20; }
            set { this.e20 = value; }
        }

        public double E21
        {
            get { return this.e21; }
            set { this.e21 = value; }
        }

        public double E22
        {
            get { return this.e22; }
            set { this.e22 = value; }
        }

        public double E23
        {
            get { return this.e23; }
            set { this.e23 = value; }
        }

        public double E30
        {
            get { return this.e30; }
            set { this.e30 = value; }
        }

        public double E31
        {
            get { return this.e31; }
            set { this.e31 = value; }
        }

        public double E32
        {
            get { return this.e32; }
            set { this.e32 = value; }
        }

        public double E33
        {
            get { return this.e33; }
            set { this.e33 = value; }
        }

        public Matrix4(Matrix4 matrix)
        {
            this.e00 = matrix.e00; this.e01 = matrix.e01; this.e02 = matrix.e02; this.e03 = matrix.e03;
            this.e10 = matrix.e10; this.e11 = matrix.e11; this.e12 = matrix.e12; this.e13 = matrix.e13;
            this.e20 = matrix.e20; this.e21 = matrix.e21; this.e22 = matrix.e22; this.e23 = matrix.e23;
            this.e30 = matrix.e30; this.e31 = matrix.e31; this.e32 = matrix.e32; this.e33 = matrix.e33;
        }

        public string Pretty()
        {
            return string.Format("{0}({1}, {2}, {3}, {4}, {5}, {6}, {7}, {8}, {9}, {10}, {11}, {12}, {13}, {14}, {15}, {16})",
                this.GetType().Name,
                this.e00, this.e01, this.e02, this.e03,
                this.e10, this.e11, this.e12, this.e13,
                this.e20, this.e21, this.e22, this.e23,
                this.e30, this.e31, this.e32, this.e33);
        }

        public void Clear(double value)
        {
            this.e00 = value; this.e01 = value; this.e02 = value; this.e03 = value;
            this.e10 = value; this.e11 = value; this.e12 = value; this.e13 = value;
            this.e20 = value; this.e21 = value; this.e22 = value; this.e23 = value;
            this.e30 = value; this.e31 = value; this.e32 = value; this.e33 = value;
        }

        public void Identity()
        {
            this.e00 = 1.0; this.e01 = 0.0; this.e02 = 0.0; this.e03 = 0.0;
            this.e10 = 0.0; this.e11 = 1.0; this.e12 = 0.0; this.e13 = 0.0;
            this.e20 = 0.0; this.e21 = 0.0; this.e22 = 1.0; this.e23 = 0.0;
            this.e30 = 0.0; this.e31 = 0.0; this.e32 = 0.0; this.e33 = 1.0;
        }

        public void Multiply(Matrix4 matrix)
        {
            Matrix4 t = new Matrix4(this);

            this.e00 = t.e00 * matrix.e00 + t.e01 * matrix.e10 + t.e02 * matrix.e20 + t.e03 * matrix.e30;
            this.e01 = t.e00 * matrix.e01 + t.e01 * matrix.e11 + t.e02 * matrix.e21 + t.e03 * matrix.e31;
            this.e02 = t.e00 * matrix.e02 + t.e01 * matrix.e12 + t.e02 * matrix.e22 + t.e03 * matrix.e32;
            this.e03 = t.e00 * matrix.e03 + t.e01 * matrix.e13 + t.e02 * matrix.e23 + t.e03 * matrix.e33;

            this.e10 = t.e10 * matrix.e00 + t.e11 * matrix.e10 + t.e12 * matrix.e20 + t.e13 * matrix.e30;
            this.e11 = t.e10 * matrix.e01 + t.e11 * matrix.e11 + t.e12 * matrix.e21 + t.e13 * matrix.e31;
            this.e12 = t.e10 * matrix.e02 + t.e11 * matrix.e12 + t.e12 * matrix.e22 + t.e13 * matrix.e32;
            this.e13 = t.e10 * matrix.e03 + t.e11 * matrix.e13 + t.e12 * matrix.e23 + t.e13 * matrix.e33;

            this.e20 = t.e20 * matrix.e00 + t.e21 * matrix.e10 + t.e22 * matrix.e20 + t.e23 * matrix.e30;
            this.e21 = t.e20 * matrix.e01 + t.e21 * matrix.e11 + t.e22 * matrix.e21 + t.e23 * matrix.e31;
            this.e22 = t.e20 * matrix.e02 + t.e21 * matrix.e12 + t.e22 * matrix.e22 + t.e23 * matrix.e32;
            this.e23 = t.e20 * matrix.e03 + t.e21 * matrix.e13 + t.e22 * matrix.e23 + t.e23 * matrix.e33;

            this.e30 = t.e30 * matrix.e00 + t.e31 * matrix.e10 + t.e32 * matrix.e20 + t.e33 * matrix.e30;
            this.e31 = t.e30 * matrix.e01 + t.e31 * matrix.e11 + t.e32 * matrix.e21 + t.e33 * matrix.e31;
            this.e32 = t.e30 * matrix.e02 + t.e31 * matrix.e12 + t.e32 * matrix.e22 + t.e33 * matrix.e32;
            this.e33 = t.e30 * matrix.e03 + t.e31 * matrix.e13 + t.e32 * matrix.e23 + t.e33 * matrix.e33;
        }

        public void Transpose()
        {
            Matrix4 t = new Matrix4(this);

            this.e00 = t.e00; this.e01 = t.e10; this.e02 = t.e20; this.e03 = t.e30;
            this.e10 = t.e01; this.e11 = t.e11; this.e12 = t.e21; this.e13 = t.e31;
            this.e20 = t.e02; this.e21 = t.e12; this.e22 = t.e22; this.e23 = t.e32;
            this.e30 = t.e03; this.e31 = t.e13; this.e32 = t.e23; this.e33 = t.e33;
        }

        public void Divide(double value)
        {
            this.e00 /= value;
            this.e01 /= value;
            this.e02 /= value;
            this.e03 /= value;

            this.e10 /= value;
            this.e11 /= value;
            this.e12 /= value;
            this.e13 /= value;

            this.e20 /= value;
            this.e21 /= value;
            this.e22 /= value;
            this.e23 /= value;

            this.e30 /= value;
            this.e31 /= value;
            this.e32 /= value;
            this.e33 /= value;
        }

        private double Determinant()
        {
            return 0.0;
        }

        private void Adjoint()
        {
            Matrix4 t = new Matrix4(this);
        }

        private void Inverse()
        {
            double determinant = Determinant();
            Adjoint();
            Divide(determinant);
        }
    }
}
