using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace Chess.Graphics
{
    public abstract class Material
    {
        private string name;
        private int shaderProgram;

        public virtual bool IsReflective { get { return false; } }
        public virtual bool IsShadowed { get { return false; } }
        public virtual bool IsLightReceiver { get { return false; } }

        public int ShaderProgram
        {
            get { return shaderProgram; }
            protected set { shaderProgram = value; }
        }

        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        public abstract void SetLights(Light[] lights);
        public abstract void SetShadowMaps(int[] shadowMaps);
        public abstract void SetShadowMapTransforms(Matrix4[] transforms);
        public abstract void SetWorldTransform(ref Matrix4 transform);
        public abstract void SetViewProjectionTransform(ref Matrix4 transform);

        public abstract void Load(ContentPackage package);
        public abstract void Update(FrameEventArgs e);
        public abstract void Apply(Renderer renderer);

        public override string ToString()
        {
            return name;
        }
    }
}
