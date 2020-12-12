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
		/// A screen for viewing high scores
		/// </summary>
		/// <param name="game"></param>
		/// <returns></returns>
		public static UIScreen HighScoreScreen(Game game)
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
			var mainsp = new StackPanel(game)
			{
				HAnchor = HorizontalAnchor.Middle,
				VAnchor = VerticalAnchor.Middle,
				SpriteBackground = true,
				Border = 4
			};
			retVal.Children.Add(mainsp);
			var scoreLabel = new TextPanel(game)
			{
				Text = "High score",
				Width = 128,
				SpriteBackground = false
			};
			mainsp.Children.Add(scoreLabel);
			int rank = 0;
			foreach (var item in g1.Score.Scores)
			{
				rank++;
				var scoresp = new StackPanel(game)
				{
					Direction = LayoutDirection.Horizontal
				};
				mainsp.Children.Add(scoresp);
				var rankText = new TextPanel(game)
				{
					Text = rank.ToString(),
					Height = 8,
					Width = 8,
					SpriteBackground = false
				};
				scoresp.Children.Add(rankText);
				var name = new TextPanel(game)
				{
					Text = item.Item1,
					SpriteBackground = false,
					Height = 8
				};
				scoresp.Children.Add(name);
				var score = new TextPanel(game)
				{
					Text = item.Item2.ToString(),
					SpriteBackground = false,
					Height = 8
				};
				scoresp.Children.Add(score);
			}
			//
			var buttonRow = new StackPanel(game)
			{
				Direction = LayoutDirection.Vertical,
				MainNavigation = true
			};
			mainsp.Children.Add(buttonRow);
			var back = new Button(game)
			{
				Text = "Back"
			};
			buttonRow.Children.Add(back);
			back.OnClick += (snder, e) =>
			{
				retVal.InvokeEscape();
			};
			retVal.UpdateLayout(retVal.Bounds);
			return retVal;
		}

	}
}
