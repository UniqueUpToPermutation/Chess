using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;

using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

using Chess.Graphics;
using Chess.Graphics.Materials;

namespace Chess
{
    class ChessGame : GameWindow
    {
        public const float BoardSqaureSize = 48.0f;
        public const string DefaultPackage = "ChessObjects";

        private Renderer renderer = new Renderer();
        protected RenderParameters parameters = new RenderParameters();
        protected ContentManager content = new ContentManager();
        protected GameScene scene;

        public Renderer Renderer
        {
            get { return renderer; }
        }

        public ContentManager Content
        {
            get { return content; }
        }

        public ChessGame()
            : base(RenderParameters.DefaultRenderWidth, RenderParameters.DefaultRenderHeight, new GraphicsMode(24, 16, 0, 4))
        {
            Icon = Resource.Game;
            Title = "Chess";
        }

        public virtual void Initialize()
        {
            renderer.Initialize(parameters);
            renderer.LoadContent(content);

            scene = new GameScene(this);
            scene.Initialize(content);

            Mouse.ButtonDown += Mouse_ButtonDown;
            Mouse.ButtonUp += Mouse_ButtonUp;
            Keyboard.KeyDown += Keyboard_KeyDown;
        }

        void Keyboard_KeyDown(object sender, OpenTK.Input.KeyboardKeyEventArgs e)
        {
            if (e.Key == OpenTK.Input.Key.Tilde)
            {
                if (WindowState == OpenTK.WindowState.Fullscreen)
                    WindowState = OpenTK.WindowState.Normal;
                else
                    WindowState = OpenTK.WindowState.Fullscreen;
            }
        }

        void Mouse_ButtonUp(object sender, OpenTK.Input.MouseButtonEventArgs e)
        {
            if (e.Button == OpenTK.Input.MouseButton.Right)
            {
                Cursor.Show();
            }
        }

        void Mouse_ButtonDown(object sender, OpenTK.Input.MouseButtonEventArgs e)
        {
            if (e.Button == OpenTK.Input.MouseButton.Right)
            {
                Cursor.Position = PointToScreen(new System.Drawing.Point(ClientSize.Width / 2, ClientSize.Height / 2));
                Cursor.Hide();
            }
        }

        public static Vector3 ConvertToPosition(Point point)
        {
            return new Vector3(point.X * BoardSqaureSize + BoardSqaureSize * .5f, 0.0f,
                point.Y * BoardSqaureSize + BoardSqaureSize * .5f);
        }

        public static Point ConvertToPoint(Vector3 position)
        {
            return new Point((int)Math.Floor(position.X / BoardSqaureSize), (int)(Math.Floor(position.Z / BoardSqaureSize)));
        }

        protected override void OnResize(EventArgs e)
        {
            parameters.RenderWidth = ClientSize.Width;
            parameters.RenderHeight = ClientSize.Height;
            renderer.Resize(parameters);

            base.OnResize(e);
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            scene.Render(renderer);

            SwapBuffers();
        }

        protected override void OnUpdateFrame(FrameEventArgs e)
        {            
            scene.OnUpdateFrame(e);

            base.OnUpdateFrame(e);
        }

        public override void Dispose()
        {
            scene.Dispose();
            renderer.Dispose();
            content.Dispose();

            base.Dispose();
        }
    }
}
