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
        bool debugMode = false;
        bool manualControl = false;
        SpriteFont Arial6;
        SpriteFont Arial12;
        Map map;
        Tile tile = new Tile();
        KeyboardState oldks;
        MouseState oldms;
        int visibleSquareWidth = 18;
        int visibleSquareHeight = 42;
        int baseOffsetX = -32;
        int baseOffsetY = -64;
        float heightRowDepthMod = 0.0000001f;
        Character vlad;
        Texture2D highlight;
        Texture2D border;
        Texture2D debugHUD;

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
            Arial6 = Content.Load<SpriteFont>("Fonts/Arial6");
            Arial12 = Content.Load<SpriteFont>("Fonts/Arial12");
            map = new Map(Content.Load<Texture2D>("Textures/Tilesets/mousemap"),
                          Content.Load<Texture2D>(@"Textures\TileSets\slopemap"));
            Camera.WorldWidth = ((map.MapWidth - 2) * Tile.StepX);
            Camera.WorldHeight = ((map.MapHeight - 2) * Tile.StepY);
            highlight = Content.Load<Texture2D>("Textures/Tilesets/highlight");
            border = Content.Load<Texture2D>("Textures/Tilesets/border");
            debugHUD = new Texture2D(GraphicsDevice, 1, 1);
            debugHUD.SetData(new Color[] { Color.White });


            //Vlad to separate class or character class with common methods
            vlad = new Character(Content.Load<Texture2D>("Textures/Characters/T_Vlad_Sword_Walking_48x48"));
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

            vlad.Position = new Vector2(200, 200);
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

            
            KeyboardState ks = Keyboard.GetState();
            MouseState ms = Mouse.GetState();

            if (ms.LeftButton == ButtonState.Pressed && oldms.LeftButton == ButtonState.Released)
            {
                Vector2 mouseLoc = new Vector2(Mouse.GetState().X, Mouse.GetState().Y);
                if (mouseLoc.X > 0 && mouseLoc.X < GraphicsDevice.Viewport.Width &&
                    mouseLoc.Y > 0 && mouseLoc.Y < GraphicsDevice.Viewport.Height)
                {
                    Vector2 mouseWorldLoc = Camera.ScreenToWorld(mouseLoc);
                    vlad.SetTarget(map, new Point((int)mouseWorldLoc.X, (int)mouseWorldLoc.Y));
                }
            }

            vlad.Move(map, manualControl, ks);

            if (ks.IsKeyDown(Keys.C) && oldks.IsKeyUp(Keys.C))
            {
                debugMode = !debugMode;
            }

            if (ks.IsKeyDown(Keys.X) && oldks.IsKeyUp(Keys.X))
            {
                manualControl = !manualControl;
            }
            oldks = ks;
            oldms = ms;

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
                                    mapx * Tile.StepX + rowOffset,
                                    mapy * Tile.StepY - (heightRow * Tile.HeightOffset))),
                            tile.GetSourceRectangle(tileID),
                            Color.White,
                            0.0f,
                            Vector2.Zero,
                            1.0f,
                            SpriteEffects.None,
                            depthOffset - ((float)heightRow * heightRowDepthMod));
                        heightRow++;
                    }

                    if (debugMode)
                    {
                        Color debugColor = new Color(255, 255 - heightRow * 50, 255 - heightRow * 50);
                        if (vlad.path.Contains(new Point(mapx, mapy)))
                            debugColor = Color.Blue;
                        spriteBatch.Draw(
                            border,
                            Camera.WorldToScreen(
                                new Vector2(
                                    mapx * Tile.StepX + rowOffset,
                                    mapy * Tile.StepY - ((heightRow - 1) * Tile.HeightOffset))),
                            new Rectangle(0, 0, 64, 32),
                            debugColor,
                            0.0f,
                            Vector2.Zero,
                            1.0f,
                            SpriteEffects.None,
                            depthOffset - ((float)heightRow * heightRowDepthMod));
                        spriteBatch.DrawString(
                            Arial6,
                            mapx.ToString() + ", " + mapy.ToString(),
                            new Vector2((x * Tile.StepX) - offsetX + rowOffset + baseOffsetX + 24,
                                        ((y * Tile.StepY) - offsetY + baseOffsetY + 48)
                                        - (heightRow * Tile.HeightOffset)),
                            debugColor,
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

                    if ((mapx == vladMapPoint.X) && (mapy == vladMapPoint.Y))
                    {
                        vlad.DrawDepth = depthOffset - (float)(heightRow + 2) * heightRowDepthMod;
                    }
                }
            }
            Vector2 highlightLoc = Camera.ScreenToWorld(new Vector2(Mouse.GetState().X, Mouse.GetState().Y));
            Point highlightWorldLoc = new Point((int)highlightLoc.X, (int)highlightLoc.Y);
            Point hilightOriginPoint = map.WorldToMapCell(highlightWorldLoc);

            vlad.Draw(spriteBatch, 0, (-1) * map.GetOverallHeight(vlad.Position));

            int hilightrowOffset = 0;
            if ((hilightOriginPoint.Y) % 2 == 1)
                hilightrowOffset = Tile.OddRowXOffset;

            if (debugMode)
            {
                float debugHUDOriginX = GraphicsDevice.Viewport.Width - 110;
                float debugHUDOriginY = 0;
                spriteBatch.Draw(
                     debugHUD,
                     new Vector2(debugHUDOriginX, debugHUDOriginY),
                     new Rectangle(0, 0, 110, GraphicsDevice.Viewport.Height),
                     Color.LightGray,
                     0.0f,
                     Vector2.Zero,
                     1.0f,
                     SpriteEffects.None,
                     0.1f);
                MapCell cell = map.GetCellAtWorldPoint(highlightLoc);
                if (cell != null)
                {
                    int yOffset = 10;
                    spriteBatch.DrawString(
                         Arial12,
                         "Current: " + cell.X + ", " + cell.Y,
                         new Vector2(debugHUDOriginX + 10, debugHUDOriginY + yOffset),
                         Color.Black,
                         0.0f,
                         Vector2.Zero,
                         1.0f,
                         SpriteEffects.None,
                         0.05f);
                    foreach (var neighbour in map.GetNeighbours(cell))
                    {
                        yOffset += 20;
                        spriteBatch.DrawString(
                         Arial12,
                         neighbour.Key + ": " + neighbour.Value.X + ", " + neighbour.Value.Y,
                         new Vector2(debugHUDOriginX + 10, debugHUDOriginY + yOffset),
                         Color.Black,
                         0.0f,
                         Vector2.Zero,
                         1.0f,
                         SpriteEffects.None,
                         0.05f);
                    }
                }
            }
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
