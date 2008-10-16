using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using Aquila;

namespace Aquila
{
    public partial class Sample : Form
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Sample());
        }

        private Stopwatch sw = new Stopwatch();
        private double angle = 0.0;
        private Bitmap bitmap;
        private Vector4 clearColor = new Vector4(0.7, 0.8, 0.9, 1.0);
        private Vector4 clearDepth = new Vector4(1.0, 0.0, 0.0, 0.0); // TODO 1.0 is default OpenGL Z, should the user care about this?
        private Texture2 colorBuffer;
        private Texture2 depthBuffer;
        private Aquila aquila;
        private Matrix4 projection = new Matrix4();
        private Matrix4 translation = new Matrix4();
        private Matrix4 rotation = new Matrix4();
        private Matrix4 modelViewProjection = new Matrix4();
        private PositionColorVertex[] vertices = new PositionColorVertex[6];

        public Sample()
        {
            InitializeComponent();

            int width = 640;
            int height = 480;

            bitmap = new Bitmap(width, height);
            colorBuffer = new Texture2(width, height);
            depthBuffer = new Texture2(width, height);
            aquila = new Aquila(colorBuffer, depthBuffer);

            pictureBox1.Image = bitmap;

            // colorized so colors could be used as texture coordinates

            vertices[0] = new PositionColorVertex(new Vector4(-1.0, -1.0, -1.0, 1.0), new Vector4(0.0, 0.0, 0.0, 1.0));
            vertices[1] = new PositionColorVertex(new Vector4(+1.0, -1.0, -1.0, 1.0), new Vector4(0.0, 1.0, 0.0, 1.0));
            vertices[2] = new PositionColorVertex(new Vector4(+1.0, -1.0, +1.0, 1.0), new Vector4(1.0, 1.0, 0.0, 1.0));
            vertices[3] = new PositionColorVertex(new Vector4(-1.0, -1.0, -1.0, 1.0), new Vector4(0.0, 0.0, 0.0, 1.0));
            vertices[4] = new PositionColorVertex(new Vector4(+1.0, -1.0, +1.0, 1.0), new Vector4(1.0, 1.0, 0.0, 1.0));
            vertices[5] = new PositionColorVertex(new Vector4(-1.0, -1.0, +1.0, 1.0), new Vector4(1.0, 0.0, 0.0, 1.0));

            // gradient (perspective correction is better visible)

            vertices[0] = new PositionColorVertex(new Vector4(-1.0, -1.0, -1.0, 1.0), new Vector4(1.0, 0.0, 0.0, 1.0));
            vertices[1] = new PositionColorVertex(new Vector4(+1.0, -1.0, -1.0, 1.0), new Vector4(1.0, 0.0, 0.0, 1.0));
            vertices[2] = new PositionColorVertex(new Vector4(+1.0, -1.0, +1.0, 1.0), new Vector4(0.0, 0.0, 1.0, 1.0));
            vertices[3] = new PositionColorVertex(new Vector4(-1.0, -1.0, -1.0, 1.0), new Vector4(1.0, 0.0, 0.0, 1.0));
            vertices[4] = new PositionColorVertex(new Vector4(+1.0, -1.0, +1.0, 1.0), new Vector4(0.0, 0.0, 1.0, 1.0));
            vertices[5] = new PositionColorVertex(new Vector4(-1.0, -1.0, +1.0, 1.0), new Vector4(0.0, 0.0, 1.0, 1.0));

            // load external file

            //verts = WavefrontObject.Load("cube.obj");
            //vertices = WavefrontObject.Load("sphere.obj");
            //vertices = WavefrontObject.Load("D://monkey.obj");
        }

        private Vector4 VertexProgramColor(Matrix4 modelViewProjection, PositionColorVertex vertex, ColorVarying varying)
        {
            Vector4 position = modelViewProjection * vertex.position;
            varying.color = vertex.color;
            return position;
        }

        // TODO not called yet
        private Vector4 FragmentProgramColor(Matrix4 modelViewProjection, ColorVarying varying)
        {
            Vector4 color = varying.color;
            return color;
        }

        // TODO fov is not exactly like Panda3D, currently don't know why
        private void Render()
        {
            double fov = Math.DegreeToRadian(45.0);
            double aspectRatio = (double)colorBuffer.Width / (double)colorBuffer.Height;

            // to test the near and far plane we limit the z range
            projection = MatrixUtility.Perspective(fov, aspectRatio, 2.7, 5.3);
            translation = MatrixUtility.Translate(new Vector3(0.0, 0.0, -4.0));
            rotation = MatrixUtility.Rotate(angle, new Vector3(0.0, 1.0, 0.0));

            modelViewProjection.Identity();
            modelViewProjection.Multiply(projection);
            modelViewProjection.Multiply(translation);
            modelViewProjection.Multiply(rotation);

            colorBuffer.Clear(clearColor);
            depthBuffer.Clear(clearDepth);

            aquila.DrawTriangles(VertexProgramColor, FragmentProgramColor, modelViewProjection, vertices, new ColorVarying());

            TextureUtility.SaveToRGB(colorBuffer, bitmap);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            angle += Math.DegreeToRadian(45.0);
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            sw.Reset();
            sw.Start();

            Render();

            sw.Stop();

            label1.Text = sw.Elapsed.TotalMilliseconds + " ms";

            pictureBox1.Invalidate();
        }
    }
}