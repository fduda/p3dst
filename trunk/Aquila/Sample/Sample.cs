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
        private float angle = 0.0f;
        private Bitmap bitmap;
        private Vector4 clearColor = new Vector4(0.7f, 0.8f, 0.9f, 1.0f);
        private float clearDepth = 1.0f; // TODO 1.0f is default OpenGL Z, should the user care about this?
        private Texture2<Vector4> colorBuffer;
        private Texture2<float> depthBuffer;
        private Aquila aquila;
        private Matrix4 perspective = new Matrix4();
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
            colorBuffer = new Texture2<Vector4>(width, height);
            depthBuffer = new Texture2<float>(width, height);
            aquila = new Aquila(colorBuffer, depthBuffer);

            pictureBox1.Image = bitmap;

            // colorized so colors could be used as texture coordinates

            vertices[0] = new PositionColorVertex(new Vector4(-1.0f, -1.0f, -1.0f, 1.0f), new Vector4(0.0f, 0.0f, 0.0f, 1.0f));
            vertices[1] = new PositionColorVertex(new Vector4(+1.0f, -1.0f, -1.0f, 1.0f), new Vector4(0.0f, 1.0f, 0.0f, 1.0f));
            vertices[2] = new PositionColorVertex(new Vector4(+1.0f, -1.0f, +1.0f, 1.0f), new Vector4(1.0f, 1.0f, 0.0f, 1.0f));
            vertices[3] = new PositionColorVertex(new Vector4(-1.0f, -1.0f, -1.0f, 1.0f), new Vector4(0.0f, 0.0f, 0.0f, 1.0f));
            vertices[4] = new PositionColorVertex(new Vector4(+1.0f, -1.0f, +1.0f, 1.0f), new Vector4(1.0f, 1.0f, 0.0f, 1.0f));
            vertices[5] = new PositionColorVertex(new Vector4(-1.0f, -1.0f, +1.0f, 1.0f), new Vector4(1.0f, 0.0f, 0.0f, 1.0f));

            // gradient (perspective correction is better visible)

            vertices[0] = new PositionColorVertex(new Vector4(-1.0f, -1.0f, -1.0f, 1.0f), new Vector4(1.0f, 0.0f, 0.0f, 1.0f));
            vertices[1] = new PositionColorVertex(new Vector4(+1.0f, -1.0f, -1.0f, 1.0f), new Vector4(1.0f, 0.0f, 0.0f, 1.0f));
            vertices[2] = new PositionColorVertex(new Vector4(+1.0f, -1.0f, +1.0f, 1.0f), new Vector4(0.0f, 0.0f, 1.0f, 1.0f));
            vertices[3] = new PositionColorVertex(new Vector4(-1.0f, -1.0f, -1.0f, 1.0f), new Vector4(1.0f, 0.0f, 0.0f, 1.0f));
            vertices[4] = new PositionColorVertex(new Vector4(+1.0f, -1.0f, +1.0f, 1.0f), new Vector4(0.0f, 0.0f, 1.0f, 1.0f));
            vertices[5] = new PositionColorVertex(new Vector4(-1.0f, -1.0f, +1.0f, 1.0f), new Vector4(0.0f, 0.0f, 1.0f, 1.0f));

            // load external file

            //vertices = WavefrontObject.Load("cube.obj"); // 36 vertices
            //vertices = WavefrontObject.Load("sphere.obj"); // 15000 vertices
            vertices = WavefrontObject.Load("monkey.obj"); // 188000 vertices

            label1.Text = vertices.Length + " vertices";
        }

        private Vector4 VertexProgramColor(MatrixUniform uniform, PositionColorVertex vertex, ColorVarying varying)
        {
            Vector4 position = uniform.modelViewProjection * vertex.position;
            varying.color = vertex.color;
            return position;
        }

        // TODO not called yet
        private Vector4 FragmentProgramColor(MatrixUniform uniform, ColorVarying varying)
        {
            Vector4 color = varying.color;
            return color;
        }

        // TODO FOV is not exactly like Panda3D, currently don't know why
        private void Render()
        {
            float fov = Math.DegreeToRadian(45.0f);
            float aspectRatio = (float)colorBuffer.Width / (float)colorBuffer.Height;

            // to test the near and far plane we limit the z range
            perspective = MatrixUtility.Perspective(fov, aspectRatio, 2.7f, 5.3f);
            translation = MatrixUtility.Translate(new Vector3(0.0f, 0.0f, -4.0f));
            rotation = MatrixUtility.Rotate(angle, new Vector3(0.0f, 1.0f, 0.0f));

            modelViewProjection.Identity();
            modelViewProjection.Multiply(perspective);
            modelViewProjection.Multiply(translation);
            modelViewProjection.Multiply(rotation);

            colorBuffer.Clear(clearColor);
            depthBuffer.Clear(clearDepth);

            MatrixUniform uniform = new MatrixUniform();
            uniform.modelViewProjection = modelViewProjection;

            aquila.DrawTriangles(VertexProgramColor, FragmentProgramColor, uniform, vertices, new ColorVarying());

            TextureUtility.SaveToRGB(colorBuffer, bitmap);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            angle += Math.DegreeToRadian(22.5f);
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            sw.Reset();
            sw.Start();

            Render();

            sw.Stop();

            label2.Text = sw.Elapsed.TotalMilliseconds + " ms";

            pictureBox1.Invalidate();
        }
    }
}