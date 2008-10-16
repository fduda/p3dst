using System.Collections.Generic;
using System.IO;

namespace Aquila
{
    /// <summary>
    /// The Wavefront Object file format is not that hard to understand.
    /// Drawback is that is does not support per vertex colors.
    /// 
    /// http://www.royriggs.com/obj.html
    /// 
    /// Currently this class only supports trinagulated meshes, without any
    /// back references. Therefore this it NOT a reference implementation.
    /// Vertex positions are translated to colors, one more fancy behaviour a
    /// serious loader never would have.
    /// </summary>
    public class WavefrontObject
    {
        public static PositionColorVertex[] Load(string filename)
        {
            List<float> vertices = new List<float>();
            List<int> faces = new List<int>();

            StreamReader sr = new StreamReader(filename);
            while (!sr.EndOfStream)
            {
                string s = sr.ReadLine();
                if (s.StartsWith("#"))
                {
                    continue;
                }
                string[] parts = s.Split(' ');

                if (parts[0] == "v")
                {
                    vertices.Add(float.Parse(parts[1]));
                    vertices.Add(float.Parse(parts[2]));
                    vertices.Add(float.Parse(parts[3]));
                }
                else if (parts[0] == "f")
                {
                    faces.Add(int.Parse(parts[1]));
                    faces.Add(int.Parse(parts[2]));
                    faces.Add(int.Parse(parts[3]));
                }

            }
            sr.Close();

            PositionColorVertex[] result = new PositionColorVertex[faces.Count];

            for (int i = 0; i < faces.Count; i++)
            {
                // face numbering starts with 1 in obj files
                int n = (faces[i] - 1) * 3;

                Vector4 position = new Vector4();
                position.X = vertices[n + 0];
                position.Y = vertices[n + 1];
                position.Z = vertices[n + 2];
                position.W = 1.0f;

                Vector4 color = new Vector4();
                color.R = Math.Saturate(position.X * 0.5f + 0.5f);
                color.G = Math.Saturate(position.Y * 0.5f + 0.5f);
                color.B = Math.Saturate(position.Z * 0.5f + 0.5f);
                color.A = 1.0f;

                result[i] = new PositionColorVertex(position, color);
            }

            return result;
        }
    }
}
