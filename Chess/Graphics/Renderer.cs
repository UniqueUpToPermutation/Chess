using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

using Chess.Graphics.Materials;

using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace Chess.Graphics
{
    public class Renderer : IDisposable
    {
        public const string RendererPackageName = "Renderer";

        protected int vertexArray;
        protected int blitVertexBuffer;
        protected Matrix4 viewProjection;
        protected Matrix4 view;
        protected Matrix4 projection;
        protected CameraInfo cameraInfo;
        protected Material shadowMapMaterial;
        protected Material blitMaterial;

        public CameraInfo CameraInfo { get { return cameraInfo; } }

        public virtual void Initialize(RenderParameters parameters)
        {
            // Enable OpenGL functions
            GL.Enable(EnableCap.DepthTest);
            GL.Enable(EnableCap.AlphaTest);
            GL.Enable(EnableCap.Texture2D);
            GL.Enable(EnableCap.CullFace);
            GL.CullFace(CullFaceMode.Back);
            GL.ClearColor(Color.DarkGray);

            // Create Vertex Array
            GL.GenVertexArrays(1, out vertexArray);
            GL.BindVertexArray(vertexArray);

            // Create the blit vertex buffer
            GL.GenBuffers(1, out blitVertexBuffer);
            GL.BindBuffer(BufferTarget.ArrayBuffer, blitVertexBuffer);
            GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(VertexArrayData.BlitData.Length * sizeof(float)), 
                VertexArrayData.BlitData, BufferUsageHint.StaticDraw);

            cameraInfo = new Graphics.CameraInfo(parameters.RenderWidth, parameters.RenderHeight);
        }

        public virtual void LoadContent(ContentManager content)
        {
            var package = CreatePackage(content);
            LoadMaterials(package);
        }

        protected virtual ContentPackage CreatePackage(ContentManager content)
        {
            ContentPackage rendererPackage;
            if (!content.Packages.TryGetValue(RendererPackageName, out rendererPackage))
            {
                rendererPackage = new ContentPackage(RendererPackageName);
                content.Add(rendererPackage);
            }
            return rendererPackage;
        }

        protected virtual void LoadMaterials(ContentPackage package)
        {
            shadowMapMaterial = new DepthPassMaterial("Reserved_ShadowMap");
            shadowMapMaterial.Load(package);

            blitMaterial = new BlitMaterial();
            blitMaterial.Load(package);
        }

        public virtual void Resize(RenderParameters parameters)
        {
            cameraInfo.RenderWidth = parameters.RenderWidth;
            cameraInfo.RenderHeight = parameters.RenderHeight;

            GL.Viewport(0, 0, parameters.RenderWidth, parameters.RenderHeight);
        }

        public virtual void RenderShadowMap(RenderScene scene, CameraInfo lightCamera, int framebufferTarget)
        {
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, framebufferTarget);
            GL.Clear(ClearBufferMask.DepthBufferBit);

            RenderScene(scene, lightCamera, shadowMapMaterial);
        }

        public void RenderScene(RenderScene scene, CameraInfo info)
        {
            RenderScene(scene, info, null);
        }

        public virtual void RenderScene(RenderScene scene, CameraInfo info, Material overrideMaterial)
        {
            info.GetView(out view);
            info.GetProjection(out projection);

            Matrix4.Mult(ref view, ref projection, out viewProjection);

            Material material = overrideMaterial;

            if (overrideMaterial != null)
            {
                overrideMaterial.Apply(this);
                overrideMaterial.SetViewProjectionTransform(ref viewProjection);
            }

            foreach (var renderNodeList in scene.RenderNodes.Values)
            {
                var model = renderNodeList[0].Model;

                for (int meshIndex = 0; meshIndex < model.MeshCount; ++meshIndex)
                {
                    var mesh = model.MeshGroup.Meshes[meshIndex];

                    if (overrideMaterial == null)
                    {
                        material = model.Material;

                        material.Apply(this);
                        material.SetViewProjectionTransform(ref viewProjection);
                    }

                    mesh.EnableAttributeArrays();

                    foreach (var meshInstance in renderNodeList)
                    {
                        if (meshInstance.Visible)
                        {
                            material.SetWorldTransform(ref meshInstance.Transform);
                            mesh.Draw();
                        }
                    }

                    mesh.DisableAttributeArrays();
                }
            }
        }

        protected void Blit(int texture)
        {
            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.Texture2D, texture);

            blitMaterial.Apply(this);

            GL.EnableVertexAttribArray(0);
            GL.BindBuffer(BufferTarget.ArrayBuffer, blitVertexBuffer);
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 0, 0);

            GL.DrawArrays(BeginMode.Triangles, 0, VertexArrayData.BlitData.Length / 3);

            GL.DisableVertexAttribArray(0);
        }

        protected virtual void RenderShadowMaps(RenderScene scene)
        {
            foreach (Light light in scene.Lights)
                light.RenderShadowMaps(this, scene);
        }

        public virtual void Render(RenderScene scene)
        {
            // Render all shadow maps
            // RenderShadowMaps(scene);

            // GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
            GL.Clear(ClearBufferMask.DepthBufferBit | ClearBufferMask.ColorBufferBit);

            //Blit(scene.Lights[0].GetShadowMaps()[0].DepthTexture);
            RenderScene(scene, cameraInfo);
        }

        public void Dispose()
        {
            GL.DeleteBuffers(1, ref blitVertexBuffer);
            GL.DeleteVertexArrays(1, ref vertexArray);
            vertexArray = -1;
        }
    }
}
