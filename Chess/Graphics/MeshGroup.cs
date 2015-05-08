using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chess.Graphics
{
    public class MeshGroup : IDisposable
    {
        protected Mesh[] meshes;
        protected string[] meshNames;

        public Mesh[] Meshes
        {
            get { return meshes; }
        }

        public string[] Names
        {
            get { return meshNames; }
        }

        public MeshGroup()
        {
        }

        public MeshGroup(Mesh[] meshes, string[] meshNames)
        {
            this.meshes = meshes;
            this.meshNames = meshNames;
        }

        ~MeshGroup()
        {
            Dispose(false);
        }

        public static MeshGroup FromFile(string source)
        {
            List<IndexedVertexGroup> groups = null;
            try
            {
                groups = Mesh.LoadIndexedMultiMeshObj(source);
            }
            catch (Exception e)
            {
                throw e;
            }

            MeshGroup group = new MeshGroup();
            group.meshes = new Mesh[groups.Count];
            group.meshNames = new string[groups.Count];

            for (int i = 0, count = groups.Count; i < count; ++i)
            {
                var currentVertexGroup = groups[i];
                group.meshes[i] = Mesh.FromVertexLists(currentVertexGroup.Vertices, currentVertexGroup.Uvs, 
                    currentVertexGroup.Normals, currentVertexGroup.Indices);
                group.meshNames[i] = currentVertexGroup.Name;
            }

            return group;
        }

        protected void Dispose(bool bFinalize)
        {
            foreach (var mesh in meshes)
                mesh.Dispose();

            if (bFinalize)
                GC.SuppressFinalize(this);
        }

        public void Dispose()
        {
            Dispose(true);
        }
    }
}
