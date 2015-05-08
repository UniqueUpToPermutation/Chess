using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OpenTK;
using OpenTK.Graphics.OpenGL;
using OpenTK.Graphics;

namespace Chess.Graphics
{
    public class ContentPackage : IDisposable
    {
        protected ContentManager manager;

        protected string name;
        protected Dictionary<string, Graphics.Mesh> meshes = new Dictionary<string, Graphics.Mesh>();
        protected Dictionary<string, int> textures = new Dictionary<string, int>();
        protected Dictionary<string, int> shaderPrograms = new Dictionary<string, int>();
        protected Dictionary<string, Graphics.MeshGroup> meshGroups = new Dictionary<string, Graphics.MeshGroup>();

        public string Name { get { return name; } }
        public Dictionary<string, Graphics.Mesh> Meshes { get { return meshes; } }
        public Dictionary<string, int> Textures { get { return textures; } }
        public Dictionary<string, int> ShaderPrograms { get { return shaderPrograms; } }
        public Dictionary<string, Graphics.MeshGroup> MeshGroups { get { return meshGroups; } }

        public ContentManager Manager { get { return manager; } set { manager = value; } }

        public ContentPackage(string name)
        {
            this.name = name;
        }

        ~ContentPackage()
        {
            Dispose(false);
        }

        protected void Dispose(bool bFinalize)
        {
            foreach (var value in textures.Values)
                GL.DeleteTexture(value);

            foreach (var value in shaderPrograms.Values)
                GL.DeleteProgram(value);

            foreach (var value in meshes.Values)
                value.Dispose();

            foreach (var value in meshGroups.Values)
                value.Dispose();

            textures.Clear();
            shaderPrograms.Clear();

            if (bFinalize)
                GC.SuppressFinalize(this);
        }

        public void AddTexture(string source, int textureID)
        {
            textures.Add(source, textureID);
        }

        public int LoadTexture(string source)
        {
            int id;

            if (textures.TryGetValue(source, out id))
                return id;

#if !DEBUG
            try
            {
#endif
            id = ContentManager.LoadTextureFromFile(source);
#if !DEBUG
            }
            catch (Exception e)
            {
                throw e;
            }
#endif

            textures.Add(source, id);
            return id;
        }

        public void AddProgram(string source, int programID)
        {
            shaderPrograms.Add(source, programID);
        }

        public int LoadProgram(string source)
        {
            int id;

            if (shaderPrograms.TryGetValue(source, out id))
                return id;

            try
            {
                id = ContentManager.LoadProgramFromFile(source + ".vert", source + ".frag");
            }
            catch (Exception e)
            {
                throw e;
            }

            shaderPrograms.Add(source, id);
            return id;
        }

        public void AddMesh(string source, Mesh mesh)
        {
            meshes.Add(source, mesh);
        }

        public Mesh LoadMesh(string source)
        {
            Mesh mesh = null;

            if (meshes.TryGetValue(source, out mesh))
                return mesh;

            try
            {
                mesh = Mesh.FromFile(source);
            }
            catch (Exception e)
            {
                throw e;
            }

            meshes.Add(source, mesh);
            return mesh;
        }

        public void AddMeshGroup(string source, MeshGroup group)
        {
            meshGroups.Add(source, group);
        }

        public MeshGroup LoadMeshGroup(string source)
        {
            MeshGroup mesh = null;

            if (meshGroups.TryGetValue(source, out mesh))
                return mesh;

            try
            {
                mesh = MeshGroup.FromFile(source);
            }
            catch (Exception e)
            {
                throw e;
            }

            meshGroups.Add(source, mesh);
            return mesh;
        }

        public void Dispose()
        {
            Dispose(true);
        }
    }
}
