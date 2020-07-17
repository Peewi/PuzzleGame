using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace PuzzleGame.UI
{
	class Button : TextPanel
	{
		int ButtonColor = 1;
		int FocusColor = 3;
		public Button(Game game) : base(game)
		{
			
		}
		public override void Update(GameTime gameTime)
		{
			base.Update(gameTime);
			//ButtonColor -= game1.Input.Scrolled;
			//ButtonColor = (ButtonColor + 5) % 5;
		}
		public override void Draw(GameTime gameTime)
		{
			//base.Draw(gameTime);
			const int SPRITESIZE = 16;
			const int SPRITEMARGIN = 2;
			int colorOffset = ButtonColor * 6;
			if (HasFocus)
			{
				colorOffset = FocusColor * 6;
			}
			int pressedOffset = MouseIsDown && MouseIsOver ? 3 : 0;
			int textPressedOffset = MouseIsDown && MouseIsOver ? (int)Math.Round(2 * Camera.Scale) : 0;
			if (Height <= 16)
			{
				Rectangle lsrc = new Rectangle((colorOffset + 0 + pressedOffset) * (SPRITESIZE + SPRITEMARGIN), 0 * (SPRITESIZE + SPRITEMARGIN), SPRITESIZE, SPRITESIZE);
				Rectangle msrc = new Rectangle((colorOffset + 1 + pressedOffset) * (SPRITESIZE + SPRITEMARGIN), 0 * (SPRITESIZE + SPRITEMARGIN), SPRITESIZE, SPRITESIZE);
				Rectangle rsrc = new Rectangle((colorOffset + 2 + pressedOffset) * (SPRITESIZE + SPRITEMARGIN), 0 * (SPRITESIZE + SPRITEMARGIN), SPRITESIZE, SPRITESIZE);
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
				Rectangle nwSrc = new Rectangle((colorOffset + 0) * (SPRITESIZE + SPRITEMARGIN), (2 + pressedOffset) * (SPRITESIZE + SPRITEMARGIN), SPRITESIZE, SPRITESIZE);
				Rectangle nSrc = new Rectangle((colorOffset + 1) * (SPRITESIZE + SPRITEMARGIN), (2 + pressedOffset) * (SPRITESIZE + SPRITEMARGIN), SPRITESIZE, SPRITESIZE);
				Rectangle neSrc = new Rectangle((colorOffset + 2) * (SPRITESIZE + SPRITEMARGIN), (2 + pressedOffset) * (SPRITESIZE + SPRITEMARGIN), SPRITESIZE, SPRITESIZE);
				Rectangle wSrc = new Rectangle((colorOffset + 0) * (SPRITESIZE + SPRITEMARGIN), (3 + pressedOffset) * (SPRITESIZE + SPRITEMARGIN), SPRITESIZE, SPRITESIZE);
				Rectangle mSrc = new Rectangle((colorOffset + 1) * (SPRITESIZE + SPRITEMARGIN), (3 + pressedOffset) * (SPRITESIZE + SPRITEMARGIN), SPRITESIZE, SPRITESIZE);
				Rectangle eSrc = new Rectangle((colorOffset + 2) * (SPRITESIZE + SPRITEMARGIN), (3 + pressedOffset) * (SPRITESIZE + SPRITEMARGIN), SPRITESIZE, SPRITESIZE);
				Rectangle swSrc = new Rectangle((colorOffset + 0) * (SPRITESIZE + SPRITEMARGIN), (4 + pressedOffset) * (SPRITESIZE + SPRITEMARGIN), SPRITESIZE, SPRITESIZE);
				Rectangle sSrc = new Rectangle((colorOffset + 1) * (SPRITESIZE + SPRITEMARGIN), (4 + pressedOffset) * (SPRITESIZE + SPRITEMARGIN), SPRITESIZE, SPRITESIZE);
				Rectangle seSrc = new Rectangle((colorOffset + 2) * (SPRITESIZE + SPRITEMARGIN), (4 + pressedOffset) * (SPRITESIZE + SPRITEMARGIN), SPRITESIZE, SPRITESIZE);
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
			DrawText();
		}
	}
}
