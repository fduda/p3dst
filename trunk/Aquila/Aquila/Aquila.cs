namespace Aquila
{
    // TODO no OpenGL fill rule, no fill rule at all
    // TODO Vector4 for depthBuffer is overkill, will change this later, or maybe we use it as stencil, and w buffer
    public class Aquila
    {
        private Texture2 colorBuffer;
        private Texture2 depthBuffer;

        private int viewportX;
        private int viewportY;
        private int viewportWidth;
        private int viewportHeight;

        private bool perspectiveCorrection = true;

        public Aquila(Texture2 colorBuffer, Texture2 depthBuffer)
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

        public delegate Vector4 VertexProgramDelegate<U>(U uniforms, Vector4[] vertices, Vector4[] varyings);
        public delegate Vector4 FragmentProgramDelegate<U>(U uniforms, Vector4[] varyings);

        public void DrawTriangles<U>(VertexProgramDelegate<U> vertexProgram, FragmentProgramDelegate<U> fragmentProgram, U uniforms, Vector4[][] vertices, int varyingCount)
        {
            // TODO -1.0 ok or not? if ok then implement SetViewport correctly
            double width = colorBuffer.Width - 1.0;
            double height = colorBuffer.Height - 1.0;
            double x = 0.0;
            double y = 0.0;

            // TODO manual loop unrolling at least for triangle?

            Vector4[] positions = new Vector4[3];
            Vector4[][] varyings = new Vector4[3][];
            for (int i = 0; i < 3; i++)
            {
                varyings[i] = new Vector4[varyingCount];
            }

            for (int i = 0; i < vertices.Length; i += 3)
            {
                for (int j = 0; j < 3; j++)
                {
                    Vector4 p0 = vertexProgram(uniforms, vertices[i + j], varyings[j]);

                    // TODO homogenous divide function in Vector4?
                    Vector4 p1 = new Vector4();
                    p1.X = p0.X / p0.W;
                    p1.Y = p0.Y / p0.W;
                    p1.Z = p0.Z / p0.W;
                    p1.W = p0.W; // TODO divdide here and multiply in DrawTriangle? is there a well defined way for homogenous divide?

                    // TODO gluProject like method in Vector 4?
                    positions[j].X = (p1.X + 1.0) * (width / 2.0) + x;
                    positions[j].Y = (-p1.Y + 1.0) * (height / 2.0) + y;
                    positions[j].Z = (p1.Z + 1.0) / 2.0;
                    positions[j].W = p1.W;
                }

                Vector4[] colors = new Vector4[] { varyings[0][0], varyings[1][0], varyings[2][0] };
                DrawTrianglesUnsorted(positions, colors);

                for (int j = 0; j < 3; j++)
                {
                    //TODO move another method
                    //DrawDot(positions[j].X, positions[j].Y, positions[j].Z, uniforms, varyings[j]);
                }
            }
        }

        private void DrawDots(double x, double y, double z, object[] uniforms, Vector4[] varyings)
        {
            int width = this.colorBuffer.Width;
            int height = this.colorBuffer.Height;

            //Vector4 color = FragmentProgram(uniforms, varyings);
            Vector4 colorBlack = new Vector4(0.0, 0.0, 0.0, 1.0);

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
            //Vector4 colorBackground = new Vector4(0.7, 0.8, 0.9, 1.0);
            //Vector4 colorWarn = new Vector4(0.0, 1.0, 0.0, 1.0);
            //Vector4 color = new Vector4(1.0, 0.0, 0.0, 1.0);
            //Vector4 colorDot = new Vector4(0.0, 0.0, 0.0, 1.0);

            int width = this.colorBuffer.Width;
            int height = this.colorBuffer.Height;

            //Texture2<Vector4> texture = new Texture2<Vector4>(200, 200);
            //System.Drawing.Bitmap bitmap = new System.Drawing.Bitmap("D:\\checkboard.bmp");
            //TextureUtility.LoadFromRGB(texture, bitmap);

            int x0 = (int)p0.X;
            int y0 = (int)p0.Y;
            double z0 = p0.Z;
            double w0 = p0.W;
            int x1 = (int)p1.X;
            int y1 = (int)p1.Y;
            double z1 = p1.Z;
            double w1 = p1.W;
            int x2 = (int)p2.X;
            int y2 = (int)p2.Y;
            double z2 = p2.Z;
            double w2 = p2.W;

            if (this.perspectiveCorrection)
            {
                c0.Divide(w0);
                c1.Divide(w1);
                c2.Divide(w2);
            }

            w0 = 1.0 / w0;
            w1 = 1.0 / w1;
            w2 = 1.0 / w2;

            double slope0to1 = (double)(x0 - x1) / (double)(y0 - y1);
            double slope0to2 = (double)(x0 - x2) / (double)(y0 - y2);
            double slope1to2 = (double)(x1 - x2) / (double)(y1 - y2);

            double zslope0to1 = (double)(z0 - z1) / (double)(y0 - y1);
            double zslope0to2 = (double)(z0 - z2) / (double)(y0 - y2);
            double zslope1to2 = (double)(z1 - z2) / (double)(y1 - y2);

            Vector4 cslope0to1 = (c0 - c1) / (y0 - y1);
            Vector4 cslope0to2 = (c0 - c2) / (y0 - y2);
            Vector4 cslope1to2 = (c1 - c2) / (y1 - y2);

            double wslope0to1 = (double)(w0 - w1) / (double)(y0 - y1);
            double wslope0to2 = (double)(w0 - w2) / (double)(y0 - y2);
            double wslope1to2 = (double)(w1 - w2) / (double)(y1 - y2);

            for (int y = y0; y < y1; y++)
            {
                // TODO move this mul out of this loop makes the app sometimes slower, more investigation needed
                int x0to1 = (int)(x1 + slope0to1 * (y - y1));
                int x0to2 = (int)(x2 + slope0to2 * (y - y2));

                double z0to1 = z1 + zslope0to1 * (y - y1);
                double z0to2 = z2 + zslope0to2 * (y - y2);

                Vector4 c0to1 = c1 + cslope0to1 * (double)(y - y1);
                Vector4 c0to2 = c2 + cslope0to2 * (double)(y - y2);

                double w0to1 = w1 + wslope0to1 * (y - y1);
                double w0to2 = w2 + wslope0to2 * (y - y2);

                int xmin;
                int xmax;
                double zmin;
                double zmax;
                Vector4 cmin;
                Vector4 cmax;
                double wmin;
                double wmax;

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
                double mz = (zmax - zmin) / (xmax - xmin);
                double mw = (wmax - wmin) / (xmax - xmin);
                for (int x = xmin; x < xmax; x++)
                {
                    double z = zmin + mz * (x - xmin);
                    double w = wmin + mw * (x - xmin);
                    Vector4 c = cmin + mcolor * (x - xmin);

                    if (this.perspectiveCorrection)
                    {
                        c.Divide(w);
                    }

                    //Vector4 t = texture.GetCoordinate(c.X, c.Y);

                    if ((x >= 0) && (y >= 0) && (x < width) && (y < height) && (z >= 0.0) && (z <= 1.0))
                    {
                        if (z < depthBuffer.Raw[y, x].R)
                        {
                            colorBuffer.Raw[y, x] = c;
                            depthBuffer.Raw[y, x].R = z;
                        }
                    }
                }
            }

            for (int y = y1; y < y2; y++)
            {
                int x1to2 = (int)(x2 + slope1to2 * (y - y2));
                int x0to2 = (int)(x2 + slope0to2 * (y - y2));

                double z1to2 = z2 + zslope1to2 * (y - y2);
                double z0to2 = z2 + zslope0to2 * (y - y2);

                Vector4 c1to2 = c2 + cslope1to2 * (double)(y - y2);
                Vector4 c0to2 = c2 + cslope0to2 * (double)(y - y2);

                double w1to2 = w2 + wslope1to2 * (y - y2);
                double w0to2 = w2 + wslope0to2 * (y - y2);

                int xmin;
                int xmax;
                double zmin;
                double zmax;
                Vector4 cmin;
                Vector4 cmax;
                double wmin;
                double wmax;

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
                double mz = (zmax - zmin) / (xmax - xmin);
                double mw = (wmax - wmin) / (xmax - xmin);
                for (int x = xmin; x < xmax; x++)
                {
                    double z = zmin + mz * (x - xmin);
                    double w = wmin + mw * (x - xmin);
                    Vector4 c = cmin + mcolor * (x - xmin);

                    if (this.perspectiveCorrection)
                    {
                        c.Divide(w);
                    }

                    //Vector4 t = texture.GetCoordinate(c.X, c.Y);

                    if ((x >= 0) && (y >= 0) && (x < width) && (y < height) && (z >= 0.0) && (z <= 1.0))
                    {
                        if (z < depthBuffer.Raw[y, x].R)
                        {
                            colorBuffer.Raw[y, x] = c;
                            depthBuffer.Raw[y, x].R = z;
                        }
                    }
                }
            }
        }
    }
}
