using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chess.Graphics
{
    public class Model
    {
        public string Name;
        public Material Material;
        public MeshGroup MeshGroup;
        protected bool bCastsShadows = false;

        public int MeshCount 
        {
            get { return MeshGroup.Meshes.Length; } 
        }
        public bool CastsShadows
        {
            get { return bCastsShadows; }
            set { bCastsShadows = value; }
        }
        public override string ToString()
        {
            return Name;
        }
    }
}
