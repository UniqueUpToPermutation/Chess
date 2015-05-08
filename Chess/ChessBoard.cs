using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

using Chess.Graphics;

using OpenTK;

namespace Chess
{
    public enum Player
    {
        Black, White
    }

    public abstract class ChessPiece
    {
        private RenderNode renderNode;
        private Vector3 position;
        private float rotation;
        private Player player;

        public abstract string ModelNameWhite { get; }
        public abstract string ModelNameBlack { get; }
        public string ModelName
        {
            get
            {
                if (player == Chess.Player.White)
                    return ModelNameWhite;
                if (player == Chess.Player.Black)
                    return ModelNameBlack;
                return string.Empty;
            }
        }

        public RenderNode RenderNode
        {
            get { return renderNode; }
        }

        public Player Player
        {
            get { return player; }
            set { player = value; }
        }

        public Vector3 Position
        {
            get { return position; }
            set
            {
                position = value;
                UpdateTransform();
            }
        }

        public float Rotation
        {
            get { return rotation; }
            set
            {
                rotation = value;
                UpdateTransform();
            }
        }

        public void Initialize(Player player, RenderScene scene)
        {
            this.player = player;

            if (player == Chess.Player.White)
                renderNode = new Graphics.RenderNode() { Model = scene.Models[ModelNameWhite] };
            else if (player == Chess.Player.Black)
                renderNode = new Graphics.RenderNode() { Model = scene.Models[ModelNameBlack] };

            UpdateTransform();
        }

        public void UpdateTransform()
        {
            // Rotate and then translate the piece
            renderNode.Transform = Matrix4.CreateRotationY(rotation) * Matrix4.CreateTranslation(position);
        }

        public ChessPiece Copy()
        {
            ChessPiece piece = BlankCopy();
            piece.Position = position;
            piece.renderNode = new RenderNode() { Model = renderNode.Model, Transform = renderNode.Transform };
            piece.rotation = rotation;
            return piece;
        }

        protected abstract ChessPiece BlankCopy();
    }

    public abstract class ChessPieceCopyable<Type> : ChessPiece
        where Type : ChessPiece, new()
    {
        protected override ChessPiece BlankCopy()
        {
            return new Type();
        }
    }

    public class Pawn : ChessPieceCopyable<Pawn>
    {
        public const string ModelWhite = "PawnWhite";
        public const string ModelBlack = "PawnBlack";
        public const string MeshSource = "Content\\Pawn.obj";

        public override string ModelNameWhite
        {
            get { return ModelWhite; }
        }

        public override string ModelNameBlack
        {
            get { return ModelBlack; }
        }
    }

    public class Knight : ChessPieceCopyable<Knight>
    {
        public const string ModelWhite = "KnightWhite";
        public const string ModelBlack = "KnightBlack";
        public const string MeshSource = "Content\\Knight.obj";

        public override string ModelNameWhite
        {
            get { return ModelWhite; }
        }

        public override string ModelNameBlack
        {
            get { return ModelBlack; }
        }
    }

    public class Bishop : ChessPieceCopyable<Bishop>
    {
        public const string ModelWhite = "BishopWhite";
        public const string ModelBlack = "BishopBlack";
        public const string MeshSource = "Content\\Bishop.obj";

        public override string ModelNameWhite
        {
            get { return ModelWhite; }
        }

        public override string ModelNameBlack
        {
            get { return ModelBlack; }
        }
    }

    public class Rook : ChessPieceCopyable<Rook>
    {
        public const string ModelWhite = "RookWhite";
        public const string ModelBlack = "RookBlack";
        public const string MeshSource = "Content\\Rook.obj";

        public override string ModelNameWhite
        {
            get { return ModelWhite; }
        }

        public override string ModelNameBlack
        {
            get { return ModelBlack; }
        }
    }

    public class Queen : ChessPieceCopyable<Queen>
    {
        public const string ModelWhite = "QueenWhite";
        public const string ModelBlack = "QueenBlack";
        public const string MeshSource = "Content\\Queen.obj";

        public override string ModelNameWhite
        {
            get { return ModelWhite; }
        }

        public override string ModelNameBlack
        {
            get { return ModelBlack; }
        }
    }

    public class King : ChessPieceCopyable<King>
    {
        public const string ModelWhite = "KingWhite";
        public const string ModelBlack = "KingBlack";
        public const string MeshSource = "Content\\King.obj";

        public override string ModelNameWhite
        {
            get { return ModelWhite; }
        }

        public override string ModelNameBlack
        {
            get { return ModelBlack; }
        }
    }

    public class ChessBoard
    {
        public const int BoardSize = 8;

        protected ChessPiece[,] pieces = new ChessPiece[BoardSize, BoardSize];

        public bool IsInBounds(Point point)
        {
            return (point.X >= 0 && point.Y >= 0 && point.X < BoardSize && point.Y < BoardSize);
        }

        public bool IsMoveAllowed(ChessPiece piece, Point destination)
        {
            return true;
        }

        public ChessPiece this[int x, int y]
        {
            get { return pieces[x, y]; }
            set { pieces[x, y] = value; }
        }

        public void Clear()
        {
            pieces = new ChessPiece[BoardSize, BoardSize];
        }
    }
}
