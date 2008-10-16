namespace Aquila
{
    // TODO this class was once a generic but nprof does not work well with generics
    // TODO GetTexelRepeat, GetTexelRepeat Mirror
    // TODO Vector2 for GetTexel*
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

        /// <summary>
        /// This method consumes ~7 ms on my 2 GHz CPU with a 640 x 480 buffer.
        /// Try to avoid this method if possible. E.g. if your scene fills the
        /// whole screen anyway, then there is no need to clear color buffer.
        /// </summary>
        public void Clear(Vector4 value)
        {
            for (int y = 0; y < this.height; y++)
            {
                for (int x = 0; x < this.width; x++)
                {
                    this.buffer[y, x] = value;
                }
            }
        }

        public void SetBorder(Vector4 border)
        {
            this.border = border;
        }

        public Vector4 GetTexel(Vector2 vector)
        {
            int x = (int)(this.widthTexel * vector.S);
            int y = (int)(this.heightTexel * vector.T);
            return this.buffer[y, x];
        }

        public Vector4 GetTexelClamp(Vector2 vector)
        {
            int x = (int)(this.widthTexel * Math.Saturate(vector.S));
            int y = (int)(this.heightTexel * Math.Saturate(vector.T));
            return this.buffer[y, x];
        }

        public Vector4 GetTexelBorder(Vector2 vector)
        {
            if ((vector.S >= 0.0) && (vector.S <= 1.0) && (vector.T >= 0.0) && (vector.T <= 1.0))
            {
                int x = (int)(this.widthTexel * vector.S);
                int y = (int)(this.heightTexel * vector.T);
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
