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
		public static UIScreen OptionsScreen(Game game)
		{
			Game1 g1 = (Game1)game;
			var retVal = new UIScreen(game)
			{
				Purpose = ScreenPurpose.Options
			};
			retVal.Escape += (sender, e) =>
			{
				if (g1.Config.UnappliedChanges)
				{
					g1.UI.OpenScreen(CloseOptionsConfirmationScreen(game));
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
			retVal.Children.Add(spLabel);
			var optionsLabel = new TextPanel(game)
			{
				Text = "Options",
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
			var volume = new StackPanel(game)
			{
				Direction = LayoutDirection.Horizontal,
				HAnchor = HorizontalAnchor.Middle,
				ChildrenShareFocus = true,
				SpriteBackground = true,
				Border = 4
			};
			spMenu.Children.Add(volume);
			var volLabel = new TextPanel(game)
			{
				Text = "Volume",
				Width = 32,
				SpriteBackground = false
			};
			volume.Children.Add(volLabel);
			var volSlide = new Slider(game)
			{
				Value = g1.Config.Config.Volume,
				InputRepeatInterval = 1f / 40f
			};
			volSlide.ValueChanged += (sender, e) =>
			{
				g1.Config.NewConfig.Volume = (int)volSlide.Value;
			};
			volume.Children.Add(volSlide);
			// resolution
			var res = new StackPanel(game)
			{
				Direction = LayoutDirection.Horizontal,
				HAnchor = HorizontalAnchor.Middle,
				ChildrenShareFocus = true,
			};
			spMenu.Children.Add(res);
			var resLabel = new TextPanel(game)
			{
				Text = "Resolution",
				SpriteBackground = false
			};
			res.Children.Add(resLabel);
			var resolutionSelect = new SelectBox(game);
			var dispModes = GraphicsAdapter.DefaultAdapter.SupportedDisplayModes.ToList();
			for (int i = 0; i < dispModes.Count; i++)
			{
				resolutionSelect.Options.Add(new SelectBoxOption($"{dispModes[i].Width}x{dispModes[i].Height}", dispModes[i]));
				if (((Game1)game).Config.Config.Resolution == dispModes[i])
				{
					resolutionSelect.Selected = i;
				}
			}
			resolutionSelect.SelectionChanged += (sender, e) =>
			{
				if (resolutionSelect.SelectedOption.Value is DisplayMode newRes)
				{
					g1.Config.NewConfig.Resolution = newRes;
				}
			};
			res.Children.Add(resolutionSelect);
			// fullscreen mode select
			var fullscreen = new StackPanel(game)
			{
				Direction = LayoutDirection.Horizontal,
				HAnchor = HorizontalAnchor.Middle,
				ChildrenShareFocus = true
			};
			spMenu.Children.Add(fullscreen);
			var fullLabel = new TextPanel(game)
			{
				Text = "Fullscreen",
				SpriteBackground = false
			};
			fullscreen.Children.Add(fullLabel);
			var fullscreenSelect = new SelectBox(game);
			fullscreenSelect.Options.Add(new SelectBoxOption("Full screen", WindowMode.Fullscreen));
			fullscreenSelect.Options.Add(new SelectBoxOption("Window", WindowMode.Windowed));
			fullscreenSelect.Options.Add(new SelectBoxOption("Borderless window", WindowMode.BorderlessWindow));
			fullscreenSelect.SelectionChanged += (sender, e) =>
			{
				if (fullscreenSelect.SelectedOption.Value is WindowMode newWindMode)
				{
					g1.Config.NewConfig.WindowMode = newWindMode;
				}
			};
			var windMode = g1.Config.Config.WindowMode;
			switch (windMode)
			{
				case WindowMode.Fullscreen:
					fullscreenSelect.Selected = 0;
					break;
				case WindowMode.Windowed:
					fullscreenSelect.Selected = 1;
					break;
				case WindowMode.BorderlessWindow:
					fullscreenSelect.Selected = 2;
					break;
				default:
					break;
			}
			fullscreen.Children.Add(fullscreenSelect);
			//
			var keys = new Button(game)
			{
				Text = "Mouse & Keyboard",
				HAnchor = HorizontalAnchor.Middle,
				Width = 128
			};
			keys.OnClick += (sender, e) =>
			{
				g1.UI.OpenScreen(KeybindsScreen(game));
			};
			spMenu.Children.Add(keys);
			var controller = new Button(game)
			{
				Text = "Controller",
				HAnchor = HorizontalAnchor.Middle,
				Width = 128
			};
			controller.OnClick += (sender, e) =>
			{
				g1.UI.OpenScreen(ControllerOptionsScreen(game));
			};
			spMenu.Children.Add(controller);
			// 
			var okCancelRow = new StackPanel(game)
			{
				HAnchor = HorizontalAnchor.Middle,
				Direction = LayoutDirection.Horizontal
			};
			spMenu.Children.Add(okCancelRow);
			var apply = new Button(game)
			{
				Text = "Apply"
			};
			apply.OnClick += (snder, e) =>
			{
				//apply
				g1.Config.Save();
				g1.ApplyGraphicsConfig();
				//g1.Input.SaveKeybinds(g1.Config.KeysPath);
			};
			okCancelRow.Children.Add(apply);
			var cancel = new Button(game)
			{
				Text = "Back"
			};
			okCancelRow.Children.Add(cancel);
			cancel.OnClick += (snder, e) =>
			{
				retVal.InvokeEscape();
			};
			retVal.UpdateLayout(retVal.Bounds);
			return retVal;
		}

	}
}
