using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using GeonBit.UI;
using Bindings;
using PSol.Data.Models;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using GeonBit.UI.Entities;

namespace PSol.Client
{
    public class Game1 : Game
    {
        public static GraphicsDeviceManager graphics;
        public static SpriteBatch spriteBatch;
        public static ParticleEngine particleEngine;
        public static SmallExplosion smallExplosion;
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

        private bool _laserCharged = true;
        private int _laserTimer;

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
            Globals.Font14 = Content.Load<SpriteFont>("GeonBit.UI/themes/classic/fonts/Size14");
            Globals.Font12 = Content.Load<SpriteFont>("GeonBit.UI/themes/classic/fonts/Size12");
            Globals.Font10 = Content.Load<SpriteFont>("GeonBit.UI/themes/classic/fonts/Size10");
            Globals.Font8 = Content.Load<SpriteFont>("GeonBit.UI/themes/classic/fonts/Size8");

            for (var i = 1; i < Constants.MAX_PLAYERS; i++)
            {
                Types.Player[i] = new User();
            }
            ctcp = new ClientTCP();
            chd = new HandleData();
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
            backgroundPos = new Vector2(-Globals.PreferredBackBufferWidth / 2.0F, -Globals.PreferredBackBufferHeight / 2.0F);

            // Particle engine textures
            List<Texture2D> engineTextures = new List<Texture2D> { Content.Load<Texture2D>("Particles/circle") };
            particleEngine = new ParticleEngine(engineTextures, new Vector2(0, 0));

            //Particle small explosion textures
            List<Texture2D> smallExplosionTextures = new List<Texture2D> { Content.Load<Texture2D>("Particles/circle") };
            smallExplosion = new SmallExplosion(smallExplosionTextures, new Vector2(0, 0));
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
                particleEngine.EmitterLocation = new Vector2(Types.Player[GameLogic.PlayerIndex].X, Types.Player[GameLogic.PlayerIndex].Y);
                particleEngine.Update(Globals.DirUp);
            }
            IGUI.lblStatus.Text = ctcp.isOnline ? "Server status:{{GREEN}} online" : "Server status:{{RED}} offline";
            CheckKeys();

            smallExplosion.Update();
            camera.Update(gameTime, this);

            IGUI.Update();

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
        protected void DrawSceneToTexture(RenderTarget2D _renderTarget, GameTime gameTime)
        {
            // Set the render target
            GraphicsDevice.SetRenderTarget(_renderTarget);
            GraphicsDevice.DepthStencilState = new DepthStencilState() { DepthBufferEnable = true };
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
            Graphics.RenderPlayers();
            smallExplosion.Draw(spriteBatch);
            spriteBatch.End();

            Graphics.DrawHud(Content);
            Graphics.DrawInfo(Content);
            UserInterface.Active.DrawMainRenderTarget(spriteBatch);

            // Drop the render target
            GraphicsDevice.SetRenderTarget(null);
        }

        private void CheckKeys()
        {
            if (KC.KeyPress(Keys.L) && Globals.Control)
            {
                Globals.Luminosity = Globals.Luminosity == Color.White ? Color.Gray : Color.White;
            }

            if (KC.KeyPress(Keys.Q) && Globals.Control)
            {
                Globals.exitgame = true;
            }

            if (KC.KeyPress(Keys.N) && Globals.Control)
            {
                Console.WriteLine(GenerateName(true));
            }

            if (KC.KeyPress(Keys.M) && Globals.Control)
            {
                MenuManager.ChangeMenu(MenuManager.Menu.Map);
            }

            if (KC.KeyPress(Keys.I) && Globals.Control)
            {
                IGUI.PopulateInventory(0);
                MenuManager.ChangeMenu(MenuManager.Menu.Inventory);
            }

            if (KC.KeyPress(Keys.D) && Globals.Control)
            {
                Globals.details = !Globals.details;
            }

            if (KC.KeyPress(Keys.R) && Globals.Control)
            {
                Globals.scanner = !Globals.scanner;
            }

            Globals.Control = KC.CheckCtrl();
            Globals.Shift = KC.CheckShift();
            Globals.Alt = KC.CheckAlt();

            // TODO: This is dumb - replace with a timer or something else when we get real weapons
            if (!_laserCharged)
            {
                _laserTimer++;
            }

            if (_laserTimer > 100)
            {
                _laserCharged = true;
                _laserTimer = 0;
            }

            if (!Globals.windowOpen && !Globals.Control) // Don't allow game input when menus are open or CTRL is pressed
            {
                if (GameLogic.Selected != "")
                {
                    var x = 0f;
                    var y = 0f;
                    if (GameLogic.SelectedType == "MOB")
                    {
                        var mob = GameLogic.LocalMobs.Find(m => m.Id == GameLogic.Selected);
                        if (mob != null)
                        {
                            x = mob.X;
                            y = mob.Y;
                            if (Mouse.GetState().RightButton == ButtonState.Pressed)
                            {
                                if (_laserCharged)
                                {
                                    _laserCharged = false;
                                    ctcp.SendCombat(mob.Id, "");
                                }
                            }
                        }
                        else
                        {
                            GameLogic.Selected = "";
                            GameLogic.SelectedType = "";
                        }
                    }
                    else if (GameLogic.SelectedType == "PLAYER")
                    {
                        if (GameLogic.Selected != Types.Player[GameLogic.PlayerIndex].Id)
                        {
                            var player = Types.Player.First(user => user?.Id == GameLogic.Selected);
                            if (player != null)
                            {
                                x = player.X;
                                y = player.Y;
                            }
                            else
                            {
                                GameLogic.Selected = "";
                                GameLogic.SelectedType = "";
                            }
                        }
                    }
                }

                Globals.DirUp = Keyboard.GetState().IsKeyDown(Keys.W);
                Globals.DirDn = Keyboard.GetState().IsKeyDown(Keys.S);
                Globals.DirLt = Keyboard.GetState().IsKeyDown(Keys.A);
                Globals.DirRt = Keyboard.GetState().IsKeyDown(Keys.D);
                Globals.Details1 = Keyboard.GetState().IsKeyDown(Keys.LeftAlt);
                Globals.Details2 = Keyboard.GetState().IsKeyDown(Keys.RightAlt);

                if (Keyboard.GetState().IsKeyDown(Keys.Escape)) { GameLogic.Selected = ""; GameLogic.selectedPlanet = ""; GameLogic.Navigating = false; }
                if (KC.KeyPress(Keys.T))
                {
                    MenuManager.ChangeMenu(MenuManager.Menu.Message);
                    InterfaceGUI.messageText.IsFocused = true;
                }

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
                if (KC.KeyPress(Keys.Enter)) { IGUI.Enter(); }
                if (KC.KeyPress(Keys.Escape)) { MenuManager.Clear(); }
            }
        }

        public string GenerateName(bool special)
        {
            string[] vowels = { "a", "e", "i", "o", "u", "a", "e", "i", "o", "u", "y", "oo", "ea" };
            string[] consonants = {
                "b", "c", "d", "f", "g", "h", "j", "k", "l", "m", "n", "p", "q", "r", "s", "t", "v", "w", "x", "z",
                "ch", "nd", "qu", "rt", "ck", "st", "rr", "sl", "pl", "'", "ph"
            };
            string[] titles =
            {
                "angry","black-hearted","brooding","brute","dangerous","deadly","deathly","death-dealer","deceitful","despairing",
                "destroyer","devouring", "egregious","enraged","evil","fatal","fiery","fighter","foul","ghostly","harmful","hateful",
                "heathen","hectic","heinous","hopeless","hazardous","ignoble","ignorant","irate","jagged","jarring","jealous",
                "killer","livid","loathing","lunatic","lurker","lying","malignant","mendacious","mephitic","nag","nefarious",
                "nightmarish","objectionable","obscene","ominous","overwhelming","paradoxical","pejorative","perturbed","punisher",
                "quarrelsome","quick","resentful","sinister","sly","tank","taunting","torturous","traitorous","traumatic",
                "unholy","ungodly","unyielding","vanquisher","vengeful","violent","warrior","wicked","wretched","zealous"
            };
            string[] prefix =
            {
                "Anger",
                "Bad",
                "Black",
                "Bleak",
                "Blood",
                "Break",
                "Dare",
                "Dead",
                "Death",
                "Devil",
                "Dire",
                "Doubt",
                "Dread",
                "Empty",
                "Evil",
                "Fear",
                "Fire",
                "Flight",
                "Frost",
                "Ghost",
                "Hate",
                "Hell",
                "Hunger",
                "Ice",
                "Ire",
                "Jagged",
                "Jarring",
                "Kill",
                "Lust",
                "Metal",
                "Moon",
                "Night",
                "Null",
                "Quick",
                "Red",
                "Shadow",
                "Slander",
                "Smoke",
                "Spark",
                "Spiked",
                "Storm",
                "Strike",
                "Thorn",
                "Thunder",
                "Vile",
                "Void",
                "Wicked",
                "Zealous"
            };
            string[] suffix =
            {
                "adder",
                "blast",
                "blood",
                "breath",
                "crush",
                "death",
                "demon",
                "devil",
                "dusk",
                "ember",
                "fade",
                "fault",
                "fear",
                "fight",
                "fire",
                "flight",
                "hate",
                "jinx",
                "lightning",
                "matrix",
                "moon",
                "night",
                "nik",
                "nova",
                "null",
                "pit",
                "poison",
                "razor",
                "rex",
                "run",
                "seeker",
                "shadow",
                "smoke",
                "smolder",
                "snare",
                "soul",
                "spark",
                "spear",
                "spike",
                "star",
                "storm",
                "strike",
                "technic",
                "thunder",
                "thorn",
                "trance",
                "titan",
                "venom",
                "void",
                "wolf"
            };

            var rnd = new Random();
            int length = rnd.Next(2, 5);

            // Given name
            string name = "";
            for (var i = 0; i < length; i++)
            {
                if (i == 0)
                {
                    if (rnd.Next(0, 1) == 1)
                    {
                        name += vowels[rnd.Next(6)].ToUpper();
                    }
                    else
                    {
                        name += consonants[rnd.Next(19)].ToUpper();
                    }
                }
                else
                {
                    name += vowels[rnd.Next(vowels.Length)];
                    name += consonants[rnd.Next(consonants.Length)];
                }
            }

            // Surname
            string p = prefix[rnd.Next(prefix.Length)];
            string s = suffix[rnd.Next(suffix.Length)];

            while (string.Equals(s, p, StringComparison.CurrentCultureIgnoreCase))
            {
                s = suffix[rnd.Next(suffix.Length)];
            }

            name += " " + p + s;

            // Title
            if (special)
            {
                name += " the ";
                name += titles[rnd.Next(titles.Length)];
            }

            return name;
        }
    }
}
