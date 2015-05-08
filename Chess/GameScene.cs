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
using OpenTK.Input;

using Chess.Graphics;
using Chess.Graphics.Materials;

namespace Chess
{
    class GameScene : IDisposable
    {
        public const string MaterialWhiteName = "PieceMaterialWhite";
        public const string MaterialBlackName = "PieceMaterialBlack";
        public const string BoardMeshSource = "Content\\Board.obj";
        public const string BoardTextureSource = "Content\\BoardTexture.jpg";
        public const string SelectorMeshSource = "Content\\Selector.obj";
        public const string BoardMaterialName = "BoardMaterial";
        public const string BoardModelName = "Board";
        public const string SelectorMaterialName = "SelectorMaterial";
        public const string SelectorModelName = "Selector";
        public const string SelectedModelName = "Selected";
        public const string SelectedMaterialName = "SelectedMaterial";
        public const float MinCameraHeight = 12f;

        public static Vector3 InitialCameraPosition = new Vector3(0.0f, 96.0f, 0.0f);

        protected RenderScene RenderScene;
        protected RenderNode selector = new RenderNode();
        protected ChessPiece selectedPiece = null;
        protected CameraObject cameraObject = new CameraObject();
        protected ChessBoard Board = new ChessBoard();

        protected MouseDevice Mouse;
        protected KeyboardDevice Keyboard;
        protected ChessGame Parent;

        public GameScene(ChessGame parent)
        {
            Mouse = parent.Mouse;
            Keyboard = parent.Keyboard;
            Parent = parent;

            Mouse.ButtonDown += Mouse_ButtonDown;
        }

        void Mouse_ButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.Button == MouseButton.Left)
            {
                Point pt = GetMouseBoardPosition();
                ChessPiece piece = null;

                if (Board.IsInBounds(pt))
                {
                    piece = Board[pt.X, pt.Y];

                    ChessPiece lastPiece = selectedPiece;
                    if (lastPiece == piece)
                        OnPieceSelected(null);
                    else if (lastPiece != null && (piece == null || piece.Player != lastPiece.Player))
                    {
                        OnPieceOrder(lastPiece, pt);
                        OnPieceSelected(null);
                    }
                    else
                        OnPieceSelected(piece);
                }
                else
                    OnPieceSelected(null);
            }
        }

        public void Render(Renderer renderer)
        {
            renderer.Render(RenderScene);
        }

        protected void PlacePiece<Type>(Player player, int x, int y, float rotation = 0.0f)
            where Type : ChessPiece, new()
        {
            Type piece = new Type();
            piece.Initialize(player, RenderScene);
            piece.Rotation = rotation;
            piece.Position = new Vector3((float)x * ChessGame.BoardSqaureSize + ChessGame.BoardSqaureSize / 2f, 0.0f,
                        (float)y * ChessGame.BoardSqaureSize + ChessGame.BoardSqaureSize / 2f);
            RenderScene.AddRenderNode(piece.RenderNode);
            Board[x, y] = piece;
        }

        public void Initialize(ContentManager Content)
        {
            // Create a new render scene
            RenderScene = new RenderScene();

            // Create the default package
            if (!Content.Packages.ContainsKey(ChessGame.DefaultPackage))
            {
                ContentPackage newPackage = new ContentPackage(ChessGame.DefaultPackage);
                Content.Add(newPackage);
            }

            // Load the content
            ContentPackage package = Content.Packages[ChessGame.DefaultPackage];
            CreateBoardModels(package);
            CreatePieceMaterials(package);
            CreatePieceModels(package);
            CreateSelector(package);

            // Place the pieces
            PlacePieces();

            // Place the lights
            PlaceLights();

            cameraObject.Position = InitialCameraPosition;
        }

        protected void CreateSelector(ContentPackage package)
        {
            SolidColorSpecularMaterial materialGray = new SolidColorSpecularMaterial(new Vector3(.5f, .5f, 1f), SelectorMaterialName);
            materialGray.Load(package);
            RenderScene.AddMaterial(materialGray);
            materialGray.LightDirection = new Vector3(1f, -1f, 0f);
            materialGray.Shininess = 3f;
            materialGray.Specularity = .5f;

            MeshGroup selectorMesh = package.LoadMeshGroup(SelectorMeshSource);
            Model selectorModel = new Model() { Name = SelectorModelName, Material = materialGray, MeshGroup = selectorMesh };
            RenderScene.AddModel(selectorModel);

            selector = new RenderNode() { Model = selectorModel, Transform = Matrix4.Identity };
            RenderScene.AddRenderNode(selector);
        }

        protected void PlacePieces()
        {
            Board.Clear();

            for (int x = 0; x < 8; ++x)
                for (int y = 0; y < 8; ++y)
                    if (y == 1)
                        PlacePiece<Pawn>(Player.White, x, y);
                    else if (y == 6)
                        PlacePiece<Pawn>(Player.Black, x, y);

            PlacePiece<Knight>(Player.White, 1, 0, (float)Math.PI);
            PlacePiece<Knight>(Player.White, 6, 0, (float)Math.PI);
            PlacePiece<Knight>(Player.Black, 1, 7);
            PlacePiece<Knight>(Player.Black, 6, 7);

            PlacePiece<Bishop>(Player.White, 2, 0);
            PlacePiece<Bishop>(Player.White, 5, 0);
            PlacePiece<Bishop>(Player.Black, 2, 7);
            PlacePiece<Bishop>(Player.Black, 5, 7);

            PlacePiece<Rook>(Player.White, 0, 0);
            PlacePiece<Rook>(Player.White, 7, 0);
            PlacePiece<Rook>(Player.Black, 0, 7);
            PlacePiece<Rook>(Player.Black, 7, 7);

            PlacePiece<Queen>(Player.White, 4, 0);
            PlacePiece<Queen>(Player.Black, 4, 7);

            PlacePiece<King>(Player.White, 3, 0);
            PlacePiece<King>(Player.Black, 3, 7);
        }

        protected void PlaceLights()
        {
            DirectionalLight light = new DirectionalLight();
            light.CastsShadows = true;
            light.CreateShadowMaps();

            RenderScene.AddLight(light);
        }

        protected void CreatePieceMaterials(ContentPackage package)
        {
            SolidColorSpecularMaterial materialWhite = new SolidColorSpecularMaterial(Vector3.One, MaterialWhiteName);
            SolidColorSpecularMaterial materialBlack = new SolidColorSpecularMaterial(new Vector3(.5f, .5f, .5f), MaterialBlackName);
            SolidColorSpecularMaterial materialSelected = new SolidColorSpecularMaterial(Color.CornflowerBlue, SelectedMaterialName);

            materialWhite.Load(package);
            materialBlack.Load(package);
            materialSelected.Load(package);
            RenderScene.AddMaterial(materialWhite);
            RenderScene.AddMaterial(materialBlack);
            RenderScene.AddMaterial(materialSelected);

            // Set material parameters
            materialWhite.LightDirection = new Vector3(1f, -1f, 0f);
            materialBlack.LightDirection = new Vector3(1f, -1f, 0f);
            materialSelected.LightDirection = new Vector3(1f, -1f, 0f);
            materialWhite.Shininess = 3f;
            materialBlack.Shininess = 3f;
            materialSelected.Shininess = 3f;
            materialWhite.Specularity = .5f;
            materialBlack.Specularity = .5f;
            materialSelected.Specularity = .5f;
        }

        protected void CreatePieceModels(ContentPackage package)
        {
            Material materialWhite = RenderScene.Materials[MaterialWhiteName];
            Material materialBlack = RenderScene.Materials[MaterialBlackName];

            MeshGroup mesh = package.LoadMeshGroup(Pawn.MeshSource);
            Model pawnWhite = new Model() { Name = Pawn.ModelWhite, Material = materialWhite, MeshGroup = mesh };
            RenderScene.AddModel(pawnWhite);
            Model pawnBlack = new Model() { Name = Pawn.ModelBlack, Material = materialBlack, MeshGroup = mesh };
            RenderScene.AddModel(pawnBlack);

            mesh = package.LoadMeshGroup(Knight.MeshSource);
            Model knightWhite = new Model() { Name = Knight.ModelWhite, Material = materialWhite, MeshGroup = mesh };
            RenderScene.AddModel(knightWhite);
            Model knightBlack = new Model() { Name = Knight.ModelBlack, Material = materialBlack, MeshGroup = mesh };
            RenderScene.AddModel(knightBlack);

            mesh = package.LoadMeshGroup(Bishop.MeshSource);
            Model bishopWhite = new Model() { Name = Bishop.ModelWhite, Material = materialWhite, MeshGroup = mesh };
            RenderScene.AddModel(bishopWhite);
            Model bishopBlack = new Model() { Name = Bishop.ModelBlack, Material = materialBlack, MeshGroup = mesh };
            RenderScene.AddModel(bishopBlack);

            mesh = package.LoadMeshGroup(Rook.MeshSource);
            Model rookWhite = new Model() { Name = Rook.ModelWhite, Material = materialWhite, MeshGroup = mesh };
            RenderScene.AddModel(rookWhite);
            Model rookBlack = new Model() { Name = Rook.ModelBlack, Material = materialBlack, MeshGroup = mesh };
            RenderScene.AddModel(rookBlack);

            mesh = package.LoadMeshGroup(Queen.MeshSource);
            Model queenWhite = new Model() { Name = Queen.ModelWhite, Material = materialWhite, MeshGroup = mesh };
            RenderScene.AddModel(queenWhite);
            Model queenBlack = new Model() { Name = Queen.ModelBlack, Material = materialBlack, MeshGroup = mesh };
            RenderScene.AddModel(queenBlack);

            mesh = package.LoadMeshGroup(King.MeshSource);
            Model kingWhite = new Model() { Name = King.ModelWhite, Material = materialWhite, MeshGroup = mesh };
            RenderScene.AddModel(kingWhite);
            Model kingBlack = new Model() { Name = King.ModelBlack, Material = materialBlack, MeshGroup = mesh };
            RenderScene.AddModel(kingBlack);
        }

        protected void CreateBoardModels(ContentPackage package)
        {
            MeshGroup group = package.LoadMeshGroup(BoardMeshSource);
            int texture = package.LoadTexture(BoardTextureSource);

            DiffuseMaterial material = new DiffuseMaterial(texture, BoardMaterialName);
            material.Load(package);
            RenderScene.AddMaterial(material);

            Model pawn = new Model() { Name = BoardModelName, Material = material, MeshGroup = group };
            RenderScene.AddModel(pawn);

            RenderNode node = new RenderNode()
            {
                Model = pawn,
                Transform = Matrix4.Identity
            };
            RenderScene.AddRenderNode(node);
        }

        public Point GetMouseBoardPosition()
        {
            Vector3 Position = cameraObject.Position;

            Matrix4 View;
            Matrix4 Projection;

            Parent.Renderer.CameraInfo.GetView(out View);
            Parent.Renderer.CameraInfo.GetProjection(out Projection);

            Vector4 unProj = ProjectionHelper.UnProject(ref Projection, View, Parent.ClientSize, new Vector2(Mouse.X, Mouse.Y));
            Vector3 unProjDirection = unProj.Xyz;

            unProjDirection -= cameraObject.Position;
            unProjDirection.Normalize();
            unProjDirection *= -Position.Y / unProjDirection.Y;

            Vector3 boardPosition = Position + unProjDirection;
            Point boardPoint = new Point((int)(Math.Floor(boardPosition.X / ChessGame.BoardSqaureSize)), (int)(Math.Floor(boardPosition.Z / ChessGame.BoardSqaureSize)));

            return boardPoint;
        }

        public void UpdateSelector()
        {
            Point boardPoint = GetMouseBoardPosition();

            Vector3 transform = new Vector3(boardPoint.X * ChessGame.BoardSqaureSize + 
                ChessGame.BoardSqaureSize * .5f, 0.0f, boardPoint.Y * ChessGame.BoardSqaureSize + 
                ChessGame.BoardSqaureSize * .5f);

            selector.Transform = Matrix4.CreateTranslation(transform);

            if (boardPoint.X < 0 || boardPoint.X >= ChessBoard.BoardSize || boardPoint.Y < 0 ||
                boardPoint.Y >= ChessBoard.BoardSize)
                selector.Visible = false;
            else
                selector.Visible = true;

            if (Mouse[MouseButton.Right])
                selector.Visible = false;
        }

        protected void OnPieceSelected(ChessPiece piece)
        {
            ContentPackage package = Parent.Content[ChessGame.DefaultPackage];
            Model selectedModel;

            if (RenderScene.Models.ContainsKey(SelectedModelName))
                RenderScene.Models.Remove(SelectedModelName);

            if (selectedPiece != null)
            {
                RenderScene.RemoveRenderNode(selectedPiece.RenderNode);
                selectedPiece.RenderNode.Model = RenderScene.Models[selectedPiece.ModelName];
                RenderScene.AddRenderNode(selectedPiece.RenderNode);
            }

            selectedPiece = piece;

            if (piece != null)
            {
                selectedModel = new Model()
                {
                    Name = SelectedModelName,
                    MeshGroup = selectedPiece.RenderNode.Model.MeshGroup,
                    Material = RenderScene.Materials[SelectedMaterialName]
                };
                RenderScene.AddModel(selectedModel);

                RenderScene.RemoveRenderNode(selectedPiece.RenderNode);
                selectedPiece.RenderNode.Model = selectedModel;
                RenderScene.AddRenderNode(selectedPiece.RenderNode);
            }
        }

        protected void OnPieceOrder(ChessPiece piece, Point destination)
        {
            Point old = ChessGame.ConvertToPoint(piece.Position);
            Board[old.X, old.Y] = null;

            piece.Position = ChessGame.ConvertToPosition(destination);

            ChessPiece pieceAtDestination = Board[destination.X, destination.Y];
            if (pieceAtDestination != null)
                OnPieceDestroyed(pieceAtDestination);

            Board[destination.X, destination.Y] = piece;
            piece.UpdateTransform();
        }

        protected void OnPieceDestroyed(ChessPiece piece)
        {
            RenderScene.RemoveRenderNode(piece.RenderNode);
        }

        public void UpdateCamera(FrameEventArgs e)
        {
            // Update camera movement
            if (Mouse[OpenTK.Input.MouseButton.Right])
            {
                cameraObject.UpdateInputMouse(Mouse, Parent.ClientSize, e);
                Cursor.Position = Parent.PointToScreen(new System.Drawing.Point(Parent.ClientSize.Width / 2, Parent.ClientSize.Height / 2));
            }
            cameraObject.UpdateInputKeyboard(Keyboard, e);
            cameraObject.Position = new Vector3(cameraObject.Position.X, Math.Max(cameraObject.Position.Y, MinCameraHeight), cameraObject.Position.Z);

            cameraObject.UpdateInfo(Parent.Renderer.CameraInfo);
        }

        public void OnUpdateFrame(FrameEventArgs e)
        {
            UpdateCamera(e);

            UpdateSelector();

            // Check if exit
            if (Keyboard[OpenTK.Input.Key.Escape])
                Parent.Exit();

            // Update the render scene
            RenderScene.Update(e);
        }

        public void Dispose()
        {
            RenderScene.Dispose();
        }
    }
}
