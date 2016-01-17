using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pathfinder
{
    class Tile
    {
        static public int Width = 33;
        static public int Height = 27;
        static public int TileStepX = 50;
        static public int TileStepY = 12;
        static public int OddRowXOffset = 24;
        public Texture2D TileSetTexture { get; set; }

        public Rectangle GetSourceRectangle(int tileIndex)
        {
            int tileY = tileIndex / (TileSetTexture.Width / Width);
            int tileX = tileIndex % (TileSetTexture.Width / Width);

            return new Rectangle(tileX * Width, tileY * Height, Width, Height);
        }
    }
}
