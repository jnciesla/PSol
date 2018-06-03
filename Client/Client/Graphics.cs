﻿using System;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Bindings;
using System.IO;
using MonoGame.Extended;

namespace Client
{
	class Graphics
	{
		public static Texture2D[] Characters = new Texture2D[2];
		public static string statusMessage = "";

		public static void InitializeGraphics(ContentManager manager)
		{
			LoadCharacters(manager);
		}

		public static void RenderGraphics()
		{
			DrawPlayers();
		}

		private static void DrawSprite(int sprite, Vector2 position, float rotation, Vector2 origin)
		{
			Game1.spriteBatch.Draw(Characters[sprite], position, null, Color.White, rotation, origin, 1, SpriteEffects.None, 0);
		}

		private static void DrawPlayers()
		{
			if (GameLogic.PlayerIndex <= -1) return;
			double X, Y;
			int SpriteNum = 1;

			for (var i = 1; i != Constants.MAX_PLAYERS; i++)
			{
				if (Types.Player[i].X.CompareTo(0.0f) == 0) continue;
				var origin = new Vector2(Characters[SpriteNum].Width / 2f, Characters[SpriteNum].Height / 2f);
				// What are these X & Y for?
				X = Types.Player[i].X * 32 + Types.Player[i].XOffset - ((Characters[SpriteNum].Width / 4 - 32) / 2);
				Y = Types.Player[i].Y * 32 + Types.Player[i].YOffset;
				DrawSprite(SpriteNum, new Vector2(Types.Player[i].X, Types.Player[i].Y), Types.Player[i].Rotation, origin);
			}
		}

		private static void LoadCharacters(ContentManager manager)
		{
			for (int i = 1; i < Characters.Length; i++)
			{
				Characters[i] = manager.Load<Texture2D>("Characters/" + i);
			}
		}

		public static void DrawHud(ContentManager manager)
		{
			if (GameLogic.PlayerIndex <= -1) return;
			SpriteFont infoFont = manager.Load<SpriteFont>("GeonBit.UI/themes/hd/fonts/Regular");
			Game1.spriteBatch.Begin();
			Game1.spriteBatch.DrawString(infoFont, (int)Types.Player[GameLogic.PlayerIndex].X / 100 + ":" + (int)Types.Player[GameLogic.PlayerIndex].Y / 100,
				new Vector2(512, 10), Color.DarkBlue);
			Game1.spriteBatch.DrawString(infoFont, GameLogic.PlayerIndex.ToString(), new Vector2(512, 30), Color.Green);
			Game1.spriteBatch.DrawString(infoFont, statusMessage, new Vector2(512, 30), Color.DarkBlue);
			Game1.spriteBatch.End();
		}
	}
}
