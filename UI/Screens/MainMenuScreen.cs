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
		/// The main menu
		/// </summary>
		/// <param name="game"></param>
		/// <returns></returns>
		public static UIScreen MainMenuScreen(Game game)
		{
			Game1 g1 = (Game1)game;
			var retVal = new UIScreen(game)
			{
				Purpose = ScreenPurpose.MainMenu
			};
			var spFull = new StackPanel(game)
			{
				HAnchor = HorizontalAnchor.Middle,
				VAnchor = VerticalAnchor.Middle,
			};
			var title = new TextPanel(game)
			{
				Text = "Puzzle Game"
			};
			spFull.Children.Add(title);
			var spMenu = new StackPanel(game)
			{
				HAnchor = HorizontalAnchor.Middle,
				VAnchor = VerticalAnchor.Middle,
				MainNavigation = true
			};
			spFull.Children.Add(spMenu);
			retVal.Children.Add(spFull);
			var ng = new Button(game)
			{
				Text = "New game"
			};
			ng.OnClick += (sender, e) =>
			{
				g1.UI.OpenScreen(DifficultySelectScreen(game));
				
			};
			spMenu.Children.Add(ng);
			//
			var hs = new Button(game)
			{
				Text = "High scores"
			};
			hs.OnClick += (sender, e) =>
			{
				g1.UI.OpenScreen(HighScoreScreen(game));
			};
			spMenu.Children.Add(hs);
			var opt = new Button(game)
			{
				Text = "Options"
			};
			opt.OnClick += (sender, e) =>
			{
				g1.UI.OpenScreen(OptionsScreen(game));
			};
			spMenu.Children.Add(opt);

			var q = new Button(game)
			{
				Text = "Quit"
			};
			spMenu.Children.Add(q);
			spMenu.Children[^1].OnClick += (sender, e) =>
			{
				game.Exit();
			};
			retVal.UpdateLayout(retVal.Bounds);
			return retVal;
		}
	}
}
