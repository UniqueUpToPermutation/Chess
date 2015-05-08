using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OpenTK.Graphics.OpenGL;

namespace Chess.Graphics.Materials
{
    class BlitMaterial : MaterialBase
    {
        protected int TextureParam;

        public BlitMaterial()
            : base("Shaders\\DepthBlit", "Blit")
        {
        }

        public override void Load(ContentPackage package)
        {
            LoadShader(package);

            TextureParam = GL.GetUniformLocation(ShaderProgram, "Texture");
        }

        public override void Update(OpenTK.FrameEventArgs e)
        {
        }

        public override void Apply(Renderer renderer)
        {
            GL.UseProgram(ShaderProgram);
            GL.Uniform1(TextureParam, 0);
        }
    }
}
