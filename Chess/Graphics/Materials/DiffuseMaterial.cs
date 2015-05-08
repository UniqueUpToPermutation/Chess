using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace Chess.Graphics.Materials
{
    public class DiffuseMaterial : MaterialBase
    {
        public const string DefaultShaderSource = "Shaders\\Diffuse";
        public const string DiffuseParameterLocation = "Diffuse";

        protected int diffuseTextureParam;
        protected int diffuseTexture;

        public int DiffuseTexture
        {
            get { return diffuseTexture; }
            set { diffuseTexture = value; }
        }

        public DiffuseMaterial(int diffuseTexture, string name) : base(DefaultShaderSource, name)
        {
            this.diffuseTexture = diffuseTexture;
        }

        public override void Load(ContentPackage package)
        {
            LoadShader(package);

            diffuseTextureParam = GL.GetUniformLocation(ShaderProgram, DiffuseParameterLocation);
        }

        public override void Update(OpenTK.FrameEventArgs e)
        {
        }

        public override void Apply(Renderer renderer)
        {
            GL.UseProgram(ShaderProgram);

            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.Texture2D, diffuseTexture);

            GL.Uniform1(diffuseTextureParam, 0);
        }
    }
}
