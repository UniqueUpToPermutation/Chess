using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

using OpenTK;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;

namespace Chess.Graphics
{
    public class CameraObject
    {
        public const float DefaultCameraSpeed = 100.0f;
        public const float DefaultCameraRotationSpeed = 0.1f;

        private Vector3 position;
        private Vector3 up = Vector3.UnitY;
        private float pitch;
        private float yaw;
        private float roll;
        private float cameraSpeed = DefaultCameraSpeed;
        private float cameraRotationSpeed = DefaultCameraRotationSpeed;

        public Vector3 Position
        {
            get { return position; }
            set { position = value; }
        }

        public Vector3 LookDirection
        {
            get
            {
                return new Vector3((float)(Math.Sin(yaw) * Math.Cos(pitch)),
                    (float)(Math.Sin(pitch)), (float)(Math.Cos(yaw) * Math.Cos(pitch)));
            }
        }

        public Vector3 Up
        {
            get { return up; }
            set { up = value; }
        }

        public float Pitch
        {
            get { return pitch; }
            set { pitch = value; }
        }

        public float Yaw
        {
            get { return yaw; }
            set { yaw = value; }
        }

        public float Roll
        {
            get { return roll; }
            set { roll = value; }
        }

        public float CameraSpeed
        {
            get { return cameraSpeed; }
            set { cameraSpeed = value; }
        }

        public float CameraRotationSpeed
        {
            get { return cameraRotationSpeed; }
            set { cameraRotationSpeed = value; }
        }

        public void UpdateInputKeyboard(KeyboardDevice keyboard, FrameEventArgs e)
        {
            Vector3 lookNormalized = LookDirection;
            Vector3 sideNormalized = Vector3.Cross(lookNormalized, up);

            lookNormalized.Normalize();
            sideNormalized.Normalize();

            if (keyboard[Key.W])
                position += lookNormalized * cameraSpeed * (float)e.Time;
            if (keyboard[Key.S])
                position -= lookNormalized * cameraSpeed * (float)e.Time;
            if (keyboard[Key.D])
                position += sideNormalized * cameraSpeed * (float)e.Time;
            if (keyboard[Key.A])
                position -= sideNormalized * cameraSpeed * (float)e.Time;
        }

        public void UpdateInputMouse(MouseDevice mouse, Size ClientSize, FrameEventArgs e)
        {
            int mouseX = mouse.X - ClientSize.Width / 2;
            int mouseY = mouse.Y - ClientSize.Height / 2;

            yaw -= (float)mouseX * cameraRotationSpeed * (float)e.Time;
            pitch -= (float)mouseY * cameraRotationSpeed * (float)e.Time;

            // Clamp pitch
            pitch = Math.Max(Math.Min(pitch, (float)(Math.PI / 2 - .1)), (float)(-Math.PI / 2 + .1));
        }

        public void UpdateInfo(CameraInfo info)
        {
            info.Position = position;
            info.Target = position + LookDirection;
            info.Up = Vector3.UnitY;
        }
    }
}
