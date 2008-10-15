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

        Stopwatch sw = new Stopwatch();
        double angle = 0.0;
        Vector4 colorBufferClear = new Vector4(0.7, 0.8, 0.9, 1.0);

        Bitmap bitmap;
        Texture2 colorBuffer;
        private Aquila aquila;

        Vector4[] vertices = new Vector4[6];
        Vector4[][] attributes = new Vector4[6][];

        Matrix4x4 projection = new Matrix4x4();
        Matrix4x4 translation = new Matrix4x4();
        Matrix4x4 rotation = new Matrix4x4();
        Matrix4x4 modelViewProjection = new Matrix4x4();
        Matrix4x4 system = new Matrix4x4();

        public Sample()
        {
            /*
            for (int i = -5; i <= 5; i++)
            {
                int x = i % (2 * 2);
            }
            */

            //Test.T4();

            InitializeComponent();

            int width = 640;
            int height = 480;

            bitmap = new Bitmap(width, height);
            colorBuffer = new Texture2(width, height);
            aquila = new Aquila(colorBuffer);

            pictureBox1.Image = bitmap;

            vertices[0] = new Vector4(-1.0, -1.0, -1.0, 1.0);
            vertices[1] = new Vector4(+1.0, -1.0, -1.0, 1.0);
            vertices[2] = new Vector4(+1.0, +1.0, -1.0, 1.0);

            vertices[3] = new Vector4(-1.0, -1.0, -1.0, 1.0);
            vertices[4] = new Vector4(+1.0, +1.0, -1.0, 1.0);
            vertices[5] = new Vector4(-1.0, +1.0, -1.0, 1.0);

            vertices[0] = new Vector4(-1.0, -1.0, -1.0, 1.0);
            vertices[1] = new Vector4(+1.0, -1.0, -1.0, 1.0);
            vertices[2] = new Vector4(+1.0, -1.0, +1.0, 1.0);

            vertices[3] = new Vector4(-1.0, -1.0, -1.0, 1.0);
            vertices[4] = new Vector4(+1.0, -1.0, +1.0, 1.0);
            vertices[5] = new Vector4(-1.0, -1.0, +1.0, 1.0);

            // UV like

            attributes[0] = new Vector4[] { new Vector4(0.0, 0.0, 0.0, 1.0) };
            attributes[1] = new Vector4[] { new Vector4(0.0, 1.0, 0.0, 1.0) };
            attributes[2] = new Vector4[] { new Vector4(1.0, 1.0, 0.0, 1.0) };

            attributes[3] = new Vector4[] { new Vector4(0.0, 0.0, 0.0, 1.0) };
            attributes[4] = new Vector4[] { new Vector4(1.0, 1.0, 0.0, 1.0) };
            attributes[5] = new Vector4[] { new Vector4(1.0, 0.0, 0.0, 1.0) };

            // Gradient

            attributes[0] = new Vector4[] { new Vector4(1.0, 0.0, 0.0, 1.0) };
            attributes[1] = new Vector4[] { new Vector4(1.0, 0.0, 0.0, 1.0) };
            attributes[2] = new Vector4[] { new Vector4(0.0, 0.0, 1.0, 1.0) };

            attributes[3] = new Vector4[] { new Vector4(1.0, 0.0, 0.0, 1.0) };
            attributes[4] = new Vector4[] { new Vector4(0.0, 0.0, 1.0, 1.0) };
            attributes[5] = new Vector4[] { new Vector4(0.0, 0.0, 1.0, 1.0) };

            //TestAccurary();
            //Test.T1();
            //Test.T2();

            //Vector4 v1 = new Vector4(1.0, 2.0, 3.0, 4.0);
            //Vector4 v2 = v1 * 2.0;
            //MessageBox.Show(v1.Pretty() + " " + v2.Pretty());
            //v1.Multiply(2.0);
            //MessageBox.Show(v1.Pretty() + " " + v2.Pretty());

            //Matrix4x4 m = aquila.matrix;
            //Vector4 color = new Vector4(0.1, 0.2, 0.4, 1.0);

            //Matrix4x4 mat = new Matrix4x4();
            //mat.CreateRotate(-Math.DegreeToRadian(90), new Vector3(1.0, 0.0, 0.0));
            //MessageBox.Show(mat.Pretty());

            //Vector3 vec1 = new Vector3(2.0, 0.0, 0.0);
            //Vector3 vec2 = vec1;
            //vec2.Normalize();
            //MessageBox.Show(vec1.Pretty() + " " + vec2.Pretty());
            //aquila.ClearcolorBuffer(c);

            //RenderTest();
        }

        private void RenderTest()
        {
            for (int i = 0; i < 1000; i++)
            {
                Render();
            }
            Environment.Exit(1);
        }

        private void Render()
        {
            //Graphics g = Graphics.FromHwnd(panel1.Handle);
            //g.DrawImage(bitmap, 0, 0);
            //g.Dispose();

            //angle += 0.05;

            double fov = Math.DegreeToRadian(45.0);
            double aspectRatio = (double)colorBuffer.Width / (double)colorBuffer.Height;

            projection = MatrixUtility.Perspective(fov, aspectRatio, 3.5, 4.5);
            //projection = MatrixUtility.Perspective(fov, aspectRatio, 3.5, 4.5);
            //system = MatrixUtility.Rotate(Math.DegreeToRadian(90.0), new Vector3(1.0, 0.0, 0.0));
            //translation = MatrixUtility.Translate(new Vector3(0.0, 4.0, 0.0));
            //rotation = MatrixUtility.Rotate(angle, new Vector3(0.0, 0.0, 1.0));
            translation = MatrixUtility.Translate(new Vector3(0.0, 0.0, -4.0));
            rotation = MatrixUtility.Rotate(angle, new Vector3(0.0, 1.0, 0.0));

            modelViewProjection.Identity();
            modelViewProjection.Multiply(projection);
            //modelViewProjection.Multiply(system);
            modelViewProjection.Multiply(translation);
            modelViewProjection.Multiply(rotation);

            label2.Text = system.Pretty();

            colorBuffer.Clear(colorBufferClear);
            aquila.Draw(modelViewProjection, vertices, attributes);

            TextureUtility.SaveToRGB(colorBuffer, bitmap);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            angle += System.Math.PI / 8.0;
        }

        private void button2_Click(object sender, EventArgs e)
        {
        }

        private void button3_Click(object sender, EventArgs e)
        {
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