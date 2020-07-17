using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;

namespace PuzzleGame.UI
{
	/// <summary>
	/// A place to put UIPanels. They can be anchored.
	/// </summary>
	partial class UIScreen : UIPanel
	{
		public int FramesOpen { get; private set; } = 0;
		public readonly List<UIPanel> Children;
		public bool ScreenBelowVisible = false;
		public ScreenPurpose Purpose = ScreenPurpose.Unspecified;
		public event EventHandler Escape;
		public bool EscapeCloses = false;

		public UIScreen(Game game) : base(game)
		{
			Width = 320;
			Height = 180;
			SpriteBackground = false;
			Children = new List<UIPanel>();
			Escape += (sender, e) => { };
		}

		public override void Update(GameTime gameTime)
		{
			FramesOpen++;
			foreach (var item in Children)
			{
				item.Update(gameTime);
			}
			base.Update(gameTime);
		}

		public void InvokeEscape()
		{
			Escape.Invoke(null, new EventArgs());
		}

		public override void UpdateLayout(Rectangle parentBounds)
		{
			foreach (var item in Children)
			{
				item.UpdateLayout(Bounds);
			}
			base.UpdateLayout(parentBounds);
		}

		public override void Draw(GameTime gameTime)
		{
			base.Draw(gameTime);
			foreach (var item in Children)
			{
				item.Draw(gameTime);
			}
		}

	}
}
