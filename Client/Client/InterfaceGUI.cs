using GeonBit.UI.Entities;
using GeonBit.UI;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GeonBit.UI.Utils;

namespace Client
{
    class InterfaceGUI
    {
        public static List<Panel> Windows = new List<Panel>();
        private ClientTCP ctcp = new ClientTCP();

        public MulticolorParagraph lblStatus;

        private string mask = "*";

        public void InitializeGUI()
        {
            CreateWindow_Login();
            CreateWindow_Register();
            // UserInterface.Active.GlobalScale = .75F;
        }

        public void CreateWindow(Panel panel)
        {
            Windows.Add(panel);
        }

        public void CreateWindow_Login()
        {
            //  Create Entities
            Panel panel = new Panel(new Vector2(500, 430));
            Button btnLogin = new Button("Login");
            TextInput txtUser = new TextInput(false);
            // txtUser.Validators.Add(new GeonBit.UI.Entities.TextValidators.SlugValidator());
            Header headerUser = new Header("Username", Anchor.TopCenter);
            TextInput txtPass = new TextInput(false);
            txtPass.HideInputWithChar = mask.ToCharArray()[0];
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
            lblRegister.OnMouseEnter += (Entity entity) => { lblRegister.FillColor = Color.Red; UserInterface.Active.SetCursor(CursorType.Pointer); };
            lblRegister.OnMouseLeave += (Entity entity) => { lblRegister.FillColor = Color.White; UserInterface.Active.SetCursor(CursorType.Default); };

            txtUser.OnMouseEnter += (Entity entity) => { UserInterface.Active.SetCursor(CursorType.IBeam); };
            txtUser.OnMouseLeave += (Entity entity) => { UserInterface.Active.SetCursor(CursorType.Default); };

            txtPass.OnMouseEnter += (Entity entity) => { UserInterface.Active.SetCursor(CursorType.IBeam); };
            txtPass.OnMouseLeave += (Entity entity) => { UserInterface.Active.SetCursor(CursorType.Default); };

            btnLogin.OnMouseEnter += (Entity entity) => { UserInterface.Active.SetCursor(CursorType.Pointer); };
            btnLogin.OnMouseLeave += (Entity entity) => { UserInterface.Active.SetCursor(CursorType.Default); };

            lblRegister.OnClick += (Entity entity) =>
            {
                MenuManager.ChangeMenu(MenuManager.Menu.Register);
            };

            btnLogin.OnClick += (Entity entity) =>
            {
                if(Globals.loginUsername == string.Empty || Globals.loginPassword == string.Empty)
                {
                    MessageBox.ShowMsgBox("No credentials", "Please enter a valid username and password before logging in!", new MessageBox.MsgBoxOption[]
                    {
                        new MessageBox.MsgBoxOption("Okay" ,() => {return true; })
                    });
                } else
                {
                    ctcp.SendLogin();
                }
            };

            txtUser.OnValueChange = (Entity textUser) => { Globals.loginUsername = txtUser.Value; };
            txtPass.OnValueChange = (Entity textPass) => { Globals.loginPassword = txtPass.Value; };

            // Create Window
            CreateWindow(panel);
        }

        public void CreateWindow_Register()
        {
            //  Create Entities
            Panel panel = new Panel(new Vector2(500, 550));
            Button btnRegister = new Button("Register");
            Button btnBack = new Button("Back");
            TextInput txtUser = new TextInput(false);
            Header headerUser = new Header("Username", Anchor.TopCenter);
            TextInput txtPass = new TextInput(false);
            txtPass.HideInputWithChar = mask.ToCharArray()[0];
            Header headerPass = new Header("Password", Anchor.AutoCenter);
            TextInput txtPass2 = new TextInput(false);
            txtPass2.HideInputWithChar = mask.ToCharArray()[0];
            Header headerPass2 = new Header("Confirm password", Anchor.AutoCenter);
            UserInterface.Active.AddEntity(panel);

            // Entity Settings
            txtUser.PlaceholderText = "Enter username";
            txtPass.PlaceholderText = "Enter password";
            txtPass2.PlaceholderText = "Confirm password";

            // Add Entities
            panel.AddChild(headerUser);
            panel.AddChild(txtUser);
            panel.AddChild(headerPass);
            panel.AddChild(txtPass);
            panel.AddChild(headerPass2);
            panel.AddChild(txtPass2);
            panel.AddChild(btnRegister);
            panel.AddChild(btnBack);

            // MouseEvents
            txtUser.OnMouseEnter += (Entity entity) => { UserInterface.Active.SetCursor(CursorType.IBeam); };
            txtUser.OnMouseLeave += (Entity entity) => { UserInterface.Active.SetCursor(CursorType.Default); };

            txtPass.OnMouseEnter += (Entity entity) => { UserInterface.Active.SetCursor(CursorType.IBeam); };
            txtPass.OnMouseLeave += (Entity entity) => { UserInterface.Active.SetCursor(CursorType.Default); };

            txtPass2.OnMouseEnter += (Entity entity) => { UserInterface.Active.SetCursor(CursorType.IBeam); };
            txtPass2.OnMouseLeave += (Entity entity) => { UserInterface.Active.SetCursor(CursorType.Default); };

            btnRegister.OnMouseEnter += (Entity entity) => { UserInterface.Active.SetCursor(CursorType.Pointer); };
            btnRegister.OnMouseLeave += (Entity entity) => { UserInterface.Active.SetCursor(CursorType.Default); };

            btnBack.OnMouseEnter += (Entity entity) => { UserInterface.Active.SetCursor(CursorType.Pointer); };
            btnBack.OnMouseLeave += (Entity entity) => { UserInterface.Active.SetCursor(CursorType.Default); };

            btnBack.OnClick += (Entity entity) =>
            {
                MenuManager.ChangeMenu(MenuManager.Menu.Login);
            };
            btnRegister.OnClick += (Entity entity) =>
            {
                if (Globals.registerUsername == string.Empty || Globals.registerPassword == string.Empty || Globals.registerValidate == string.Empty)
                {
                    MessageBox.ShowMsgBox("No credentials", "Please enter a valid username and password, and confirm your password, before attempting to register!", new MessageBox.MsgBoxOption[]
                    {
                        new MessageBox.MsgBoxOption("Okay" ,() => {return true; })
                    });
                }
                else
                {
                    if (Globals.registerPassword != Globals.registerValidate)
                    {
                        MessageBox.ShowMsgBox("Passwords do not match", "The passwords you have entered to not match.  Please try again.", new MessageBox.MsgBoxOption[]
                        {
                        new MessageBox.MsgBoxOption("Okay" ,() => {return true; })
                        });
                    }
                    else
                    {
                        ctcp.SendRegister();
                    }
                }
            };

            txtUser.OnValueChange = (Entity textUser) => { Globals.registerUsername = txtUser.Value; };
            txtPass.OnValueChange = (Entity textPass) => { Globals.registerPassword = txtPass.Value; };
            txtPass2.OnValueChange = (Entity textPass2) => { Globals.registerValidate = txtPass2.Value; };

            // Create Window
            CreateWindow(panel);
        }
    }
}
