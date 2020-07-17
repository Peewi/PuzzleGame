using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace PuzzleGame.UI
{
	/// <summary>
	/// The basis for the UI elements
	/// </summary>
	class UIPanel : DrawableGameComponent
	{
		protected Game1 game1;
		private Vector2 position = Vector2.Zero;
		public int Width = 64;
		public int Height = 16;
		public Color BackgroundColor = Color.Transparent;
		//public Color MouseOverBackground = Color.Red * 0.8f;
		public bool SpriteBackground = true;
		public bool MouseIsOver { get; private set; } = false;
		public bool MouseIsDown { get; private set; } = false;
		public virtual bool HasFocus { get; set; } = false;
		public virtual Rectangle ParentBounds { get; set; } = Rectangle.Empty;
		public Rectangle Bounds =>
			new Rectangle(
				(int)Position.X,
				(int)Position.Y, 
				Width, 
				Height);
		public VerticalAnchor VAnchor = VerticalAnchor.Unanchored;
		public HorizontalAnchor HAnchor = HorizontalAnchor.Unanchored;
		protected SpriteFont Font => game1.Font;
		protected Texture2D Pixel => game1.Pixel;
		protected Texture2D Sheet => game1.UISheet;
		protected Camera Camera => game1.UICamera;
		protected SpriteBatch SpriteBatch => game1.SpriteBatch;

		public Vector2 Position
		{
			get
			{
				return position;
			}
			set
			{
				position = value;
				PositionChanged.Invoke(null, new EventArgs());
			}
		}

		public event EventHandler OnClick;
		public event EventHandler UpdateEvent;
		public event EventHandler PositionChanged;

		public UIPanel(Game game) : base(game)
		{
			if (game is Game1 g1)
			{
				game1 = g1;
			}
			else
			{
				throw new Exception();
			}
			OnClick += (sender, e) => { };
			UpdateEvent += (sender, e) => { };
			PositionChanged += (sender, e) => { };
		}

		public override void Update(GameTime gameTime)
		{
			base.Update(gameTime);
			UpdateEvent.Invoke(null, new EventArgs());
			MouseIsOver = Camera.ScreenRect(Bounds).Contains(game1.Input.MousePos)
				&& Camera.ScreenRect(ParentBounds).Contains(game1.Input.MousePos);
			if (MouseIsOver)
			{
				if (game1.Input.Click)
				{
					MouseIsDown = true;
				}
			}
			if (MouseIsDown)
			{
				if (!game1.Input.MouseIsDown)
				{
					MouseIsDown = false;
					if (MouseIsOver)
					{
						Click();
					}
				}
			}
		}

		/// <summary>
		/// Invokes the OnClick event
		/// </summary>
		public void Click()
		{
			OnClick.Invoke(null, new EventArgs());
		}
		public virtual void UpdateLayout(Rectangle parentBounds)
		{
			ParentBounds = parentBounds;
			Vector2 tmp = Position;
			switch (HAnchor)
			{
				case HorizontalAnchor.Unanchored:
					break;
				case HorizontalAnchor.Left:
					tmp.X = parentBounds.X;
					break;
				case HorizontalAnchor.Middle:
					tmp.X = parentBounds.Center.X - Width / 2;
					break;
				case HorizontalAnchor.Right:
					tmp.X = parentBounds.Right - Width;
					break;
			}
			switch (VAnchor)
			{
				case VerticalAnchor.Unanchored:
					break;
				case VerticalAnchor.Top:
					tmp.Y = parentBounds.Y;
					break;
				case VerticalAnchor.Middle:
					tmp.Y = parentBounds.Center.Y - Height / 2;
					break;
				case VerticalAnchor.Bottom:
					tmp.Y = parentBounds.Bottom - Height;
					break;
			}
			Position = tmp;
		}
		public override void Draw(GameTime gameTime)
		{
			//Color bg = MouseIsOver ? MouseOverBackground : Background;
			Rectangle drawRect = new Rectangle((int)Position.X, (int)Position.Y, Width, Height);
			drawRect = Camera.ScreenRect(drawRect);
			SpriteBatch.Draw(Pixel, drawRect, BackgroundColor);
			if (!SpriteBackground)
			{
				return;
			}
			const int SPRITESIZE = 16;
			const int SPRITEMARGIN = 2;
			int colorOffset = 1 * 6;
			if (HasFocus)
			{
				colorOffset = 3 * 6;
			}
			if (Height <= 16)
			{
				Rectangle lsrc = new Rectangle((colorOffset + 3) * (SPRITESIZE + SPRITEMARGIN), 8 * (SPRITESIZE + SPRITEMARGIN), SPRITESIZE, SPRITESIZE);
				Rectangle msrc = new Rectangle((colorOffset + 4) * (SPRITESIZE + SPRITEMARGIN), 8 * (SPRITESIZE + SPRITEMARGIN), SPRITESIZE, SPRITESIZE);
				Rectangle rsrc = new Rectangle((colorOffset + 5) * (SPRITESIZE + SPRITEMARGIN), 8 * (SPRITESIZE + SPRITEMARGIN), SPRITESIZE, SPRITESIZE);
				Rectangle leftDrawRect = new Rectangle((int)Position.X, (int)Position.Y, SPRITESIZE, Height);
				Rectangle middleDrawRect = new Rectangle((int)(Position.X + SPRITESIZE), (int)Position.Y, Width - 2 * SPRITESIZE, Height);
				Rectangle rightDrawRect = new Rectangle((int)(Position.X + Width - SPRITESIZE), (int)Position.Y, SPRITESIZE, Height);
				leftDrawRect = Rectangle.Intersect(leftDrawRect, ParentBounds);
				middleDrawRect = Rectangle.Intersect(middleDrawRect, ParentBounds);
				rightDrawRect = Rectangle.Intersect(rightDrawRect, ParentBounds);
				leftDrawRect = Camera.ScreenRect(leftDrawRect);
				middleDrawRect = Camera.ScreenRect(middleDrawRect);
				rightDrawRect = Camera.ScreenRect(rightDrawRect);
				SpriteBatch.Draw(Sheet, leftDrawRect, lsrc, Color.White);
				SpriteBatch.Draw(Sheet, middleDrawRect, msrc, Color.White);
				SpriteBatch.Draw(Sheet, rightDrawRect, rsrc, Color.White);
			}
			else
			{
				Rectangle nwSrc = new Rectangle((colorOffset + 0) * (SPRITESIZE + SPRITEMARGIN), 8 * (SPRITESIZE + SPRITEMARGIN), SPRITESIZE, SPRITESIZE);
				Rectangle nSrc = new Rectangle((colorOffset + 1) * (SPRITESIZE + SPRITEMARGIN), 8 * (SPRITESIZE + SPRITEMARGIN), SPRITESIZE, SPRITESIZE);
				Rectangle neSrc = new Rectangle((colorOffset + 2) * (SPRITESIZE + SPRITEMARGIN), 8 * (SPRITESIZE + SPRITEMARGIN), SPRITESIZE, SPRITESIZE);
				Rectangle wSrc = new Rectangle((colorOffset + 0) * (SPRITESIZE + SPRITEMARGIN), 9 * (SPRITESIZE + SPRITEMARGIN), SPRITESIZE, SPRITESIZE);
				Rectangle mSrc = new Rectangle((colorOffset + 1) * (SPRITESIZE + SPRITEMARGIN), 9 * (SPRITESIZE + SPRITEMARGIN), SPRITESIZE, SPRITESIZE);
				Rectangle eSrc = new Rectangle((colorOffset + 2) * (SPRITESIZE + SPRITEMARGIN), 9 * (SPRITESIZE + SPRITEMARGIN), SPRITESIZE, SPRITESIZE);
				Rectangle swSrc = new Rectangle((colorOffset + 0) * (SPRITESIZE + SPRITEMARGIN), 10 * (SPRITESIZE + SPRITEMARGIN), SPRITESIZE, SPRITESIZE);
				Rectangle sSrc = new Rectangle((colorOffset + 1) * (SPRITESIZE + SPRITEMARGIN), 10 * (SPRITESIZE + SPRITEMARGIN), SPRITESIZE, SPRITESIZE);
				Rectangle seSrc = new Rectangle((colorOffset + 2) * (SPRITESIZE + SPRITEMARGIN), 10 * (SPRITESIZE + SPRITEMARGIN), SPRITESIZE, SPRITESIZE);
				Rectangle nwTgt = new Rectangle((int)Position.X, (int)Position.Y, SPRITESIZE, SPRITESIZE);
				Rectangle nTgt = new Rectangle((int)Position.X + SPRITESIZE, (int)Position.Y, Width - 2 * SPRITESIZE, SPRITESIZE);
				Rectangle neTgt = new Rectangle((int)Position.X + Width - SPRITESIZE, (int)Position.Y, SPRITESIZE, SPRITESIZE);
				Rectangle wTgt = new Rectangle((int)Position.X, (int)Position.Y + SPRITESIZE, SPRITESIZE, Height - 2 * SPRITESIZE);
				Rectangle mTgt = new Rectangle((int)Position.X + SPRITESIZE, (int)Position.Y + SPRITESIZE, Width - 2 * SPRITESIZE, Height - 2 * SPRITESIZE);
				Rectangle eTgt = new Rectangle((int)Position.X + Width - SPRITESIZE, (int)Position.Y + SPRITESIZE, SPRITESIZE, Height - 2 * SPRITESIZE);
				Rectangle swTgt = new Rectangle((int)Position.X, (int)Position.Y + Height - SPRITESIZE, SPRITESIZE, SPRITESIZE);
				Rectangle sTgt = new Rectangle((int)Position.X + SPRITESIZE, (int)Position.Y + Height - SPRITESIZE, Width - 2 * SPRITESIZE, SPRITESIZE);
				Rectangle seTgt = new Rectangle((int)Position.X + Width - SPRITESIZE, (int)Position.Y + Height - SPRITESIZE, SPRITESIZE, SPRITESIZE);
				nwTgt = Rectangle.Intersect(nwTgt, ParentBounds);
				nTgt = Rectangle.Intersect(nTgt, ParentBounds);
				neTgt = Rectangle.Intersect(neTgt, ParentBounds);
				wTgt = Rectangle.Intersect(wTgt, ParentBounds);
				mTgt = Rectangle.Intersect(mTgt, ParentBounds);
				eTgt = Rectangle.Intersect(eTgt, ParentBounds);
				swTgt = Rectangle.Intersect(swTgt, ParentBounds);
				sTgt = Rectangle.Intersect(sTgt, ParentBounds);
				seTgt = Rectangle.Intersect(seTgt, ParentBounds);
				nwTgt = Camera.ScreenRect(nwTgt);
				nTgt = Camera.ScreenRect(nTgt);
				neTgt = Camera.ScreenRect(neTgt);
				wTgt = Camera.ScreenRect(wTgt);
				mTgt = Camera.ScreenRect(mTgt);
				eTgt = Camera.ScreenRect(eTgt);
				swTgt = Camera.ScreenRect(swTgt);
				sTgt = Camera.ScreenRect(sTgt);
				seTgt = Camera.ScreenRect(seTgt);
				SpriteBatch.Draw(Sheet, nwTgt, nwSrc, Color.White);
				SpriteBatch.Draw(Sheet, nTgt, nSrc, Color.White);
				SpriteBatch.Draw(Sheet, neTgt, neSrc, Color.White);
				SpriteBatch.Draw(Sheet, wTgt, wSrc, Color.White);
				SpriteBatch.Draw(Sheet, mTgt, mSrc, Color.White);
				SpriteBatch.Draw(Sheet, eTgt, eSrc, Color.White);
				SpriteBatch.Draw(Sheet, swTgt, swSrc, Color.White);
				SpriteBatch.Draw(Sheet, sTgt, sSrc, Color.White);
				SpriteBatch.Draw(Sheet, seTgt, seSrc, Color.White);
			}
		}
	}
}
