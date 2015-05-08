using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

using OpenTK;
using OpenTK.Graphics.OpenGL;
using OpenTK.Graphics;

namespace Chess.Graphics
{
    public class VertexGroup
    {
        public List<Vector3> Vertices = new List<Vector3>();
        public List<Vector2> Uvs = new List<Vector2>();
        public List<Vector3> Normals = new List<Vector3>();
        public string Name;
    }

    public class IndexedVertexGroup
    {
        public List<Vector3> Vertices = new List<Vector3>();
        public List<Vector2> Uvs = new List<Vector2>();
        public List<Vector3> Normals = new List<Vector3>();
        public List<ushort> Indices = new List<ushort>();
        public string Name;
    }

    public class IndexDataObj
    {
        public List<int> VertexIndices = new List<int>();
        public List<int> UvIndices = new List<int>();
        public List<int> NormalIndices = new List<int>();
    }

    public class Mesh : IDisposable
    {
        protected int[] vertexAttribBuffers;
        protected int[] vertexAttribSizes;
        protected int[] vertexStrides;
        protected VertexAttribPointerType[] pointerTypes;
        protected BeginMode drawMode = BeginMode.Triangles;
        protected int indexBuffer;
        protected int vertexCount;
        protected int bufferSize;
        protected int index;
        protected int count;

        public int[] AttributeBuffers
        {
            get { return vertexAttribBuffers; }
        }

        public int[] AttributeElementSizes
        {
            get { return vertexAttribSizes; }
        }

        public VertexAttribPointerType[] PointerTypes
        {
            get { return pointerTypes; }
        }

        public int[] VertexStrides
        {
            get { return vertexStrides; }
        }

        public int IndexBuffer
        {
            get { return indexBuffer; }
        }

        public int VertexCount
        {
            get { return vertexCount; }
        }

        public int BufferSize
        {
            get { return bufferSize; }
        }

        public BeginMode DrawMode
        {
            get { return drawMode; }
            set { drawMode = value; }
        }

        public Mesh()
        {
        }

        public Mesh(int attributeBufferSizes, int attributeBufferCount)
        {
            CreateAttribBuffers(attributeBufferSizes, attributeBufferCount);
        }

        ~Mesh()
        {
            Dispose(false);
        }

        public void CreateAttribBuffers(int attributeBufferSizes, int attributeBufferCount)
        {
            bufferSize = attributeBufferSizes;
            vertexCount = attributeBufferSizes;

            vertexAttribBuffers = new int[attributeBufferCount];
            vertexAttribSizes = new int[attributeBufferCount];
            vertexStrides = new int[attributeBufferCount];
            pointerTypes = new VertexAttribPointerType[attributeBufferCount];
        }

        public void SetIndexBuffer(int indexBuffer, int indexBufferSize)
        {
            vertexCount = indexBufferSize;
            this.indexBuffer = indexBuffer;
        }

        public void EnableAttributeArrays()
        {
            for (index = 0, count = vertexAttribBuffers.Length; index < count; ++index)
            {
                GL.EnableVertexAttribArray(index);
                GL.BindBuffer(BufferTarget.ArrayBuffer, vertexAttribBuffers[index]);
                GL.VertexAttribPointer(index, vertexAttribSizes[index], pointerTypes[index], false, vertexStrides[index], 0);
            }

            if (indexBuffer != 0)
                GL.BindBuffer(BufferTarget.ElementArrayBuffer, IndexBuffer);
        }

        public void DisableAttributeArrays()
        {
            if (indexBuffer != 0)
                GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);

            for (index = 0, count = vertexAttribBuffers.Length; index < count; ++index)
                GL.DisableVertexAttribArray(index);
        }

        public void Draw()
        {
            if (IndexBuffer == 0)
                GL.DrawArrays(drawMode, 0, vertexCount);
            else
                GL.DrawElements(drawMode, vertexCount, DrawElementsType.UnsignedShort, 0);
        }

        protected void Dispose(bool bFinalize)
        {
            for (index = 0, count = vertexAttribBuffers.Length; index < count; ++index)
                GL.DeleteBuffers(1, ref vertexAttribBuffers[index]);

            if (indexBuffer != 0)
                GL.DeleteBuffers(1, ref indexBuffer);

            if (bFinalize)
                GC.SuppressFinalize(this);
        }

        public void Dispose()
        {
            Dispose(true);
        }

        public static Mesh FromVertexLists(List<Vector3> vertices, List<Vector2> uvs, List<Vector3> normals, List<ushort> indices)
        {
            Mesh mesh = new Mesh(vertices.Count, 3);

            int vertexBuffer;
            int uvBuffer;
            int normalBuffer;
            int indexBuffer;

            // Create buffers
            GL.GenBuffers(1, out vertexBuffer);
            GL.BindBuffer(BufferTarget.ArrayBuffer, vertexBuffer);
            GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(vertices.Count * Vector3.SizeInBytes),
                vertices.ToArray(), BufferUsageHint.StaticDraw);

            GL.GenBuffers(1, out uvBuffer);
            GL.BindBuffer(BufferTarget.ArrayBuffer, uvBuffer);
            GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(uvs.Count * Vector2.SizeInBytes),
                uvs.ToArray(), BufferUsageHint.StaticDraw);

            GL.GenBuffers(1, out normalBuffer);
            GL.BindBuffer(BufferTarget.ArrayBuffer, normalBuffer);
            GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(normals.Count * Vector3.SizeInBytes),
                normals.ToArray(), BufferUsageHint.StaticDraw);

            GL.GenBuffers(1, out indexBuffer);
            GL.BindBuffer(BufferTarget.ArrayBuffer, indexBuffer);
            GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(indices.Count * sizeof(ushort)),
                indices.ToArray(), BufferUsageHint.StaticDraw);

            // Set model parameters
            mesh.vertexAttribBuffers[0] = vertexBuffer;
            mesh.pointerTypes[0] = VertexAttribPointerType.Float;
            mesh.vertexAttribSizes[0] = 3;
            mesh.vertexAttribBuffers[1] = uvBuffer;
            mesh.pointerTypes[1] = VertexAttribPointerType.Float;
            mesh.vertexAttribSizes[1] = 2;
            mesh.vertexAttribBuffers[2] = normalBuffer;
            mesh.pointerTypes[2] = VertexAttribPointerType.Float;
            mesh.vertexAttribSizes[2] = 3;

            // Set index buffer
            mesh.SetIndexBuffer(indexBuffer, indices.Count);

            return mesh;
        }

        public static Mesh FromFile(string filename)
        {
            List<Vector3> vertices = new List<Vector3>();
            List<Vector2> uvs = new List<Vector2>();
            List<Vector3> normals = new List<Vector3>();
            List<ushort> indices = new List<ushort>();

            // Load data from file
            Mesh.LoadIndexedObj(filename, vertices, uvs, normals, indices);

            return Mesh.FromVertexLists(vertices, uvs, normals, indices);
        }

        public static void LoadObj(string source, List<Vector3> vertices, List<Vector2> uvs, List<Vector3> normals)
        {
            var vertexIndices = new List<int>();
            var uvIndices = new List<int>();
            var normalIndices = new List<int>();
            var tempVertices = new List<Vector3>();
            var tempUVs = new List<Vector2>();
            var tempNormals = new List<Vector3>();

            string line = string.Empty;
            string lineHeader = string.Empty;
            StreamReader file;
            char[] splitters = new char[] { ' ', '/' };

            try
            {
                file = new StreamReader(source);
            }
            catch (Exception e)
            {
                throw e;
            }

            try
            {
                while (!file.EndOfStream)
                {
                    line = file.ReadLine();
                    string[] split = line.Split(splitters, StringSplitOptions.RemoveEmptyEntries);

                    if (split.Length == 0)
                        continue;

                    if (split[0] == "v")
                    {
                        tempVertices.Add(new Vector3(Single.Parse(split[1]), Single.Parse(split[2]), Single.Parse(split[3])));
                    }
                    else if (split[0] == "vt")
                    {
                        tempUVs.Add(new Vector2(Single.Parse(split[1]), 1f - Single.Parse(split[2])));
                    }
                    else if (split[0] == "vn")
                    {
                        tempNormals.Add(new Vector3(Single.Parse(split[1]), Single.Parse(split[2]), Single.Parse(split[3])));
                    }
                    else if (split[0] == "f")
                    {
                        vertexIndices.Add(Int32.Parse(split[1]));
                        uvIndices.Add(Int32.Parse(split[2]));
                        normalIndices.Add(Int32.Parse(split[3]));

                        vertexIndices.Add(Int32.Parse(split[4]));
                        uvIndices.Add(Int32.Parse(split[5]));
                        normalIndices.Add(Int32.Parse(split[6]));

                        vertexIndices.Add(Int32.Parse(split[7]));
                        uvIndices.Add(Int32.Parse(split[8]));
                        normalIndices.Add(Int32.Parse(split[9]));
                    }
                }
            }
            catch (Exception e)
            {
                throw e;
            }

            file.Close();

            int vertexIndex = 0;
            int uvIndex = 0;
            int normalIndex = 0;

            Vector3 vertex;
            Vector3 normal;
            Vector2 uv;

            for (int i = 0, count = vertexIndices.Count; i < count; ++i)
            {
                vertexIndex = vertexIndices[i];
                uvIndex = uvIndices[i];
                normalIndex = normalIndices[i];

                vertex = tempVertices[vertexIndex - 1];
                vertices.Add(vertex);

                normal = tempNormals[normalIndex - 1];
                normals.Add(normal);

                uv = tempUVs[uvIndex - 1];
                uvs.Add(uv);
            }
        }

        public static List<VertexGroup> LoadMultiMeshObj(string source)
        {
            List<VertexGroup> data = new List<VertexGroup>();
            List<IndexDataObj> indexData = new List<IndexDataObj>();
            IndexDataObj currentIndexData = new IndexDataObj();

            StreamReader file;

            string line = string.Empty;
            string lineHeader = string.Empty;
            char[] splitters = new char[] { ' ', '/' };

            var tempVertices = new List<Vector3>();
            var tempUVs = new List<Vector2>();
            var tempNormals = new List<Vector3>();

            try
            {
                file = new StreamReader(source);
            }
            catch (Exception e)
            {
                throw e;
            }

            indexData.Add(currentIndexData);

            try
            {
                while (!file.EndOfStream)
                {
                    line = file.ReadLine();
                    string[] split = line.Split(splitters, StringSplitOptions.RemoveEmptyEntries);

                    if (split.Length == 0)
                        continue;

                    if (split[0] == "v")
                    {
                        tempVertices.Add(new Vector3(Single.Parse(split[1]), Single.Parse(split[2]), Single.Parse(split[3])));
                    }
                    else if (split[0] == "vt")
                    {
                        tempUVs.Add(new Vector2(Single.Parse(split[1]), 1f - Single.Parse(split[2])));
                    }
                    else if (split[0] == "vn")
                    {
                        tempNormals.Add(new Vector3(Single.Parse(split[1]), Single.Parse(split[2]), Single.Parse(split[3])));
                    }
                    else if (split[0] == "g")
                    {
                        // Recylce old group
                        if (currentIndexData.VertexIndices.Count != 0)
                        {
                            // Create a new mesh group
                            currentIndexData = new IndexDataObj();
                            indexData.Add(currentIndexData);
                        }
                    }
                    else if (split[0] == "f")
                    {
                        currentIndexData.VertexIndices.Add(Int32.Parse(split[1]));
                        currentIndexData.UvIndices.Add(Int32.Parse(split[2]));
                        currentIndexData.NormalIndices.Add(Int32.Parse(split[3]));

                        currentIndexData.VertexIndices.Add(Int32.Parse(split[4]));
                        currentIndexData.UvIndices.Add(Int32.Parse(split[5]));
                        currentIndexData.NormalIndices.Add(Int32.Parse(split[6]));

                        currentIndexData.VertexIndices.Add(Int32.Parse(split[7]));
                        currentIndexData.UvIndices.Add(Int32.Parse(split[8]));
                        currentIndexData.NormalIndices.Add(Int32.Parse(split[9]));
                    }
                }
            }
            catch (Exception e)
            {
                throw e;
            }

            file.Close();

            int vertexIndex = 0;
            int uvIndex = 0;
            int normalIndex = 0;

            Vector3 vertex;
            Vector3 normal;
            Vector2 uv;

            // Copy over groups into the final mesh data
            for (int groupIndex = 0, groupCount = indexData.Count; groupIndex < groupCount; ++groupIndex)
            {
                VertexGroup currentData = new VertexGroup();
                data.Add(currentData);

                currentIndexData = indexData[groupIndex];

                for (int i = 0, count = currentIndexData.VertexIndices.Count; i < count; ++i)
                {
                    vertexIndex = currentIndexData.VertexIndices[i];
                    uvIndex = currentIndexData.UvIndices[i];
                    normalIndex = currentIndexData.NormalIndices[i];

                    vertex = tempVertices[vertexIndex - 1];
                    currentData.Vertices.Add(vertex);

                    normal = tempNormals[normalIndex - 1];
                    currentData.Normals.Add(normal);

                    uv = tempUVs[uvIndex - 1];
                    currentData.Uvs.Add(uv);
                }
            }

            return data;
        }

        public static void LoadIndexedObj(string source, List<Vector3> vertices, List<Vector2> uvs, List<Vector3> normals, List<ushort> Indices)
        {
            List<Vector3> Vertices = new List<Vector3>();
            List<Vector2> Uvs = new List<Vector2>();
            List<Vector3> Normals = new List<Vector3>();

            try
            {
                LoadObj(source, Vertices, Uvs, Normals);
                MeshIndexer.IndexMesh(Vertices, Uvs, Normals, Indices, vertices, uvs, normals);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public static List<IndexedVertexGroup> LoadIndexedMultiMeshObj(string source)
        {
            List<VertexGroup> groups = null;
            try
            {
                groups = LoadMultiMeshObj(source);
            }
            catch (Exception e)
            {
                throw e;
            }

            List<IndexedVertexGroup> outGroups = new List<IndexedVertexGroup>();

            for (int i = 0, count = groups.Count; i < count; ++i)
            {
                var currentGroup = groups[i];
                var indexedMesh = new IndexedVertexGroup();
                indexedMesh.Name = currentGroup.Name;

                MeshIndexer.IndexMesh(currentGroup.Vertices, currentGroup.Uvs, currentGroup.Normals, 
                    indexedMesh.Indices, indexedMesh.Vertices, indexedMesh.Uvs, indexedMesh.Normals);

                outGroups.Add(indexedMesh);
            }

            return outGroups;
        }
    }
}
