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
        static public int Width = 64;
        static public int Height = 64;
        static public int StepX = 64;
        static public int StepY = 16;
        static public int OddRowXOffset = 32;
        static public int HeightTileOffset = 32;
        public Texture2D TileSetTexture { get; set; }

        public Rectangle GetSourceRectangle(int tileIndex)
        {
            int tileY = tileIndex / (TileSetTexture.Width / Width);
            int tileX = tileIndex % (TileSetTexture.Width / Width);

            return new Rectangle(tileX * Width, tileY * Height, Width, Height);
        }
    }
}
