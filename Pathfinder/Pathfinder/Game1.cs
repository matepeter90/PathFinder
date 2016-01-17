using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

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
        Map map = new Map();
        Tile tile = new Tile();
        KeyboardState oldks;
        int visibleSquareWidth = 18;
        int visibleSquareHeight = 42;
        int baseOffsetX = -32;
        int baseOffsetY = -64;
        float heightRowDepthMod = 0.0000001f;

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
            // TODO: Add your initialization logic here

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
            tile.TileSetTexture = Content.Load<Texture2D>("tileset");
            font = Content.Load<SpriteFont>("Arial6");

            // TODO: use this.Content to load your game content here
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
            if (ks.IsKeyDown(Keys.Left))
            {
                Camera.Location.X = MathHelper.Clamp(Camera.Location.X - 2, 0,
                    (map.MapWidth - visibleSquareWidth) * Tile.StepX);
            }

            if (ks.IsKeyDown(Keys.Right))
            {
                Camera.Location.X = MathHelper.Clamp(Camera.Location.X + 2, 0,
                    (map.MapWidth - visibleSquareWidth) * Tile.StepX);
            }

            if (ks.IsKeyDown(Keys.Up))
            {
                Camera.Location.Y = MathHelper.Clamp(Camera.Location.Y - 2, 0,
                    (map.MapHeight - visibleSquareHeight) * Tile.StepY);
            }

            if (ks.IsKeyDown(Keys.Down))
            {
                Camera.Location.Y = MathHelper.Clamp(Camera.Location.Y + 2, 0,
                    (map.MapHeight - visibleSquareHeight) * Tile.StepY);
            }
            if (ks.IsKeyDown(Keys.C) && oldks.IsKeyUp(Keys.C))
            {
                showCoordinates = !showCoordinates;
            }
            oldks = ks;

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
                    foreach (int tileID in map.Rows[mapy].Columns[mapx].BaseTiles)
                    {
                        spriteBatch.Draw(
                            tile.TileSetTexture,
                            new Rectangle(
                                (x * Tile.StepX) - offsetX + rowOffset + baseOffsetX,
                                (y * Tile.StepY) - offsetY + baseOffsetY,
                                Tile.Width, Tile.Height),
                            tile.GetSourceRectangle(tileID),
                            Color.White,
                            0.0f,
                            Vector2.Zero,
                            SpriteEffects.None,
                            1.0f);
                    }

                    int heightRow = 0;
                    foreach (int tileID in map.Rows[mapy].Columns[mapx].HeightTiles)
                    {
                        spriteBatch.Draw(
                            tile.TileSetTexture,
                            new Rectangle(
                                (x * Tile.StepX) - offsetX + rowOffset + baseOffsetX,
                                (y * Tile.StepY) - offsetY + baseOffsetY - (heightRow * Tile.HeightTileOffset),
                                Tile.Width, Tile.Height),
                            tile.GetSourceRectangle(tileID),
                            Color.White,
                            0.0f,
                            Vector2.Zero,
                            SpriteEffects.None,
                            depthOffset - ((float)heightRow * heightRowDepthMod));
                        heightRow++;
                    }

                    foreach (int tileID in map.Rows[y + firstY].Columns[x + firstX].TopperTiles)
                    {
                        spriteBatch.Draw(
                            tile.TileSetTexture,
                            new Rectangle(
                                (x * Tile.StepX) - offsetX + rowOffset + baseOffsetX,
                                (y * Tile.StepY) - offsetY + baseOffsetY - (heightRow * Tile.HeightTileOffset),
                                Tile.Width, Tile.Height),
                            tile.GetSourceRectangle(tileID),
                            Color.White,
                            0.0f,
                            Vector2.Zero,
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

                }
            }

            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
