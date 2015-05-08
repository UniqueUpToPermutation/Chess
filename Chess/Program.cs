using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chess
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            using (ChessGame game = new ChessGame())
            {
                game.Initialize();
                game.Run();
            }
        }
    }
}
