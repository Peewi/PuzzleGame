using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace PuzzleGame.UI
{
	/// <summary>
	/// Arranges the UIPanels within sequentially. Can be horizontal or vertical.
	/// Child panels should not be anchored.
	/// </summary>
	class StackPanel : UIPanel
	{
		public readonly List<UIPanel> Children;
		public LayoutDirection Direction = LayoutDirection.Vertical;
		public bool MainNavigation = false;
		public bool ChildrenShareFocus = false;
		public int Selection = 0;
		public int Border = 0;
		public override bool HasFocus
		{
			get => base.HasFocus;
			set
			{
				base.HasFocus = value;
				if (ChildrenShareFocus)
				{
					foreach (var child in Children)
					{
						child.HasFocus = value;
					}
				}
				else
				{
					Children[Selection].HasFocus = value;
				}
			}
		}
		public override Rectangle ParentBounds
		{
			get => base.ParentBounds;
			set
			{
				base.ParentBounds = value;
				Rectangle intersect = Rectangle.Intersect(Bounds, value);
				foreach (var child in Children)
				{
					child.ParentBounds = intersect;
				}
			}
		}

		public StackPanel(Game game) : base(game)
		{
			Children = new List<UIPanel>();
			SpriteBackground = false;
			PositionChanged += (sender, e) =>
			{
				UpdateChildPositions();
			};
		}

		public override void UpdateLayout(Rectangle parentBounds)
		{
			// base.UpdateLayout sets Position, which makes UpdateChildPositions run
			Width = 0;
			Height = 0;
			foreach (var child in Children)
			{
				child.UpdateLayout(Bounds);
				switch (Direction)
				{
					case LayoutDirection.Horizontal:
						Width += child.Width;
						Height = Math.Max(Height, child.Height);
						break;
					case LayoutDirection.Vertical:
						Width = Math.Max(Width, child.Width);
						Height += child.Height;
						break;
				}
			}
			Width += Border * 2;
			Height += Border * 2;
			base.UpdateLayout(parentBounds);
			//UpdateChildPositions();
		}

		private void UpdateChildPositions()
		{
			Vector2 pos = Position;
			pos.X += Border;
			pos.Y += Border;
			Rectangle butt = Rectangle.Intersect(Bounds, ParentBounds);
			foreach (var child in Children)
			{
				Vector2 newChildPos = pos;
				//child.UpdateLayout(Bounds);
				// Do anchoring for children, but only perpendicular to the layout direction
				switch (Direction)
				{
					case LayoutDirection.Horizontal:
						pos.X += child.Width;
						switch (child.VAnchor)
						{
							case VerticalAnchor.Unanchored:
								break;
							case VerticalAnchor.Top:
								newChildPos.Y = Bounds.Y;
								break;
							case VerticalAnchor.Middle:
								newChildPos.Y = Bounds.Center.Y - child.Height / 2;
								break;
							case VerticalAnchor.Bottom:
								newChildPos.Y = Bounds.Bottom - child.Height;
								break;
							default:
								break;
						}
						break;
					case LayoutDirection.Vertical:
						pos.Y += child.Height;
						switch (child.HAnchor)
						{
							case HorizontalAnchor.Unanchored:
								break;
							case HorizontalAnchor.Left:
								newChildPos.X = Bounds.X;
								break;
							case HorizontalAnchor.Middle:
								newChildPos.X = Bounds.Center.X - child.Width / 2;
								break;
							case HorizontalAnchor.Right:
								newChildPos.X = Bounds.Right - child.Width;
								break;
							default:
								break;
						}
						break;
				}
				child.Position = newChildPos;
				child.ParentBounds = butt;
			}
		}

		public override void Update(GameTime gameTime)
		{
			base.Update(gameTime);
			// stuff for keyboard navigation
			if (ChildrenShareFocus && HasFocus)
			{
				if (game1.Input.MenuConfirm)
				{
					foreach (var child in Children)
					{
						child.Click();
					}
				}
			}
			else if (MainNavigation || HasFocus)
			{
				if (game1.Input.MouseIsMoving)
				{
					for (int i = 0; i < Children.Count; i++)
					{
						if (Children[i].MouseIsOver)
						{
							Children[Selection].HasFocus = false;
							Selection = i;
							break;
						}
					}
				}
				Children[Selection].HasFocus = false;
				switch (Direction)
				{
					case LayoutDirection.Horizontal:
						if (game1.Input.MenuLeft)
						{
							Selection--;
						}
						if (game1.Input.MenuRight)
						{
							Selection++;
						}
						break;
					case LayoutDirection.Vertical:
						if (game1.Input.MenuUp)
						{
							Selection--;
						}
						if (game1.Input.MenuDown)
						{
							Selection++;
						}
						break;
					case LayoutDirection.Undefined:
						break;
					default:
						break;
				}
				Selection = (Selection + Children.Count) % Children.Count;
				Children[Selection].HasFocus = true;
				if (game1.Input.MenuConfirm)
				{
					Children[Selection].Click();
				}
			}
			//children need updating
			foreach (var item in Children)
			{
				item.Update(gameTime);
			}
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
