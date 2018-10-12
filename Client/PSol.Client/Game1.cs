using System;
using System.Collections.Generic;
using System.Net.Sockets;
using Bindings;
using GeonBit.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using PSol.Data.Models;

namespace PSol.Client
{
    public class Game1 : Game
    {
        public static GraphicsDeviceManager graphics;
        public static SpriteBatch spriteBatch;
        public static ParticleEngine particleEngine;
        public static List<SmallExplosion> Explosion = new List<SmallExplosion>();
        private Texture2D backGroundTexture;
        private Texture2D splashScreen;
        private Vector2 backgroundPos;
        private RenderTarget2D renderTarget;

        public static ClientTCP ctcp;
        private ClientData chd;
        private Camera camera;

        public static InterfaceGUI IGUI = new InterfaceGUI();
        private static readonly KeyControl KC = new KeyControl();

        private float WalkTimer;
        public new static int Tick;
        public static int ElapsedTime;
        public static int FrameTime;
        public static double i = 0.0;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            graphics.PreferredBackBufferWidth = Globals.PreferredBackBufferWidth;
            graphics.PreferredBackBufferHeight = Globals.PreferredBackBufferHeight;
        }

        public void setFullScreen()
        {
            Globals.graphicsChange = false;
            Globals.PreferredBackBufferHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
            Globals.PreferredBackBufferWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
            graphics.PreferredBackBufferWidth = Globals.PreferredBackBufferWidth;
            graphics.PreferredBackBufferHeight = Globals.PreferredBackBufferHeight;
            graphics.ApplyChanges();
            renderTarget = new RenderTarget2D(GraphicsDevice, GraphicsDevice.PresentationParameters.BackBufferWidth, GraphicsDevice.PresentationParameters.BackBufferHeight, false, GraphicsDevice.PresentationParameters.BackBufferFormat, DepthFormat.Depth24);
            camera = new Camera(GraphicsDevice.Viewport);
            Graphics.InitializeGraphics(Content);
            graphics.ApplyChanges();
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
            Globals.Font14 = Content.Load<SpriteFont>("GeonBit.UI/themes/classic/fonts/Size14");
            Globals.Font12 = Content.Load<SpriteFont>("GeonBit.UI/themes/classic/fonts/Size12");
            Globals.Font10 = Content.Load<SpriteFont>("GeonBit.UI/themes/classic/fonts/Size10");
            Globals.Font8 = Content.Load<SpriteFont>("GeonBit.UI/themes/classic/fonts/Size8");

            for (var n = 0; n < Constants.MAX_PLAYERS; n++)
            {
                Types.Player[n] = new User();
            }
            ctcp = new ClientTCP();
            chd = new ClientData();
            renderTarget = new RenderTarget2D(GraphicsDevice, GraphicsDevice.PresentationParameters.BackBufferWidth, GraphicsDevice.PresentationParameters.BackBufferHeight, false, GraphicsDevice.PresentationParameters.BackBufferFormat, DepthFormat.Depth24);
            camera = new Camera(GraphicsDevice.Viewport);
            chd.InitializeMessages();
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
            splashScreen = Content.Load<Texture2D>("Panels/Splash");
            backgroundPos = new Vector2(-Globals.PreferredBackBufferWidth / 2.0F, -Globals.PreferredBackBufferHeight / 2.0F);

            // Particle engine textures
            var engineTextures = new List<Texture2D> { Graphics.spark };
            particleEngine = new ParticleEngine(engineTextures, Vector2.Zero);
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        protected override void OnExiting(object sender, EventArgs args)
        {
            base.OnExiting(sender, args);
            ctcp.PlayerSocket.Client.Shutdown(SocketShutdown.Both);
            ctcp.PlayerSocket.Client.Close();
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
                if (Globals.graphicsChange)
                {
                    setFullScreen();
                }
                particleEngine.EmitterLocation = new Vector2(Types.Player[GameLogic.PlayerIndex].X, Types.Player[GameLogic.PlayerIndex].Y);
                particleEngine.Update(Globals.DirUp);
            }
            IGUI.lblStatus.Text = ctcp.isOnline ? "Server status:{{GREEN}} online" : "Server status:{{RED}} offline";
            CheckKeys();
            for (var x = 0; x < Explosion.Count; x++)
            {
                Explosion[x].Update();
            }
            camera.Update(gameTime, this);

            IGUI.Update();
            Globals.strobe = (gameTime.TotalGameTime.Seconds % 2 == 0);
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
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.LinearClamp, DepthStencilState.Default, RasterizerState.CullNone);
            spriteBatch.Draw(renderTarget, new Rectangle(0, 0, Globals.PreferredBackBufferWidth, Globals.PreferredBackBufferHeight), Globals.Luminosity);
            spriteBatch.End();
            base.Draw(gameTime);
        }

        /// <summary>
        /// Draws the entire scene in the given render target.
        /// </summary>
        /// <returns>A texture2D with the scene drawn in it.</returns>
        protected void DrawSceneToTexture(RenderTarget2D _renderTarget, GameTime gameTime)
        {
            // Set the render target
            GraphicsDevice.SetRenderTarget(_renderTarget);
            GraphicsDevice.DepthStencilState = new DepthStencilState { DepthBufferEnable = true };
            // Draw the scene
            GraphicsDevice.Clear(Color.Black);

            Tick = (int)gameTime.TotalGameTime.TotalMilliseconds;
            ElapsedTime = Tick - FrameTime;
            FrameTime = Tick;

            if (WalkTimer < Tick)
            {
                GameLogic.Navigate();
                GameLogic.CheckMovement();
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
            Graphics.DrawWeapons(spriteBatch);
            particleEngine.Draw(spriteBatch);
            Graphics.RenderObjects();
            Graphics.RenderPlayers();
            foreach (var explosion in Explosion)
            {
                explosion.Draw(spriteBatch);
            }
            spriteBatch.End();
            Graphics.DrawHud(Content);
            Graphics.DrawWeaponsBar(Content);
            Graphics.DrawInfo(Content);
            UserInterface.Active.DrawMainRenderTarget(spriteBatch);

            // Drop the render target
            GraphicsDevice.SetRenderTarget(null);
        }

        private void CheckKeys()
        {
            if (Globals.Control)
            {
                if (KC.KeyPress(Keys.A))
                {
                    Actions.Aerology();
                }
                if (KC.KeyPress(Keys.D))
                {
                    Globals.details = !Globals.details;
                }
                if (KC.KeyPress(Keys.G))
                {
                    MenuManager.ChangeMenu(MenuManager.Menu.Map);
                }
                if (KC.KeyPress(Keys.I))
                {
                    Globals.inventoryMode = 1;
                    IGUI.PopulateInventory();
                    MenuManager.ChangeMenu(MenuManager.Menu.Inventory);
                }
                if (KC.KeyPress(Keys.L))
                {
                    Globals.Luminosity = Globals.Luminosity == Color.White ? Color.Gray : Color.White;
                }
                if (KC.KeyPress(Keys.Q))
                {
                    MenuManager.ChangeMenu(MenuManager.Menu.Exit);
                }
                if (KC.KeyPress(Keys.M))
                {
                    Actions.Mine();
                }
                if (KC.KeyPress(Keys.R))
                {
                    Globals.scanner = !Globals.scanner;
                }
                if (KC.KeyPress(Keys.T))
                {
                    Actions.Trade();
                }
                if (KC.KeyPress(Keys.L))
                {
                    Actions.Trade();
                }
                if (KC.KeyPress(Keys.W))
                {
                    Globals.weaponsBar = !Globals.weaponsBar;
                }
            }

            Globals.Control = KC.CheckCtrl();
            Globals.Shift = KC.CheckShift();
            Globals.Alt = KC.CheckAlt();

            if (!Globals.windowOpen && !Globals.Control) // Don't allow game input when menus are open or CTRL is pressed
            {
                Globals.DirUp = Keyboard.GetState().IsKeyDown(Keys.W);
                Globals.DirDn = Keyboard.GetState().IsKeyDown(Keys.S);
                Globals.DirLt = Keyboard.GetState().IsKeyDown(Keys.A);
                Globals.DirRt = Keyboard.GetState().IsKeyDown(Keys.D);
                Globals.Details1 = Keyboard.GetState().IsKeyDown(Keys.LeftAlt);
                Globals.Details2 = Keyboard.GetState().IsKeyDown(Keys.RightAlt);

                if (Mouse.GetState().LeftButton == ButtonState.Pressed)
                {
                    if (Globals.HoveringMob || Globals.HoveringItem) return;
                    Types.Player[GameLogic.PlayerIndex].Rotation = (float)GameLogic.GetAngleFromPlayer(Mouse.GetState().Position);
                    Globals.DirUp = true;
                }

                if (Mouse.GetState().RightButton == ButtonState.Released)
                {
                    Globals.Attacking = false;
                }

                if (Keyboard.GetState().IsKeyDown(Keys.Escape)) { GameLogic.Selected = ""; GameLogic.selectedPlanet = ""; GameLogic.Navigating = false; }

                if (KC.KeyPress(Keys.Tab))
                {
                    // Cycle through local mobs
                    if (GameLogic.Selected == "" || GameLogic.LocalMobs.FindIndex(m => m.Id == GameLogic.Selected) >= GameLogic.LocalMobs.Count - 1)
                    {
                        GameLogic.SelectedType = "MOB";
                        GameLogic.selectedPlanet = "";
                        GameLogic.Selected = GameLogic.LocalMobs[0].Id;
                    }
                    else
                    {
                        GameLogic.Selected = GameLogic.LocalMobs[GameLogic.LocalMobs.FindIndex(m => m.Id == GameLogic.Selected) + 1].Id;
                        GameLogic.SelectedType = "MOB";
                        GameLogic.selectedPlanet = "";
                    }
                }
            }
            else
            {
                if (KC.KeyPress(Keys.Tab)) { IGUI.TabThrough(); }
                if (KC.KeyPress(Keys.Escape))
                {
                    if (Globals.equipAmmo) { Globals.equipAmmo = false; return; }
                    if (Globals.equipWeapon) { Globals.equipWeapon = false; return; }
                    MenuManager.Clear();
                }
            }
            if (KC.KeyPress(Keys.Enter)) { IGUI.Enter(); }
        }
    }
}
