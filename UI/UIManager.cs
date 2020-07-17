using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Encodings.Web;

namespace PuzzleGame.UI
{
	/// <summary>
	/// The root of the UI. Updates and draws the topmost screen.
	/// Screens underneath also drawn if top screen set to allow it.
	/// </summary>
	class UIManager : DrawableGameComponent
	{
		readonly Game1 Game1;
		List<UIScreen> ScreenStack = new List<UIScreen>();
		public ScreenPurpose CurrentScreenPurpose
		{
			get
			{
				if (ScreenStack.Count > 0)
				{
					return ScreenStack[^1].Purpose;
				}
				else
				{
					return ScreenPurpose.Unspecified;
				}
			}
		}

		public UIManager(Game game) : base(game)
		{
			if (game is Game1 g1)
			{
				Game1 = g1;
			}
			else
			{
				throw new Exception();
			}
			ScreenStack.Add(UIScreen.MainMenuScreen(game));
			UpdateOrder = 2;
			DrawOrder = 1;
		}

		public override void Update(GameTime gameTime)
		{
			base.Update(gameTime);
			if (Game1.Input.MenuBack)
			{
				Escape();
			}
			ScreenStack[^1].Update(gameTime);
		}

		public void TogglePause()
		{
			if (CurrentScreenPurpose == ScreenPurpose.PauseMenu)
			{
				CloseTopScreen();
			}
			else
			{
				ScreenStack.Add(UIScreen.PauseScreen(Game));
			}
		}

		public void OpenScreen(UIScreen screen)
		{
			ScreenStack.Add(screen);
		}

		public void Escape()
		{
			if (ScreenStack[^1].FramesOpen == 0)
			{
				return;
			}
			if (ScreenStack[^1].EscapeCloses)
			{
				CloseTopScreen();
			}
			else
			{
				ScreenStack[^1].InvokeEscape();
			}
		}

		public void CloseTopScreen()
		{
			if (ScreenStack.Count <= 1)
			{
				return;
			}
			if (ScreenStack[^1].Purpose != ScreenPurpose.MainMenu)
			{
				ScreenStack.RemoveAt(ScreenStack.Count - 1);
			}
		}

		public override void Draw(GameTime gameTime)
		{
			base.Draw(gameTime);
			int start = 0;
			for (int i = ScreenStack.Count - 1; i >= 0; i--)
			{
				start = i;
				if (!ScreenStack[i].ScreenBelowVisible)
				{
					break;
				}
			}
			for (int i = start; i < ScreenStack.Count; i++)
			{
				ScreenStack[i].Draw(gameTime);
			}
		}
	}
}
