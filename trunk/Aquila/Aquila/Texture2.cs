namespace Aquila
{
    // TODO GetTexelRepeat, GetTexelRepeat Mirror
    // TODO Vector2 for GetTexel*
    public class Texture2<T>
    {
        private T[,] buffer;
        private int width;
        private int height;
        private float widthTexel;
        private float heightTexel;
        private T border;

        public Texture2(int width, int height)
        {
            this.width = width;
            this.height = height;
            this.widthTexel = width - 1.0f;
            this.heightTexel = height - 1.0f;
            this.buffer = new T[height, width];
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
        public void Clear(T value)
        {
            for (int y = 0; y < this.height; y++)
            {
                for (int x = 0; x < this.width; x++)
                {
                    this.buffer[y, x] = value;
                }
            }
        }

        public void SetBorder(T border)
        {
            this.border = border;
        }

        public T GetTexel(Vector2 vector)
        {
            int x = (int)(this.widthTexel * vector.S);
            int y = (int)(this.heightTexel * vector.T);
            return this.buffer[y, x];
        }

        public T GetTexelClamp(Vector2 vector)
        {
            int x = (int)(this.widthTexel * Math.Saturate(vector.S));
            int y = (int)(this.heightTexel * Math.Saturate(vector.T));
            return this.buffer[y, x];
        }

        public T GetTexelBorder(Vector2 vector)
        {
            if ((vector.S >= 0.0f) && (vector.S <= 1.0f) && (vector.T >= 0.0f) && (vector.T <= 1.0f))
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
        public T[,] Raw
        {
            get { return this.buffer; }
        }
    }
}
