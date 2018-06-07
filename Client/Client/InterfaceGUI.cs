using GeonBit.UI.Entities;
using GeonBit.UI;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using GeonBit.UI.Utils;

namespace Client
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
			if (Windows[0].Visible == true) // Tab through login window when visible
			{
				if (txtUser.IsFocused == true)
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

			if (Windows[1].Visible == true) // Tab through register window when visible
			{
				if (txtUserReg.IsFocused == true)
				{
					txtUserReg.IsFocused = false;
					txtPassReg.IsFocused = true;
				}
				else if (txtPassReg.IsFocused == true)
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

			if (Windows[3].Visible)
			{
				if (messageText.Value != "" || messageText.Value != messageText.ValueWhenEmpty)
				{
					// FOR TESTING PURPOSES
					// Send message directly to chats instead of server.  Not hard to implement server-side but I didn't
					// sleep well and I need a nap and I just don't feel like doing it right now
					AddChats(messageText.Value);
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
				Login();
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
			txtUserReg.OnMouseEnter += (Entity entity) => { UserInterface.Active.SetCursor(CursorType.IBeam); };
			txtUserReg.OnMouseLeave += (Entity entity) => { UserInterface.Active.SetCursor(CursorType.Default); };

			txtPassReg.OnMouseEnter += (Entity entity) => { UserInterface.Active.SetCursor(CursorType.IBeam); };
			txtPassReg.OnMouseLeave += (Entity entity) => { UserInterface.Active.SetCursor(CursorType.Default); };

			txtPas2Reg.OnMouseEnter += (Entity entity) => { UserInterface.Active.SetCursor(CursorType.IBeam); };
			txtPas2Reg.OnMouseLeave += (Entity entity) => { UserInterface.Active.SetCursor(CursorType.Default); };

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
				Register();
			};

			txtUserReg.OnValueChange = (Entity textUserReg) => { Globals.registerUsername = txtUserReg.Value; };
			txtPassReg.OnValueChange = (Entity textPassReg) => { Globals.registerPassword = txtPassReg.Value; };
			txtPas2Reg.OnValueChange = (Entity textPas2Reg) => { Globals.registerValidate = txtPas2Reg.Value; };

			// Create Window
			CreateWindow(panel);
		}

		public void CreateChats()
		{
			//  Create Entities
			Panel panel = new Panel(new Vector2(500, 250), PanelSkin.None, Anchor.TopLeft);
			//panel.PanelOverflowBehavior = PanelOverflowBehavior.Clipped;
			//Globals.chatPanel = new Panel(new Vector2(500, 250), PanelSkin.None, Anchor.CenterLeft, new Vector2(-55, -50));
			//Globals.chats = new SelectList(new Vector2(500, 250), Anchor.Auto, null, PanelSkin.None);
			UserInterface.Active.AddEntity(panel);

			panel.PanelOverflowBehavior = PanelOverflowBehavior.VerticalScroll;
			panel.Scrollbar.Visible = false;
			/*Globals.chats.ItemsScale = .5F;
			Globals.chats.OnMouseWheelScroll = (Entity chatScroll) =>
			{
				var Delta = new GeonBit.UI.InputHelper();
				if (Delta.MouseWheelChange == 1 && Globals.chats.ScrollPosition >= 0)
				{
					Globals.chats.ScrollPosition--;
				}
				else
				{
					Globals.chats.ScrollPosition++;
				}

				Globals.chats.SelectedValue = null;
			};
			Globals.chats.FillColor = Color.Coral;
			panel.FillColor = Color.Bisque;

			// Add Entities
			panel.AddChild(Globals.chats);

			Globals.chats.OnListChange = entity =>
			{
				if (((Panel)entity.Parent).Scrollbar != null)
				{
					((Panel)entity.Parent).Scrollbar.Visible = false;
				}
			};*/

			// Create Window
			//CreateWindow(panel);
			//Globals.chatPanel.PanelOverflowBehavior = PanelOverflowBehavior.VerticalScroll;
			// Globals.chatPanel.AddChild(new MulticolorParagraph("Welcome to the Server.", Anchor.Auto, Color.Aqua));
			Globals.chatPanel = panel;
			CreateWindow(Globals.chatPanel);
		}

		public static void AddChats(string message)
		{
			//Globals.chats.AddItem(message);
			var para = new MulticolorParagraph(message, Anchor.Auto, Color.Honeydew);
			para.Scale = 0.5f;
			Globals.chatPanel.AddChild(para);

			// If a message is selected, assume they want to lock the list and don't scroll to new.
			/*if (Globals.chats.SelectedValue == null)
			{
				Globals.chats.scrollToEnd();
			}*/
		}

		public void CreateMessage()
		{
			Panel panel = new Panel(new Vector2(500, 50), PanelSkin.None, Anchor.BottomLeft, new Vector2(-10, 40));
			messageText = new TextInput(false) { Scale = .5F, CharactersLimit = 500, PanelOverflowBehavior = PanelOverflowBehavior.Overflow };
			UserInterface.Active.AddEntity(panel);
			panel.AddChild(messageText);
			CreateWindow(panel);
		}
	}
}
