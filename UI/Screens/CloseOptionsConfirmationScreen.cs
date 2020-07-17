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
		/// A screen that asks if you want to save your changed options before closing
		/// </summary>
		/// <param name="game"></param>
		/// <returns></returns>
		public static UIScreen CloseOptionsConfirmationScreen(Game game)
		{
			Game1 g1 = (Game1)game;
			var retVal = new UIScreen(game)
			{
				EscapeCloses = true
			};
			var sp = new StackPanel(game)
			{
				HAnchor = HorizontalAnchor.Middle,
				VAnchor = VerticalAnchor.Middle,
				SpriteBackground = true,
				Border = 4
			};
			retVal.Children.Add(sp);
			var label1 = new TextPanel(game)
			{
				Text = "Settings changed",
				SpriteBackground = false
			};
			sp.Children.Add(label1);
			var label2 = new TextPanel(game)
			{
				Text = "Save changes?",
				SpriteBackground = false
			};
			sp.Children.Add(label2);
			var buttonRow = new StackPanel(game)
			{
				Direction = LayoutDirection.Horizontal,
				MainNavigation = true
			};
			sp.Children.Add(buttonRow);
			var saveButton = new Button(game)
			{
				Text = "Save",
				Width = 48
			};
			saveButton.OnClick += (snder, e) =>
			{
				g1.Config.Save();
				g1.ApplyGraphicsConfig();
				g1.UI.CloseTopScreen();
				g1.UI.CloseTopScreen();
			};
			buttonRow.Children.Add(saveButton);
			var discardButton = new Button(game)
			{
				Text = "Discard",
				Width = 48
			};
			discardButton.OnClick += (snder, e) =>
			{
				g1.UI.CloseTopScreen();
				g1.UI.CloseTopScreen();
			};
			buttonRow.Children.Add(discardButton);
			var cancelButton = new Button(game)
			{
				Text = "Cancel",
				Width = 48
			};
			cancelButton.OnClick += (snder, e) =>
			{
				g1.UI.CloseTopScreen();
			};
			buttonRow.Children.Add(cancelButton);

			retVal.UpdateLayout(retVal.Bounds);
			return retVal;
		}
	}
}
