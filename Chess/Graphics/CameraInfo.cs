using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace Chess.Graphics
{
    public enum CameraType
    {
        Perspective,
        Orthographic
    }

    public class CameraInfo
    {
        public const float DefaultFieldOfView = (float)Math.PI / 3f;
        public const float DefaultNearPlane = 1.0f;
        public const float DefaultFarPlane = 1000.0f;

        protected float fieldOfView = DefaultFieldOfView;
        protected float nearPlane = DefaultNearPlane;
        protected float farPlane = DefaultFarPlane;

        protected Vector3 target;
        protected Vector3 position;
        protected Vector3 up = Vector3.UnitY;
        protected CameraType type = CameraType.Perspective;

        protected int renderWidth;
        protected int renderHeight;

        public int RenderWidth { get { return renderWidth; } set { renderWidth = value; } }
        public int RenderHeight { get { return renderHeight; } set { renderHeight = value; } }

        public float FieldOfView 
        {
            get
            {
                return fieldOfView;
            }
            set
            {
                fieldOfView = value;
            }
        }

        public float NearPlane
        {
            get
            {
                return nearPlane;
            }
            set
            {
                nearPlane = value;
            }
        }

        public float FarPlane
        {
            get
            {
                return farPlane;
            }
            set
            {
                farPlane = value;
            }
        }

        public CameraInfo(int renderWidth, int renderHeight)
        {
            this.renderWidth = renderWidth;
            this.renderHeight = renderHeight;
        }

        public void CopyTo(CameraInfo other)
        {
            other.fieldOfView = fieldOfView;
            other.nearPlane = nearPlane;
            other.farPlane = farPlane;
            other.target = target;
            other.up = up;
            other.position = position;
            other.type = type;
            other.renderHeight = renderHeight;
            other.renderWidth = renderWidth;
        }

        public void GetView(out OpenTK.Matrix4 matrix)
        {
            matrix = Matrix4.LookAt(position, target, up);
        }

        public void GetProjection(out OpenTK.Matrix4 projection)
        {
            if (type == CameraType.Perspective)
            {
                float aspectRatio = (float)renderWidth / (float)renderHeight;

                Matrix4.CreatePerspectiveFieldOfView(fieldOfView,
                    aspectRatio, nearPlane, farPlane, out projection);
            }
            else
            {
                Matrix4.CreateOrthographic((float)renderWidth, (float)renderHeight, 
                    nearPlane, farPlane, out projection);
            }
        }

        public Vector3 Target
        {
            get { return target; }
            set { target = value; }
        }

        public Vector3 Up
        {
            get { return up; }
            set { up = value; }
        }

        public Vector3 Position
        {
            get { return position; }
            set { position = value; }
        }

        public CameraType Type
        {
            get
            {
                return type;
            }
            set
            {
                type = value;
            }
        }
    }
}
