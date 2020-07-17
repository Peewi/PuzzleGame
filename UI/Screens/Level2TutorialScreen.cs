using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace PuzzleGame.UI
{
	partial class UIScreen
	{
		/// <summary>
		/// The game HUD, in screen form
		/// </summary>
		/// <param name="game"></param>
		/// <returns></returns>
		public static UIScreen Level2TutorialScreen(Game game)
		{
			Game1 g1 = (Game1)game;
			//TODO: Put any HUD stuff here
			var retVal = new UIScreen(game)
			{
				Purpose = ScreenPurpose.Tutorial
			};
			retVal.Escape += (sender, e) =>
			{
				//g1.UI.TogglePause();
			};
			var tutor = new TextPanel(game)
			{
				Text = "Destroy all targets to win",
				//Width = 128+64,
				HAnchor = HorizontalAnchor.Middle,
				VAnchor = VerticalAnchor.Top
			};
			retVal.Children.Add(tutor);
			retVal.UpdateLayout(retVal.Bounds);
			return retVal;
		}
	}
}
