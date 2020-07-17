using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
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
		public static UIScreen GlobalVarsScreen(Game game)
		{
			Game1 g1 = (Game1)game;
			var retVal = new UIScreen(game)
			{
				Purpose = ScreenPurpose.Options,
				EscapeCloses = true
			};
			retVal.Escape += (sender, e) =>
			{

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
				Text = "stuff",
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

			var move = new StackPanel(game)
			{
				Direction = LayoutDirection.Horizontal,
				HAnchor = HorizontalAnchor.Middle,
				ChildrenShareFocus = true,
				SpriteBackground = true,
				Border = 4
			};
			spMenu.Children.Add(move);
			var moveLabel = new TextPanel(game)
			{
				Text = "move Speed",
				Width = 32,
				SpriteBackground = false
			};
			move.Children.Add(moveLabel);
			var moveSlide = new Slider(game)
			{
				Value = GlobalVarsHolder.PlayerRunSpeed,
				Minimum = 50,
				Maximum = 1000

			};
			moveSlide.ValueChanged += (sender, e) =>
			{
				GlobalVarsHolder.PlayerRunSpeed = moveSlide.Value;
			};
			move.Children.Add(moveSlide);

			var jump = new StackPanel(game)
			{
				Direction = LayoutDirection.Horizontal,
				HAnchor = HorizontalAnchor.Middle,
				ChildrenShareFocus = true,
				SpriteBackground = true,
				Border = 4
			};
			spMenu.Children.Add(jump);
			var jumpLabel = new TextPanel(game)
			{
				Text = "jump Speed",
				Width = 32,
				SpriteBackground = false
			};
			jump.Children.Add(jumpLabel);
			var jumpSlide = new Slider(game)
			{
				Value = -GlobalVarsHolder.PlayerJump,
				Minimum = 50,
				Maximum = 1000

			};
			jumpSlide.ValueChanged += (sender, e) =>
			{
				GlobalVarsHolder.PlayerJump = -jumpSlide.Value;
			};
			jump.Children.Add(jumpSlide);

			var gravity = new StackPanel(game)
			{
				Direction = LayoutDirection.Horizontal,
				HAnchor = HorizontalAnchor.Middle,
				ChildrenShareFocus = true,
				SpriteBackground = true,
				Border = 4
			};
			spMenu.Children.Add(gravity);
			var gravityLabel = new TextPanel(game)
			{
				Text = "gravity",
				Width = 32,
				SpriteBackground = false
			};
			gravity.Children.Add(gravityLabel);
			var gravitySlide = new Slider(game)
			{
				Value = GlobalVarsHolder.Gravity,
				Minimum = 100,
				Maximum = 2000,
				IncrementSize = 50

			};
			gravitySlide.ValueChanged += (sender, e) =>
			{
				GlobalVarsHolder.Gravity = gravitySlide.Value;
			};
			gravity.Children.Add(gravitySlide);

			var terminalVelocity = new StackPanel(game)
			{
				Direction = LayoutDirection.Horizontal,
				HAnchor = HorizontalAnchor.Middle,
				ChildrenShareFocus = true,
				SpriteBackground = true,
				Border = 4
			};
			spMenu.Children.Add(terminalVelocity);
			var terminalVelocityLabel = new TextPanel(game)
			{
				Text = "terminalVelocity",
				Width = 32,
				SpriteBackground = false
			};
			terminalVelocity.Children.Add(terminalVelocityLabel);
			var terminalVelocitySlide = new Slider(game)
			{
				Value = GlobalVarsHolder.TerminalVelocity,
				Minimum = 500,
				Maximum = 1000

			};
			terminalVelocitySlide.ValueChanged += (sender, e) =>
			{
				GlobalVarsHolder.TerminalVelocity = terminalVelocitySlide.Value;
			};
			terminalVelocity.Children.Add(terminalVelocitySlide);

			var foo3000 = new StackPanel(game)
			{
				Direction = LayoutDirection.Horizontal,
				HAnchor = HorizontalAnchor.Middle,
				ChildrenShareFocus = true,
				SpriteBackground = true,
				Border = 4
			};
			spMenu.Children.Add(foo3000);
			var foo3000Label = new TextPanel(game)
			{
				Text = "foo3000",
				Width = 32,
				SpriteBackground = false
			};
			foo3000.Children.Add(foo3000Label);
			var foo3000Slide = new Slider(game)
			{
				Value = 50,
				Minimum = -100,
				Maximum = 100

			};
			foo3000Slide.ValueChanged += (sender, e) =>
			{
				//GlobalVarsHolder.TerminalVelocity = foo3000Slide.Value;
			};
			foo3000.Children.Add(foo3000Slide);

			retVal.UpdateLayout(retVal.Bounds);
			return retVal;
		}
	}
}
