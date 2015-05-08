using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;

namespace Chess.Graphics
{
    public static class MeshIndexer
    {
        public struct PackedVertex
        {
            public Vector3 position;
            public Vector2 uv;
            public Vector3 normal;
        }

        public static void IndexMesh(List<Vector3> inVertices, List<Vector2> inUvs, List<Vector3> inNormals,
            List<ushort> outIndices, List<Vector3> outVertices, List<Vector2> outUvs, List<Vector3> outNormals)
        {
            Dictionary<PackedVertex, ushort> VertexToOutIndex = new Dictionary<PackedVertex, ushort>();

            int vertexInSize = inVertices.Count;

            for (int i = 0; i < vertexInSize; ++i)
            {
                PackedVertex packed = new PackedVertex() { position = inVertices[i], normal = inNormals[i], uv = inUvs[i] };

                ushort index;
                bool found = VertexToOutIndex.TryGetValue(packed, out index);

                if (found)
                    outIndices.Add(index);
                else
                {
                    outVertices.Add(inVertices[i]);
                    outNormals.Add(inNormals[i]);
                    outUvs.Add(inUvs[i]);
                    ushort newindex = (ushort)(outVertices.Count - 1);
                    outIndices.Add(newindex);
                    VertexToOutIndex[packed] = newindex;
                }
            }
        }
    }
}
