using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace Pathfinder
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        bool showCoordinates = false;
        SpriteFont font;
        Map map;
        Tile tile = new Tile();
        KeyboardState oldks;
        int visibleSquareWidth = 18;
        int visibleSquareHeight = 42;
        int baseOffsetX = -32;
        int baseOffsetY = -64;
        float heightRowDepthMod = 0.0000001f;
        SpriteAnimation vlad;
        Texture2D highlight;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            Camera.ViewWidth = this.graphics.PreferredBackBufferWidth;
            Camera.ViewHeight = this.graphics.PreferredBackBufferHeight;
            Camera.DisplayOffset = new Vector2(baseOffsetX, baseOffsetY);
            this.IsMouseVisible = true;
            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
            tile.TileSetTexture = Content.Load<Texture2D>("Textures/Tilesets/tileset");
            font = Content.Load<SpriteFont>("Fonts/Arial6");
            map = new Map(Content.Load<Texture2D>("Textures/Tilesets/mousemap"),
                          Content.Load<Texture2D>(@"Textures\TileSets\slopemap"));
            Camera.WorldWidth = ((map.MapWidth - 2) * Tile.StepX);
            Camera.WorldHeight = ((map.MapHeight - 2) * Tile.StepY);
            highlight = Content.Load<Texture2D>("Textures/Tilesets/highlight");

            //Vlad to separate class or character class with common methods
            vlad = new SpriteAnimation(Content.Load<Texture2D>("Textures/Characters/T_Vlad_Sword_Walking_48x48"));
            vlad.AddAnimation("WalkEast", 0, 48 * 0, 48, 48, 8, 0.1f);
            vlad.AddAnimation("WalkNorth", 0, 48 * 1, 48, 48, 8, 0.1f);
            vlad.AddAnimation("WalkNorthEast", 0, 48 * 2, 48, 48, 8, 0.1f);
            vlad.AddAnimation("WalkNorthWest", 0, 48 * 3, 48, 48, 8, 0.1f);
            vlad.AddAnimation("WalkSouth", 0, 48 * 4, 48, 48, 8, 0.1f);
            vlad.AddAnimation("WalkSouthEast", 0, 48 * 5, 48, 48, 8, 0.1f);
            vlad.AddAnimation("WalkSouthWest", 0, 48 * 6, 48, 48, 8, 0.1f);
            vlad.AddAnimation("WalkWest", 0, 48 * 7, 48, 48, 8, 0.1f);

            vlad.AddAnimation("IdleEast", 0, 48 * 0, 48, 48, 1, 0.2f);
            vlad.AddAnimation("IdleNorth", 0, 48 * 1, 48, 48, 1, 0.2f);
            vlad.AddAnimation("IdleNorthEast", 0, 48 * 2, 48, 48, 1, 0.2f);
            vlad.AddAnimation("IdleNorthWest", 0, 48 * 3, 48, 48, 1, 0.2f);
            vlad.AddAnimation("IdleSouth", 0, 48 * 4, 48, 48, 1, 0.2f);
            vlad.AddAnimation("IdleSouthEast", 0, 48 * 5, 48, 48, 1, 0.2f);
            vlad.AddAnimation("IdleSouthWest", 0, 48 * 6, 48, 48, 1, 0.2f);
            vlad.AddAnimation("IdleWest", 0, 48 * 7, 48, 48, 1, 0.2f);

            vlad.Position = new Vector2(100, 100);
            vlad.DrawOffset = new Vector2(-24, -38);
            vlad.CurrentAnimation = "WalkEast";
            vlad.IsAnimating = true;
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            Vector2 moveVector = Vector2.Zero;
            Vector2 moveDir = Vector2.Zero;
            string animation = "";

            KeyboardState ks = Keyboard.GetState();

            if (ks.IsKeyDown(Keys.NumPad7))
            {
                moveDir = new Vector2(-2, -1);
                animation = "WalkNorthWest";
                moveVector += new Vector2(-2, -1);
            }

            if (ks.IsKeyDown(Keys.NumPad8))
            {
                moveDir = new Vector2(0, -1);
                animation = "WalkNorth";
                moveVector += new Vector2(0, -1);
            }

            if (ks.IsKeyDown(Keys.NumPad9))
            {
                moveDir = new Vector2(2, -1);
                animation = "WalkNorthEast";
                moveVector += new Vector2(2, -1);
            }

            if (ks.IsKeyDown(Keys.NumPad4))
            {
                moveDir = new Vector2(-2, 0);
                animation = "WalkWest";
                moveVector += new Vector2(-2, 0);
            }

            if (ks.IsKeyDown(Keys.NumPad6))
            {
                moveDir = new Vector2(2, 0);
                animation = "WalkEast";
                moveVector += new Vector2(2, 0);
            }

            if (ks.IsKeyDown(Keys.NumPad1))
            {
                moveDir = new Vector2(-2, 1);
                animation = "WalkSouthWest";
                moveVector += new Vector2(-2, 1);
            }

            if (ks.IsKeyDown(Keys.NumPad2))
            {
                moveDir = new Vector2(0, 1);
                animation = "WalkSouth";
                moveVector += new Vector2(0, 1);
            }

            if (ks.IsKeyDown(Keys.NumPad3))
            {
                moveDir = new Vector2(2, 1);
                animation = "WalkSouthEast";
                moveVector += new Vector2(2, 1);
            }

            if (map.GetCellAtWorldPoint(vlad.Position + moveDir).Walkable == false)
            {
                moveDir = Vector2.Zero;
            }

            if (Math.Abs(map.GetOverallHeight(vlad.Position) - map.GetOverallHeight(vlad.Position + moveDir)) > 10)
            {
                moveDir = Vector2.Zero;
            }

            if (moveDir.Length() != 0)
            {
                vlad.MoveBy((int)moveDir.X, (int)moveDir.Y);
                if (vlad.CurrentAnimation != animation)
                    vlad.CurrentAnimation = animation;
            }
            else
            {
                vlad.CurrentAnimation = "Idle" + vlad.CurrentAnimation.Substring(4);
            }

            if (ks.IsKeyDown(Keys.C) && oldks.IsKeyUp(Keys.C))
            {
                showCoordinates = !showCoordinates;
            }
            oldks = ks;

            float vladX = MathHelper.Clamp(
                vlad.Position.X, 0 + (-2) * vlad.DrawOffset.X, Camera.WorldWidth);
            float vladY = MathHelper.Clamp(
                vlad.Position.Y, 0 + (-2)* vlad.DrawOffset.Y, Camera.WorldHeight);

            vlad.Position = new Vector2(vladX, vladY);

            vlad.Update(gameTime);
            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend);
            float maxdepth = ((map.MapWidth + 1) + ((map.MapHeight + 1) * Tile.Width)) * 10;
            float depthOffset;
            Point vladMapPoint = map.WorldToMapCell(new Point((int)vlad.Position.X, (int)vlad.Position.Y));

            Vector2 firstSquare = new Vector2(Camera.Location.X / Tile.StepX, Camera.Location.Y / Tile.StepY);
            int firstX = (int)firstSquare.X;
            int firstY = (int)firstSquare.Y;

            Vector2 squareOffset = new Vector2(Camera.Location.X % Tile.StepX, Camera.Location.Y % Tile.StepY);
            int offsetX = (int)squareOffset.X;
            int offsetY = (int)squareOffset.Y;

            for (int y = 0; y < visibleSquareHeight; y++)
            {
                int rowOffset = 0;
                if ((firstY + y) % 2 == 1)
                    rowOffset = Tile.OddRowXOffset;
                for (int x = 0; x < visibleSquareWidth; x++)
                {
                    int mapx = (firstX + x);
                    int mapy = (firstY + y);
                    depthOffset = 0.7f - ((mapx + (mapy * Tile.Width)) / maxdepth);
                    if ((mapx >= map.MapWidth) || (mapy >= map.MapHeight))
                        continue;
                    foreach (int tileID in map.Rows[mapy].Columns[mapx].BaseTiles)
                    {
                        spriteBatch.Draw(

                            tile.TileSetTexture,
                            Camera.WorldToScreen(
                                new Vector2((mapx * Tile.StepX) + rowOffset, mapy * Tile.StepY)),
                            tile.GetSourceRectangle(tileID),
                            Color.White,
                            0.0f,
                            Vector2.Zero,
                            1.0f,
                            SpriteEffects.None,
                            1.0f);
                    }
                    int heightRow = 0;

                    foreach (int tileID in map.Rows[mapy].Columns[mapx].HeightTiles)
                    {
                        spriteBatch.Draw(
                            tile.TileSetTexture,
                            Camera.WorldToScreen(
                                new Vector2(
                                    (mapx * Tile.StepX) + rowOffset,
                                    mapy * Tile.StepY - (heightRow * Tile.HeightTileOffset))),
                            tile.GetSourceRectangle(tileID),
                            Color.White,
                            0.0f,
                            Vector2.Zero,
                            1.0f,
                            SpriteEffects.None,
                            depthOffset - ((float)heightRow * heightRowDepthMod));
                        heightRow++;
                    }

                    foreach (int tileID in map.Rows[y + firstY].Columns[x + firstX].TopperTiles)
                    {
                        spriteBatch.Draw(
                            tile.TileSetTexture,
                            Camera.WorldToScreen(
                                new Vector2((mapx * Tile.StepX) + rowOffset, mapy * Tile.StepY)),
                            tile.GetSourceRectangle(tileID),
                            Color.White,
                            0.0f,
                            Vector2.Zero,
                            1.0f,
                            SpriteEffects.None,
                            depthOffset - ((float)heightRow * heightRowDepthMod));
                    }
                    if (showCoordinates)
                    {
                        spriteBatch.DrawString(font, (x + firstX).ToString() + ", " + (y + firstY).ToString(),
                                new Vector2((x * Tile.StepX) - offsetX + rowOffset + baseOffsetX + 24,
                                (y * Tile.StepY) - offsetY + baseOffsetY + 48), Color.White, 0f, Vector2.Zero,
                                1.0f, SpriteEffects.None, 0.0f);
                    }
                    if ((mapx == vladMapPoint.X) && (mapy == vladMapPoint.Y))
                    {
                        vlad.DrawDepth = depthOffset - (float)(heightRow + 2) * heightRowDepthMod;
                    }
                }
            }
            Vector2 highlightLoc = Camera.ScreenToWorld(new Vector2(Mouse.GetState().X, Mouse.GetState().Y));
            Point hilightPoint = map.WorldToMapCell(new Point((int)highlightLoc.X, (int)highlightLoc.Y));

            vlad.Draw(spriteBatch, 0, (-1)*map.GetOverallHeight(vlad.Position));

            int hilightrowOffset = 0;
            if ((hilightPoint.Y) % 2 == 1)
                hilightrowOffset = Tile.OddRowXOffset;

            spriteBatch.Draw(
                            highlight,
                            Camera.WorldToScreen(

                                new Vector2(

                                    (hilightPoint.X * Tile.StepX) + hilightrowOffset,

                                    (hilightPoint.Y + 2) * Tile.StepY)),
                            new Rectangle(0, 0, 64, 32),
                            Color.White * 0.3f,
                            0.0f,
                            Vector2.Zero,
                            1.0f,
                            SpriteEffects.None,
                            0.0f);
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
