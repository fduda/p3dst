namespace Aquila
{
    public class MatrixUtility
    {
        // http://www.opengl.org/sdk/docs/man/xhtml/glTranslate.xml
        public static Matrix4x4 Translate(Vector3 vector)
        {
            Matrix4x4 matrix = new Matrix4x4();

            double x = vector.X;
            double y = vector.Y;
            double z = vector.Z;

            matrix.E00 = 1.0; matrix.E01 = 0.0; matrix.E02 = 0.0; matrix.E03 = x;
            matrix.E10 = 0.0; matrix.E11 = 1.0; matrix.E12 = 0.0; matrix.E13 = y;
            matrix.E20 = 0.0; matrix.E21 = 0.0; matrix.E22 = 1.0; matrix.E23 = z;
            matrix.E30 = 0.0; matrix.E31 = 0.0; matrix.E32 = 0.0; matrix.E33 = 1.0;

            return matrix;
        }

        // http://www.opengl.org/sdk/docs/man/xhtml/glRotate.xml
        public static Matrix4x4 Rotate(double radians, Vector3 vector)
        {
            Matrix4x4 matrix = new Matrix4x4();

            double x = vector.X;
            double y = vector.Y;
            double z = vector.Z;

            double c = Math.Cos(radians);
            double s = Math.Sin(radians);

            matrix.E00 = x * x * (1.0 - c) + c;
            matrix.E01 = x * y * (1.0 - c) - z * s;
            matrix.E02 = x * z * (1.0 - c) + y * s;
            matrix.E03 = 0.0;

            matrix.E10 = y * x * (1.0 - c) + z * s;
            matrix.E11 = y * y * (1.0 - c) + c;
            matrix.E12 = y * z * (1.0 - c) - x * s;
            matrix.E13 = 0.0;

            matrix.E20 = x * z * (1.0 - c) - y * s;
            matrix.E21 = y * z * (1.0 - c) + x * s;
            matrix.E22 = z * z * (1.0 - c) + c;
            matrix.E23 = 0.0;

            matrix.E30 = 0.0;
            matrix.E31 = 0.0;
            matrix.E32 = 0.0;
            matrix.E33 = 1.0;

            return matrix;
        }

        // http://www.opengl.org/sdk/docs/man/xhtml/gluLookAt.xml
        public static Matrix4x4 LookAt(Vector3 eye, Vector3 center, Vector3 up)
        {
            Matrix4x4 matrix = new Matrix4x4();

            Vector3 f = new Vector3(center.X - eye.X, center.Y - eye.Y, center.Z - eye.Z);
            f.Normalize();

            up.Normalize();

            Vector3 s = Vector3.Cross(f, up);
            Vector3 u = Vector3.Cross(s, f);

            matrix.E00 = s.X;
            matrix.E01 = s.Y;
            matrix.E02 = s.Z;
            matrix.E03 = 0.0;

            matrix.E10 = u.X;
            matrix.E11 = u.Y;
            matrix.E12 = u.Z;
            matrix.E13 = 0.0;

            matrix.E20 = -f.X;
            matrix.E21 = -f.Y;
            matrix.E22 = -f.Z;
            matrix.E23 = 0.0;

            matrix.E30 = 0.0;
            matrix.E31 = 0.0;
            matrix.E32 = 0.0;
            matrix.E33 = 1.0;

            return matrix;
        }

        // http://www.opengl.org/sdk/docs/man/xhtml/gluPerspective.xml
        public static Matrix4x4 Perspective(double fieldOfView, double aspectRatio, double zNear, double zFar)
        {
            Matrix4x4 matrix = new Matrix4x4();

            double f = Math.Cot(fieldOfView / 2.0);

            matrix.E00 = f / aspectRatio;
            matrix.E01 = 0.0;
            matrix.E02 = 0.0;
            matrix.E03 = 0.0;

            matrix.E10 = 0.0;
            matrix.E11 = f;
            matrix.E12 = 0.0;
            matrix.E13 = 0.0;

            matrix.E20 = 0.0;
            matrix.E21 = 0.0;
            matrix.E22 = (zFar + zNear) / (zNear - zFar);
            matrix.E23 = (2 * zFar * zNear) / (zNear - zFar);

            matrix.E30 = 0.0;
            matrix.E31 = 0.0;
            matrix.E32 = -1.0;
            matrix.E33 = 0.0;

            return matrix;
        }
    }
}
