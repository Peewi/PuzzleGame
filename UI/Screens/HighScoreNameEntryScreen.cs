using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace PuzzleGame.UI
{
	partial class UIScreen
	{
		public static UIScreen HighScoreNameEntryScreen(Game game)
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
				Text = $"You scored {((Game1)game).Board.Points} points, it's a new high score!\nEnter your name!",
				Width = 128,
				SpriteBackground = false
			};
			spLabel.Children.Add(score);
			var name = new TextPanel(game)
			{
				Text = "",
				Width = 128
			};
			((Game1)game).Window.TextInput += (sender, e) =>
			{
				if (e.Key == Microsoft.Xna.Framework.Input.Keys.Back)
				{
					name.Text = name.Text[0..^1];
				}
				else
				{
					name.Text += e.Character;
				}
			};
			spLabel.Children.Add(name);
			spLabel.Children.Add(spMenu);
			var q = new Button(game)
			{
				Text = "Done",
				HAnchor = HorizontalAnchor.Middle,
				Width = 64
			};
			spMenu.Children.Add(q);
			spMenu.Children[^1].OnClick += (snder, e) =>
			{
				if (game is Game1 g1)
				{
					g1.Score.Add(name.Text, g1.Board.Points);
					g1.Score.Save();
					g1.MainMenu();
					g1.UI.OpenScreen(HighScoreScreen(game));
				}
			};
			retVal.UpdateLayout(retVal.Bounds);
			return retVal;
		}
	}
}
