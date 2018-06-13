using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using GeonBit.UI;
using System.Diagnostics.CodeAnalysis;
using Bindings;
using PSol.Data.Models;

namespace PSol.Client
{
    public class Game1 : Game
    {
        public static GraphicsDeviceManager graphics;
        public static SpriteBatch spriteBatch;
        private Texture2D backGroundTexture;
        private Vector2 backgroundPos;
        RenderTarget2D renderTarget;

        private ClientTCP ctcp;
        private HandleData chd;
        Camera camera;

        private readonly InterfaceGUI IGUI = new InterfaceGUI();
        private static readonly KeyControl KC = new KeyControl();

        private float WalkTimer;
        public new static int Tick;
        public static int ElapsedTime;
        public static int FrameTime;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            // graphics.IsFullScreen = true;
            Content.RootDirectory = "Content";

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
            UserInterface.Initialize(Content, BuiltinThemes.editor);
            UserInterface.Active.UseRenderTarget = true;
            IGUI.InitializeGUI();
            MenuManager.ChangeMenu(MenuManager.Menu.Login);

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        [SuppressMessage("ReSharper", "PossibleLossOfFraction")]
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here
            backGroundTexture = Content.Load<Texture2D>("stars3");
            backgroundPos = new Vector2(-Globals.PreferredBackBufferWidth / 2, -Globals.PreferredBackBufferHeight / 2);

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
            if (Globals.exitgame) this.Exit();
            UserInterface.Active.Update(gameTime);
            // TODO: Add your update logic here
            if (ctcp.isOnline)
            {
                IGUI.lblStatus.Text = "Server status:{{GREEN}} online";
            }
            else
            {
                IGUI.lblStatus.Text = "Server status:{{RED}} offline";
            }

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

            spriteBatch.Draw(renderTarget, new Rectangle(0, 0, 1024, 768), Globals.Luminosity);

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
            CheckKeys();


            // TODO: Add your drawing code here
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.LinearWrap, null, null, null, Camera.transform);
            spriteBatch.Draw(backGroundTexture, backgroundPos, Globals.mapSize, Color.White);
            Graphics.DrawBorder(Globals.playArea, 1, Color.DarkOliveGreen);
            Graphics.RenderGraphics();
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
                Globals.DirUp = Keyboard.GetState().IsKeyDown(Keys.W);
                Globals.DirDn = Keyboard.GetState().IsKeyDown(Keys.S);
                Globals.DirLt = Keyboard.GetState().IsKeyDown(Keys.A);
                Globals.DirRt = Keyboard.GetState().IsKeyDown(Keys.D);
                Globals.ZoomIn = Keyboard.GetState().IsKeyDown(Keys.E);
                Globals.ZoomOut = Keyboard.GetState().IsKeyDown(Keys.Q);
                Globals.ZoomDefault = Keyboard.GetState().IsKeyDown(Keys.R);
                Globals.Details1 = Keyboard.GetState().IsKeyDown(Keys.LeftAlt);
                Globals.Details2 = Keyboard.GetState().IsKeyDown(Keys.RightAlt);
                if (Keyboard.GetState().IsKeyDown(Keys.Escape)) { Globals.Selected = -1; }
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
