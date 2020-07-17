using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PuzzleGame.UI
{
	partial class UIScreen
	{
		/// <summary>
		/// A screen for changing the options
		/// </summary>
		/// <param name="game"></param>
		/// <returns></returns>
		public static UIScreen DifficultySelectScreen(Game game)
		{
			Game1 g1 = (Game1)game;
			var retVal = new UIScreen(game)
			{
				Purpose = ScreenPurpose.Options
			};
			retVal.Escape += (sender, e) =>
			{
				g1.UI.CloseTopScreen();
			};
			var spLabel = new StackPanel(game)
			{
				HAnchor = HorizontalAnchor.Middle,
				VAnchor = VerticalAnchor.Middle,
				SpriteBackground = true,
				Border = 4
			};
			retVal.Children.Add(spLabel);
			var optionsLabel = new TextPanel(game)
			{
				Text = "New game",
				Width = 128,
				SpriteBackground = false
			};
			spLabel.Children.Add(optionsLabel);
			var spMenu = new StackPanel(game)
			{
				HAnchor = HorizontalAnchor.Middle,
				VAnchor = VerticalAnchor.Middle,
				MainNavigation = true
			};
			spLabel.Children.Add(spMenu);
			var difficulty = new StackPanel(game)
			{
				Direction = LayoutDirection.Horizontal,
				HAnchor = HorizontalAnchor.Middle,
				ChildrenShareFocus = true,
				SpriteBackground = true,
				Border = 4
			};
			spMenu.Children.Add(difficulty);
			var diffLabel = new TextPanel(game)
			{
				Text = "Difficulty",
				Width = 32,
				SpriteBackground = false
			};
			difficulty.Children.Add(diffLabel);
			var diffSlide = new Slider(game)
			{
				Value = 0,
				Minimum = 0,
				Maximum = 20,
				IncrementSize = 1
			};
			diffSlide.ValueChanged += (sender, e) =>
			{
				g1.Config.NewConfig.Volume = (int)diffSlide.Value;
			};
			difficulty.Children.Add(diffSlide);
			// 
			var okCancelRow = new StackPanel(game)
			{
				HAnchor = HorizontalAnchor.Middle,
				Direction = LayoutDirection.Horizontal
			};
			spMenu.Children.Add(okCancelRow);
			var cancel = new Button(game)
			{
				Text = "Back"
			};
			okCancelRow.Children.Add(cancel);
			cancel.OnClick += (snder, e) =>
			{
				retVal.InvokeEscape();
			};
			var newGame = new Button(game)
			{
				Text = "New Game"
			};
			newGame.OnClick += (snder, e) =>
			{
				g1.NewGame((int)diffSlide.Value);
			};
			okCancelRow.Children.Add(newGame);
			retVal.UpdateLayout(retVal.Bounds);
			return retVal;
		}

	}
}
