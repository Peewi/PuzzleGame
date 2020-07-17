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
		/// A screen for changing keybinds
		/// </summary>
		/// <param name="game"></param>
		/// <returns></returns>
		public static UIScreen KeybindsScreen(Game game)
		{

			Game1 g1 = (Game1)game;
			g1.Input.SetRevertState();
			var retVal = new UIScreen(game)
			{
				//EscapeCloses = true
			};
			List<KeybindPanel> kbPanels = new List<KeybindPanel>();
			retVal.Escape += (sender, e) =>
			{
				foreach (var item in kbPanels)
				{
					if (item.Active)
					{
						return;
					}
				}
				if (g1.Input.UnsavedChanges)
				{
					g1.UI.OpenScreen(CloseKeybindsConfirmationScreen(game));
				}
				else
				{
					g1.UI.CloseTopScreen();
				}
			};
			var spLabel = new StackPanel(game)
			{
				HAnchor = HorizontalAnchor.Middle,
				VAnchor = VerticalAnchor.Middle,
				SpriteBackground = true,
				Border = 4
			};
			var label = new TextPanel(game)
			{
				Text = "Keybinds",
				HAnchor = HorizontalAnchor.Middle,
				SpriteBackground = false
			};
			spLabel.Children.Add(label);
			var spMenu = new StackPanel(game)
			{
				HAnchor = HorizontalAnchor.Middle,
				VAnchor = VerticalAnchor.Middle,
				MainNavigation = true
			};
			spLabel.Children.Add(spMenu);
			foreach (var action in g1.Input.BindableActions)
			{
				var row = new StackPanel(game)
				{
					Direction = LayoutDirection.Horizontal,
					ChildrenShareFocus = true
				};
				var actionName = new TextPanel(game)
				{
					Text = action.ToString(),
					SpriteBackground = false,
					Width = 48
				};
				var keythingy = new KeybindPanel(game)
				{
					//Input = g1.Input.GetKeybind(action),
					Action = action,
					Width = 80,
					Filter = new InputTypes[] { InputTypes.Keyboard, InputTypes.Mouse }
				};
				keythingy.SetText();
				keythingy.KeyAssigned += (sender, e) =>
				{
					foreach (var item in kbPanels)
					{
						item.SetText();
					}
				};
				kbPanels.Add(keythingy);
				row.Children.Add(actionName);
				row.Children.Add(keythingy);
				spMenu.Children.Add(row);
			}
			var defaultButton = new Button(game)
			{
				Text = "Use defaults",
				HAnchor = HorizontalAnchor.Right
			};
			defaultButton.OnClick += (sender, e) =>
			{
				g1.Input.SetDefaultKeys();
				foreach (var item in kbPanels)
				{
					item.SetText();
				}
			};
			spMenu.Children.Add(defaultButton);
			var closeButtons = new StackPanel(game)
			{
				Direction = LayoutDirection.Horizontal
			};
			var saveButton = new Button(game)
			{
				Text = "Save & close"
			};
			saveButton.OnClick += (sender, e) =>
			{
				g1.Input.SaveKeybinds(g1.Config.KeysFilePath);
				g1.UI.CloseTopScreen();
			};
			var dontsaveButton = new Button(game)
			{
				Text = "Close without saving"
			};
			dontsaveButton.OnClick += (sender, e) =>
			{
				g1.Input.RevertChanges();
				g1.UI.CloseTopScreen();
			};
			closeButtons.Children.Add(saveButton);
			closeButtons.Children.Add(dontsaveButton);
			spMenu.Children.Add(closeButtons);
			retVal.Children.Add(spLabel);
			retVal.UpdateLayout(retVal.Bounds);
			return retVal;
		}
	}
}
