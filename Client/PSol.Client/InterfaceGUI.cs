using System;
using System.Collections.Generic;
using System.Linq;
using Bindings;
using GeonBit.UI;
using GeonBit.UI.Entities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using PSol.Data.Models;
using Validators;

namespace PSol.Client
{
    public class InterfaceGUI
    {
        public static List<Panel> Windows = new List<Panel>();
        private readonly ClientTCP ctcp = new ClientTCP();
        public MulticolorParagraph lblStatus;
        public TextInput txtUser;
        public TextInput txtPass;
        public TextInput txtUserReg;
        public TextInput txtPassReg;
        public TextInput txtPas2Reg;
        public static TextInput messageText;

        // Map stuff
        public static Image mapPlayer;
        public static Panel galaxyMap;
        public static Paragraph starLabel;
        public static Image starDetail;
        private static Paragraph[] mapLine;
        private static Paragraph canxLabel;

        // Inventory stuff
        public bool pInv;
        private static Panel invPanel;
        private static Texture2D equipImage;
        private static Panel hoverPanel;
        private static Paragraph detailsHeader;
        private static Paragraph detailsSubHeader;
        private static Paragraph detailsBody;
        private static Image detailsImage;
        private static Image equipButton;
        private static Paragraph equipLabel;
        private static Image jettButton;
        private static Paragraph jettLabel;
        private static Paragraph qtyStr;
        private static Paragraph costStr;
        private static Image decBtn;
        private static Image incBtn;
        private static Image invImage;
        private static Panel detailsPanel;
        private static Image invCloseBtn;
        private static Paragraph qtyLbl;
        private static readonly int[] qty = new int[60];
        private static ColoredRectangle selectangle;
        private static ColoredRectangle weapangle1;
        private static ColoredRectangle weapangle2;
        private static ColoredRectangle weapangle3;
        private static ColoredRectangle weapangle4;
        private static ColoredRectangle weapangle5;
        private static ColoredRectangle amtangle1;
        private static ColoredRectangle amtangle2;
        private static ColoredRectangle amtangle3;
        private static readonly Image[] slot = new Image[60];
        private static readonly Paragraph[] slotQty = new Paragraph[60];
        private static readonly Rectangle[] slotBounds = new Rectangle[60];

        // Loot
        readonly Image[] lootItem = new Image[9];
        private static Paragraph detailsHeaderLoot;
        private static Paragraph detailsSubHeaderLoot;
        private static Paragraph detailsBodyLoot;
        private static Image destroyBtn;

        // IGUI textures
        private static Texture2D closeIcon;
        private static Texture2D plusIcon;
        private static Texture2D minusIcon;
        private Texture2D mapPanel;
        private Texture2D inventoryPanel;
        private Texture2D registerPanel;
        private Texture2D lootPanel;
        private Texture2D loginPanel;
        private Texture2D exitPanel;
        private Texture2D button;
        private Texture2D button_hover;
        private Texture2D button_down;
        private static Texture2D tab;
        private const string mask = "*";
        private Item selectedItem = new Item();
        private int selectedSlot = -1;

        public void InitializeGUI(ContentManager content)
        {
            // Initialize custom graphics
            loginPanel = content.Load<Texture2D>("Panels/Login");
            registerPanel = content.Load<Texture2D>("Panels/Register");
            exitPanel = content.Load<Texture2D>("Panels/exit");
            inventoryPanel = content.Load<Texture2D>("Panels/Inventory");
            lootPanel = content.Load<Texture2D>("Panels/Loot");
            button = content.Load<Texture2D>("Panels/button_default");
            button_hover = content.Load<Texture2D>("Panels/button_default_hover");
            button_down = content.Load<Texture2D>("Panels/button_default_down");
            mapPanel = content.Load<Texture2D>("Panels/Map");
            closeIcon = content.Load<Texture2D>("Panels/closeIco");
            plusIcon = content.Load<Texture2D>("Panels/plusIco");
            minusIcon = content.Load<Texture2D>("Panels/minusIco");
            equipImage = content.Load<Texture2D>("Panels/Equipment");
            tab = content.Load<Texture2D>("Panels/Tab");

            mapLine = new Paragraph[10];

            CreateChats();
            CreateWindow_Login();
            CreateWindow_Register();
            CreateMessage();
            CreateMap();
            CreateInventory();
            CreateWindow_Exit();
            CreateLoot();
        }

        public void CreateWindow(Panel panel)
        {
            Windows.Add(panel);
        }

        public void TabThrough()
        {
            if (Windows[1].Visible) // Tab through login window when visible
            {
                if (txtUser.IsFocused)
                {
                    txtUser.IsFocused = false;
                    txtPass.IsFocused = true;
                }
                else
                {
                    txtUser.IsFocused = true;
                    txtPass.IsFocused = false;
                }
            }

            if (!Windows[2].Visible) return;
            if (txtUserReg.IsFocused)
            {
                txtUserReg.IsFocused = false;
                txtPassReg.IsFocused = true;
            }
            else if (txtPassReg.IsFocused)
            {
                txtPas2Reg.IsFocused = true;
                txtPassReg.IsFocused = false;
            }
            else
            {
                txtPas2Reg.IsFocused = false;
                txtUserReg.IsFocused = true;
            }

        }

        public void Enter()
        {
            if (Windows[1].Visible) // Enter on login window when visible
            {
                Login();
            }
            if (Windows[2].Visible) // Enter on register window when visible
            {
                Register();
            }
            if (Windows[3].Visible) // Ingame console
            {
                if (messageText.Value != "" && messageText.Value != messageText.ValueWhenEmpty)
                {
                    try
                    {
                        if (messageText.Value.ToLower() == "/exit" || messageText.Value.ToLower() == "/quit")
                        {
                            Globals.exitgame = true;
                        }
                        else if (messageText.Value.ToLower().Substring(0, 5) == "/scan")
                        {
                            Globals.scanner = !Globals.scanner;
                        }
                        else if (messageText.Value.ToLower().Substring(0, 4) == "/det")
                        {
                            Globals.scanner = !Globals.scanner;
                        }
                        else if (messageText.Value.ToLower() == "/look")
                        {
                            AddChats(GameLogic.LocalMobs.Count + " hostiles detected in the immediate vicinity.",
                                Color.BurlyWood);
                        }
                        else if (messageText.Value.ToLower().Substring(0, 4) == "/nav")
                        {
                            int X = 0, Y = 0;
                            bool resultX = false, resultY = false;
                            var temp = messageText.Value.Substring(messageText.Value.LastIndexOf(' ') + 1);
                            if (temp.Contains(","))
                            {
                                resultX = int.TryParse(temp.Substring(0, temp.IndexOf(',')), out X);
                                resultY = int.TryParse(temp.Substring(temp.IndexOf(',') + 1), out Y);
                            }
                            if (temp.Contains(":"))
                            {
                                resultX = int.TryParse(temp.Substring(0, temp.IndexOf(':')), out X);
                                resultY = int.TryParse(temp.Substring(temp.IndexOf(':') + 1), out Y);
                            }

                            if (!resultX || !resultY)
                            {
                                AddChats(@"Invalid input to navigation computer", Color.DarkGoldenrod);
                                return;
                            }

                            AddChats(@"Navigating to " + X + "," + Y, Color.BurlyWood);
                            GameLogic.Destination = new Vector2(X, Y);
                            GameLogic.navAngle = -1;
                            GameLogic.Navigating = true;
                        }
                        else
                        {
                            ctcp.SendChat(messageText.Value);
                        }
                    }
                    catch (ArgumentOutOfRangeException)
                    {
                        // Ignoring substr issues
                    }
                    messageText.Value = "";
                    MenuManager.Clear(3);
                    return;
                }
                MenuManager.Clear(3);
                return;
            }
            if (Globals.windowOpen) return;
            MenuManager.ChangeMenu(MenuManager.Menu.Message);
            messageText.IsFocused = true;
        }

        private void Login()
        {
            if (Globals.loginUsername == string.Empty || Globals.loginPassword == string.Empty)
            {
                AddChats("You must enter a username and password to login.", Color.DarkRed);
            }
            else
            {
                ctcp.SendLogin();

            }
        }

        private void Register()
        {
            if (Globals.registerUsername == string.Empty || Globals.registerPassword == string.Empty || Globals.registerValidate == string.Empty)
            {
                AddChats("You must enter a valid username and password, and confirm your password, to register.", Color.DarkRed);
            }
            else
            {
                if (Globals.registerPassword != Globals.registerValidate)
                {
                    AddChats("Passwords do not match- please try again.", Color.DarkRed);
                }
                else
                {
                    Globals.loginUsername = Globals.registerUsername;
                    Globals.loginPassword = Globals.registerPassword;
                    ctcp.SendRegister();
                }
            }
        }

        public void CreateWindow_Login()
        {
            //  Create Entities
            var splash = new Image(Graphics.splash, new Vector2(1024, 768), ImageDrawMode.Stretch, Anchor.TopLeft, new Vector2(-290, -200));
            var panel = new Panel(new Vector2(500, 430), PanelSkin.None);
            var image = new Image(loginPanel, new Vector2(496, 427), ImageDrawMode.Panel, Anchor.TopLeft, new Vector2(-30, -29)) { ShadowColor = Color.Black * .5F, ShadowOffset = new Vector2(10, 10) };
            var loginButton = new Image(button, new Vector2(400, 60), ImageDrawMode.Panel, Anchor.AutoCenter);
            var loginLabel = new Paragraph("LOGIN", Anchor.AutoCenter, null, new Vector2(0, -50)) { FillColor = Color.DarkGray };
            txtUser = new TextInput(false, Anchor.TopCenter, new Vector2(0, 40)) { Skin = PanelSkin.None };
            txtUser.Validators.Add(new AlphaNumValidator());
            txtPass = new TextInput(false, Anchor.Auto, new Vector2(0, 38))
            {
                HideInputWithChar = mask.ToCharArray()[0],
                Skin = PanelSkin.None
            };
            txtPass.Validators.Add(new AlphaNumValidator());
            var lblRegister = new Label("No account?  Register here", Anchor.AutoCenter, null, new Vector2(0, 25));

            lblStatus = new MulticolorParagraph("Server status:{{RED}} offline", Anchor.BottomLeft);
            UserInterface.Active.AddEntity(panel);

            // Entity Settings
            txtUser.PlaceholderText = "Enter username";
            txtPass.PlaceholderText = "Enter password";

            // Add Entities
            panel.AddChild(splash);
            Windows[0].BringToFront();
            panel.AddChild(image);
            panel.AddChild(txtUser);
            panel.AddChild(txtPass);
            panel.AddChild(loginButton);
            panel.AddChild(loginLabel);
            panel.AddChild(lblRegister);
            panel.AddChild(lblStatus);

            // MouseEvents
            lblRegister.OnMouseEnter += entity => { lblRegister.FillColor = Color.Red; UserInterface.Active.SetCursor(Graphics.Cursors[2], 32, new Point(-4, 0)); };
            lblRegister.OnMouseLeave += entity => { lblRegister.FillColor = Color.White; UserInterface.Active.SetCursor(CursorType.Default); };

            txtUser.OnMouseEnter += entity => { UserInterface.Active.SetCursor(CursorType.IBeam); };
            txtUser.OnMouseLeave += entity => { UserInterface.Active.SetCursor(CursorType.Default); };

            txtPass.OnMouseEnter += entity => { UserInterface.Active.SetCursor(CursorType.IBeam); };
            txtPass.OnMouseLeave += entity => { UserInterface.Active.SetCursor(CursorType.Default); };

            loginButton.OnMouseEnter += entity => { loginButton.Texture = button_hover; UserInterface.Active.SetCursor(Graphics.Cursors[2], 32, new Point(-4, 0)); };
            loginButton.OnMouseLeave += entity => { loginButton.Texture = button; UserInterface.Active.SetCursor(CursorType.Default); };
            loginButton.OnMouseDown += entity => { loginButton.Texture = button_down; };
            loginButton.OnMouseReleased += entity => { loginButton.Texture = button; };

            loginLabel.ClickThrough = true;

            lblRegister.OnClick += entity =>
            {
                MenuManager.ChangeMenu(MenuManager.Menu.Register);
            };

            loginButton.OnClick += entity =>
            {
                Login();
            };

            txtUser.OnValueChange = textUser => { Globals.loginUsername = txtUser.Value; };
            txtPass.OnValueChange = textPass => { Globals.loginPassword = txtPass.Value; };

            // Create Window
            CreateWindow(panel);
        }

        public void CreateWindow_Exit()
        {
            //  Create Entities
            var panel = new Panel(new Vector2(500, 198), PanelSkin.None);
            var image = new Image(exitPanel, new Vector2(498, 198), ImageDrawMode.Stretch, Anchor.TopLeft, new Vector2(-30, -29));
            var exitButton = new Image(button, new Vector2(300, 60), ImageDrawMode.Stretch, Anchor.AutoCenter, new Vector2(0, -100));
            var exitLabel = new Paragraph("EXIT PSol", Anchor.AutoCenter, null, new Vector2(0, -50)) { FillColor = Color.DarkGray, ClickThrough = true };
            var closeBtn = new Image(closeIcon, new Vector2(15, 15), ImageDrawMode.Stretch, Anchor.TopRight, new Vector2(-30, -29));

            UserInterface.Active.AddEntity(panel);
            panel.AddChild(image);
            panel.AddChild(exitButton);
            panel.AddChild(exitLabel);
            panel.AddChild(closeBtn);

            closeBtn.OnMouseEnter += closeEnter => { UserInterface.Active.SetCursor(Graphics.Cursors[2], 32, new Point(-4, 0)); };
            closeBtn.OnMouseLeave += closeLeave => { UserInterface.Active.SetCursor(CursorType.Default); };
            closeBtn.OnClick += closeClick => { MenuManager.Clear(6); };

            exitButton.OnMouseEnter += entity => { exitButton.Texture = button_hover; UserInterface.Active.SetCursor(Graphics.Cursors[2], 32, new Point(-4, 0)); };
            exitButton.OnMouseLeave += entity => { exitButton.Texture = button; UserInterface.Active.SetCursor(CursorType.Default); };
            exitButton.OnMouseDown += entity => { exitButton.Texture = button_down; };
            exitButton.OnMouseReleased += entity => { exitButton.Texture = button; };
            exitButton.OnClick += entity =>
            {
                Globals.exitgame = true;
            };

            CreateWindow(panel);
        }

        public void CreateWindow_Register()
        {
            //  Create Entities
            var splash = new Image(Graphics.splash, new Vector2(1024, 768), ImageDrawMode.Stretch, Anchor.TopLeft, new Vector2(-290, -200));
            var panel = new Panel(new Vector2(500, 430));
            var image = new Image(registerPanel, new Vector2(496, 427), ImageDrawMode.Panel, Anchor.TopLeft, new Vector2(-30, -29)) { ShadowColor = Color.Black * .5F, ShadowOffset = new Vector2(10, 10) };
            var backButton = new Image(button, new Vector2(210, 60), ImageDrawMode.Panel, Anchor.AutoInline);
            var registerButton = new Image(button, new Vector2(210, 60), ImageDrawMode.Panel, Anchor.AutoInline, new Vector2(20, 0));
            var backLabel = new Paragraph("BACK", Anchor.AutoInline, null, new Vector2(80, -36)) { FillColor = Color.DarkGray };
            var registerLabel = new Paragraph("REGISTER", Anchor.AutoInline, null, new Vector2(290, -50)) { FillColor = Color.DarkGray };
            txtUserReg = new TextInput(false, Anchor.TopCenter, new Vector2(0, 22)) { Skin = PanelSkin.None };
            txtUserReg.Validators.Add(new AlphaNumValidator());
            txtPassReg = new TextInput(false, Anchor.AutoCenter, new Vector2(0, 40)) { Skin = PanelSkin.None };
            txtPassReg.Validators.Add(new AlphaNumValidator());
            txtPassReg.HideInputWithChar = mask.ToCharArray()[0];
            txtPas2Reg = new TextInput(false, Anchor.AutoCenter, new Vector2(0, 36)) { Skin = PanelSkin.None };
            txtPas2Reg.Validators.Add(new AlphaNumValidator());
            txtPas2Reg.HideInputWithChar = mask.ToCharArray()[0];
            UserInterface.Active.AddEntity(panel);

            panel.Skin = PanelSkin.None;

            // Entity Settings
            txtUserReg.PlaceholderText = "Enter username";
            txtPassReg.PlaceholderText = "Enter password";
            txtPas2Reg.PlaceholderText = "Confirm password";

            // Add Entities
            panel.AddChild(splash);
            Windows[0].BringToFront();
            panel.AddChild(image);
            panel.AddChild(txtUserReg);
            panel.AddChild(txtPassReg);
            panel.AddChild(txtPas2Reg);
            panel.AddChild(backButton);
            panel.AddChild(registerButton);
            panel.AddChild(registerLabel);
            panel.AddChild(backLabel);

            // MouseEvents
            txtUserReg.OnMouseEnter += entity => { UserInterface.Active.SetCursor(CursorType.IBeam); };
            txtUserReg.OnMouseLeave += entity => { UserInterface.Active.SetCursor(CursorType.Default); };

            txtPassReg.OnMouseEnter += entity => { UserInterface.Active.SetCursor(CursorType.IBeam); };
            txtPassReg.OnMouseLeave += entity => { UserInterface.Active.SetCursor(CursorType.Default); };

            txtPas2Reg.OnMouseEnter += entity => { UserInterface.Active.SetCursor(CursorType.IBeam); };
            txtPas2Reg.OnMouseLeave += entity => { UserInterface.Active.SetCursor(CursorType.Default); };

            registerButton.OnMouseEnter += entity => { registerButton.Texture = button_hover; UserInterface.Active.SetCursor(Graphics.Cursors[2], 32, new Point(-4, 0)); };
            registerButton.OnMouseLeave += entity => { registerButton.Texture = button; UserInterface.Active.SetCursor(CursorType.Default); };
            registerButton.OnMouseDown += entity => { registerButton.Texture = button_down; };
            registerButton.OnMouseReleased += entity => { registerButton.Texture = button; };

            backButton.OnMouseEnter += entity => { backButton.Texture = button_hover; UserInterface.Active.SetCursor(Graphics.Cursors[2], 32, new Point(-4, 0)); };
            backButton.OnMouseLeave += entity => { backButton.Texture = button; UserInterface.Active.SetCursor(CursorType.Default); };
            backButton.OnMouseDown += entity => { backButton.Texture = button_down; };
            backButton.OnMouseReleased += entity => { backButton.Texture = button; };

            backButton.OnClick += entity =>
            {
                MenuManager.ChangeMenu(MenuManager.Menu.Login);
            };
            backLabel.ClickThrough = true;
            registerButton.OnClick += entity =>
            {
                Register();
            };
            registerLabel.ClickThrough = true;

            txtUserReg.OnValueChange = textUserReg => { Globals.registerUsername = txtUserReg.Value; };
            txtPassReg.OnValueChange = textPassReg => { Globals.registerPassword = txtPassReg.Value; };
            txtPas2Reg.OnValueChange = textPas2Reg => { Globals.registerValidate = txtPas2Reg.Value; };

            // Create Window
            CreateWindow(panel);
        }

        public void CreateChats()
        {
            //  Create Entities
            var panel = new Panel(new Vector2(Globals.PreferredBackBufferWidth * .5f - 50, 250), PanelSkin.None, Anchor.TopLeft) { Padding = Vector2.Zero };
            UserInterface.Active.AddEntity(panel);

            panel.PanelOverflowBehavior = PanelOverflowBehavior.VerticalScroll;
            panel.OnMouseEnter = chatOver =>
            {
                panel.Scrollbar.Opacity = 175;
                Globals.pauseChat = true;
            };
            panel.OnMouseLeave = chatOver =>
            {
                panel.Scrollbar.Opacity = 0;
                Globals.pauseChat = false;
            };
            panel.Scrollbar.Opacity = 0;

            Globals.pauseChat = false;
            Globals.chatPanel = panel;
            Globals.chatPanel.Scrollbar.AdjustMaxAutomatically = true;
            CreateWindow(Globals.chatPanel);
        }

        public static void AddChats(string message, Color color)
        {
            var para = new MulticolorParagraph(message, Anchor.Auto, color)
            {
                FontOverride = Globals.Font10,
                BackgroundColor = Color.Black * .75F,
                BackgroundColorPadding = new Point(0, 4)
            };
            Globals.chatPanel.AddChild(para);
            if (!Globals.pauseChat)
            {
                Globals.chatPanel.Scrollbar.Value = (int)Globals.chatPanel.Scrollbar.Max;
            }
        }

        public void CreateMessage()
        {
            var panel = new Panel(new Vector2(1024, 40), PanelSkin.None, Anchor.BottomLeft, new Vector2(-100, 40));
            messageText = new TextInput(false)
            {
                Skin = PanelSkin.None,
                TextParagraph = { FontOverride = Globals.Font10, BackgroundColor = Color.DarkOliveGreen * .4F, BackgroundColorUseBoxSize = true }
            };
            UserInterface.Active.AddEntity(panel);
            panel.AddChild(messageText);
            CreateWindow(panel);
        }

        public void CreateMap()
        {
            //  Create Entities
            galaxyMap = new Panel(new Vector2(800, 500));
            var image = new Image(mapPanel, new Vector2(800, 500), ImageDrawMode.Panel, Anchor.TopLeft, new Vector2(-30, -29));
            starLabel = new Paragraph("", Anchor.TopLeft, new Vector2(500, 20), new Vector2(-30, -10)) { FillColor = Color.DarkGray, FontOverride = Globals.Font10, AlignToCenter = true };
            var navButton = new Image(button, new Vector2(125, 40), ImageDrawMode.Stretch, Anchor.BottomRight, new Vector2(-10, 0));
            var navLabel = new Paragraph("Navigate", Anchor.BottomRight, null, new Vector2(0, 5)) { FillColor = Color.DarkGray };
            var canxButton = new Image(button, new Vector2(125, 40), ImageDrawMode.Stretch, Anchor.BottomRight, new Vector2(140, 0));
            canxLabel = new Paragraph("Cancel", Anchor.BottomRight, null, new Vector2(165, 5)) { FillColor = Color.DarkGray };
            mapLine[0] = new Paragraph("Name: ", Anchor.BottomLeft, null, new Vector2(475, 200)) { FillColor = Color.DarkOliveGreen, FontOverride = Globals.Font10 };
            mapLine[1] = new Paragraph("Classification: ", Anchor.BottomLeft, null, new Vector2(475, 185)) { FillColor = Color.DarkOliveGreen, FontOverride = Globals.Font10 };
            mapLine[2] = new Paragraph("Coordinates: ", Anchor.BottomLeft, null, new Vector2(475, 170)) { FillColor = Color.DarkOliveGreen, FontOverride = Globals.Font10 };
            mapLine[3] = new Paragraph("Belligerence: ", Anchor.BottomLeft, null, new Vector2(475, 155)) { FillColor = Color.DarkOliveGreen, FontOverride = Globals.Font10 };
            mapLine[4] = new Paragraph("Planets: ", Anchor.BottomLeft, null, new Vector2(475, 140)) { FillColor = Color.DarkOliveGreen, FontOverride = Globals.Font10 };
            mapLine[5] = new Paragraph("", Anchor.BottomLeft, null, new Vector2(500, 125)) { FillColor = Color.DarkOliveGreen, FontOverride = Globals.Font10 };
            mapLine[6] = new Paragraph("", Anchor.BottomLeft, null, new Vector2(500, 110)) { FillColor = Color.DarkOliveGreen, FontOverride = Globals.Font10 };
            mapLine[7] = new Paragraph("", Anchor.BottomLeft, null, new Vector2(500, 95)) { FillColor = Color.DarkOliveGreen, FontOverride = Globals.Font10 };
            mapLine[8] = new Paragraph("", Anchor.BottomLeft, null, new Vector2(500, 80)) { FillColor = Color.DarkOliveGreen, FontOverride = Globals.Font10 };
            mapLine[9] = new Paragraph("", Anchor.BottomLeft, null, new Vector2(500, 65)) { FillColor = Color.DarkOliveGreen, FontOverride = Globals.Font10 };
            galaxyMap.Draggable = true;
            galaxyMap.Skin = PanelSkin.None;

            // Add Entities
            galaxyMap.AddChild(image);
            galaxyMap.AddChild(starLabel);
            galaxyMap.AddChild(navButton);
            galaxyMap.AddChild(navLabel);
            galaxyMap.AddChild(canxButton);
            galaxyMap.AddChild(canxLabel);
            for (int i = 0; i < 10; i++) { galaxyMap.AddChild(mapLine[i]); }
            UserInterface.Active.AddEntity(galaxyMap);

            // Actions
            navButton.OnMouseEnter += entity => { navButton.Texture = button_hover; UserInterface.Active.SetCursor(Graphics.Cursors[2], 32, new Point(-4, 0)); };
            navButton.OnMouseLeave += entity => { navButton.Texture = button; UserInterface.Active.SetCursor(CursorType.Default); };
            navButton.OnMouseDown += entity => { navButton.Texture = button_down; };
            navButton.OnMouseReleased += entity => { navButton.Texture = button; };
            navLabel.ClickThrough = true;
            navButton.OnClick += entity =>
            {
                if (GameLogic.selectedMapItem == -1) { return; }
                MenuManager.Clear(4);
                AddChats(@"Navigating to " + GameLogic.Galaxy[GameLogic.selectedMapItem].Name, Color.BurlyWood);
                GameLogic.Destination = new Vector2(GameLogic.Galaxy[GameLogic.selectedMapItem].X,
                    GameLogic.Galaxy[GameLogic.selectedMapItem].Y);
                GameLogic.navAngle = -1;
                GameLogic.Navigating = true;
            };

            canxButton.OnMouseEnter += entity => { canxButton.Texture = button_hover; UserInterface.Active.SetCursor(Graphics.Cursors[2], 32, new Point(-4, 0)); };
            canxButton.OnMouseLeave += entity => { canxButton.Texture = button; UserInterface.Active.SetCursor(CursorType.Default); };
            canxButton.OnMouseDown += entity => { canxButton.Texture = button_down; };
            canxButton.OnMouseReleased += entity => { canxButton.Texture = button; };
            canxLabel.ClickThrough = true;
            canxButton.OnClick += entity =>
            {
                if (GameLogic.selectedMapItem != -1)
                {
                    starDetail.Texture = Graphics.Planets[0];
                    GameLogic.selectedMapItem = -1;
                    mapLine[0].Text = "Name: ";
                    mapLine[1].Text = "Classification: ";
                    mapLine[2].Text = "Coordinates: ";
                    mapLine[3].Text = "Belligerence: ";
                    mapLine[4].Text = "Planets: ";
                    for (int i = 5; i < 10; i++)
                    {
                        mapLine[i].Text = "";
                        mapLine[i].OnMouseEnter += pEntity => { UserInterface.Active.SetCursor(CursorType.Default); };
                    }
                }
                else
                {
                    MenuManager.Clear(4);
                }
            };

            mapLine[5].OnMouseLeave += pEntity => { UserInterface.Active.SetCursor(CursorType.Default); };
            mapLine[6].OnMouseLeave += pEntity => { UserInterface.Active.SetCursor(CursorType.Default); };
            mapLine[7].OnMouseLeave += pEntity => { UserInterface.Active.SetCursor(CursorType.Default); };
            mapLine[8].OnMouseLeave += pEntity => { UserInterface.Active.SetCursor(CursorType.Default); };
            mapLine[9].OnMouseLeave += pEntity => { UserInterface.Active.SetCursor(CursorType.Default); };

            CreateWindow(galaxyMap);
        }

        public void CreateInventory()
        {
            invPanel = new Panel(new Vector2(600, 400), PanelSkin.None, Anchor.AutoCenter) { Draggable = true, Padding = Vector2.Zero };
            invImage = new Image(inventoryPanel, new Vector2(600, 400), ImageDrawMode.Panel, Anchor.TopLeft);
            detailsPanel = new Panel(new Vector2(219, 218), PanelSkin.None, Anchor.TopLeft, new Vector2(10, 125))
            { Padding = Vector2.Zero, PanelOverflowBehavior = PanelOverflowBehavior.VerticalScroll };
            hoverPanel = new Panel(new Vector2(300, 100), PanelSkin.Default, Anchor.TopLeft) { Opacity = 240, Visible = false };
            detailsImage = new Image(Graphics.Objects[0], new Vector2(64, 64), ImageDrawMode.Stretch, Anchor.TopLeft, new Vector2(69, 5)) { Visible = false };
            detailsHeader = new Paragraph("", Anchor.TopLeft, Color.DarkGray, null, new Vector2(200, 10), new Vector2(0, 69))
            { FontOverride = Globals.Font12, OutlineOpacity = 0, AlignToCenter = true, WrapWords = false };
            detailsSubHeader = new Paragraph("", Anchor.TopLeft, Color.DarkGray * .6F, null, new Vector2(200, 10), new Vector2(0, 86))
            { FontOverride = Globals.Font8, OutlineOpacity = 0, AlignToCenter = true };
            detailsBody = new MulticolorParagraph("", Anchor.TopLeft, Color.DarkGray, null, new Vector2(200, 10), new Vector2(0, 110))
            { FontOverride = Globals.Font10, OutlineOpacity = 0, AlignToCenter = true, WrapWords = true };
            invCloseBtn = new Image(closeIcon, new Vector2(15, 15), ImageDrawMode.Stretch, Anchor.TopRight);
            equipButton = new Image(button, new Vector2(90, 30), ImageDrawMode.Stretch, Anchor.BottomLeft, new Vector2(10, 12)) { Enabled = false, FillColor = Color.DarkGray };
            equipLabel = new Paragraph("Install", Anchor.BottomLeft, null, new Vector2(20, 16)) { FillColor = Color.DarkGray, ClickThrough = true, FontOverride = Globals.Font12 };
            jettButton = new Image(button, new Vector2(90, 30), ImageDrawMode.Stretch, Anchor.BottomLeft, new Vector2(120, 12)) { Enabled = false, FillColor = Color.DarkGray };
            jettLabel = new Paragraph("Jettison", Anchor.BottomLeft, null, new Vector2(125, 16)) { FillColor = Color.DarkGray, ClickThrough = true, FontOverride = Globals.Font12 };
            qtyLbl = new Paragraph("Qty:", Anchor.BottomRight, Color.WhiteSmoke, null, null, new Vector2(340, 15)) { FontOverride = Globals.Font8 };
            qtyStr = new Paragraph("", Anchor.BottomRight, Color.WhiteSmoke, null, new Vector2(18, 8), new Vector2(300, 14)) { FontOverride = Globals.Font8, AlignToCenter = true };
            decBtn = new Image(minusIcon, new Vector2(15, 15), ImageDrawMode.Stretch, Anchor.BottomRight, new Vector2(320, 14));
            incBtn = new Image(plusIcon, new Vector2(15, 15), ImageDrawMode.Stretch, Anchor.BottomRight, new Vector2(280, 14));
            costStr = new Paragraph("", Anchor.BottomRight, Color.WhiteSmoke, null, null, new Vector2(30, 14)) { FontOverride = Globals.Font8 };
            selectangle = new ColoredRectangle(Color.Transparent, Color.Gray * .25F, 1, new Vector2(31, 31), Anchor.TopLeft);
            weapangle1 = new ColoredRectangle(Color.Black, Color.Black, 1, new Vector2(13, 13), Anchor.TopRight, new Vector2(349, 34)) { Visible = false };
            weapangle2 = new ColoredRectangle(Color.Black, Color.Black, 1, new Vector2(13, 13), Anchor.TopRight, new Vector2(331, 34)) { Visible = false };
            weapangle3 = new ColoredRectangle(Color.Black, Color.Black, 1, new Vector2(13, 13), Anchor.TopRight, new Vector2(315, 34)) { Visible = false };
            weapangle4 = new ColoredRectangle(Color.Black, Color.Black, 1, new Vector2(13, 13), Anchor.TopRight, new Vector2(298, 34)) { Visible = false };
            weapangle5 = new ColoredRectangle(Color.Black, Color.Black, 1, new Vector2(13, 13), Anchor.TopRight, new Vector2(281, 34)) { Visible = false };
            amtangle1 = new ColoredRectangle(Color.Black, Color.Black, 1, new Vector2(22, 12), Anchor.TopRight, new Vector2(338, 80)) { Visible = false };
            amtangle2 = new ColoredRectangle(Color.Black, Color.Black, 1, new Vector2(22, 12), Anchor.TopRight, new Vector2(310, 80)) { Visible = false };
            amtangle3 = new ColoredRectangle(Color.Black, Color.Black, 1, new Vector2(22, 12), Anchor.TopRight, new Vector2(282, 80)) { Visible = false };
            UserInterface.Active.AddEntity(invPanel);
            CreateWindow(invPanel);
            var initial = new Vector2(230, 129);
            for (var n = 0; n < 6; n++)
            {
                for (var i = 0; i < 10; i++)
                {
                    var temp = initial + new Vector2(35 * i, 35 * n);
                    slotBounds[i + 10 * n].X = (int)temp.X;
                    slotBounds[i + 10 * n].Y = (int)temp.Y;
                    slotBounds[i + 10 * n].Width = 32;
                    slotBounds[i + 10 * n].Height = 32;
                }
            }
        }

        public void PopulateInventory()
        {
            invPanel.ClearChildren();
            detailsPanel.ClearChildren();
            Globals.newInventory = false;
            selectangle.Visible = false;
            detailsBody.Text = "";
            detailsImage.Visible = false;
            detailsHeader.Text = "";
            detailsSubHeader.Text = "";
            costStr.Text = "";
            selectedSlot = -1;
            selectedItem = new Item();
            equipButton.Enabled = false;
            jettButton.Enabled = false;
            equipButton.FillColor = Color.DarkGray;
            jettButton.FillColor = Color.DarkGray;
            equipLabel.Text = "Install";
            for (var loop = 0; loop < 60; loop++) { qty[loop] = 0; }
            qtyStr.Text = "0";
            var P = Types.Player[GameLogic.PlayerIndex];
            var par = new Paragraph[16];
            var parX = new Paragraph[3];
            var img = new Image[8];
            ICollection<Inventory> shopInventory = new List<Inventory>();
            img[0] = new Image(equipImage, new Vector2(340, 100), ImageDrawMode.Panel, Anchor.TopRight, new Vector2(27, 15));                                   // Equipment background
            img[1] = new Image(Graphics.Characters[1], new Vector2(64, 64), ImageDrawMode.Panel, Anchor.TopRight, new Vector2(195, 40));                        // Hull
            par[3] = new Paragraph("Flight Computer", Anchor.TopRight, Color.DarkGray, null, null, new Vector2(50, 17));                                        // FCA
            par[5] = new Paragraph("Shield Generator", Anchor.TopRight, Color.DarkGray, null, null, new Vector2(50, 34));                                       // SGA
            par[6] = new Paragraph("Hull Plating", Anchor.TopRight, Color.DarkGray, null, null, new Vector2(50, 51));                                           // SPA
            par[2] = new Paragraph("Propulsion Drive", Anchor.TopRight, Color.DarkGray, null, null, new Vector2(50, 68));                                       // PDS
            par[4] = new Paragraph("Auxiliary Payload", Anchor.TopRight, Color.DarkGray, null, null, new Vector2(50, 85));                                      // APy
            par[0] = new Paragraph("Weapons:", Anchor.TopRight, Color.DarkGray, null, null, new Vector2(308, 17));                                              // Wps
            par[1] = new Paragraph("Ammunition:", Anchor.TopRight, Color.DarkGray, null, null, new Vector2(288, 48));                                           // AMO
            par[7] = new Paragraph("1", Anchor.TopRight, Color.DarkRed, null, null, new Vector2(351, 34));                                                      // Mn1
            par[8] = new Paragraph("2", Anchor.TopRight, Color.DarkRed, null, null, new Vector2(334, 34));                                                      // Mn2
            par[9] = new Paragraph("3", Anchor.TopRight, Color.DarkRed, null, null, new Vector2(317, 34));                                                      // Mn3
            par[10] = new Paragraph("4", Anchor.TopRight, Color.DarkRed, null, null, new Vector2(300, 34));                                                     // Mn4
            par[11] = new Paragraph("5", Anchor.TopRight, Color.DarkRed, null, null, new Vector2(283, 34));                                                     // Mn5
            par[12] = new Paragraph("", Anchor.TopRight, Color.DarkGray, null, null, new Vector2(339, 80));                                                     // Ss1
            par[13] = new Paragraph("", Anchor.TopRight, Color.DarkGray, null, null, new Vector2(311, 80));                                                     // Ss2
            par[14] = new Paragraph("", Anchor.TopRight, Color.DarkGray, null, null, new Vector2(283, 80));                                                     // Ss3
            par[15] = new Paragraph("", Anchor.TopRight, Color.DarkGray, null, null, new Vector2(283, 97));                                                     // Fls
            img[2] = new Image(Graphics.diamond, new Vector2(12, 12), ImageDrawMode.Stretch, Anchor.TopRight, new Vector2(30, 17)) { FillColor = Color.Red };   // FCA stat
            img[3] = new Image(Graphics.diamond, new Vector2(12, 12), ImageDrawMode.Stretch, Anchor.TopRight, new Vector2(30, 34)) { FillColor = Color.Red };   // SGA stat
            img[4] = new Image(Graphics.diamond, new Vector2(12, 12), ImageDrawMode.Stretch, Anchor.TopRight, new Vector2(30, 51)) { FillColor = Color.Red };   // SPA stat
            img[5] = new Image(Graphics.diamond, new Vector2(12, 12), ImageDrawMode.Stretch, Anchor.TopRight, new Vector2(30, 68)) { FillColor = Color.Red };   // PDS stat
            img[6] = new Image(Graphics.diamond, new Vector2(12, 12), ImageDrawMode.Stretch, Anchor.TopRight, new Vector2(30, 84)) { FillColor = Color.Red };   // APy stat
            img[7] = new Image(Graphics.diamond, new Vector2(12, 12), ImageDrawMode.Stretch, Anchor.TopRight, new Vector2(30, 100)) { FillColor = Color.Red };  // UNK stat
            var par15 = new Paragraph("Fuel:", Anchor.TopRight, Color.DarkGray, null, null, new Vector2(327, 97)) { FontOverride = Globals.Font8 };             // Fls
            var invTab = new Image(tab, new Vector2(150, 20), ImageDrawMode.Stretch, Anchor.BottomRight, new Vector2(20, 38)) { FillColor = Color.DarkGray };
            var invLabel = new Paragraph("Cargo hold", Anchor.BottomRight, null, new Vector2(54, 42)) { ClickThrough = true, FontOverride = Globals.Font10, OutlineOpacity = 0, FillColor = Color.Black };
            var invLabel2 = new Paragraph("Cargo hold", Anchor.BottomRight, null, new Vector2(53, 42)) { ClickThrough = true, FontOverride = Globals.Font10, OutlineOpacity = 0, FillColor = Color.Black * .75F };
            var shopTab = new Image(tab, new Vector2(150, 20), ImageDrawMode.Stretch, Anchor.BottomRight, new Vector2(222, 38));
            var shopLabel = new Paragraph("Shop Inventory", Anchor.BottomRight, null, new Vector2(242, 42)) { ClickThrough = true, FontOverride = Globals.Font10, OutlineOpacity = 0, FillColor = Color.Black };
            var shopLabel2 = new Paragraph("Shop Inventory", Anchor.BottomRight, null, new Vector2(241, 42)) { ClickThrough = true, FontOverride = Globals.Font10, OutlineOpacity = 0, FillColor = Color.Black * .75F };
            parX[0] = new Paragraph(Types.Player[GameLogic.PlayerIndex].Rank + " " + Types.Player[GameLogic.PlayerIndex].Name, Anchor.TopLeft, Color.DarkGray, null, new Vector2(200, 20), new Vector2(5, 17))
            { FontOverride = Globals.Font10, OutlineOpacity = 0, AlignToCenter = true, WrapWords = false };
            parX[1] = new Paragraph("Level: " + Types.Player[GameLogic.PlayerIndex].Level, Anchor.TopLeft, Color.DarkGray, null, null, new Vector2(10, 29))
            { FontOverride = Globals.Font8, OutlineOpacity = 0 };
            parX[2] = new Paragraph("Credits: ¢" + Types.Player[GameLogic.PlayerIndex].Credits, Anchor.TopLeft, Color.DarkGray, null, null, new Vector2(10, 40))
            { FontOverride = Globals.Font8, OutlineOpacity = 0 };
            invPanel.AddChild(invImage);
            invPanel.AddChild(detailsPanel);
            invPanel.AddChild(par15);
            detailsPanel.AddChild(detailsHeader);
            detailsPanel.AddChild(detailsSubHeader);
            detailsPanel.AddChild(detailsBody);
            detailsPanel.AddChild(detailsImage);
            invPanel.AddChild(equipButton);
            invPanel.AddChild(equipLabel);
            invPanel.AddChild(jettButton);
            invPanel.AddChild(jettLabel);
            invPanel.AddChild(qtyLbl);
            invPanel.AddChild(qtyStr);
            invPanel.AddChild(decBtn);
            invPanel.AddChild(incBtn);
            invPanel.AddChild(invCloseBtn);
            invPanel.AddChild(selectangle);
            invPanel.AddChild(weapangle1);
            invPanel.AddChild(weapangle2);
            invPanel.AddChild(weapangle3);
            invPanel.AddChild(weapangle4);
            invPanel.AddChild(weapangle5);
            invPanel.AddChild(amtangle1);
            invPanel.AddChild(amtangle2);
            invPanel.AddChild(amtangle3);
            invPanel.AddChild(costStr);
            invPanel.AddChild(hoverPanel);
            invPanel.OnMouseLeave = panelLeave => { hoverPanel.Visible = false; };
            invCloseBtn.OnMouseEnter = closeEnter => { UserInterface.Active.SetCursor(Graphics.Cursors[2], 32, new Point(-4, 0)); };
            invCloseBtn.OnMouseLeave = closeLeave => { UserInterface.Active.SetCursor(CursorType.Default); };
            invCloseBtn.OnClick = closeClick => { MenuManager.Clear(5); };
            jettButton.OnMouseEnter = entity => { jettButton.Texture = button_hover; UserInterface.Active.SetCursor(Graphics.Cursors[2], 32, new Point(-4, 0)); };
            jettButton.OnMouseLeave = entity => { jettButton.Texture = button; UserInterface.Active.SetCursor(CursorType.Default); };
            jettButton.OnMouseDown = entity => { jettButton.Texture = button_down; };
            jettButton.OnMouseReleased = entity => { jettButton.Texture = button; };
            jettButton.OnClick = entity =>
            {
                var QTY = qty[selectedSlot - 101];
                if (QTY == 0) QTY = 1;
                switch (jettLabel.Text)
                {
                    case "  Sell  ":
                        var _first = Types.Player[GameLogic.PlayerIndex].Inventory
                            .FirstOrDefault(unknown => unknown.Slot == selectedSlot);
                        if (_first != null)
                        {
                            ctcp.BuyOrSell(2, _first.Id, QTY);
                        }
                        break;
                    case "   Buy   ":
                        ctcp.BuyOrSell(1, selectedItem.Id, QTY);
                        break;
                    default:
                        if (Globals.Control && Globals.Alt)
                        {
                            if (Types.Player == null) return;
                            var first = Types.Player[GameLogic.PlayerIndex].Inventory
                                .FirstOrDefault(unknown => unknown.Slot == selectedSlot);
                            if (first != null)
                            {
                                ctcp.TransactItem(first.Id, "X");
                            }
                        }
                        else
                        {
                            AddChats(@"Press and hold [ctrl] and [alt] to jettison items", Color.DarkGoldenrod);
                        }
                        break;
                }
            };
            equipButton.OnMouseEnter = entity => { equipButton.Texture = button_hover; UserInterface.Active.SetCursor(Graphics.Cursors[2], 32, new Point(-4, 0)); };
            equipButton.OnMouseLeave = entity => { equipButton.Texture = button; UserInterface.Active.SetCursor(CursorType.Default); };
            equipButton.OnMouseDown = entity => { equipButton.Texture = button_down; };
            equipButton.OnMouseReleased = entity => { equipButton.Texture = button; };
            equipButton.OnClick = entity =>
            {
                if (Types.Player == null) return;
                var first = Types.Player[GameLogic.PlayerIndex].Inventory
                    .FirstOrDefault(unknown => unknown.Slot == selectedSlot);
                if (first == null) return;
                if (first.Slot < 100)
                {
                    ctcp.EquipItem(first.Id, -1);
                    return;
                }
                switch (selectedItem.Slot)
                {
                    case 7:
                        Globals.equipWeapon = true;
                        break;
                    case 12:
                        Globals.equipAmmo = true;
                        break;
                    default:
                        ctcp.EquipItem(first.Id, 0);
                        break;
                }
            };
            for (var i = 0; i < 3; i++)
                invPanel.AddChild(parX[i]);
            for (var i = 0; i < 8; i++)
                invPanel.AddChild(img[i]);
            if (P.Inventory?.FirstOrDefault(I => I.Slot == 3)?.Id != null) { img[2].FillColor = Color.DarkGreen; }
            if (P.Inventory?.FirstOrDefault(I => I.Slot == 5)?.Id != null) { img[3].FillColor = Color.DarkGreen; }
            if (P.Inventory?.FirstOrDefault(I => I.Slot == 6)?.Id != null) { img[4].FillColor = Color.DarkGreen; }
            if (P.Inventory?.FirstOrDefault(I => I.Slot == 2)?.Id != null) { img[5].FillColor = Color.DarkGreen; }
            if (P.Inventory?.FirstOrDefault(I => I.Slot == 4)?.Id != null) { img[6].FillColor = Color.DarkGreen; }
            if (P.Inventory?.FirstOrDefault(I => I.Slot == 16)?.Id != null) { img[7].FillColor = Color.DarkGreen; }
            if (P.Inventory?.FirstOrDefault(I => I.Slot == 7)?.Id != null) { par[7].FillColor = Color.DarkGray; }
            if (P.Inventory?.FirstOrDefault(I => I.Slot == 8)?.Id != null) { par[8].FillColor = Color.DarkGray; }
            if (P.Inventory?.FirstOrDefault(I => I.Slot == 9)?.Id != null) { par[9].FillColor = Color.DarkGray; }
            if (P.Inventory?.FirstOrDefault(I => I.Slot == 10)?.Id != null) { par[10].FillColor = Color.DarkGray; }
            if (P.Inventory?.FirstOrDefault(I => I.Slot == 11)?.Id != null) { par[11].FillColor = Color.DarkGray; }
            par[12].Text = P.Inventory?.FirstOrDefault(I => I.Slot == 12)?.Quantity.ToString() ?? "000";
            par[13].Text = P.Inventory?.FirstOrDefault(I => I.Slot == 13)?.Quantity.ToString() ?? "000";
            par[14].Text = P.Inventory?.FirstOrDefault(I => I.Slot == 14)?.Quantity.ToString() ?? "000";
            par[15].Text = P.Inventory?.FirstOrDefault(I => I.Slot == 15)?.Quantity.ToString() ?? "000";
            for (var i = 0; i < 16; i++)
            {
                var n = i; // Access to modified closure
                par[i].FontOverride = Globals.Font8;
                par[i].OutlineOpacity = 0;
                invPanel.AddChild(par[i]);
                if (n < 2) continue;
                par[n].OnMouseEnter = parEnter =>
                {
                    UserInterface.Active.SetCursor(Graphics.Cursors[2], 32, new Point(-4, 0));
                    par[n].FillColor = par[n].FillColor == Color.DarkRed ? Color.Red : Color.WhiteSmoke;
                };
                par[n].OnMouseLeave = parLeave =>
                {
                    UserInterface.Active.SetCursor(CursorType.Default);
                    par[n].FillColor = par[n].FillColor == Color.Red ? Color.DarkRed : Color.DarkGray;
                };
                par[n].OnClick = parClick =>
                {
                    if (Globals.equipWeapon)
                    {
                        var first = Types.Player[GameLogic.PlayerIndex].Inventory
                            .FirstOrDefault(unknown => unknown.Slot == selectedSlot);
                        if (first == null) return;
                        if (n == 7) { ctcp.EquipItem(first.Id, 7); }
                        if (n == 8) { ctcp.EquipItem(first.Id, 8); }
                        if (n == 9) { ctcp.EquipItem(first.Id, 9); }
                        if (n == 10) { ctcp.EquipItem(first.Id, 10); }
                        if (n == 11) { ctcp.EquipItem(first.Id, 11); }
                        Globals.equipWeapon = false;
                        return;
                    }
                    if (Globals.equipAmmo)
                    {
                        var first = Types.Player[GameLogic.PlayerIndex].Inventory
                            .FirstOrDefault(unknown => unknown.Slot == selectedSlot);
                        if (first == null) return;
                        if (n == 12) { ctcp.EquipItem(first.Id, 12); }
                        if (n == 13) { ctcp.EquipItem(first.Id, 13); }
                        if (n == 14) { ctcp.EquipItem(first.Id, 14); }
                        Globals.equipAmmo = false;
                        return;
                    }
                    var temp = GameLogic.Items.FirstOrDefault(pI => pI.Id == P.Inventory.FirstOrDefault(I => I.Slot == n)?.ItemId);
                    if (temp != null)
                    {
                        DisplayDetails(temp);
                        equipLabel.Text = "Remove ";
                        equipButton.Enabled = true;
                        equipButton.FillColor = Color.White;
                        selectedItem = temp;
                        selectedSlot = n;
                    }
                    else
                    {
                        detailsImage.Visible = false;
                        detailsHeader.Text = "";
                        detailsSubHeader.Text = "";
                        detailsBody.Text = "{{RED}}No equipment installed";
                    }
                };
            }
            img[1].OnMouseEnter = parEnter =>
            {
                UserInterface.Active.SetCursor(Graphics.Cursors[2], 32, new Point(-4, 0));
            };
            img[1].OnMouseLeave = parLeave =>
            {
                UserInterface.Active.SetCursor(CursorType.Default);
            };
            img[1].OnClick = parClick =>
            {
                var temp = GameLogic.Items.FirstOrDefault(pI => pI.Id == P.Inventory.FirstOrDefault(I => I.Slot == 1)?.ItemId);
                if (temp != null)
                {
                    DisplayDetails(temp);
                }
                else
                {
                    detailsImage.Visible = false;
                    detailsHeader.Text = "";
                    detailsSubHeader.Text = "";
                    detailsBody.Text = "{{RED}}No equipment installed";
                }
            };
            invTab.OnMouseEnter = tabEnter =>
            {
                UserInterface.Active.SetCursor(Graphics.Cursors[2], 32, new Point(-4, 0));
            };
            invTab.OnMouseLeave = tabEnter =>
            {
                UserInterface.Active.SetCursor(CursorType.Default);
            };
            invTab.OnClick = tabClick =>
            {
                pInv = true;
                selectangle.Visible = false;
                selectedSlot = -1;
                selectedItem = new Item();
                DisplayDetails();
                costStr.Text = "";
                ArrangeInventory(Types.Player[GameLogic.PlayerIndex].Inventory);
                shopTab.FillColor = Color.DarkGray;
                invTab.FillColor = Color.White;
                equipButton.Enabled = false;
                equipButton.FillColor = Color.DarkGray;
                jettButton.Enabled = false;
                jettButton.FillColor = Color.DarkGray;
            };
            shopTab.OnMouseEnter = tabEnter =>
            {
                UserInterface.Active.SetCursor(Graphics.Cursors[2], 32, new Point(-4, 0));
            };
            shopTab.OnMouseLeave = tabEnter =>
            {
                UserInterface.Active.SetCursor(CursorType.Default);
            };
            shopTab.OnClick = tabClick =>
            {
                pInv = false;
                selectangle.Visible = false;
                selectedSlot = -1;
                costStr.Text = "";
                selectedItem = new Item();
                DisplayDetails();
                // ReSharper disable once AccessToModifiedClosure
                ArrangeInventory(shopInventory);
                invTab.FillColor = Color.DarkGray;
                shopTab.FillColor = Color.White;
                equipButton.Enabled = false;
                equipButton.FillColor = Color.DarkGray;
                jettButton.Enabled = false;
                jettButton.FillColor = Color.DarkGray;
            };
            switch (Globals.inventoryMode)
            {
                case 1:
                    ArrangeInventory(Types.Player[GameLogic.PlayerIndex].Inventory);
                    break;
                case 2:
                    invPanel.AddChild(invTab);
                    invPanel.AddChild(invLabel);
                    invPanel.AddChild(invLabel2);
                    invPanel.AddChild(shopTab);
                    invPanel.AddChild(shopLabel);
                    invPanel.AddChild(shopLabel2);
                    if (pInv)
                    {
                        invTab.FillColor = Color.White;
                        shopTab.FillColor = Color.DarkGray;
                    }
                    else
                    {
                        invTab.FillColor = Color.DarkGray;
                        shopTab.FillColor = Color.White;
                    }
                    foreach (var star in GameLogic.Galaxy)
                    {
                        var planet = star.Planets.FirstOrDefault(p => p.Id == GameLogic.selectedPlanet);
                        if (planet == null) continue;
                        shopInventory = planet.Inventory;
                        ArrangeInventory(!pInv ? shopInventory : Types.Player[GameLogic.PlayerIndex].Inventory);
                    }
                    break;
            }
        }

        public void ArrangeInventory(ICollection<Inventory> collection)
        {
            var dragging = false;
            for (var x = 0; x < 60; x++)
            {
                if (invPanel.Children.Contains(slot[x]))
                    invPanel.RemoveChild(slot[x]);
                if (invPanel.Children.Contains(slotQty[x]))
                    invPanel.RemoveChild(slotQty[x]);
            }
            if (Globals.inventoryMode == 2)
            {
                jettLabel.Text = !pInv ? "   Buy   " : "  Sell  ";
            }
            else
            {
                jettLabel.Text = "Jettison";
            }
            if (collection == null) return;
            foreach (var invItem in collection)
            {
                if (invItem.Slot < 101) continue;
                var SLOT = invItem.Slot - 101;
                var ITEM = GameLogic.Items.FirstOrDefault(i => i.Id == invItem.ItemId);
                slot[SLOT] = new Image(Graphics.Objects[ITEM?.Image ?? 0], new Vector2(32, 32), ImageDrawMode.Stretch, Anchor.TopLeft,
                    new Vector2(slotBounds[SLOT].X, slotBounds[SLOT].Y))
                { Draggable = true, FillColor = Graphics.COLOR(ITEM?.Color) };
                if (Globals.inventoryMode == 2 && !pInv) { slot[SLOT].Draggable = false; }
                invPanel.AddChild(slot[SLOT]);
                if (invItem.Quantity > 1)
                {
                    slotQty[SLOT] = new Paragraph(invItem.Quantity.ToString(), Anchor.TopLeft, Color.DarkGray * .75F,
                            null, new Vector2(32, 10), new Vector2(slotBounds[SLOT].X, slotBounds[SLOT].Y + 22))
                    { FontOverride = Globals.Font8, OutlineOpacity = 0, AlignToCenter = true };
                    invPanel.AddChild(slotQty[SLOT]);
                }
                slot[SLOT].OnClick = entity =>
                {
                    if (dragging) return;
                    Globals.equipAmmo = false;
                    Globals.equipWeapon = false;
                    jettButton.Enabled = true;
                    jettButton.FillColor = Color.White;
                    selectedItem = ITEM;
                    selectedSlot = SLOT + 101;
                    selectangle.SetOffset(new Vector2(slotBounds[SLOT].X, slotBounds[SLOT].Y));
                    selectangle.Visible = true;
                    if (Globals.Control && jettLabel.Text == "  Sell  ")
                    {
                        var _first = Types.Player[GameLogic.PlayerIndex].Inventory
                            .FirstOrDefault(unknown => unknown.Slot == SLOT + 101);
                        if (_first != null)
                        {
                            ctcp.BuyOrSell(2, _first.Id, 1);
                        }
                        return;
                    }
                    if (Globals.Control && Globals.Alt && jettLabel.Text == "  Sell  ")
                    {
                        var _first = Types.Player[GameLogic.PlayerIndex].Inventory
                            .FirstOrDefault(unknown => unknown.Slot == SLOT + 101);
                        if (_first != null)
                        {
                            ctcp.BuyOrSell(2, _first.Id, _first.Quantity);
                        }
                        return;
                    }
                    if (Globals.Control && jettLabel.Text == "   Buy   ")
                    {
                        ctcp.BuyOrSell(1, selectedItem?.Id, 1);
                        return;
                    }
                    DisplayDetails(ITEM);
                    if (ITEM?.Slot == 0) return;
                    if (!pInv && Globals.inventoryMode == 2) return;
                    equipButton.Enabled = true;
                    equipButton.FillColor = Color.White;
                };
            }
            for (var i = 0; i < 60; i++)
            {
                if (!invPanel.Children.Contains(slot[i])) continue;
                var z = i;
                var dropTest = false;
                slot[i].OnStartDrag = entity => { dragging = true; };
                slot[i].OnStopDrag = entity =>
                {
                    for (var n = 0; n < 60; n++)
                    {
                        var off = slot[z].GetRelativeOffset();
                        if (off.X + 16 >= slotBounds[n].X && off.X + 16 <= slotBounds[n].X + 32 && off.Y + 16 >= slotBounds[n].Y && off.Y + 16 <= slotBounds[n].Y + 32)
                        {
                            if (n == z)
                            {
                                ArrangeInventory(collection);
                                return;
                            }
                            dropTest = true;
                            if (Types.Player != null)
                            {
                                var _first = collection.FirstOrDefault(current => current.Slot == n + 101);
                                var first = collection.FirstOrDefault(unknown => unknown.Slot == z + 101);
                                if (first != null)
                                {
                                    if (_first != null)
                                    {
                                        if (_first.ItemId == first.ItemId)
                                        {
                                            ctcp.StackItems(first.Id, _first.Id);
                                        }
                                        break;
                                    }
                                    ctcp.EquipItem(first.Id, 101 + n);
                                }
                            }
                            break;
                        }
                    }
                    if (dropTest) return;
                    ArrangeInventory(collection);
                };
            }
            if (selectedSlot > 100) { qtyStr.Text = qty[selectedSlot - 101].ToString(); }
            decBtn.OnMouseEnter = decEnter =>
            {
                UserInterface.Active.SetCursor(Graphics.Cursors[2], 32, new Point(-4, 0));
            };
            decBtn.OnMouseLeave = decLeave => { UserInterface.Active.SetCursor(CursorType.Default); };
            decBtn.OnClick = decClick =>
            {
                if (selectedSlot < 0) return;
                if (qty[selectedSlot - 101] <= 0) return;
                if (Globals.Control)
                {
                    qty[selectedSlot - 101] -= 10;
                    if (qty[selectedSlot - 101] < 0)
                        qty[selectedSlot - 101] = 0;
                }
                else if (Globals.Alt)
                {
                    qty[selectedSlot - 101] -= 100;
                    if (qty[selectedSlot - 101] < 0)
                        qty[selectedSlot - 101] = 0;
                }
                else if (Globals.Shift)
                {
                    qty[selectedSlot - 101] = 0;
                }
                else
                {
                    qty[selectedSlot - 101]--;
                }

                qtyStr.Text = qty[selectedSlot - 101].ToString();
                if (pInv && Globals.inventoryMode == 2)
                    costStr.Text = "¢" + qty[selectedSlot - 101] * selectedItem.Cost;
                if (!pInv && Globals.inventoryMode == 2)
                    costStr.Text = "(¢" + qty[selectedSlot - 101] * selectedItem.Cost + ")";
            };
            incBtn.OnMouseEnter = incEnter =>
            {
                UserInterface.Active.SetCursor(Graphics.Cursors[2], 32, new Point(-4, 0));
            };
            incBtn.OnMouseLeave = incLeave => { UserInterface.Active.SetCursor(CursorType.Default); };
            incBtn.OnClick = incClick =>
            {
                if (selectedSlot < 0) return;
                if (qty[selectedSlot - 101] >= 999) return;
                if (Globals.Control)
                {
                    qty[selectedSlot - 101] += 10;
                    if (qty[selectedSlot - 101] > 999)
                        qty[selectedSlot - 101] = 999;
                }
                else if (Globals.Alt)
                {
                    qty[selectedSlot - 101] += 100;
                    if (qty[selectedSlot - 101] > 999)
                        qty[selectedSlot - 101] = 999;
                }
                else if (Globals.Shift)
                {
                    qty[selectedSlot - 101] = 999;
                }
                else
                {
                    qty[selectedSlot - 101]++;
                }

                qtyStr.Text = qty[selectedSlot - 101].ToString();
                if (pInv && Globals.inventoryMode == 2)
                    costStr.Text = "¢" + qty[selectedSlot - 101] * selectedItem.Cost;
                if (!pInv && Globals.inventoryMode == 2)
                    costStr.Text = "(¢" + qty[selectedSlot - 101] * selectedItem.Cost + ")";
            };
        }

        public static void PopulateMap()
        {
            const float scale = (float)500 / Constants.PLAY_AREA_WIDTH;
            var closeBtn = new Image(closeIcon, new Vector2(15, 15), ImageDrawMode.Stretch, Anchor.TopRight, new Vector2(-30, -29));
            starDetail = new Image(Graphics.Planets[0], new Vector2(175, 175), ImageDrawMode.Stretch, Anchor.TopRight, new Vector2(40, 10))
            {
                Enabled = false,
                Opacity = 100
            };
            galaxyMap.AddChild(starDetail);
            mapPlayer = new Image(Graphics.diamond, new Vector2(12, 12), ImageDrawMode.Stretch, Anchor.TopLeft, new Vector2(Types.Player[GameLogic.PlayerIndex].X * scale, Types.Player[GameLogic.PlayerIndex].Y * scale))
            { ClickThrough = true, FillColor = Color.OliveDrab };
            galaxyMap.AddChild(mapPlayer);
            for (var i = 0; i < GameLogic.Galaxy.Count; i++)
            {
                var n = i; // Idk why this works but it does- trust it
                var image = new Image(Graphics.star, new Vector2(12, 12), ImageDrawMode.Stretch, Anchor.TopLeft, new Vector2(GameLogic.Galaxy[n].X * scale, GameLogic.Galaxy[n].Y * scale));
                galaxyMap.AddChild(image);
                image.OnMouseEnter += starEnter => { starLabel.Text = GameLogic.Galaxy[n].Name; UserInterface.Active.SetCursor(Graphics.Cursors[2], 32, new Point(-4, 0)); };
                image.OnMouseLeave += starLeave => { starLabel.Text = ""; UserInterface.Active.SetCursor(CursorType.Default); };
                image.OnClick += starClick =>
                {
                    GameLogic.selectedMapItem = n;
                    MapDetail(n);
                };
            }
            galaxyMap.AddChild(closeBtn);
            closeBtn.OnMouseEnter += closeEnter => { UserInterface.Active.SetCursor(Graphics.Cursors[2], 32, new Point(-4, 0)); };
            closeBtn.OnMouseLeave += closeLeave => { UserInterface.Active.SetCursor(CursorType.Default); };
            closeBtn.OnClick += closeClick => { MenuManager.Clear(4); };
        }

        private static void MapDetail(int n)
        {
            starDetail.Texture = Graphics.Planets[4];
            mapLine[0].Text = "Name: " + GameLogic.Galaxy[n].Name;
            mapLine[1].Text = "Classification: " + GameLogic.Galaxy[n].Class;
            mapLine[2].Text = "Coordinates: " + GameLogic.Galaxy[n].X / 100 + ":" + GameLogic.Galaxy[n].Y / 100;
            mapLine[3].Text = "Belligerence: " + GameLogic.Galaxy[n].Belligerence;
            for (var index = 5; index < 10; index++)
            {
                mapLine[index].Text = "";
            }
            if (GameLogic.Galaxy[n].Planets.Count == 0)
            {
                mapLine[4].Text = "Planets: None";
            }
            else
            {
                mapLine[4].Text = "Planets: " + GameLogic.Galaxy[n].Planets.Count;
                var j = 5;
                foreach (var planet in GameLogic.Galaxy[n].Planets)
                {
                    mapLine[j].OnMouseEnter += entity => { UserInterface.Active.SetCursor(Graphics.Cursors[2], 32, new Point(-4, 0)); };
                    mapLine[j].OnClick += entity =>
                    {
                        starDetail.Texture = Graphics.Planets[planet.Sprite];
                        mapLine[0].Text = "Name: " + planet.Name;
                        mapLine[1].Text = "Classification: " + planet.Class;
                        mapLine[2].Text = "Coordinates: " + (int)planet.X / 100 + ":" + (int)planet.Y / 100;
                        mapLine[3].Text = "Belligerence: " + planet.Belligerence;
                        for (int index = 4; index < 9; index++)
                        {
                            mapLine[index].Text = "";
                        }

                        mapLine[9].Text = "<- Back";
                        mapLine[9].OnMouseEnter += Entity => { UserInterface.Active.SetCursor(Graphics.Cursors[2], 32, new Point(-4, 0)); };
                        mapLine[9].OnMouseLeave += Entity => { UserInterface.Active.SetCursor(CursorType.Default); };
                        mapLine[9].OnClick += Entity => { MapDetail(n); };
                    };
                    mapLine[j].Text = planet.Name;
                    j++;
                }
            }
        }

        public void CreateLoot()
        {
            var panel = new Panel(new Vector2(451, 287), PanelSkin.None, Anchor.AutoCenter) { Draggable = true, Padding = Vector2.Zero };
            var image = new Image(lootPanel, new Vector2(451, 287), ImageDrawMode.Stretch, Anchor.TopLeft);
            var dPanel = new Panel(new Vector2(219, 218), PanelSkin.None, Anchor.TopLeft, new Vector2(10, 10))
            { Padding = Vector2.Zero, PanelOverflowBehavior = PanelOverflowBehavior.VerticalScroll };
            var closeButton = new Image(closeIcon, new Vector2(15, 15), ImageDrawMode.Stretch, Anchor.TopRight, new Vector2(-2, -2));
            destroyBtn = new Image(button, new Vector2(90, 30), ImageDrawMode.Stretch, Anchor.BottomRight, new Vector2(12, 12));
            var destroyLbl = new Paragraph("Destroy", Anchor.BottomRight, null, new Vector2(17, 16)) { FillColor = Color.DarkGray, ClickThrough = true, FontOverride = Globals.Font12 };
            lootItem[0] = new Image(Graphics.Objects[0], new Vector2(64, 64), ImageDrawMode.Stretch, Anchor.TopRight, new Vector2(156, 16)) { Visible = false };
            lootItem[1] = new Image(Graphics.Objects[0], new Vector2(64, 64), ImageDrawMode.Stretch, Anchor.TopRight, new Vector2(86, 16)) { Visible = false };
            lootItem[2] = new Image(Graphics.Objects[0], new Vector2(64, 64), ImageDrawMode.Stretch, Anchor.TopRight, new Vector2(16, 16)) { Visible = false };
            lootItem[3] = new Image(Graphics.Objects[0], new Vector2(64, 64), ImageDrawMode.Stretch, Anchor.TopRight, new Vector2(156, 86)) { Visible = false };
            lootItem[4] = new Image(Graphics.Objects[0], new Vector2(64, 64), ImageDrawMode.Stretch, Anchor.TopRight, new Vector2(86, 86)) { Visible = false };
            lootItem[5] = new Image(Graphics.Objects[0], new Vector2(64, 64), ImageDrawMode.Stretch, Anchor.TopRight, new Vector2(16, 86)) { Visible = false };
            lootItem[6] = new Image(Graphics.Objects[0], new Vector2(64, 64), ImageDrawMode.Stretch, Anchor.TopRight, new Vector2(156, 156)) { Visible = false };
            lootItem[7] = new Image(Graphics.Objects[0], new Vector2(64, 64), ImageDrawMode.Stretch, Anchor.TopRight, new Vector2(86, 156)) { Visible = false };
            lootItem[8] = new Image(Graphics.Objects[0], new Vector2(64, 64), ImageDrawMode.Stretch, Anchor.TopRight, new Vector2(16, 156)) { Visible = false };
            detailsHeaderLoot = new Paragraph("", Anchor.TopLeft, Color.DarkGray, null, new Vector2(200, 10), new Vector2(0, 5))
            { FontOverride = Globals.Font12, OutlineOpacity = 0, AlignToCenter = true, WrapWords = false };
            detailsSubHeaderLoot = new Paragraph("", Anchor.TopLeft, Color.DarkGray * .6F, null, new Vector2(200, 10), new Vector2(0, 22))
            { FontOverride = Globals.Font8, OutlineOpacity = 0, AlignToCenter = true };
            detailsBodyLoot = new MulticolorParagraph("", Anchor.TopLeft, Color.DarkGray, null, new Vector2(200, 10), new Vector2(0, 50))
            { FontOverride = Globals.Font10, OutlineOpacity = 0, AlignToCenter = true, WrapWords = true };
            UserInterface.Active.AddEntity(panel);
            panel.AddChild(image);
            for (var i = 0; i < 9; i++)
            {
                panel.AddChild(lootItem[i]);
                lootItem[i].OnMouseEnter = e => { UserInterface.Active.SetCursor(Graphics.Cursors[2], 32, new Point(-4, 0)); };
                lootItem[i].OnMouseLeave = e => { UserInterface.Active.SetCursor(CursorType.Default); };
            }
            panel.AddChild(destroyBtn);
            panel.AddChild(destroyLbl);
            panel.AddChild(closeButton);
            panel.AddChild(dPanel);
            dPanel.AddChild(detailsHeaderLoot);
            dPanel.AddChild(detailsSubHeaderLoot);
            dPanel.AddChild(detailsBodyLoot);
            closeButton.OnMouseEnter = e => { UserInterface.Active.SetCursor(Graphics.Cursors[2], 32, new Point(-4, 0)); };
            closeButton.OnMouseLeave = e => { UserInterface.Active.SetCursor(CursorType.Default); };
            closeButton.OnClick = e => { MenuManager.Clear(7); };
            destroyBtn.OnMouseEnter = e => { UserInterface.Active.SetCursor(Graphics.Cursors[2], 32, new Point(-4, 0)); };
            destroyBtn.OnMouseLeave = e => { UserInterface.Active.SetCursor(CursorType.Default); };
            CreateWindow(panel);
        }

        public void PopulateLoot(string lootId)
        {
            for (var i = 0; i < 9; i++)
                lootItem[i].Visible = false;

            var loot = GameLogic.RealLoot.FirstOrDefault(L => L.Id == lootId);
            if (loot == null)
            {
                MenuManager.Clear();
                Globals.selectedLoot = null;
                return;
            }
            for (var i = 0; i < 9; i++)
            {
                if (loot.Items[i] == null) continue;
                var temp = GameLogic.Items.FirstOrDefault(itm => itm.Id == loot.Items[i]);
                lootItem[i].Visible = true;
                lootItem[i].Texture = Graphics.Objects[temp?.Image ?? 0];
                lootItem[i].FillColor = Graphics.COLOR(temp?.Color);
                var i1 = i;
                lootItem[i].OnClick = e =>
                {
                    if (new KeyControl().CheckCtrl())
                    {
                        ctcp.ProcessLoot(2, loot.Id, i1);
                    }
                    else
                    {
                        DisplayLootDetails(temp);
                    }
                };
            }
            destroyBtn.OnClick = e =>
            {
                if (Globals.Control && Globals.Alt)
                {
                    ctcp.ProcessLoot(1, loot.Id);
                    MenuManager.Clear();
                    Globals.selectedLoot = null;
                }
                else
                {
                    AddChats(@"Press and hold [ctrl] and [alt] to destroy loots", Color.DarkGoldenrod);
                }
            };
        }

        public void Update()
        {
            if (GameLogic.PlayerIndex < 0) return;
            if (Types.Player[GameLogic.PlayerIndex].Inventory == null) return;
            if (Windows[4].Visible)
            {
                const float scale = (float)500 / Constants.PLAY_AREA_WIDTH;
                mapPlayer.SetOffset(new Vector2(Types.Player[GameLogic.PlayerIndex].X * scale, Types.Player[GameLogic.PlayerIndex].Y * scale));
            }
            if (Globals.newInventory)
            {
                PopulateInventory();
                Globals.newInventory = false;
            }

            if (Windows[7].IsVisible())
            {
                PopulateLoot(Globals.selectedLoot);
            }
            if (Globals.equipWeapon)
            {
                weapangle1.Visible = true;
                weapangle2.Visible = true;
                weapangle3.Visible = true;
                weapangle4.Visible = true;
                weapangle5.Visible = true;
                weapangle1.OutlineColor = Color.Green;
                weapangle2.OutlineColor = Color.Green;
                weapangle3.OutlineColor = Color.Green;
                weapangle4.OutlineColor = Color.Green;
                weapangle5.OutlineColor = Color.Green;
                if (Globals.strobe)
                {
                    weapangle1.OutlineColor = Color.Black;
                    weapangle2.OutlineColor = Color.Black;
                    weapangle3.OutlineColor = Color.Black;
                    weapangle4.OutlineColor = Color.Black;
                    weapangle5.OutlineColor = Color.Black;
                }
            }
            else
            {
                weapangle1.Visible = false;
                weapangle2.Visible = false;
                weapangle3.Visible = false;
                weapangle4.Visible = false;
                weapangle5.Visible = false;
            }
            if (Globals.equipAmmo)
            {
                amtangle1.Visible = true;
                amtangle2.Visible = true;
                amtangle3.Visible = true;
                amtangle1.OutlineColor = Color.Green;
                amtangle2.OutlineColor = Color.Green;
                amtangle3.OutlineColor = Color.Green;
                if (Globals.strobe)
                {
                    amtangle1.OutlineColor = Color.Black;
                    amtangle2.OutlineColor = Color.Black;
                    amtangle3.OutlineColor = Color.Black;
                }
            }
            else
            {
                amtangle1.Visible = false;
                amtangle2.Visible = false;
                amtangle3.Visible = false;
            }
        }

        public void DisplayDetails(Item item = null)
        {
            if (item == null)
            {
                detailsImage.Visible = false;
                detailsHeader.Text = "";
                detailsSubHeader.Text = "";
                detailsBody.Text = "";
                return;
            }
            ColorInstruction.AddCustomColor("DARKGRAY", Color.DarkGray);
            detailsImage.Texture = Graphics.Objects[item.Image];
            detailsImage.FillColor = Graphics.COLOR(item.Color);
            detailsImage.Visible = true;
            detailsHeader.Text = item.Name + "\n";
            detailsSubHeader.Text = "Level " + item.Level + " " + item.Type + "\n Cost: ¢" + item.Cost;
            detailsBody.Text = item.Description + "\n";
            if (item.Armor != 0) { detailsBody.Text += "Armor: " + item.Armor + "\n"; }
            if (item.Damage != 0) { detailsBody.Text += "Damage: " + item.Damage + "\n"; }
            if (item.Defense != 0) { detailsBody.Text += "Defense: " + item.Defense + "\n"; }
            if (item.Hull != 0) { detailsBody.Text += "Hull strength: " + item.Hull + "\n"; }
            if (item.Offense != 0) { detailsBody.Text += "Offense: " + item.Offense + "\n"; }
            if (item.Power != 0) { detailsBody.Text += "Power: " + item.Power + "GW\n"; }
            if (item.Recharge != 0) { detailsBody.Text += "Recharge capacity: " + item.Recharge + "GW\n"; }
            if (item.Repair != 0) { detailsBody.Text += "Repair: " + item.Repair + "\n"; }
            if (item.Shield != 0) { detailsBody.Text += "Shield: " + item.Shield + "\n"; }
            if (item.Thrust != 0) { detailsBody.Text += "Thrust: " + item.Thrust + "\n"; }
            if (item.Weapons != 0) { detailsBody.Text += "Weapons slots: " + item.Weapons + "\n"; }
        }

        public void DisplayLootDetails(Item item = null)
        {
            if (item == null)
            {
                detailsHeaderLoot.Text = "";
                detailsSubHeaderLoot.Text = "";
                detailsBodyLoot.Text = "";
                return;
            }
            ColorInstruction.AddCustomColor("DARKGRAY", Color.DarkGray);
            detailsHeaderLoot.Text = item.Name + "\n";
            detailsSubHeaderLoot.Text = "Level " + item.Level + " " + item.Type + "\n Cost: ¢" + item.Cost;
            detailsBodyLoot.Text = item.Description + "\n";
            if (item.Armor != 0) { detailsBodyLoot.Text += "Armor: " + item.Armor + "\n"; }
            if (item.Damage != 0) { detailsBodyLoot.Text += "Damage: " + item.Damage + "\n"; }
            if (item.Defense != 0) { detailsBodyLoot.Text += "Defense: " + item.Defense + "\n"; }
            if (item.Hull != 0) { detailsBodyLoot.Text += "Hull strength: " + item.Hull + "\n"; }
            if (item.Offense != 0) { detailsBodyLoot.Text += "Offense: " + item.Offense + "\n"; }
            if (item.Power != 0) { detailsBodyLoot.Text += "Power: " + item.Power + "GW\n"; }
            if (item.Recharge != 0) { detailsBodyLoot.Text += "Recharge capacity: " + item.Recharge + "GW\n"; }
            if (item.Repair != 0) { detailsBodyLoot.Text += "Repair: " + item.Repair + "\n"; }
            if (item.Shield != 0) { detailsBodyLoot.Text += "Shield: " + item.Shield + "\n"; }
            if (item.Thrust != 0) { detailsBodyLoot.Text += "Thrust: " + item.Thrust + "\n"; }
            if (item.Weapons != 0) { detailsBodyLoot.Text += "Weapons slots: " + item.Weapons + "\n"; }
        }
    }
}