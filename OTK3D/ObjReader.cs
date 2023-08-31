using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OTK3D
{
    internal class ObjReader
    {
        public static void ReadFile(string path, out uint[] indices, out float[] vxs)
        {
            var text = File.ReadAllLines(path);
            int readyPoly = 0;
            vxs = null;
            indices = null;
            List<float[]> vertexes = new List<float[]>();
            List<float[]> normals = new List<float[]>();
            List<float[]> texture = new List<float[]>();
            List<uint> ids = new List<uint>();

            List<int[]> pairs = new List<int[]>();

            foreach (var line in text)
            {
                var parts = line.Split(' ');

                float[] arr;
                switch (parts[0])
                {
                    case "v":
                        arr = ParceArray(parts);
                        vertexes.Add(arr);
                        break;
                    case "vt":
                        arr = ParceArray(parts);
                        texture.Add(arr);
                        break;
                    //case "vn":
                    //    arr = ParceArray(parts);
                    //    normals.Add(arr);
                    //    break;
                    case "f":
                        var data = parts.Skip(1).Select(i => i.Split('/').Select(i => int.Parse(i)).ToArray()).ToArray();
                        ids.Add((uint)data[0][0] - 1);
                        ids.Add((uint)data[1][0] - 1);
                        ids.Add((uint)data[2][0] - 1);
                        pairs.AddRange(data);
                        break;
                }
            }
            pairs = pairs.DistinctBy(i => i[0].ToString() + "n" + i[1].ToString()).ToList();
            pairs = pairs.OrderBy(i => i[0]).ToList();
            vxs = new float[pairs.Count * 5];
            foreach (var pair in pairs)
            {
                var totalVertex = new float[5];
                vertexes[pair[0] - 1].CopyTo(totalVertex, 0);
                texture[pair[1] - 1].CopyTo(totalVertex, 3);
                totalVertex.CopyTo(vxs, 5 * (pair[0] - 1));
            }
            indices = ids.ToArray();
        }
        public static float[] ParceArray(string[] nums)
        {
            var ret = new float[nums.Length - 1];
            for (int i = 1; i < nums.Length; i++)
            {
                ret[i - 1] = Single.Parse(nums[i], CultureInfo.InvariantCulture);
            }
            return ret;
        }
    }
}
