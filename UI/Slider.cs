using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace PuzzleGame.UI
{
	/// <summary>
	/// Slider for selecting a value in a range.
	/// </summary>
	class Slider : UIPanel
	{
		public float Minimum = 0;
		public float Maximum = 100;
		public float Value = 50;
		public float IncrementSize = 1;
		public event EventHandler ValueChanged;
		int SliderColor = 1;
		int FocusColor = 3;
		bool grabbed = false;
		public Slider(Game game) : base(game)
		{
			Width = 96;
			ValueChanged += (sender, e) =>
			{
				System.Diagnostics.Debug.WriteLine(Value);
			};
		}
		public override void Update(GameTime gameTime)
		{
			base.Update(gameTime);
			grabbed = grabbed || (game1.Input.Click && MouseIsOver);
			grabbed = game1.Input.MouseIsDown ? grabbed : false;
			if (grabbed)
			{
				var butt = Camera.WorldPos(game1.Input.MousePos);
				float fillAmount = (butt.X - Position.X) / Width;
				fillAmount = MathHelper.Clamp(fillAmount, 0, 1);
				float newValue = MathHelper.Lerp(Minimum, Maximum, fillAmount);
				newValue = IncrementSize * (float)Math.Round(newValue / IncrementSize);
				if (Value != newValue)
				{
					Value = newValue;
					ValueChanged.Invoke(null, new EventArgs());
				}
			}
			if (HasFocus)
			{
				float newValue = Value;
				if (game1.Input.MenuLeft)
				{
					newValue -= IncrementSize;
				}
				if (game1.Input.MenuRight)
				{
					newValue += IncrementSize;
				}
				newValue = MathHelper.Clamp(newValue, Minimum, Maximum);
				if (Value != newValue)
				{
					Value = newValue;
					ValueChanged.Invoke(null, new EventArgs());
				}
			}
		}
		public override void Draw(GameTime gameTime)
		{
			const int SPRITESIZE = 16;
			const int SPRITEMARGIN = 2;
			int colorOffset = SliderColor * 6;
			if (HasFocus)
			{
				colorOffset = FocusColor * 6;
			}
			int sliderValX = (int)(Position.X + Width * (Value - Minimum) / (Maximum - Minimum));

			Rectangle lSrc = new Rectangle(3 * (SPRITESIZE + SPRITEMARGIN), 8 * (SPRITESIZE + SPRITEMARGIN), SPRITESIZE, SPRITESIZE);
			Rectangle mSrc = new Rectangle(4 * (SPRITESIZE + SPRITEMARGIN), 8 * (SPRITESIZE + SPRITEMARGIN), SPRITESIZE, SPRITESIZE);
			Rectangle rSrc = new Rectangle(5 * (SPRITESIZE + SPRITEMARGIN), 8 * (SPRITESIZE + SPRITEMARGIN), SPRITESIZE, SPRITESIZE);
			Rectangle arrowSrc = new Rectangle((5 + colorOffset) * (SPRITESIZE + SPRITEMARGIN), 6 * (SPRITESIZE + SPRITEMARGIN), SPRITESIZE, SPRITESIZE);
			Rectangle lTgt = new Rectangle((int)Position.X, (int)Position.Y, SPRITESIZE, Height);
			Rectangle mTgt = new Rectangle((int)Position.X + SPRITESIZE, (int)Position.Y, Width - 2 * SPRITESIZE, Height);
			Rectangle rTgt = new Rectangle((int)(Position.X + Width - SPRITESIZE), (int)Position.Y, SPRITESIZE, Height);
			Rectangle arrowTgt = new Rectangle(
				sliderValX - SPRITESIZE / 2,
				(int)Position.Y,
				SPRITESIZE,
				SPRITESIZE);
			lTgt = Camera.ScreenRect(lTgt);
			mTgt = Camera.ScreenRect(mTgt);
			rTgt = Camera.ScreenRect(rTgt);
			arrowTgt = Camera.ScreenRect(arrowTgt);
			SpriteBatch.Draw(Sheet, lTgt, lSrc, Color.White);
			SpriteBatch.Draw(Sheet, mTgt, mSrc, Color.White);
			SpriteBatch.Draw(Sheet, rTgt, rSrc, Color.White);
			SpriteBatch.Draw(Sheet, arrowTgt, arrowSrc, Color.White);

			const float TEXTBASESCALE = 4;
			float textScale = Camera.Scale / TEXTBASESCALE;
			Vector2 textSize = Font.MeasureString(Value.ToString()) * textScale;
			Vector2 sizeVector = new Vector2(Width, Height) * Camera.Scale;
			Vector2 textPos = Camera.ScreenPos(new Vector2(sliderValX, Position.Y + Height / 2)) - textSize / 2;
			SpriteBatch.DrawString(
				Font,
				Value.ToString(),
				textPos,
				Color.Black,
				0,
				Vector2.Zero,
				textScale,
				SpriteEffects.None,
				1f);
		}
	}
}
