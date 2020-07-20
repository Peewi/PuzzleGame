using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace PuzzleGame.UI
{
	partial class UIScreen
	{
		public static UIScreen GameOverScreen(Game game)
		{
			var retVal = new UIScreen(game)
			{
				ScreenBelowVisible = true
			};
			retVal.Purpose = ScreenPurpose.PauseMenu;
			var spLabel = new StackPanel(game)
			{
				HAnchor = HorizontalAnchor.Middle,
				VAnchor = VerticalAnchor.Middle,
				SpriteBackground = true,
				Border = 4
			};
			var spMenu = new StackPanel(game)
			{
				MainNavigation = true
			};
			retVal.Children.Add(spLabel);
			var go = new TextPanel(game)
			{
				Text = "Game Over",
				SpriteBackground = false
			};
			spLabel.Children.Add(go);
			var score = new TextPanel(game)
			{
				Text = $"You scored {((Game1)game).Board.Points} points",
				SpriteBackground = false
			};
			spLabel.Children.Add(score);
			spLabel.Children.Add(spMenu);
			//var ng = new Button(game)
			//{
			//	Text = "New Game"
			//};
			//spMenu.Children.Add(ng);
			//spMenu.Children[^1].OnClick += (snder, e) =>
			//{
			//	if (game is Game1 g1)
			//	{
			//		g1.NewGame();
			//	}
			//};
			var q = new Button(game)
			{
				Text = "Quit"
			};
			spMenu.Children.Add(q);
			spMenu.Children[^1].OnClick += (snder, e) =>
			{
				if (game is Game1 g1)
				{
					g1.MainMenu();
				}
			};
			retVal.UpdateLayout(retVal.Bounds);
			return retVal;
		}
	}
}
