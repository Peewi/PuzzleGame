using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace PuzzleGame.UI
{
	partial class UIScreen
	{
		public static UIScreen UITestPlace(Game game)
		{
			Game1 g1 = (Game1)game;
			var retVal = new UIScreen(game)
			{
				SpriteBackground = false,
				ScreenBelowVisible = true,
				EscapeCloses = true,
				BackgroundColor = Color.Black * 0.5f
			};
			var keyThing = new TextPanel(game)
			{
				HAnchor = HorizontalAnchor.Left,
				VAnchor = VerticalAnchor.Top
			};
			keyThing.UpdateEvent += (sender, e) =>
			{
				var k = g1.Input.RecentKey();
				if (k != Microsoft.Xna.Framework.Input.Keys.None)
				{
					keyThing.Text = k.ToString();
				}
			};
			retVal.Children.Add(keyThing);
			var inputThing = new TextPanel(game)
			{
				HAnchor = HorizontalAnchor.Left,
				VAnchor = VerticalAnchor.Middle
			};
			inputThing.UpdateEvent += (sender, e) =>
			{
				var input = g1.Input.RecentInput();
				if (input.Type != InputTypes.None)
				{
					inputThing.Text = input.ToString();
				}
			};
			retVal.Children.Add(inputThing);
			//retVal.Children.Add(new UIPanel(game));
			//retVal.Children[^1].HAnchor = HorizontalAnchor.Left;
			//retVal.Children[^1].VAnchor = VerticalAnchor.Top;
			retVal.Children.Add(new Slider(game));
			retVal.Children[^1].HAnchor = HorizontalAnchor.Middle;
			retVal.Children[^1].VAnchor = VerticalAnchor.Bottom;
			retVal.Children.Add(new Checkbox(game));
			retVal.Children[^1].HAnchor = HorizontalAnchor.Middle;
			retVal.Children[^1].VAnchor = VerticalAnchor.Middle;
			StackPanel sp = new StackPanel(game);
			sp.HAnchor = HorizontalAnchor.Right;
			sp.VAnchor = VerticalAnchor.Bottom;
			sp.Children.Add(new UIPanel(game));
			//sp.Children[^1].Text = "first";
			sp.Children.Add(new UIPanel(game));
			//sp.Children[^1].Text = "second";
			sp.Children.Add(new UIPanel(game));
			//sp.Children[^1].Text = "third";
			Button tp = new Button(game)
			{
				Width = 48
			};
			Button tp2 = new Button(game)
			{
				Height = 48,
				Width = 48
			};
			Button tp3 = new Button(game)
			{
				Height = 64,
				Width = 64
			};
			sp.Children.Add(tp);
			sp.Children.Add(tp2);
			sp.Children.Add(tp3);
			retVal.Children.Add(sp);
			retVal.UpdateLayout(retVal.Bounds);
			return retVal;
		}
	}
}
