using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace PuzzleGame.UI
{
	partial class UIScreen
	{
		public static UIScreen PauseScreen(Game game)
		{
			Game1 g1 = (Game1)game;
			var retVal = new UIScreen(game)
			{
				EscapeCloses = true,
				BackgroundColor = Color.Black * 0.5f
			};
			retVal.Escape += (sender, e) =>
			{
				g1.UI.TogglePause();
			};
			retVal.Purpose = ScreenPurpose.PauseMenu;
			var spLabel = new StackPanel(game)
			{
				HAnchor = HorizontalAnchor.Middle,
				VAnchor = VerticalAnchor.Middle,
				Border = 4,
				SpriteBackground = true
			};
			var spMenu = new StackPanel(game)
			{
				MainNavigation = true
			};
			retVal.Children.Add(spLabel);
			var pLabel = new TextPanel(game)
			{
				Text = "Pause",
				SpriteBackground = false
			};
			spLabel.Children.Add(pLabel);
			spLabel.Children.Add(spMenu);
			var resume = new Button(game)
			{
				Text = "Resume"
			};
			spMenu.Children.Add(resume);
			spMenu.Children[^1].OnClick += (snder, e) =>
			{
				//resume game
				if (game is Game1 g1)
				{
					g1.UI.TogglePause();
				}
			};
			var q = new Button(game)
			{
				Text = "Quit"
			};
			spMenu.Children.Add(q);
			spMenu.Children[^1].OnClick += (snder, e) =>
			{
				//Quit
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
