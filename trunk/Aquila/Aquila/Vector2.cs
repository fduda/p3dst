namespace Aquila
{
    public struct Vector2
    {
        private float e0;
        private float e1;

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

        public Vector2(float e0, float e1)
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
