using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Diagnostics;

using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace Chess.Graphics
{
    public class ContentManager : IDisposable
    {
        protected bool bDisposed = false;
        public const string GlobalPackageName = "Global";

        protected Dictionary<string, ContentPackage> contentPackages = new Dictionary<string, ContentPackage>();

        public Dictionary<string, ContentPackage> Packages
        {
            get { return contentPackages; }
        }

        public ContentPackage this[string packageName]
        {
            get { return contentPackages[packageName]; }
        }

        public ContentPackage GetPackage(string packageName)
        {
            return contentPackages[packageName];
        }

        ~ContentManager()
        {
            Dispose(false);
        }

        public void Add(ContentPackage package)
        {
            contentPackages.Add(package.Name, package);
            package.Manager = this;
        }

        public void Destroy(ContentPackage package)
        {
            contentPackages.Remove(package.Name);
            package.Dispose();
        }

        public void Destroy(string packageName)
        {
            ContentPackage package;

            if (contentPackages.TryGetValue(packageName, out package))
            {
                contentPackages.Remove(packageName);
                package.Dispose();
            }
        }

        protected void Dispose(bool bFinalize)
        {
            foreach (ContentPackage package in contentPackages.Values)
                package.Dispose();

            contentPackages.Clear();

            if (bFinalize)
                GC.SuppressFinalize(this);
        }

        public void Dispose()
        {
 	        Dispose(true);
        }

        public static int LoadTextureFromFile(string filename)
        {
            // Validate name
            if (String.IsNullOrEmpty(filename))
                throw new ArgumentException(filename);

            // Create a new texture2D and bind it
            int id = GL.GenTexture();
            GL.BindTexture(TextureTarget.Texture2D, id);

            // Load bitmap data
            Bitmap bmp = new Bitmap(filename);
            BitmapData bmp_data = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            // Copy data
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, bmp_data.Width, bmp_data.Height, 0,
                OpenTK.Graphics.OpenGL.PixelFormat.Bgra, PixelType.UnsignedByte, bmp_data.Scan0);

            bmp.UnlockBits(bmp_data);

            // Generate mipmaps
            GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);

            // Linear filtering
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.LinearMipmapLinear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);

            return id;
        }

        public static int LoadTextureArrayFromFiles(string[] filenames)
        {
            // Create a texture
            int textureArray = GL.GenTexture();
            // Bind this texture to Texture2DArray
            GL.BindTexture(TextureTarget.Texture2DArray, textureArray);

            int textureWidth = 0;
            int textureHeight = 0;
            int textureDepth = filenames.Length;
            int textureLayer = 0;

            try
            {
                // For each file
                foreach (string filename in filenames)
                {
                    if (String.IsNullOrEmpty(filename))
                        throw new ArgumentException(filename);

                    Bitmap bmp = new Bitmap(filename);

                    if (textureWidth == 0 && textureHeight == 0)
                    {
                        // Set the texture width and heights
                        textureWidth = bmp.Width;
                        textureHeight = bmp.Height;

                        // Allocate texture memory
                        GL.TexImage3D(TextureTarget.Texture2DArray, 0, PixelInternalFormat.Rgba,
                            textureWidth, textureHeight, textureDepth, 0, OpenTK.Graphics.OpenGL.PixelFormat.Rgba,
                            PixelType.UnsignedByte, IntPtr.Zero);
                    }
                    else
                    {
                        // Make sure this image is the same size as the other images
                        if (bmp.Width != textureWidth || bmp.Height != textureHeight)
                        {
                            GL.DeleteTexture(textureArray);
                            GL.BindTexture(TextureTarget.Texture2DArray, 0);

                            throw new Exception("Error - Images are not the same resolution");
                        }
                    }

                    // Copy over bitmap data into the 2D texture array
                    BitmapData bmp_data = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

                    GL.TexSubImage3D(TextureTarget.Texture2DArray, 0, 0, 0, textureLayer, textureWidth, textureHeight, 1, OpenTK.Graphics.OpenGL.PixelFormat.Bgra,
                        PixelType.UnsignedByte, bmp_data.Scan0);

                    bmp.UnlockBits(bmp_data);

                    ++textureLayer;
                }
            }
            catch (Exception e)
            {
                // Failed! Delete texture!
                GL.DeleteTexture(textureArray);
                GL.BindTexture(TextureTarget.Texture2DArray, 0);

                throw e;
            }

            // Generate mipmaps
            GL.GenerateMipmap(GenerateMipmapTarget.Texture2DArray);

            // Linear interpolation
            GL.TexParameter(TextureTarget.Texture2DArray, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.LinearMipmapLinear);
            GL.TexParameter(TextureTarget.Texture2DArray, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);

            return textureArray;
        }

        public static int LoadProgramFromFile(string vertexShaderSource, string fragmentShaderSource)
        {
            int vertexShaderHandle = GL.CreateShader(ShaderType.VertexShader);
            int fragmentShaderHandle = GL.CreateShader(ShaderType.FragmentShader);

            string strVertex;
            string strFragment;

            // Read files
            try
            {
                StreamReader file = new StreamReader(vertexShaderSource);
                strVertex = file.ReadToEnd();
                file.Close();

                file = new StreamReader(fragmentShaderSource);
                strFragment = file.ReadToEnd();
                file.Close();
            }
            catch (Exception e)
            {
                throw e;
            }

            string log;
            int status_code;

            // Compile vertex shader
            Debug.WriteLine("Compiling " + vertexShaderSource + "...");
            GL.ShaderSource(vertexShaderHandle, strVertex);
            GL.CompileShader(vertexShaderHandle);
            GL.GetShaderInfoLog(vertexShaderHandle, out log);
            GL.GetShader(vertexShaderHandle, ShaderParameter.CompileStatus, out status_code);          
            Debug.Write(log);

            if (status_code != 1)
                return 0;

            // Compile vertex shader
            Debug.WriteLine("Compiling " + fragmentShaderSource + "...");
            GL.ShaderSource(fragmentShaderHandle, strFragment);
            GL.CompileShader(fragmentShaderHandle);
            GL.GetShaderInfoLog(fragmentShaderHandle, out log);
            GL.GetShader(fragmentShaderHandle, ShaderParameter.CompileStatus, out status_code);
            Debug.Write(log);

            if (status_code != 1)
                return 0;

            // Create the shader program
            int shaderProgram = GL.CreateProgram();

            // Attach shaders
            GL.AttachShader(shaderProgram, vertexShaderHandle);
            GL.AttachShader(shaderProgram, fragmentShaderHandle);

            // Link the program
            Debug.WriteLine("Linking " + vertexShaderSource + " and " + fragmentShaderSource + "...");
            GL.LinkProgram(shaderProgram);

            // Output program log info
            GL.GetProgramInfoLog(shaderProgram, out log);
            Debug.Write(log);

            // Delete used shaders
            GL.DeleteShader(vertexShaderHandle);
            GL.DeleteShader(fragmentShaderHandle);

            return shaderProgram;
        }
    }
}