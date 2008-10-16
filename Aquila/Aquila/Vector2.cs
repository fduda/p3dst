using System;
using System.Collections.Generic;
using System.Text;

namespace Aquila
{
    public struct Vector2
    {
        private double e0;
        private double e1;

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

        public Vector2(double e0, double e1)
        {
            this.e0 = e0;
            this.e1 = e1;
        }

        public Vector2(Vector2 vector)
        {
            this.e0 = vector.e0;
            this.e1 = vector.e1;
        }

        public string Pretty()
        {
            return string.Format("{0}({1}, {2})",
                this.GetType().Name, this.e0, this.e1);
        }
    }
}
