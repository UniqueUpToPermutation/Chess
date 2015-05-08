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
    class SolidColorSpecularMaterial : MaterialBase
    {
        public const string DefaultDiffuseColorParamLocation = "DiffuseColor";
        public const string DefaultLightDirectionParamLocation = "LightDirection";
        public const string DefaultSpecularColorParamLocation = "SpecularColor";
        public const string DefaultSpecularityParamLocation = "Specularity";
        public const string DefaultShininessParamLocation = "Shininess";
        public const string DefaultCameraPositionParamLocation = "CameraPosition";
        public const string DefaultAmbientColorParamLocation = "AmbientColor";
        public const string DefaultAmbientStrengthParamLocation = "AmbientStrength";

        private Vector3 diffuseColor = new Vector3(1f, 1f, 1f);
        private Vector3 lightDirection = new Vector3(0f, -1f, 1f);
        private Vector3 specularColor = new Vector3(1f, 1f, 1f);
        private Vector3 ambientColor = new Vector3(1f, 1f, 1f);
        private float ambientStrength = 0f;
        private float specularity = 1f;
        private float shininess = 1f;

        protected int diffuseColorParam;
        protected int lightDirectionParam;
        protected int specularColorParam;
        protected int specularityParam;
        protected int shininessParam;
        protected int cameraPositionParam;
        protected int ambientColorParam;
        protected int ambientStrengthParam;

        public float AmbientStrength
        {
            get { return ambientStrength; }
            set { ambientStrength = value; }
        }

        public Vector3 AmbientColor
        {
            get { return ambientColor; }
            set { ambientColor = value; }
        }

        public Vector3 DiffuseColor
        {
            get { return diffuseColor; }
            set { diffuseColor = value; }
        }

        public Vector3 LightDirection
        {
            get { return lightDirection; }
            set { lightDirection = value; }
        }

        public Vector3 SpecularColor
        {
            get { return specularColor; }
            set { specularColor = value; }
        }

        public float Specularity
        {
            get { return specularity; }
            set { specularity = value; }
        }

        public float Shininess
        {
            get { return shininess; }
            set { shininess = value; }
        }

        public SolidColorSpecularMaterial(Vector3 solidColor, string name)
            : base("Shaders\\SolidColorSpecular", name)
        {
            this.diffuseColor = solidColor;
        }

        public SolidColorSpecularMaterial(Color solidColor, string name)
            : base("Shaders\\SolidColorSpecular", name)
        {
            this.diffuseColor = new Vector3(solidColor.R / 255f, solidColor.G / 255f, solidColor.B / 255f);
        }

        public SolidColorSpecularMaterial(Vector3 solidColor, string name, string shaderSource)
            : base(shaderSource, name)
        {
            this.diffuseColor = solidColor;
        }

        public override void Load(ContentPackage package)
        {
            LoadShader(package);

            diffuseColorParam = GL.GetUniformLocation(ShaderProgram, DefaultDiffuseColorParamLocation);
            lightDirectionParam = GL.GetUniformLocation(ShaderProgram, DefaultLightDirectionParamLocation);
            specularColorParam = GL.GetUniformLocation(ShaderProgram, DefaultSpecularColorParamLocation);
            specularityParam = GL.GetUniformLocation(ShaderProgram, DefaultSpecularityParamLocation);
            shininessParam = GL.GetUniformLocation(ShaderProgram, DefaultShininessParamLocation);
            cameraPositionParam = GL.GetUniformLocation(ShaderProgram, DefaultCameraPositionParamLocation);
            ambientColorParam = GL.GetUniformLocation(ShaderProgram, DefaultAmbientColorParamLocation);
            ambientStrengthParam = GL.GetUniformLocation(ShaderProgram, DefaultAmbientStrengthParamLocation);
        }

        public override void Update(OpenTK.FrameEventArgs e)
        {
        }

        public override void Apply(Renderer renderer)
        {
            GL.UseProgram(ShaderProgram);
           
            lightDirection.Normalize();
            GL.Uniform3(lightDirectionParam, ref lightDirection);

            GL.Uniform3(diffuseColorParam, ref diffuseColor);
            GL.Uniform3(ambientColorParam, ref ambientColor);
            GL.Uniform3(specularColorParam, ref specularColor);
            GL.Uniform1(specularityParam, specularity);
            GL.Uniform1(shininessParam, shininess);
            GL.Uniform1(ambientStrengthParam, ambientStrength);

            CameraInfo camera = renderer.CameraInfo;
            Vector3 position = camera.Position;
            GL.Uniform3(cameraPositionParam, ref position);
        }
    }
}
