using GeonBit.UI.Entities;
using GeonBit.UI;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using GeonBit.UI.Utils;

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
        public Button btnLogin;
        public bool openBox;

        private const string mask = "*";

        public void InitializeGUI()
        {
            CreateChats();
            CreateWindow_Login();
            CreateWindow_Register();
            CreateMessage();
            // UserInterface.Active.GlobalScale = .75F;
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
                    else
                    {
                        ctcp.SendChat(messageText.Value);
                        messageText.Value = "";
                    }
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
                if (openBox == false)
                {
                    openBox = true;
                    MessageBox.ShowMsgBox("No credentials",
                        "Please enter a valid username and password before logging in!", new[]
                        {
                            new MessageBox.MsgBoxOption("Okay", () =>
                            {
                                openBox = false;
                                return true;
                            })
                        });
                }
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
                MessageBox.ShowMsgBox("No credentials", "Please enter a valid username and password, and confirm your password, before attempting to register!", new MessageBox.MsgBoxOption[]
                {
                        new MessageBox.MsgBoxOption("Okay" ,() => true)
                });
            }
            else
            {
                if (Globals.registerPassword != Globals.registerValidate)
                {
                    MessageBox.ShowMsgBox("Passwords do not match", "The passwords you have entered to not match.  Please try again.", new MessageBox.MsgBoxOption[]
                    {
                        new MessageBox.MsgBoxOption("Okay" ,() => true)
                    });
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
            btnLogin = new Button("Login");
            txtUser = new TextInput(false);
            txtUser.Validators.Add(new Validators.AlphaNumValidator());
            Header headerUser = new Header("Username", Anchor.TopCenter);
            txtPass = new TextInput(false);
            txtPass.HideInputWithChar = mask.ToCharArray()[0];
            txtPass.Validators.Add(new Validators.AlphaNumValidator());
            Header headerPass = new Header("Password", Anchor.AutoCenter);
            Label lblRegister = new Label("No account?  Register here", Anchor.AutoCenter);
            lblStatus = new MulticolorParagraph("Server status:{{RED}} offline", Anchor.BottomLeft);
            UserInterface.Active.AddEntity(panel);

            // Entity Settings
            txtUser.PlaceholderText = "Enter username";
            txtPass.PlaceholderText = "Enter password";

            // Add Entities
            panel.AddChild(headerUser);
            panel.AddChild(txtUser);
            panel.AddChild(headerPass);
            panel.AddChild(txtPass);
            panel.AddChild(btnLogin);
            panel.AddChild(lblRegister);
            panel.AddChild(lblStatus);

            // MouseEvents
            lblRegister.OnMouseEnter += entity => { lblRegister.FillColor = Color.Red; UserInterface.Active.SetCursor(CursorType.Pointer); };
            lblRegister.OnMouseLeave += entity => { lblRegister.FillColor = Color.White; UserInterface.Active.SetCursor(CursorType.Default); };

            txtUser.OnMouseEnter += entity => { UserInterface.Active.SetCursor(CursorType.IBeam); };
            txtUser.OnMouseLeave += entity => { UserInterface.Active.SetCursor(CursorType.Default); };

            txtPass.OnMouseEnter += entity => { UserInterface.Active.SetCursor(CursorType.IBeam); };
            txtPass.OnMouseLeave += entity => { UserInterface.Active.SetCursor(CursorType.Default); };

            btnLogin.OnMouseEnter += entity => { UserInterface.Active.SetCursor(CursorType.Pointer); };
            btnLogin.OnMouseLeave += entity => { UserInterface.Active.SetCursor(CursorType.Default); };

            lblRegister.OnClick += entity =>
            {
                MenuManager.ChangeMenu(MenuManager.Menu.Register);
            };

            btnLogin.OnClick += entity =>
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
            Panel panel = new Panel(new Vector2(500, 550));
            Button btnRegister = new Button("Register");
            Button btnBack = new Button("Back");
            txtUserReg = new TextInput(false);
            txtUserReg.Validators.Add(new Validators.AlphaNumValidator());
            Header headerUser = new Header("Username", Anchor.TopCenter);
            txtPassReg = new TextInput(false);
            txtPassReg.Validators.Add(new Validators.AlphaNumValidator());
            txtPassReg.HideInputWithChar = mask.ToCharArray()[0];
            Header headerPass = new Header("Password", Anchor.AutoCenter);
            txtPas2Reg = new TextInput(false);
            txtPas2Reg.Validators.Add(new Validators.AlphaNumValidator());
            txtPas2Reg.HideInputWithChar = mask.ToCharArray()[0];
            Header headerPass2 = new Header("Confirm password", Anchor.AutoCenter);
            UserInterface.Active.AddEntity(panel);

            // Entity Settings
            txtUserReg.PlaceholderText = "Enter username";
            txtPassReg.PlaceholderText = "Enter password";
            txtPas2Reg.PlaceholderText = "Confirm password";

            // Add Entities
            panel.AddChild(headerUser);
            panel.AddChild(txtUserReg);
            panel.AddChild(headerPass);
            panel.AddChild(txtPassReg);
            panel.AddChild(headerPass2);
            panel.AddChild(txtPas2Reg);
            panel.AddChild(btnRegister);
            panel.AddChild(btnBack);

            // MouseEvents
            txtUserReg.OnMouseEnter += entity => { UserInterface.Active.SetCursor(CursorType.IBeam); };
            txtUserReg.OnMouseLeave += entity => { UserInterface.Active.SetCursor(CursorType.Default); };

            txtPassReg.OnMouseEnter += entity => { UserInterface.Active.SetCursor(CursorType.IBeam); };
            txtPassReg.OnMouseLeave += entity => { UserInterface.Active.SetCursor(CursorType.Default); };

            txtPas2Reg.OnMouseEnter += entity => { UserInterface.Active.SetCursor(CursorType.IBeam); };
            txtPas2Reg.OnMouseLeave += entity => { UserInterface.Active.SetCursor(CursorType.Default); };

            btnRegister.OnMouseEnter += entity => { UserInterface.Active.SetCursor(CursorType.Pointer); };
            btnRegister.OnMouseLeave += entity => { UserInterface.Active.SetCursor(CursorType.Default); };

            btnBack.OnMouseEnter += entity => { UserInterface.Active.SetCursor(CursorType.Pointer); };
            btnBack.OnMouseLeave += entity => { UserInterface.Active.SetCursor(CursorType.Default); };

            btnBack.OnClick += entity =>
            {
                MenuManager.ChangeMenu(MenuManager.Menu.Login);
            };
            btnRegister.OnClick += entity =>
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
            Panel panel = new Panel(new Vector2(500, 250), PanelSkin.None, Anchor.TopLeft);
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
                FontOverride = Globals.Font8
            };
            Globals.chatPanel.AddChild(para);
            if (!Globals.pauseChat)
            {
                Globals.chatPanel.Scrollbar.Value = (int)Globals.chatPanel.Scrollbar.Max;
            }
        }

        public void CreateMessage()
        {
            Panel panel = new Panel(new Vector2(1024, 50), PanelSkin.None, Anchor.BottomLeft, new Vector2(-10, 40));
            messageText = new TextInput(false) {Skin = PanelSkin.None};
            messageText.TextParagraph.FontOverride = Globals.Font10;
            UserInterface.Active.AddEntity(panel);
            panel.AddChild(messageText);
            CreateWindow(panel);
        }
    }
}
