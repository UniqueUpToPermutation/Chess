using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace Chess.Graphics.Materials
{
    public class SolidColorMaterial : MaterialBase
    {
        public const string DefaultColorParamLocation = "Color";
        public const string DefaultLightDirectionParamLocation = "LightDirection";

        private Vector3 solidColor = new Vector3(1f, 1f, 1f);
        private Vector3 lightDirection = new Vector3(0f, -1f, 1f);
        protected int colorParam;
        protected int lightDirectionParam;

        public Vector3 SolidColor
        {
            get { return solidColor; }
            set { solidColor = value; }
        }

        public Vector3 LightDirection
        {
            get { return lightDirection; }
            set { lightDirection = value; }
        }

        public SolidColorMaterial(Vector3 solidColor, string name)
            : base("Shaders\\SolidColor", name)
        {
            this.solidColor = solidColor;
        }

        public SolidColorMaterial(Vector3 solidColor, string name, string shaderSource)
            : base(shaderSource, name)
        {
            this.solidColor = solidColor;
        }

        public override void Load(ContentPackage package)
        {
            LoadShader(package);

            colorParam = GL.GetUniformLocation(ShaderProgram, DefaultColorParamLocation);
            lightDirectionParam = GL.GetUniformLocation(ShaderProgram, DefaultLightDirectionParamLocation);
        }

        public override void Update(OpenTK.FrameEventArgs e)
        {
        }

        public override void Apply(Renderer renderer)
        {
            GL.UseProgram(ShaderProgram);
            GL.Uniform3(colorParam, ref solidColor);

            lightDirection.Normalize();
            GL.Uniform3(lightDirectionParam, ref lightDirection);
        }
    }
}
