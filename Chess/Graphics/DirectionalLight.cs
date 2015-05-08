using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace Chess.Graphics
{
    public class DirectionalLight : Light
    {
        public const float DefaultLightDistance = 512f;
        public const float DefaultNearDistance = 1f;
        public const float DefaultFarDistance = 1024f + 512f;

        protected ShadowMapInfo[] shadowMap = new ShadowMapInfo[1];
        protected Vector3 lightDirection = new Vector3(-1f, -1f, 0f);
        protected float lightDistance = DefaultLightDistance;
        protected CameraInfo renderCameraInfo = new CameraInfo(0, 0);
        protected float nearDistance = DefaultNearDistance;
        protected float farDistance = DefaultFarDistance;

        public Vector3 LightDirection
        {
            get { return lightDirection; }
            set { lightDirection = value; }
        }

        public float LightDistance
        {
            get { return lightDistance; }
            set { lightDistance = value; }
        }

        public float NearPlaneDistance
        {
            get { return nearDistance; }
            set { nearDistance = value; }
        }

        public float FarPlaneDistance
        {
            get { return farDistance; }
            set { farDistance = value; }
        }

        public override void CreateShadowMaps()
        {
            if (CastsShadows)
            {
                Light.CreateShadowMapFramebuffer(ref shadowMap[0], preferredShadowMapResolution);
            }
        }

        public override ShadowMapInfo[] GetShadowMaps()
        {
            return shadowMap;
        }

        public override void RenderShadowMaps(Renderer renderer, RenderScene scene)
        {
            CameraInfo info = renderer.CameraInfo;

            Vector3 position = info.Position;
            position -= lightDistance * lightDirection;
            lightDirection.Normalize();

            // Orthographic camera
            info.CopyTo(renderCameraInfo);
            renderCameraInfo.Position = position;
            renderCameraInfo.Target = position + lightDirection;
            renderCameraInfo.NearPlane = nearDistance;
            renderCameraInfo.FarPlane = farDistance;
            renderCameraInfo.RenderWidth = preferredShadowMapResolution.Width;
            renderCameraInfo.RenderHeight = preferredShadowMapResolution.Height;
            renderCameraInfo.Type = CameraType.Orthographic;

            Matrix4 proj;
            Matrix4 view;

            renderCameraInfo.GetProjection(out proj);
            renderCameraInfo.GetView(out view);

            shadowMap[0].LightViewProjection = view * proj;

            renderer.RenderShadowMap(scene, renderCameraInfo, shadowMap[0].FrameBuffer);
        }

        protected override void Dispose(bool bFinalize)
        {
            if (bFinalize)
            {
                GL.DeleteTexture(shadowMap[0].DepthTexture);
                GL.DeleteFramebuffers(1, ref shadowMap[0].FrameBuffer);
                GC.SuppressFinalize(this);
            }
        }
    }
}
