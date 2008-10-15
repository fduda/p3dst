namespace Aquila
{
    // TODO this class was once a generic but nprof does not work well with generics
    // TODO GetTexelRepeat, GetTexelRepeat Mirror
    // TODO Vector2 for GetTexel*
    // TODO linear interpolation
    public class Texture2
    {
        private Vector4[,] buffer;
        private int width;
        private int height;
        private double widthTexel;
        private double heightTexel;
        private Vector4 border;

        public Texture2(int width, int height)
        {
            this.width = width;
            this.height = height;
            this.widthTexel = width - 1.0;
            this.heightTexel = height - 1.0;
            this.buffer = new Vector4[height, width];
        }

        public int Width
        {
            get { return this.width; }
        }

        public int Height
        {
            get { return this.height; }
        }

        public void Clear(Vector4 value)
        {
            unsafe
            {
                for (int y = 0; y < this.height; y++)
                {
                    for (int x = 0; x < this.width; x++)
                    {
                        this.buffer[y, x] = value;
                    }
                }
            }
        }

        public void SetBorder(Vector4 border)
        {
            this.border = border;
        }

        public Vector4 GetTexel(double s, double t)
        {
            int x = (int)(this.widthTexel * s);
            int y = (int)(this.heightTexel * t);
            return this.buffer[y, x];
        }

        public Vector4 GetTexelClamp(double s, double t)
        {
            int x = (int)(this.widthTexel * Math.Saturate(s));
            int y = (int)(this.heightTexel * Math.Saturate(t));
            return this.buffer[y, x];
        }

        public Vector4 GetTexelBorder(double s, double t)
        {
            if ((s >= 0.0) && (s <= 1.0) && (t >= 0.0) && (t <= 1.0))
            {
                int x = (int)(this.widthTexel * s);
                int y = (int)(this.heightTexel * t);
                return this.buffer[y, x];
            }
            else
            {
                return this.border;
            }
        }

        // TODO good or bad idea?
        public Vector4[,] Raw
        {
            get { return this.buffer; }
        }
    }
}
