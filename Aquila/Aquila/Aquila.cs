using System.Runtime.InteropServices;

namespace Aquila
{
    public interface Varying
    {
        float[] CopyToArray();
        // TODO a new is needed in this function, maybe this is not a good idea
        void CopyFromArray(float[] values);

        // TODO second possibility
        //void CopyToArrayEx(float[] values);
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

    // TODO what about unions?
    // TODO internally save as array any only provide Vector4 properties? (first implement fragment shader)
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

        /*
        public void CopyToArrayEx(float[] values)
        {
            values[0] = this.color.R;
            values[1] = this.color.G;
            values[2] = this.color.B;
            values[3] = this.color.A;
        }
        */
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

            float[][] varyings = new float[3][];
            int count = any.CopyToArray().Length;
            for (int i = 0; i < 3; i++)
            {
                varyings[i] = new float[count];
            }

            //W[] varyings = new W[3];
            //for (int i = 0; i < 3; i++)
            //{
            //    varyings[i] = new W();
            //}

            float[] c0 = new float[4];
            float[] c1 = new float[4];
            float[] c2 = new float[4];

            Vector4[] colors = new Vector4[3];

            for (int i = 0; i < vertices.Length; i += 3)
            {
                for (int j = 0; j < 3; j++)
                {
                    Vector4 p = vertexProgram(uniform, vertices[i + j], any);
                    p.HomogenousDivide();

                    // TODO gluProject like method in Vector4?
                    positions[j].X = (p.X + 1.0f) * (width / 2.0f) + x;
                    positions[j].Y = (-p.Y + 1.0f) * (height / 2.0f) + y;
                    positions[j].Z = (p.Z + 1.0f) / 2.0f;
                    positions[j].W = p.W;

                    varyings[j] = any.CopyToArray();
                }

                // Sort the three vertices of a triangle along the y axis from top to down. Simplifies the method that does the real work.

                int i0 = 0;
                int i1 = 1;
                int i2 = 2;

                if (positions[i0].Y > positions[i1].Y)
                {
                    Math.Swap(ref i0, ref i1);
                }
                if (positions[i1].Y > positions[i2].Y)
                {
                    Math.Swap(ref i1, ref i2);
                }
                if (positions[i0].Y > positions[i1].Y)
                {
                    Math.Swap(ref i0, ref i1);
                }

                DrawTrianglesSorted(positions[i0], positions[i1], positions[i2], varyings[i0], varyings[i1], varyings[i2], uniform, fragmentProgram, any);
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

        // some simple profiling test show me that the this method is not too slow even with 10000 triangles (20 fps), the app spends sometimes more than 50% in Clear and ToRGB methods.
        // TODO clean up this whole method and make it more generic, looks horrible yet
        private void DrawTrianglesSorted<U, W>(Vector4 p0, Vector4 p1, Vector4 p2, float[] v0, float[] v1, float[] v2, U uniforms, FragmentProgramDelegate<U, W> fragmentProgram, W any) where W : Varying
        {
            int length = v0.Length;

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
                for (int i = 0; i < length; i++)
                {
                    v0[i] *= w0;
                    v1[i] *= w0;
                    v2[i] *= w0;
                }
            }

            //w0 = 1.0f / w0;
            //w1 = 1.0f / w1;
            //w2 = 1.0f / w2;

            float slope0to1 = (float)(x0 - x1) / (float)(y0 - y1);
            float slope0to2 = (float)(x0 - x2) / (float)(y0 - y2);
            float slope1to2 = (float)(x1 - x2) / (float)(y1 - y2);

            float zslope0to1 = (z0 - z1) / (y0 - y1);
            float zslope0to2 = (z0 - z2) / (y0 - y2);
            float zslope1to2 = (z1 - z2) / (y1 - y2);

            float[] vslope0to1 = new float[length];
            float[] vslope0to2 = new float[length];
            float[] vslope1to2 = new float[length];

            for (int i = 0; i < length; i++)
            {
                vslope0to1[i] = (v0[i] - v1[i]) / (y0 - y1);
                vslope0to2[i] = (v0[i] - v2[i]) / (y0 - y2);
                vslope1to2[i] = (v1[i] - v2[i]) / (y1 - y2);
            }

            float[] v0to1 = new float[length];
            float[] v0to2 = new float[length];
            float[] v1to2 = new float[length];

            float[] vmin; //= new float[length];
            float[] vmax; //= new float[length];

            float[] mv = new float[length];
            float[] v = new float[length];


            //Vector4 cslope0to1 = (v0 - v1) / (y0 - y1);
            //Vector4 cslope0to2 = (v0 - v2) / (y0 - y2);
            //Vector4 cslope1to2 = (v1 - v2) / (y1 - y2);

            float wslope0to1 = (w0 - w1) / (y0 - y1);
            float wslope0to2 = (w0 - w2) / (y0 - y2);
            float wslope1to2 = (w1 - w2) / (y1 - y2);

            for (int y = y0; y < y1; y++)
            {
                // TODO move this mul out of this loop makes the app sometimes slower, more investigation needed
                int x0to1 = (int)(x1 + slope0to1 * (y - y1));
                int x0to2 = (int)(x2 + slope0to2 * (y - y2));

                float z0to1 = z1 + zslope0to1 * (y - y1);
                float z0to2 = z2 + zslope0to2 * (y - y2);

                for (int i = 0; i < length; i++)
                {
                    v0to1[i] = v1[i] + vslope0to1[i] * (y - y1);
                    v0to2[i] = v2[i] + vslope0to2[i] * (y - y2);
                }

                float w0to1 = w1 + wslope0to1 * (y - y1);
                float w0to2 = w2 + wslope0to2 * (y - y2);

                int xmin;
                int xmax;
                float zmin;
                float zmax;
                float wmin;
                float wmax;

                if (x0to1 > x0to2)
                {
                    xmax = x0to1;
                    xmin = x0to2;
                    zmax = z0to1;
                    zmin = z0to2;
                    vmax = v0to1;
                    vmin = v0to2;
                    wmax = w0to1;
                    wmin = w0to2;
                }
                else
                {
                    xmax = x0to2;
                    xmin = x0to1;
                    zmax = z0to2;
                    zmin = z0to1;
                    vmax = v0to2;
                    vmin = v0to1;
                    wmax = w0to2;
                    wmin = w0to1;
                }

                for (int i = 0; i < length; i++)
                {
                    mv[i] = (vmax[i] - vmin[i]) / (xmax - xmin);
                }

                float mz = (zmax - zmin) / (xmax - xmin);
                float mw = (wmax - wmin) / (xmax - xmin);
                for (int x = xmin; x < xmax; x++)
                {
                    float z = zmin + mz * (x - xmin);
                    float w = wmin + mw * (x - xmin);

                    for (int i = 0; i < length; i++)
                    {
                        v[i] = vmin[i] + mv[i] * (x - xmin);
                    }

                    w = 1.0f / w;

                    if (this.perspectiveCorrection)
                    {
                        for (int i = 0; i < length; i++)
                        {
                            v[i] *= w;
                        }
                    }

                    //Vector4 t = texture.GetCoordinate(c.X, c.Y);

                    if ((x >= 0) && (y >= 0) && (x < width) && (y < height) && (z >= 0.0f) && (z <= 1.0f))
                    {
                        if (z < depthBuffer.Raw[y, x])
                        {
                            //colorBuffer.Raw[y, x] = new Vector4(v[0], v[1], v[2], v[3]);
                            any.CopyFromArray(v);
                            colorBuffer.Raw[y, x] = fragmentProgram(uniforms, any);

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

                for (int i = 0; i < length; i++)
                {
                    v1to2[i] = v2[i] + vslope1to2[i] * (y - y2);
                    v0to2[i] = v2[i] + vslope0to2[i] * (y - y2);
                }

                float w1to2 = w2 + wslope1to2 * (y - y2);
                float w0to2 = w2 + wslope0to2 * (y - y2);

                int xmin;
                int xmax;
                float zmin;
                float zmax;
                float wmin;
                float wmax;

                if (x1to2 > x0to2)
                {
                    xmax = x1to2;
                    xmin = x0to2;
                    zmax = z1to2;
                    zmin = z0to2;
                    vmax = v1to2;
                    vmin = v0to2;
                    wmax = w1to2;
                    wmin = w0to2;
                }
                else
                {
                    xmax = x0to2;
                    xmin = x1to2;
                    zmax = z0to2;
                    zmin = z1to2;
                    vmax = v0to2;
                    vmin = v1to2;
                    wmax = w0to2;
                    wmin = w1to2;
                }

                for (int i = 0; i < length; i++)
                {
                    mv[i] = (vmax[i] - vmin[i]) / (xmax - xmin);
                }

                float mz = (zmax - zmin) / (xmax - xmin);
                float mw = (wmax - wmin) / (xmax - xmin);
                for (int x = xmin; x < xmax; x++)
                {
                    float z = zmin + mz * (x - xmin);
                    float w = wmin + mw * (x - xmin);
                    
                    for (int i = 0; i < length; i++)
                    {
                        v[i] = vmin[i] + mv[i] * (x - xmin);
                    }

                    w = 1.0f / w;

                    if (this.perspectiveCorrection)
                    {
                        for (int i = 0; i < length; i++)
                        {
                            v[i] *= w;
                        }
                    }

                    //Vector4 t = texture.GetCoordinate(c.X, c.Y);

                    if ((x >= 0) && (y >= 0) && (x < width) && (y < height) && (z >= 0.0f) && (z <= 1.0f))
                    {
                        if (z < depthBuffer.Raw[y, x])
                        {
                            //colorBuffer.Raw[y, x] = new Vector4(v[0], v[1], v[2], v[3]);
                            any.CopyFromArray(v);
                            colorBuffer.Raw[y, x] = fragmentProgram(uniforms, any);

                            depthBuffer.Raw[y, x] = z;
                        }
                    }
                }
            }
        }
    }
}
