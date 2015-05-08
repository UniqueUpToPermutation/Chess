using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace Chess.Graphics.Materials
{
    public class DepthPassMaterial : MaterialBase
    {
        public const string ShaderSourcePath = "Shaders\\Depth";

        public DepthPassMaterial(string name)
            : base(ShaderSourcePath, name)
        {
        }

        public override void Load(ContentPackage package)
        {
            LoadShader(package);
        }

        public override void Update(OpenTK.FrameEventArgs e)
        {
        }

        public override void Apply(Renderer renderer)
        {
            GL.UseProgram(ShaderProgram);

        }
    }
}
