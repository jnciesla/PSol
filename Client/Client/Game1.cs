using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using GeonBit.UI;
using Microsoft.Xna.Framework.Content;
using System;
using System.Diagnostics.CodeAnalysis;

namespace Client
{
    public class Game1 : Game
    {
        public static GraphicsDeviceManager graphics;
        public static SpriteBatch spriteBatch;
        private Texture2D backGroundTexture;
        private Vector2 backgroundPos;

        private ClientTCP ctcp;
        private HandleData chd;
        private Camera camera;

        private readonly InterfaceGUI IGUI = new InterfaceGUI();
        private static readonly KeyControl KC = new KeyControl();

        private float WalkTimer;
        public new static int Tick;
        public static int ElapsedTime;
        public static int FrameTime;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
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
            // TODO: Add your initialization logic here
            ctcp = new ClientTCP();
            chd = new HandleData();
            camera = new Camera(GraphicsDevice.Viewport);
            chd.InitializeMesssages();
            ctcp.ConnectToServer();
            Graphics.InitializeGraphics(Content);
            UserInterface.Initialize(Content, BuiltinThemes.editor);
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
            GraphicsDevice.Clear(Color.Black);

            Tick = (int)gameTime.TotalGameTime.TotalMilliseconds;
            ElapsedTime = Tick - FrameTime;
            FrameTime = Tick;

            if (WalkTimer < Tick)
            {
                GameLogic.CheckMovement();
                camera.ZoomController();
                WalkTimer = Tick + 15;
            }
            CheckKeys();

            // TODO: Add your drawing code here

            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.LinearWrap, null, null, null, camera.transform);
            spriteBatch.Draw(backGroundTexture, backgroundPos, Globals.mapSize, Color.White);
            Graphics.DrawBorder(Globals.playArea, 1, Color.DarkOliveGreen);
            Graphics.RenderGraphics();
            spriteBatch.End();
            Graphics.DrawHud(Content);
            UserInterface.Active.Draw(spriteBatch);

            base.Draw(gameTime);
        }

        private void CheckKeys()
        {
            if (!Globals.windowOpen) // Don't allow game input when menus are open
            {
                Globals.DirUp = Keyboard.GetState().IsKeyDown(Keys.W);
                Globals.DirDn = Keyboard.GetState().IsKeyDown(Keys.S);
                Globals.DirLt = Keyboard.GetState().IsKeyDown(Keys.A);
                Globals.DirRt = Keyboard.GetState().IsKeyDown(Keys.D);
                Globals.ZoomIn = Keyboard.GetState().IsKeyDown(Keys.Q);
                Globals.ZoomOut = Keyboard.GetState().IsKeyDown(Keys.E);
                Globals.ZoomDefault = Keyboard.GetState().IsKeyDown(Keys.R);
                if (KC.KeyPress(Keys.T)) { MenuManager.ChangeMenu(MenuManager.Menu.Message);
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
