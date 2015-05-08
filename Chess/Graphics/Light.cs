using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace Chess.Graphics
{
    public struct ShadowMapInfo
    {
        public Matrix4 LightViewProjection;
        public int FrameBuffer;
        public int DepthTexture;
    }

    public abstract class Light : IDisposable
    {
        public const int DefaultShadowMapWidth = 1024;
        public const int DefaultShadowMapHeight = 1024;
        public const float DefaultShadowMapVisibleDistance = 8192f;
        public const float DefaultLightVisibleDistance = DefaultShadowMapVisibleDistance * 2f;

        protected bool bDisposed = false;
        protected bool bCastsShadows;
        protected Vector3 position;
        protected float shadowMapVisibleDistance = DefaultShadowMapVisibleDistance;
        protected float lightVisibleDistance = DefaultLightVisibleDistance;
        protected Size preferredShadowMapResolution = new Size(DefaultShadowMapWidth, DefaultShadowMapHeight);

        ~Light()
        {
            Dispose(false);
        }

        public Vector3 Position
        {
            get { return position; }
            set { position = value; }
        }

        public bool CastsShadows
        {
            get { return bCastsShadows; }
            set { bCastsShadows = value; }
        }

        public float ShadowMapVisibleDistance
        {
            get { return shadowMapVisibleDistance; }
            set { shadowMapVisibleDistance = value; }
        }

        public float LightVisibleDistance
        {
            get { return lightVisibleDistance; }
            set { lightVisibleDistance = value; }
        }

        public Size PreferredShadowMapResolution
        {
            get { return preferredShadowMapResolution; }
            set { preferredShadowMapResolution = value; }
        }

        public virtual void CreateShadowMaps()
        {
        }

        public virtual ShadowMapInfo[] GetShadowMaps()
        {
            return null;
        }

        public virtual void RenderShadowMaps(Renderer renderer, RenderScene scene)
        {
        }

        public static void CreateShadowMapFramebuffer(ref ShadowMapInfo info, Size preferredResolution)
        {
            // The framebuffer, which regroups 0, 1, or more textures, and 0 or 1 depth buffer.
            GL.GenFramebuffers(1, out info.FrameBuffer);
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, info.FrameBuffer);

            // Depth texture. Slower than a depth buffer, but you can sample it later in your shader
            GL.GenTextures(1, out info.DepthTexture);
            GL.BindTexture(TextureTarget.Texture2D, info.DepthTexture);
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.DepthComponent16, preferredResolution.Width,
                preferredResolution.Height, 0, PixelFormat.DepthComponent, PixelType.Float, IntPtr.Zero);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToEdge);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToEdge);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.DepthTextureMode, (int)All.Intensity);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureCompareFunc, (int)All.Lequal);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureCompareMode, (int)TextureCompareMode.CompareRToTexture);

            GL.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferAttachment.DepthAttachment, TextureTarget.Texture2D, info.DepthTexture, 0);

            GL.DrawBuffer(DrawBufferMode.None); // No color buffer is drawn to.

            // Always check that our framebuffer is ok
            if (GL.CheckFramebufferStatus(FramebufferTarget.Framebuffer) != FramebufferErrorCode.FramebufferComplete)
                throw new Exception("Failed to create framebuffer!");

            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
        }

        public void Dispose()
        {
            Dispose(true);
        }

        protected abstract void Dispose(bool bFinalize);
    }
}
