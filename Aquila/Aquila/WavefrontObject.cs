using System.Collections.Generic;
using System.IO;

namespace Aquila
{
    public class WavefrontObject
    {
        // TODO Maybe It is a good idea to define some a standard vertex shader and one with tons of vertex coords and the like. Arrays are somewhat unreadable. Such a class can the implement some kind of an interpolator.
        // TODO Only triangulated obj files are supported
        public static Vector4[][] Load(string filename)
        {
            List<double> vertices = new List<double>();
            List<int> faces = new List<int>();

            StreamReader sr = new StreamReader(filename);
            while (!sr.EndOfStream)
            {
                string s = sr.ReadLine();
                if(s.StartsWith("#"))
                {
                    continue;
                }
                string[] parts = s.Split(' ');

                if (parts[0] == "v")
                {
                    vertices.Add(double.Parse(parts[1]));
                    vertices.Add(double.Parse(parts[2]));
                    vertices.Add(double.Parse(parts[3]));
                }
                else if (parts[0] == "f")
                {
                    faces.Add(int.Parse(parts[1]));
                    faces.Add(int.Parse(parts[2]));
                    faces.Add(int.Parse(parts[3]));
                }

            }
            sr.Close();

            int count = faces.Count;

            Vector4[][] result = new Vector4[count][];

            for (int i = 0; i < count; i++)
            {
                // face numbering starts with 1 in obj files
                int n = (faces[i] - 1) * 3;
                double x = vertices[n + 0];
                double y = vertices[n + 1];
                double z = vertices[n + 2];
                double r = Math.Saturate(x);
                double g = Math.Saturate(y);
                double b = Math.Saturate(z);
                result[i] = new Vector4[] { new Vector4(x, y, z, 1.0), new Vector4(r, g, b, 1.0) };
            }

            return result;
        }
    }
}
