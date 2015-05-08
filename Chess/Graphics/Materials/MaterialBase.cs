using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace Chess.Graphics.Materials
{
    public abstract class MaterialBase : Material
    {
        public const string WorldParameterLocation = "World";
        public const string ViewProjectionParameterLocation = "ViewProjection";

        protected int WorldMatrixParam;
        protected int ViewProjectionMatrixParam;
        protected string ShaderSource;

        public MaterialBase(string shaderSource, string name)
        {
            ShaderSource = shaderSource;
            Name = name;
        }

        public override void SetWorldTransform(ref OpenTK.Matrix4 transform)
        {
            GL.UniformMatrix4(WorldMatrixParam, false, ref transform);
        }

        public override void SetViewProjectionTransform(ref OpenTK.Matrix4 transform)
        {
            GL.UniformMatrix4(ViewProjectionMatrixParam, false, ref transform);
        }

        protected void LoadShader(ContentPackage package)
        {
            ShaderProgram = package.LoadProgram(ShaderSource);

            WorldMatrixParam = GL.GetUniformLocation(ShaderProgram, WorldParameterLocation);
            ViewProjectionMatrixParam = GL.GetUniformLocation(ShaderProgram, ViewProjectionParameterLocation);
        }

        public override void SetLights(Light[] lights)
        {
        }

        public override void SetShadowMaps(int[] shadowMaps)
        {
        }

        public override void SetShadowMapTransforms(Matrix4[] transforms)
        {
        }
    }
}
