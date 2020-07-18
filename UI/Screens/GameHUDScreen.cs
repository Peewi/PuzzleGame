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
		public static UIScreen GameHUDScreen(Game game)
		{
			Game1 g1 = (Game1)game;
			//TODO: Put any HUD stuff here
			var retVal = new UIScreen(game)
			{
				Purpose = ScreenPurpose.HUD
			};
			retVal.Escape += (sender, e) =>
			{
				//g1.UI.TogglePause();
			};
			var score = new TextPanel(game)
			{
				Text = "0",
				HAnchor = HorizontalAnchor.Left,
				VAnchor = VerticalAnchor.Top
			};
			retVal.Children.Add(score);
			retVal.Children[^1].UpdateEvent += (sender, e) =>
			{
				score.Text = g1.Board.Points.ToString();
			};
			retVal.UpdateLayout(retVal.Bounds);
			return retVal;
		}
	}
}
