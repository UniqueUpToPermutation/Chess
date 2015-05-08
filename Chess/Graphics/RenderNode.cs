using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OpenTK;

namespace Chess.Graphics
{
    public class RenderNode
    {
        public Model Model;
        public Matrix4 Transform;
        public bool Visible = true;

        public override string ToString()
        {
            return Model.ToString();
        }
    }
}
