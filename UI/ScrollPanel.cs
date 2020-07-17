using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace PuzzleGame.UI
{
	/// <summary>
	/// Enable scrolling on a child UIPanel
	/// </summary>
	class ScrollPanel : UIPanel
	{
		public UIPanel Child;
		public int MaxWidth;
		public int MaxHeight;
		public int ScrollSpeed = 4;
		Vector2 ScrollPos = Vector2.Zero;
		Vector2 ScrollTarget = Vector2.Zero;
		bool MouseScrolling = false;
		public ScrollPanel(Game game) : base(game)
		{
			MaxWidth = 64;
			MaxHeight = 128;
			SpriteBackground = false;
		}

		public override void UpdateLayout(Rectangle parentBounds)
		{
			Child.UpdateLayout(Bounds);
			Width = Math.Min(MaxWidth, Child.Width);
			Height = Math.Min(MaxHeight, Child.Height);
			base.UpdateLayout(parentBounds);
			Child.Position = Position + ScrollPos;
			Child.ParentBounds = Bounds;
		}

		public override void Update(GameTime gameTime)
		{
			base.Update(gameTime);
			Child?.Update(gameTime);
			Vector2 newScrollPos = ScrollTarget;
			if (Child is StackPanel sp && !MouseScrolling)
			{
				if (sp.Children[sp.Selection].Bounds.Bottom > Bounds.Bottom)
				{
					newScrollPos.Y = Position.Y + Height + ScrollPos.Y - sp.Children[sp.Selection].Position.Y - sp.Children[sp.Selection].Height;
				}
				if (sp.Children[sp.Selection].Bounds.Top < Bounds.Top)
				{
					newScrollPos = Position + ScrollPos - sp.Children[sp.Selection].Position;
				}
			}
			newScrollPos.Y += game1.Input.Scrolled * ScrollSpeed * 4;
			if (game1.Input.Scrolled != 0)
			{
				MouseScrolling = true;
			}
			if (game1.Input.MenuDown || game1.Input.MenuUp)
			{
				MouseScrolling = false;
			}
			int minScroll = -Child.Height + Height;
			int maxScroll = 0;
			newScrollPos.Y = MathHelper.Clamp(newScrollPos.Y, minScroll, maxScroll);
			ScrollTarget = newScrollPos;
			if (ScrollPos != ScrollTarget)
			{
				int scrollMulti = 1;
				if (MathF.Abs(ScrollPos.Y - ScrollTarget.Y) >= 32)
				{
					scrollMulti = 4;
				}
				if (ScrollPos.Y < ScrollTarget.Y)
				{
					ScrollPos.Y += ScrollSpeed * scrollMulti;
				}
				if (ScrollPos.Y > ScrollTarget.Y)
				{
					ScrollPos.Y -= ScrollSpeed * scrollMulti;
				}
				Child.Position = Position + ScrollPos;
			}
		}

		public override void Draw(GameTime gameTime)
		{
			base.Draw(gameTime);
			Child?.Draw(gameTime);
		}
	}
}
