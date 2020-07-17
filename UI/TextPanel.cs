using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace PuzzleGame.UI
{
	/// <summary>
	/// It's a panel, but with text on it.
	/// </summary>
	class TextPanel : UIPanel
	{
		public virtual string Text { get; set; } = "Uninitialized";
		public Color TextColor = Color.Black;
		public TextPanel(Game game) : base(game)
		{
		}

		public override void Draw(GameTime gameTime)
		{
			base.Draw(gameTime);
			DrawText();
		}
		protected void DrawText()
		{
			if (!ParentBounds.Contains(Bounds.Center))
			{
				return;
			}
			const float TEXTBASESCALE = 4;
			float textScale = Camera.Scale / TEXTBASESCALE;
			Vector2 textSize = Font.MeasureString(Text) * textScale;
			Vector2 sizeVector = new Vector2(Width, Height) * Camera.Scale;
			Vector2 textPosAdjustment = new Vector2(0, -1);
			if (this is Button b)
			{
				if (b.MouseIsDown && b.MouseIsOver)
				{
					textPosAdjustment.Y += 2;
				}
			}
			Vector2 textPos = Camera.ScreenPos(Position + textPosAdjustment) + sizeVector / 2 - textSize / 2;
			SpriteBatch.DrawString(
				Font,
				Text,
				textPos,
				TextColor,
				0,
				Vector2.Zero,
				textScale,
				SpriteEffects.None,
				1f);
		}
	}
}
