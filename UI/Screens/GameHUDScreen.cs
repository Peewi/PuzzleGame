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
			var sp = new StackPanel(game)
			{
				HAnchor = HorizontalAnchor.Left,
				VAnchor=VerticalAnchor.Top
			};
			retVal.Children.Add(sp);
			var score = new TextPanel(game)
			{
				Text = "score"
			};
			sp.Children.Add(score);
			score.UpdateEvent += (sender, e) =>
			{
				score.Text = $"Score: {g1.Board.Points}";
			};
			var lvl = new TextPanel(game)
			{
				Text = "lvl"
			};
			sp.Children.Add(lvl);
			lvl.UpdateEvent += (sender, e) =>
			{
				lvl.Text = $"Level: {g1.Board.CurrentLevel}";
			};
			var virus = new TextPanel(game)
			{
				Text = "virus"
			};
			sp.Children.Add(virus);
			virus.UpdateEvent += (sender, e) =>
			{
				virus.Text = $"Viruses: {g1.Board.VirusCount}";
			};
			var speed = new TextPanel(game)
			{
				Text = "0"
			};
			sp.Children.Add(speed);
			sp.Children[^1].UpdateEvent += (sender, e) =>
			{
				speed.Text = $"Speed: {g1.Board.Speed}";
			};
			retVal.UpdateLayout(retVal.Bounds);
			return retVal;
		}
	}
}
