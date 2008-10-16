namespace Aquila
{
    public interface Varying
    {
        float[] CopyToArray();
        // TODO a new is needed in this function, maybe this is not a good idea
        void CopyFromArray(float[] values);
    }

    public class MatrixUniform
    {
        public Matrix4 modelViewProjection;
    }

    public class PositionColorVertex
    {
        public Vector4 position;
        public Vector4 color;

        public PositionColorVertex(Vector4 position, Vector4 color)
        {
            this.position = position;
            this.color = color;
        }
    }

    public class ColorVarying : Varying
    {
        public Vector4 color;

        public float[] CopyToArray()
        {
            return new float[] { this.color.R, this.color.G, this.color.B, this.color.A };
        }

        public void CopyFromArray(float[] values)
        {
            this.color = new Vector4(values[0], values[1], values[2], values[3]);
        }
    }

    // TODO no OpenGL fill rule, no fill rule at all
    public class Aquila
    {
        private Texture2<Vector4> colorBuffer;
        private Texture2<float> depthBuffer;

        private int viewportX;
        private int viewportY;
        private int viewportWidth;
        private int viewportHeight;

        private bool perspectiveCorrection = true;

        public Aquila(Texture2<Vector4> colorBuffer, Texture2<float> depthBuffer)
        {
            this.colorBuffer = colorBuffer;
            this.depthBuffer = depthBuffer;
        }

        public void SetPerspectiveCorrection(bool enable)
        {
            this.perspectiveCorrection = enable;
        }

        // http://www.opengl.org/sdk/docs/man/xhtml/glViewport.xml
        private void SetViewport(int x, int y, int width, int height)
        {
            this.viewportX = x;
            this.viewportY = y;
            this.viewportWidth = width;
            this.viewportHeight = height;
        }

        public delegate Vector4 VertexProgramDelegate<U, V, W>(U uniform, V vertex, W varying);
        public delegate Vector4 FragmentProgramDelegate<U, W>(U uniform, W varying);

        // currently I have no better idea than to create an empty type, or you have to define all types yourself, somehow C# can not inferre the type here
        public void DrawTriangles<U, V, W>(VertexProgramDelegate<U, V, W> vertexProgram, FragmentProgramDelegate<U, W> fragmentProgram, U uniform, V[] vertices, W any) where W : Varying, new()
        {
            // TODO -1.0f ok or not? if ok then implement SetViewport correctly
            float width = colorBuffer.Width - 1.0f;
            float height = colorBuffer.Height - 1.0f;
            float x = 0.0f;
            float y = 0.0f;

            // TODO manual loop unrolling at least for triangle?

            Vector4[] positions = new Vector4[3];
            W[] varyings = new W[3];
            for (int i = 0; i < 3; i++)
            {
                varyings[i] = new W();
            }

            for (int i = 0; i < vertices.Length; i += 3)
            {
                for (int j = 0; j < 3; j++)
                {
                    Vector4 p = vertexProgram(uniform, vertices[i + j], varyings[j]);
                    p.HomogenousDivide();

                    // TODO gluProject like method in Vector4?
                    positions[j].X = (p.X + 1.0f) * (width / 2.0f) + x;
                    positions[j].Y = (-p.Y + 1.0f) * (height / 2.0f) + y;
                    positions[j].Z = (p.Z + 1.0f) / 2.0f;
                    positions[j].W = p.W;
                }

                float[] c0 = varyings[0].CopyToArray();
                float[] c1 = varyings[1].CopyToArray();
                float[] c2 = varyings[2].CopyToArray();

                Vector4[] colors = new Vector4[] { new Vector4(c0[0], c0[1], c0[2], c0[3]), new Vector4(c1[0], c1[1], c1[2], c1[3]), new Vector4(c2[0], c2[1], c2[2], c2[3]) };
                DrawTrianglesUnsorted(positions, colors);
            }
        }

        private void DrawDots(float x, float y, float z, object[] uniforms, Vector4[] varyings)
        {
            int width = this.colorBuffer.Width;
            int height = this.colorBuffer.Height;

            //Vector4 color = FragmentProgram(uniforms, varyings);
            Vector4 colorBlack = new Vector4(0.0f, 0.0f, 0.0f, 1.0f);

            int sx = (int)x;
            int sy = (int)y;

            if ((sx > 0) && (sx < width) && (sy > 0) && (sy < height))
            {
                colorBuffer.Raw[sy, sx] = colorBlack;
            }
        }


        /// <summary>
        /// Sort the three vertices of a triangle along the y axis from top to
        /// down. Simplifies the method that does the real work.
        /// </summary>
        private void DrawTrianglesUnsorted(Vector4[] p, Vector4[] c)
        {
            int i0 = 0;
            int i1 = 1;
            int i2 = 2;

            if (p[i0].Y > p[i1].Y)
            {
                Math.Swap(ref i0, ref i1);
            }
            if (p[i1].Y > p[i2].Y)
            {
                Math.Swap(ref i1, ref i2);
            }
            if (p[i0].Y > p[i1].Y)
            {
                Math.Swap(ref i0, ref i1);
            }

            DrawTrianglesSorted(p[i0], p[i1], p[i2], c[i0], c[i1], c[i2]);
        }

        // some simple profiling test show me that the this method is not too slow even with 10000 triangles (20 fps), the app spends sometimes more than 50% in Clear and ToRGB methods.
        // TODO clean up this whole method and make it more generic, looks horrible yet
        private void DrawTrianglesSorted(Vector4 p0, Vector4 p1, Vector4 p2, Vector4 c0, Vector4 c1, Vector4 c2)
        {
            //Vector4 colorBackground = new Vector4(0.7, 0.8, 0.9, 1.0f);
            //Vector4 colorWarn = new Vector4(0.0f, 1.0f, 0.0f, 1.0f);
            //Vector4 color = new Vector4(1.0f, 0.0f, 0.0f, 1.0f);
            //Vector4 colorDot = new Vector4(0.0f, 0.0f, 0.0f, 1.0f);

            int width = this.colorBuffer.Width;
            int height = this.colorBuffer.Height;

            //Texture2<Vector4><Vector4> texture = new Texture2<Vector4><Vector4>(200, 200);
            //System.Drawing.Bitmap bitmap = new System.Drawing.Bitmap("D:\\checkboard.bmp");
            //TextureUtility.LoadFromRGB(texture, bitmap);

            int x0 = (int)p0.X;
            int y0 = (int)p0.Y;
            float z0 = p0.Z;
            float w0 = p0.W;
            int x1 = (int)p1.X;
            int y1 = (int)p1.Y;
            float z1 = p1.Z;
            float w1 = p1.W;
            int x2 = (int)p2.X;
            int y2 = (int)p2.Y;
            float z2 = p2.Z;
            float w2 = p2.W;

            if (this.perspectiveCorrection)
            {
                c0.Multiply(w0);
                c1.Multiply(w1);
                c2.Multiply(w2);
            }

            //w0 = 1.0f / w0;
            //w1 = 1.0f / w1;
            //w2 = 1.0f / w2;

            float slope0to1 = (float)(x0 - x1) / (float)(y0 - y1);
            float slope0to2 = (float)(x0 - x2) / (float)(y0 - y2);
            float slope1to2 = (float)(x1 - x2) / (float)(y1 - y2);

            float zslope0to1 = (float)(z0 - z1) / (float)(y0 - y1);
            float zslope0to2 = (float)(z0 - z2) / (float)(y0 - y2);
            float zslope1to2 = (float)(z1 - z2) / (float)(y1 - y2);

            Vector4 cslope0to1 = (c0 - c1) / (y0 - y1);
            Vector4 cslope0to2 = (c0 - c2) / (y0 - y2);
            Vector4 cslope1to2 = (c1 - c2) / (y1 - y2);

            float wslope0to1 = (float)(w0 - w1) / (float)(y0 - y1);
            float wslope0to2 = (float)(w0 - w2) / (float)(y0 - y2);
            float wslope1to2 = (float)(w1 - w2) / (float)(y1 - y2);

            for (int y = y0; y < y1; y++)
            {
                // TODO move this mul out of this loop makes the app sometimes slower, more investigation needed
                int x0to1 = (int)(x1 + slope0to1 * (y - y1));
                int x0to2 = (int)(x2 + slope0to2 * (y - y2));

                float z0to1 = z1 + zslope0to1 * (y - y1);
                float z0to2 = z2 + zslope0to2 * (y - y2);

                Vector4 c0to1 = c1 + cslope0to1 * (float)(y - y1);
                Vector4 c0to2 = c2 + cslope0to2 * (float)(y - y2);

                float w0to1 = w1 + wslope0to1 * (y - y1);
                float w0to2 = w2 + wslope0to2 * (y - y2);

                int xmin;
                int xmax;
                float zmin;
                float zmax;
                Vector4 cmin;
                Vector4 cmax;
                float wmin;
                float wmax;

                if (x0to1 > x0to2)
                {
                    xmax = x0to1;
                    xmin = x0to2;
                    zmax = z0to1;
                    zmin = z0to2;
                    cmax = c0to1;
                    cmin = c0to2;
                    wmax = w0to1;
                    wmin = w0to2;
                }
                else
                {
                    xmax = x0to2;
                    xmin = x0to1;
                    zmax = z0to2;
                    zmin = z0to1;
                    cmax = c0to2;
                    cmin = c0to1;
                    wmax = w0to2;
                    wmin = w0to1;
                }

                Vector4 mcolor = (cmax - cmin) / (xmax - xmin);
                float mz = (zmax - zmin) / (xmax - xmin);
                float mw = (wmax - wmin) / (xmax - xmin);
                for (int x = xmin; x < xmax; x++)
                {
                    float z = zmin + mz * (x - xmin);
                    float w = wmin + mw * (x - xmin);
                    Vector4 c = cmin + mcolor * (x - xmin);

                    w = 1.0f / w;

                    if (this.perspectiveCorrection)
                    {
                        c.Multiply(w);
                    }

                    //Vector4 t = texture.GetCoordinate(c.X, c.Y);

                    if ((x >= 0) && (y >= 0) && (x < width) && (y < height) && (z >= 0.0f) && (z <= 1.0f))
                    {
                        if (z < depthBuffer.Raw[y, x])
                        {
                            colorBuffer.Raw[y, x] = c;
                            depthBuffer.Raw[y, x] = z;
                        }
                    }
                }
            }

            for (int y = y1; y < y2; y++)
            {
                int x1to2 = (int)(x2 + slope1to2 * (y - y2));
                int x0to2 = (int)(x2 + slope0to2 * (y - y2));

                float z1to2 = z2 + zslope1to2 * (y - y2);
                float z0to2 = z2 + zslope0to2 * (y - y2);

                Vector4 c1to2 = c2 + cslope1to2 * (float)(y - y2);
                Vector4 c0to2 = c2 + cslope0to2 * (float)(y - y2);

                float w1to2 = w2 + wslope1to2 * (y - y2);
                float w0to2 = w2 + wslope0to2 * (y - y2);

                int xmin;
                int xmax;
                float zmin;
                float zmax;
                Vector4 cmin;
                Vector4 cmax;
                float wmin;
                float wmax;

                if (x1to2 > x0to2)
                {
                    xmax = x1to2;
                    xmin = x0to2;
                    zmax = z1to2;
                    zmin = z0to2;
                    cmax = c1to2;
                    cmin = c0to2;
                    wmax = w1to2;
                    wmin = w0to2;
                }
                else
                {
                    xmax = x0to2;
                    xmin = x1to2;
                    zmax = z0to2;
                    zmin = z1to2;
                    cmax = c0to2;
                    cmin = c1to2;
                    wmax = w0to2;
                    wmin = w1to2;
                }

                Vector4 mcolor = (cmax - cmin) / (xmax - xmin);
                float mz = (zmax - zmin) / (xmax - xmin);
                float mw = (wmax - wmin) / (xmax - xmin);
                for (int x = xmin; x < xmax; x++)
                {
                    float z = zmin + mz * (x - xmin);
                    float w = wmin + mw * (x - xmin);
                    Vector4 c = cmin + mcolor * (x - xmin);

                    w = 1.0f / w;

                    if (this.perspectiveCorrection)
                    {
                        c.Multiply(w);
                    }

                    //Vector4 t = texture.GetCoordinate(c.X, c.Y);

                    if ((x >= 0) && (y >= 0) && (x < width) && (y < height) && (z >= 0.0f) && (z <= 1.0f))
                    {
                        if (z < depthBuffer.Raw[y, x])
                        {
                            colorBuffer.Raw[y, x] = c;
                            depthBuffer.Raw[y, x] = z;
                        }
                    }
                }
            }
        }
    }
}
