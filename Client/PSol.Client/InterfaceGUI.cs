using System;
using GeonBit.UI.Entities;
using GeonBit.UI;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

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

        // IGUI textures
        private Texture2D registerPanel;
        private Texture2D loginPanel;
        private Texture2D button;
        private Texture2D button_hover;
        private Texture2D button_down;

        private const string mask = "*";

        public void InitializeGUI(ContentManager content)
        {
            // Initialize custom graphics
            loginPanel = content.Load<Texture2D>("Panels/Login");
            registerPanel = content.Load<Texture2D>("Panels/Register");
            button = content.Load<Texture2D>("Panels/button_default");
            button_hover = content.Load<Texture2D>("Panels/button_default_hover");
            button_down = content.Load<Texture2D>("Panels/button_default_down");

            CreateChats();
            CreateWindow_Login();
            CreateWindow_Register();
            CreateMessage();
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

            if (Windows[2].Visible) // Tab through register window when visible
            {
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
                    else if (messageText.Value.ToLower() == "/scan" || messageText.Value.ToLower() == "/scanner")
                    {
                        Globals.scanner = !Globals.scanner;
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
            Panel panel = new Panel(new Vector2(500, 430));
            Image image = new Image(loginPanel, new Vector2(496, 427), ImageDrawMode.Panel, Anchor.TopLeft, new Vector2(-30, -29));
            Image loginButton = new Image(button, new Vector2(400, 60), ImageDrawMode.Panel, Anchor.AutoCenter);
            txtUser = new TextInput(false, Anchor.TopCenter, new Vector2(0, 40)) { Skin = PanelSkin.None };
            txtUser.Validators.Add(new Validators.AlphaNumValidator());
            txtPass = new TextInput(false, Anchor.Auto, new Vector2(0, 38))
            {
                HideInputWithChar = mask.ToCharArray()[0],
                Skin = PanelSkin.None
            };
            txtPass.Validators.Add(new Validators.AlphaNumValidator());
            Label lblRegister = new Label("No account?  Register here", Anchor.AutoCenter, null, new Vector2(0, 25));
            Paragraph loginLabel = new Paragraph("LOGIN", Anchor.AutoCenter, null, new Vector2(0, -50)) { FillColor = Color.DarkGray };
            lblStatus = new MulticolorParagraph("Server status:{{RED}} offline", Anchor.BottomLeft);
            UserInterface.Active.AddEntity(panel);

            panel.Skin = PanelSkin.None;

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

            loginLabel.OnClick += entity => { Login(); };

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

        public void CreateWindow_Register()
        {
            //  Create Entities
            Panel panel = new Panel(new Vector2(500, 430));
            Image image = new Image(registerPanel, new Vector2(496, 427), ImageDrawMode.Panel, Anchor.TopLeft, new Vector2(-30, -29));
            Image backButton = new Image(button, new Vector2(210, 60), ImageDrawMode.Panel, Anchor.AutoInline);
            Image registerButton = new Image(button, new Vector2(210, 60), ImageDrawMode.Panel, Anchor.AutoInline, new Vector2(20, 0));
            Paragraph backLabel = new Paragraph("BACK", Anchor.AutoInline, null, new Vector2(80, -36)) { FillColor = Color.DarkGray };
            Paragraph registerLabel = new Paragraph("REGISTER", Anchor.AutoInline, null, new Vector2(290, -50)) { FillColor = Color.DarkGray };
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
            backLabel.OnClick += entity =>
            {
                MenuManager.ChangeMenu(MenuManager.Menu.Login);
            };
            registerButton.OnClick += entity =>
            {
                Register();
            };
            registerLabel.OnClick += entity =>
            {
                Register();
            };

            txtUserReg.OnValueChange = textUserReg => { Globals.registerUsername = txtUserReg.Value; };
            txtPassReg.OnValueChange = textPassReg => { Globals.registerPassword = txtPassReg.Value; };
            txtPas2Reg.OnValueChange = textPas2Reg => { Globals.registerValidate = txtPas2Reg.Value; };

            // Create Window
            CreateWindow(panel);
        }

        public void CreateChats()
        {
            //  Create Entities
            Panel panel = new Panel(new Vector2(Globals.PreferredBackBufferWidth * .5f, 250), PanelSkin.None, Anchor.TopLeft);
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
            Panel panel = new Panel(new Vector2(1024, 40), PanelSkin.None, Anchor.BottomLeft, new Vector2(-100, 40));
            messageText = new TextInput(false)
            {
                Skin = PanelSkin.None,
                TextParagraph = { FontOverride = Globals.Font10, BackgroundColor = Color.DarkOliveGreen * .4F, BackgroundColorUseBoxSize = true }
            };
            UserInterface.Active.AddEntity(panel);
            panel.AddChild(messageText);
            CreateWindow(panel);
        }
    }
}
