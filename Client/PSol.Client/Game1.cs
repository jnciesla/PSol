using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using GeonBit.UI;

using Bindings;
using PSol.Data.Models;
using System.Collections.Generic;

namespace PSol.Client
{
    public class Game1 : Game
    {
        public static GraphicsDeviceManager graphics;
        public static SpriteBatch spriteBatch;
        public static ParticleEngine particleEngine;
        private Texture2D backGroundTexture;
        private Vector2 backgroundPos;
        private RenderTarget2D renderTarget;

        private ClientTCP ctcp;
        private HandleData chd;
        Camera camera;

        private readonly InterfaceGUI IGUI = new InterfaceGUI();
        private static readonly KeyControl KC = new KeyControl();

        private float WalkTimer;
        public new static int Tick;
        public static int ElapsedTime;
        public static int FrameTime;
        Laser.Beam beam;
        Laser bolt;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            if (Globals.Fullscreen)
            {
                Globals.PreferredBackBufferHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
                Globals.PreferredBackBufferWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
            }

            graphics.PreferredBackBufferWidth = Globals.PreferredBackBufferWidth;
            graphics.PreferredBackBufferHeight = Globals.PreferredBackBufferHeight;
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // Initialize fonts
            Globals.Font10 = Content.Load<SpriteFont>("GeonBit.UI/themes/editor/fonts/Size10");
            Globals.Font8 = Content.Load<SpriteFont>("GeonBit.UI/themes/editor/fonts/Size8");

            for (var i = 1; i < Constants.MAX_PLAYERS; i++)
            {
                Types.Player[i] = new User();
            }
            ctcp = new ClientTCP();
            chd = new HandleData();
            renderTarget = new RenderTarget2D(GraphicsDevice, GraphicsDevice.PresentationParameters.BackBufferWidth, GraphicsDevice.PresentationParameters.BackBufferHeight, false, GraphicsDevice.PresentationParameters.BackBufferFormat, DepthFormat.Depth24);
            camera = new Camera(GraphicsDevice.Viewport);
            chd.InitializeMesssages();
            ctcp.ConnectToServer();
            Graphics.InitializeGraphics(Content);
            UserInterface.Initialize(Content, "classic");
            UserInterface.Active.UseRenderTarget = true;
            IGUI.InitializeGUI(Content);
            MenuManager.ChangeMenu(MenuManager.Menu.Login);

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
            backGroundTexture = Content.Load<Texture2D>("stars3");
            backgroundPos = new Vector2(-Globals.PreferredBackBufferWidth / 2.0F, -Globals.PreferredBackBufferHeight / 2.0F);

            // Particle engine textures
            List<Texture2D> textures = new List<Texture2D>();
            textures.Add(Content.Load<Texture2D>("Particles/circle"));
            particleEngine = new ParticleEngine(textures, new Vector2(0, 0));

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
            if (Globals.exitgame) Exit();
            UserInterface.Active.Update(gameTime);
            if (!Globals.cursorOverride) { UserInterface.Active.SetCursor(CursorType.Default); }

            if (GameLogic.PlayerIndex > -1)
            {
                particleEngine.EmitterLocation = new Vector2(Types.Player[GameLogic.PlayerIndex].X, Types.Player[GameLogic.PlayerIndex].Y);
                particleEngine.Update(Globals.DirUp);
            }
            IGUI.lblStatus.Text = ctcp.isOnline ? "Server status:{{GREEN}} online" : "Server status:{{RED}} offline";
            CheckKeys();

            bolt?.Update();
            beam?.Update();

            camera.Update(gameTime, this);
            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            DrawSceneToTexture(renderTarget, gameTime);

            GraphicsDevice.Clear(Color.Black);
            UserInterface.Active.Draw(spriteBatch);

            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend,
                SamplerState.LinearClamp, DepthStencilState.Default,
                RasterizerState.CullNone);

            spriteBatch.Draw(renderTarget, new Rectangle(0, 0, Globals.PreferredBackBufferWidth, Globals.PreferredBackBufferHeight), Globals.Luminosity);

            spriteBatch.End();


            base.Draw(gameTime);
        }

        /// <summary>
        /// Draws the entire scene in the given render target.
        /// </summary>
        /// <returns>A texture2D with the scene drawn in it.</returns>
        protected void DrawSceneToTexture(RenderTarget2D renderTarget, GameTime gameTime)
        {
            // Set the render target
            GraphicsDevice.SetRenderTarget(renderTarget);
            GraphicsDevice.DepthStencilState = new DepthStencilState() { DepthBufferEnable = true };
            // Draw the scene
            GraphicsDevice.Clear(Color.Black);

            Tick = (int)gameTime.TotalGameTime.TotalMilliseconds;
            ElapsedTime = Tick - FrameTime;
            FrameTime = Tick;

            if (WalkTimer < Tick)
            {
                GameLogic.CheckMovement();
                camera.ZoomController();
                WalkTimer = Tick + 15;
                Globals.PlanetaryRotation += MathHelper.ToRadians(.01f);
            }

            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.LinearWrap, null, null, null, Camera.transform);
            spriteBatch.Draw(backGroundTexture, backgroundPos, Globals.mapSize, Color.White);
            Graphics.DrawBorder(Globals.playArea, 2, Color.DarkOliveGreen);
            if (GameLogic.Galaxy != null)
            {
                Graphics.DrawSystems();
            }

            bolt?.Draw(spriteBatch);
            beam?.Draw(spriteBatch, Color.Red);

            particleEngine.Draw(spriteBatch);
            Graphics.RenderPlayers();
            spriteBatch.End();

            Graphics.DrawHud(Content);
            UserInterface.Active.DrawMainRenderTarget(spriteBatch);
            
            // Drop the render target
            GraphicsDevice.SetRenderTarget(null);
        }

        private void CheckKeys()
        {
            if (KC.KeyPress(Keys.L) && Globals.Control)
            {
                if (Globals.Luminosity == Color.White)
                {
                    Globals.Luminosity = Color.Gray;
                }
                else
                {
                    Globals.Luminosity = Color.White;
                }
            }

            if (KC.KeyPress(Keys.Q) && Globals.Control)
            {
                Globals.exitgame = true;
            }

            Globals.Control = KC.CheckCtrl();

            if (!Globals.windowOpen && !Globals.Control) // Don't allow game input when menus are open or CTRL is pressed
            {
                
                if (Keyboard.GetState().IsKeyDown(Keys.Space)) { beam = new Laser.Beam(new Vector2(100,100), new Vector2(1000,1000), 2); }
                if (Keyboard.GetState().IsKeyDown(Keys.Space)) { bolt = new Laser(new Vector2(100, 100), new Vector2(1000, 1000), Color.Red); }
                Globals.DirUp = Keyboard.GetState().IsKeyDown(Keys.W);
                Globals.DirDn = Keyboard.GetState().IsKeyDown(Keys.S);
                Globals.DirLt = Keyboard.GetState().IsKeyDown(Keys.A);
                Globals.DirRt = Keyboard.GetState().IsKeyDown(Keys.D);
                Globals.ZoomIn = Keyboard.GetState().IsKeyDown(Keys.E);
                Globals.ZoomOut = Keyboard.GetState().IsKeyDown(Keys.Q);
                Globals.ZoomDefault = Keyboard.GetState().IsKeyDown(Keys.R);
                Globals.Details1 = Keyboard.GetState().IsKeyDown(Keys.LeftAlt);
                Globals.Details2 = Keyboard.GetState().IsKeyDown(Keys.RightAlt);
                if (Keyboard.GetState().IsKeyDown(Keys.Escape)) { GameLogic.Selected = -1; GameLogic.selectedPlanet = -1; }
                if (KC.KeyPress(Keys.T))
                {
                    MenuManager.ChangeMenu(MenuManager.Menu.Message);
                    InterfaceGUI.messageText.IsFocused = true;
                }
            }
            else
            {
                if (KC.KeyPress(Keys.Tab)) { IGUI.TabThrough(); }
                if (KC.KeyPress(Keys.Enter)) { IGUI.Enter(); }
                if (KC.KeyPress(Keys.Escape)) { MenuManager.Clear(3); }
            }
        }

    }
}
