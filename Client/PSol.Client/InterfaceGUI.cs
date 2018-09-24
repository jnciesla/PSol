using System;
using GeonBit.UI.Entities;
using GeonBit.UI;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Bindings;
using Microsoft.Xna.Framework.Input;
using PSol.Data.Models;

namespace PSol.Client
{
    internal class InterfaceGUI
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
        private static Panel invPanel;
        private static Texture2D equipImage;
        private static Panel hoverPanel;
        private static Paragraph detailsHeader;
        private static Paragraph detailsBody;
        private static Image detailsImage;
        private static readonly Image[] slot = new Image[60];
        private static readonly Rectangle[] slotBounds = new Rectangle[60];

        // IGUI textures
        private static Texture2D closeIcon;
        private static Texture2D plusIcon;
        private static Texture2D minusIcon;
        private Texture2D mapPanel;
        private Texture2D inventoryPanel;
        private Texture2D registerPanel;
        private Texture2D loginPanel;
        private Texture2D exitPanel;
        private Texture2D button;
        private Texture2D button_hover;
        private Texture2D button_down;


        private const string mask = "*";

        public void InitializeGUI(ContentManager content)
        {
            // Initialize custom graphics
            loginPanel = content.Load<Texture2D>("Panels/Login");
            registerPanel = content.Load<Texture2D>("Panels/Register");
            exitPanel = content.Load<Texture2D>("Panels/exit");
            inventoryPanel = content.Load<Texture2D>("Panels/Inventory");
            button = content.Load<Texture2D>("Panels/button_default");
            button_hover = content.Load<Texture2D>("Panels/button_default_hover");
            button_down = content.Load<Texture2D>("Panels/button_default_down");
            mapPanel = content.Load<Texture2D>("Panels/Map");
            closeIcon = content.Load<Texture2D>("Panels/closeIco");
            plusIcon = content.Load<Texture2D>("Panels/plusIco");
            minusIcon = content.Load<Texture2D>("Panels/minusIco");
            equipImage = content.Load<Texture2D>("Panels/Equipment");

            mapLine = new Paragraph[10];

            CreateChats();
            CreateWindow_Login();
            CreateWindow_Register();
            CreateMessage();
            CreateMap();
            CreateInventory();
            CreateWindow_Exit();
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
                        AddChats(GameLogic.LocalMobs.Count + " hostiles detected in the immediate vicinity.", Color.BurlyWood);
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

                        if (!resultX || !resultY)
                        {
                            AddChats(@"Invalid input to navigation computer", Color.DarkGoldenrod);
                            return;
                        }

                        AddChats(@"Navigating to " + X + "," + Y, Color.BurlyWood);
                        GameLogic.Destination = new Vector2(X, Y);
                        GameLogic.Navigating = true;
                    }
                    else
                    {
                        ctcp.SendChat(messageText.Value);
                    }
                    messageText.Value = "";
                    MenuManager.Clear(3);
                }
                else
                {
                    MenuManager.Clear(3);
                }
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
            var panel = new Panel(new Vector2(500, 430), PanelSkin.None);
            var image = new Image(loginPanel, new Vector2(496, 427), ImageDrawMode.Panel, Anchor.TopLeft, new Vector2(-30, -29));
            var loginButton = new Image(button, new Vector2(400, 60), ImageDrawMode.Panel, Anchor.AutoCenter);
            var loginLabel = new Paragraph("LOGIN", Anchor.AutoCenter, null, new Vector2(0, -50)) { FillColor = Color.DarkGray };
            txtUser = new TextInput(false, Anchor.TopCenter, new Vector2(0, 40)) { Skin = PanelSkin.None };
            txtUser.Validators.Add(new Validators.AlphaNumValidator());
            txtPass = new TextInput(false, Anchor.Auto, new Vector2(0, 38))
            {
                HideInputWithChar = mask.ToCharArray()[0],
                Skin = PanelSkin.None
            };
            txtPass.Validators.Add(new Validators.AlphaNumValidator());
            var lblRegister = new Label("No account?  Register here", Anchor.AutoCenter, null, new Vector2(0, 25));

            lblStatus = new MulticolorParagraph("Server status:{{RED}} offline", Anchor.BottomLeft);
            UserInterface.Active.AddEntity(panel);

            // Entity Settings
            txtUser.PlaceholderText = "Enter username";
            txtPass.PlaceholderText = "Enter password";

            // Add Entities
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

            closeBtn.OnMouseEnter += (closeEnter) => { UserInterface.Active.SetCursor(Graphics.Cursors[2], 32, new Point(-4, 0)); };
            closeBtn.OnMouseLeave += (closeLeave) => { UserInterface.Active.SetCursor(CursorType.Default); };
            closeBtn.OnClick += (closeClick) => { MenuManager.Clear(6); };

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
            var panel = new Panel(new Vector2(500, 430));
            var image = new Image(registerPanel, new Vector2(496, 427), ImageDrawMode.Panel, Anchor.TopLeft, new Vector2(-30, -29));
            var backButton = new Image(button, new Vector2(210, 60), ImageDrawMode.Panel, Anchor.AutoInline);
            var registerButton = new Image(button, new Vector2(210, 60), ImageDrawMode.Panel, Anchor.AutoInline, new Vector2(20, 0));
            var backLabel = new Paragraph("BACK", Anchor.AutoInline, null, new Vector2(80, -36)) { FillColor = Color.DarkGray };
            var registerLabel = new Paragraph("REGISTER", Anchor.AutoInline, null, new Vector2(290, -50)) { FillColor = Color.DarkGray };
            txtUserReg = new TextInput(false, Anchor.TopCenter, new Vector2(0, 22)) { Skin = PanelSkin.None };
            txtUserReg.Validators.Add(new Validators.AlphaNumValidator());
            txtPassReg = new TextInput(false, Anchor.AutoCenter, new Vector2(0, 40)) { Skin = PanelSkin.None };
            txtPassReg.Validators.Add(new Validators.AlphaNumValidator());
            txtPassReg.HideInputWithChar = mask.ToCharArray()[0];
            txtPas2Reg = new TextInput(false, Anchor.AutoCenter, new Vector2(0, 36)) { Skin = PanelSkin.None };
            txtPas2Reg.Validators.Add(new Validators.AlphaNumValidator());
            txtPas2Reg.HideInputWithChar = mask.ToCharArray()[0];
            UserInterface.Active.AddEntity(panel);

            panel.Skin = PanelSkin.None;

            // Entity Settings
            txtUserReg.PlaceholderText = "Enter username";
            txtPassReg.PlaceholderText = "Enter password";
            txtPas2Reg.PlaceholderText = "Confirm password";

            // Add Entities
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
            var panel = new Panel(new Vector2(Globals.PreferredBackBufferWidth * .5f, 250), PanelSkin.None, Anchor.TopLeft);
            UserInterface.Active.AddEntity(panel);

            panel.PanelOverflowBehavior = PanelOverflowBehavior.VerticalScroll;
            panel.OnMouseEnter = (chatOver) =>
            {
                panel.Scrollbar.Opacity = 175;
                Globals.pauseChat = true;
            };
            panel.OnMouseLeave = (chatOver) =>
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
                FontOverride = Globals.Font10
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
            var image = new Image(inventoryPanel, new Vector2(600, 400), ImageDrawMode.Panel, Anchor.TopLeft);
            hoverPanel = new Panel(new Vector2(300, 100), PanelSkin.Default, Anchor.TopLeft) { Opacity = 240, Visible = false };
            detailsImage = new Image(Graphics.Objects[0], new Vector2(64, 64), ImageDrawMode.Stretch, Anchor.TopLeft, new Vector2(79, 35)) { Visible = false };
            detailsHeader = new Paragraph("", Anchor.TopLeft, Color.DarkGray, null, new Vector2(200, 10), new Vector2(10, 99))
            { FontOverride = Globals.Font12, OutlineOpacity = 0, AlignToCenter = true, WrapWords = false };
            detailsBody = new MulticolorParagraph("", Anchor.TopLeft, Color.DarkGray, null, new Vector2(200, 10), new Vector2(10, 114))
            { FontOverride = Globals.Font10, OutlineOpacity = 0, AlignToCenter = true };
            var closeBtn = new Image(closeIcon, new Vector2(15, 15), ImageDrawMode.Stretch, Anchor.TopRight);

            var itemName = new Label("", Anchor.TopRight);

            detailsBody.WrapWords = true;

            UserInterface.Active.AddEntity(invPanel);
            invPanel.AddChild(image);

            invPanel.AddChild(closeBtn);
            invPanel.AddChild(itemName);
            invPanel.AddChild(detailsHeader);
            invPanel.AddChild(detailsBody);
            invPanel.AddChild(detailsImage);
            invPanel.AddChild(hoverPanel);

            invPanel.OnMouseLeave += (panelLeave) => { hoverPanel.Visible = false; };
            closeBtn.OnMouseEnter += (closeEnter) => { UserInterface.Active.SetCursor(Graphics.Cursors[2], 32, new Point(-4, 0)); };
            closeBtn.OnMouseLeave += (closeLeave) => { UserInterface.Active.SetCursor(CursorType.Default); };
            closeBtn.OnClick += (closeClick) => { MenuManager.Clear(5); };

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

        public void PopulateInventory(int type)
        {
            //detailsBody.Text = "";
            //detailsImage.Visible = false;
            //detailsHeader.Text = "";

            var P = Types.Player[GameLogic.PlayerIndex];
            if (type == 0)
            {
                var par = new Paragraph[15];
                var img = new Image[7];
                img[0] = new Image(equipImage, new Vector2(340, 80), ImageDrawMode.Panel, Anchor.TopRight, new Vector2(27, 35));                                    // Equipment background
                img[1] = new Image(Graphics.Characters[1], new Vector2(64, 64), ImageDrawMode.Panel, Anchor.TopRight, new Vector2(195, 38));                        // Hull
                par[3] = new Paragraph("Flight Computer", Anchor.TopRight, Color.DarkGray, null, null, new Vector2(50, 35));                                        // FCA
                par[5] = new Paragraph("Shield Generator", Anchor.TopRight, Color.DarkGray, null, null, new Vector2(50, 52));                                       // SGA
                par[6] = new Paragraph("Hull Plating", Anchor.TopRight, Color.DarkGray, null, null, new Vector2(50, 69));                                           // SPA
                par[2] = new Paragraph("Propulsion Drive", Anchor.TopRight, Color.DarkGray, null, null, new Vector2(50, 86));                                       // PDS
                par[4] = new Paragraph("Auxiliary Payload", Anchor.TopRight, Color.DarkGray, null, null, new Vector2(50, 103));                                     // APy
                par[0] = new Paragraph("Weapons:", Anchor.TopRight, Color.DarkGray, null, null, new Vector2(308, 35));                                              // Wps
                par[1] = new Paragraph("Ammunition:", Anchor.TopRight, Color.DarkGray, null, null, new Vector2(288, 66));                                           // AMO
                par[7] = new Paragraph("1", Anchor.TopRight, Color.DarkGray, null, null, new Vector2(352, 52));                                                     // Mn1
                par[8] = new Paragraph("2", Anchor.TopRight, Color.DarkGray, null, null, new Vector2(334, 52));                                                     // Mn2
                par[9] = new Paragraph("3", Anchor.TopRight, Color.DarkGray, null, null, new Vector2(318, 52));                                                     // Mn3
                par[10] = new Paragraph("4", Anchor.TopRight, Color.DarkGray, null, null, new Vector2(301, 52));                                                    // Mn4
                par[11] = new Paragraph("5", Anchor.TopRight, Color.DarkGray, null, null, new Vector2(284, 52));                                                    // Mn5
                par[12] = new Paragraph("", Anchor.TopRight, Color.DarkGray, null, null, new Vector2(283, 98));                                                     // Ss1
                par[13] = new Paragraph("", Anchor.TopRight, Color.DarkGray, null, null, new Vector2(311, 98));                                                     // Ss2
                par[14] = new Paragraph("", Anchor.TopRight, Color.DarkGray, null, null, new Vector2(339, 98));                                                     // Ss3
                img[2] = new Image(Graphics.diamond, new Vector2(12, 12), ImageDrawMode.Stretch, Anchor.TopRight, new Vector2(30, 35)) { FillColor = Color.Red };   // FCA stat
                img[3] = new Image(Graphics.diamond, new Vector2(12, 12), ImageDrawMode.Stretch, Anchor.TopRight, new Vector2(30, 52)) { FillColor = Color.Red };   // SGA stat
                img[4] = new Image(Graphics.diamond, new Vector2(12, 12), ImageDrawMode.Stretch, Anchor.TopRight, new Vector2(30, 69)) { FillColor = Color.Red };   // SPA stat
                img[5] = new Image(Graphics.diamond, new Vector2(12, 12), ImageDrawMode.Stretch, Anchor.TopRight, new Vector2(30, 86)) { FillColor = Color.Red };   // PDS stat
                img[6] = new Image(Graphics.diamond, new Vector2(12, 12), ImageDrawMode.Stretch, Anchor.TopRight, new Vector2(30, 103)) { FillColor = Color.Red };  // APy stat

                for (var i = 0; i < 7; i++)
                    invPanel.AddChild(img[i]);
                if (P.Inventory.FirstOrDefault(I => I.Slot == 3)?.Id != null) { img[2].FillColor = Color.DarkGreen; }
                if (P.Inventory.FirstOrDefault(I => I.Slot == 5)?.Id != null) { img[3].FillColor = Color.DarkGreen; }
                if (P.Inventory.FirstOrDefault(I => I.Slot == 6)?.Id != null) { img[4].FillColor = Color.DarkGreen; }
                if (P.Inventory.FirstOrDefault(I => I.Slot == 2)?.Id != null) { img[5].FillColor = Color.DarkGreen; }
                if (P.Inventory.FirstOrDefault(I => I.Slot == 4)?.Id != null) { img[6].FillColor = Color.DarkGreen; }

                par[12].Text = P.Inventory.FirstOrDefault(I => I.Slot == 12)?.Quantity.ToString() ?? "000";
                par[13].Text = P.Inventory.FirstOrDefault(I => I.Slot == 13)?.Quantity.ToString() ?? "000";
                par[14].Text = P.Inventory.FirstOrDefault(I => I.Slot == 14)?.Quantity.ToString() ?? "000";

                for (var i = 0; i < 15; i++)
                {
                    var n = i; // Access to modified closure
                    par[i].FontOverride = Globals.Font8;
                    par[i].OutlineOpacity = 0;
                    invPanel.AddChild(par[i]);
                    if (n < 2) continue;
                    par[n].OnMouseEnter += (parEnter) =>
                    {
                        UserInterface.Active.SetCursor(Graphics.Cursors[2], 32, new Point(-4, 0));
                        par[n].FillColor = Color.WhiteSmoke;
                    };
                    par[n].OnMouseLeave += (parLeave) =>
                    {
                        UserInterface.Active.SetCursor(CursorType.Default);
                        par[n].FillColor = Color.DarkGray;
                    };
                    par[n].OnClick += (parClick) =>
                    {
                        var temp = GameLogic.Items.FirstOrDefault(pI => pI.Id == P.Inventory.FirstOrDefault(I => I.Slot == n)?.ItemId);
                        if (temp != null)
                        {
                            DisplayDetails(temp);
                        }
                        else
                        {
                            detailsHeader.Text = "";
                            detailsBody.Text = "{{RED}}No equipment installed";
                        }
                    };
                }
                img[1].OnMouseEnter += (parEnter) =>
                {
                    UserInterface.Active.SetCursor(Graphics.Cursors[2], 32, new Point(-4, 0));
                };
                img[1].OnMouseLeave += (parLeave) =>
                {
                    UserInterface.Active.SetCursor(CursorType.Default);
                };
                img[1].OnClick += (parClick) =>
                {
                    var temp = GameLogic.Items.FirstOrDefault(pI => pI.Id == P.Inventory.FirstOrDefault(I => I.Slot == 1)?.ItemId);
                    if (temp != null)
                    {
                        DisplayDetails(temp);
                    }
                    else
                    {
                        detailsHeader.Text = "";
                        detailsBody.Text = "{{RED}}No equipment installed";
                    }
                };
            }

            ArrangeInventory();
        }

        public void ArrangeInventory()
        {
            foreach (var invItem in Types.Player[GameLogic.PlayerIndex].Inventory)
            {
                if (invItem.Slot < 101) continue;

                var SLOT = invItem.Slot - 101;
                var ITEM = GameLogic.Items.FirstOrDefault(i => i.Id == invItem.ItemId);

                if (invPanel.GetChildren().Contains(slot[SLOT]))
                    invPanel.RemoveChild(slot[SLOT]);
                slot[SLOT] = new Image(Graphics.Objects[ITEM?.Image ?? 0], new Vector2(32, 32), ImageDrawMode.Stretch, Anchor.TopLeft,
                    new Vector2(slotBounds[SLOT].X, slotBounds[SLOT].Y))
                { Draggable = true };
                invPanel.AddChild(slot[SLOT]);

                for (var i = 0; i < 60; i++)
                {
                    if (!invPanel.GetChildren().Contains(slot[i])) continue;
                    var z = i;
                    var dropTest = false;
                    slot[i].OnStopDrag += (entity) =>
                    {
                        for (var n = 0; n < 60; n++)
                        {
                            if (slot[z].GetRelativeOffset().X + 16 >= slotBounds[n].X &&
                                slot[z].GetRelativeOffset().X + 16 <= slotBounds[n].X + 32 &&
                                slot[z].GetRelativeOffset().Y + 16 >= slotBounds[n].Y &&
                                slot[z].GetRelativeOffset().Y + 16 <= slotBounds[n].Y + 32)
                            {
                                dropTest = true;
                                invPanel.RemoveChild(slot[z]);

                                if (Types.Player != null)
                                {
                                    Inventory first = Types.Player[GameLogic.PlayerIndex].Inventory.FirstOrDefault(unknown => unknown.Id == invItem.Id);

                                    if (first != null) first.Slot = 101 + n;
                                }

                                ctcp.UpdateInventory();
                                ArrangeInventory();
                                break;
                            }
                        }

                        if (dropTest) return;
                        invPanel.RemoveChild(slot[z]);
                        ArrangeInventory();
                    };
                    slot[i].OnClick += (entity) =>
                    {
                        DisplayDetails(ITEM);
                    };
                }
            }
        }

        public Panel GenerateItem(string ID, int quantity, int type)
        {
            int qty = 0, BUY = 1, SELL = 2, max;
            var temp = GameLogic.Items.FirstOrDefault(i => i.Id == ID);
            string tempName;

            var item = new Panel(new Vector2(180, 64), PanelSkin.Default, Anchor.Auto) { OutlineColor = Color.Black };
            var itemImage = new Image(Graphics.Objects[temp?.Image ?? 0], new Vector2(32, 32), ImageDrawMode.Stretch, Anchor.TopLeft, new Vector2(-26, -16));
            if (temp?.Name.Length > 16)
            {
                tempName = temp.Name.Substring(0, 13) + "...";
            }
            else
            {
                tempName = temp?.Name ?? "NULL";
            }
            item.AddChild(new Paragraph(tempName, Anchor.TopLeft, Color.DarkGoldenrod, null, null, new Vector2(12, -28)) { FontOverride = Globals.Font10, WrapWords = false });
            item.AddChild(new Paragraph(temp?.Type ?? "NULL", Anchor.Auto, Color.AntiqueWhite, null, null, new Vector2(12, -8)) { FontOverride = Globals.Font10 });
            max = quantity;                 // Quantity in inventory for sell or other
            if (type == BUY) { max = 255; } // Allow user to buy 255 of anything (right now)
            if (type == BUY || type == SELL)
            {
                item.AddChild(
                    new Paragraph("$" + temp?.Cost, Anchor.Auto, Color.DimGray, null, null, new Vector2(12, -8))
                    {
                        FontOverride = Globals.Font14
                    });
            }
            else
            {
                item.AddChild(
                    new Paragraph(" " + quantity, Anchor.Auto, Color.DimGray, null, null, new Vector2(12, -8))
                    {
                        FontOverride = Globals.Font14
                    });
            }

            var qtyStr = new Paragraph(qty.ToString(), Anchor.BottomRight, Color.WhiteSmoke, null, new Vector2(18, 8), new Vector2(-7, -17)) { FontOverride = Globals.Font8, AlignToCenter = true };
            var decBtn = new Image(minusIcon, new Vector2(15, 15), ImageDrawMode.Stretch, Anchor.BottomRight, new Vector2(15, -17));
            var incBtn = new Image(plusIcon, new Vector2(15, 15), ImageDrawMode.Stretch, Anchor.BottomRight, new Vector2(-25, -17));

            item.AddChild(qtyStr);
            item.AddChild(decBtn);
            item.AddChild(incBtn);
            item.AddChild(itemImage);

            itemImage.WhileMouseHover += (imageHover) =>
            {
                hoverPanel.Visible = true;
                var ms = Mouse.GetState();
                var x = ms.X - invPanel.GetRelativeOffset().X;
                var y = ms.Y - invPanel.GetRelativeOffset().Y;
                hoverPanel.SetOffset(new Vector2(x - 150, y));

            };
            itemImage.OnMouseEnter += (imageEnter) => { UserInterface.Active.SetCursor(Graphics.Cursors[2], 32, new Point(-4, 0)); };
            itemImage.OnMouseLeave += (imageLeave) =>
            {
                UserInterface.Active.SetCursor(CursorType.Default);
                hoverPanel.Visible = false;
            };
            itemImage.OnClick += (imageClick) =>
            {
                /*                for (var n = 0; n < itemList.GetChildren().Count; n++)
                                {
                                    itemList.GetChildren()[n].OutlineWidth = 0;
                                }*/
                item.OutlineWidth = 2;
                DisplayDetails(temp);
            };
            decBtn.OnMouseEnter += (decEnter) => { UserInterface.Active.SetCursor(Graphics.Cursors[2], 32, new Point(-4, 0)); };
            decBtn.OnMouseLeave += (decLeave) => { UserInterface.Active.SetCursor(CursorType.Default); };
            decBtn.OnClick += (decClick) =>
            {
                if (qty <= 0) return;
                if (Globals.Control)
                {
                    qty -= 10;
                    if (qty < 0)
                        qty = 0;
                }
                else if (Globals.Alt)
                {
                    qty -= 100;
                    if (qty < 0)
                        qty = 0;
                }
                else if (Globals.Shift)
                {
                    qty = 0;
                }
                else
                {
                    qty--;
                }
                qtyStr.Text = qty.ToString();
            };
            incBtn.OnMouseEnter += (incEnter) => { UserInterface.Active.SetCursor(Graphics.Cursors[2], 32, new Point(-4, 0)); };
            incBtn.OnMouseLeave += (incLeave) => { UserInterface.Active.SetCursor(CursorType.Default); };
            incBtn.OnClick += (incClick) =>
            {
                if (qty >= max) return;
                if (Globals.Control)
                {
                    qty += 10;
                    if (qty > max)
                        qty = max;
                }
                else if (Globals.Alt)
                {
                    qty += 100;
                    if (qty > max)
                        qty = max;
                }
                else if (Globals.Shift)
                {
                    qty = max;
                }
                else
                {
                    qty++;
                }
                qtyStr.Text = qty.ToString();
            };
            return item;
        }

        public static void PopulateMap()
        {

            const float scale = (float)500 / Constants.PLAY_AREA_WIDTH;
            var closeBtn = new Image(closeIcon, new Vector2(15, 15), ImageDrawMode.Stretch, Anchor.TopRight, new Vector2(-30, -29));
            starDetail = new Image(Graphics.Planets[0], new Vector2(175, 175), ImageDrawMode.Stretch, Anchor.TopRight, new Vector2(40, 10))
            {
                Disabled = true,
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
                image.OnMouseEnter += (starEnter) => { starLabel.Text = GameLogic.Galaxy[n].Name; UserInterface.Active.SetCursor(Graphics.Cursors[2], 32, new Point(-4, 0)); };
                image.OnMouseLeave += (starLeave) => { starLabel.Text = ""; UserInterface.Active.SetCursor(CursorType.Default); };
                image.OnClick += (starClick) =>
                {
                    GameLogic.selectedMapItem = n;
                    MapDetail(n);
                };
            }
            galaxyMap.AddChild(closeBtn);
            closeBtn.OnMouseEnter += (closeEnter) => { UserInterface.Active.SetCursor(Graphics.Cursors[2], 32, new Point(-4, 0)); };
            closeBtn.OnMouseLeave += (closeLeave) => { UserInterface.Active.SetCursor(CursorType.Default); };
            closeBtn.OnClick += (closeClick) => { MenuManager.Clear(4); };

        }

        private static void MapDetail(int n)
        {
            starDetail.Texture = Graphics.Planets[4];
            mapLine[0].Text = "Name: " + GameLogic.Galaxy[n].Name;
            mapLine[1].Text = "Classification: " + GameLogic.Galaxy[n].Class;
            mapLine[2].Text = "Coordinates: " + GameLogic.Galaxy[n].X / 100 + ":" + GameLogic.Galaxy[n].Y / 100;
            mapLine[3].Text = "Belligerence: " + GameLogic.Galaxy[n].Belligerence;
            for (int index = 5; index < 10; index++)
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
                int j = 5;
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

        public void Update()
        {
            if (Windows[4].Visible)
            {
                const float scale = (float)500 / Constants.PLAY_AREA_WIDTH;
                mapPlayer.SetOffset(new Vector2(Types.Player[GameLogic.PlayerIndex].X * scale, Types.Player[GameLogic.PlayerIndex].Y * scale));
            }
        }

        public void DisplayDetails(Item item)
        {
            detailsImage.Texture = Graphics.Objects[item?.Image ?? 0];
            detailsImage.Visible = true;
            detailsHeader.Text = item?.Name + "\n";
            detailsHeader.Text += item?.Type + "\n";
            detailsHeader.Text += "Level: " + item?.Level;
            detailsBody.Text = item?.Description + "\n";
            if (item?.Armor != 0) { detailsBody.Text += "Armor: " + item?.Armor + "\n"; }
            if (item?.Damage != 0) { detailsBody.Text += "Damage: " + item?.Damage + "\n"; }
            if (item?.Defense != 0) { detailsBody.Text += "Defense: " + item?.Defense + "\n"; }
            if (item?.Hull != 0) { detailsBody.Text += "Hull strength: " + item?.Hull + "\n"; }
            if (item?.Offense != 0) { detailsBody.Text += "Offense: " + item?.Offense + "\n"; }
            if (item?.Power != 0) { detailsBody.Text += "Power: " + item?.Power + "GW\n"; }
            if (item?.Recharge != 0) { detailsBody.Text += "Recharge capacity: " + item?.Recharge + "GW\n"; }
            if (item?.Repair != 0) { detailsBody.Text += "Repair: " + item?.Repair + "\n"; }
            if (item?.Shield != 0) { detailsBody.Text += "Shield: " + item?.Shield + "\n"; }
            if (item?.Thrust != 0) { detailsBody.Text += "Thrust: " + item?.Thrust + "\n"; }
            if (item?.Weapons != 0) { detailsBody.Text += "Weapons slots: " + item?.Weapons + "\n"; }
        }
    }
}