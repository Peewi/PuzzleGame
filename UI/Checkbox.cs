using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace PuzzleGame.UI
{
	/// <summary>
	/// A checkbox
	/// </summary>
	class Checkbox : UIPanel
	{
		int CheckColor = 1;
		public bool Checked = false;
		public Checkbox(Game game) : base(game)
		{
			Width = 16;
			OnClick += (sender, e) =>
			{
				Checked = !Checked;
			};
		}
		public override void Draw(GameTime gameTime)
		{
			const int SPRITESIZE = 16;
			const int SPRITEMARGIN = 2;
			int colorOffset = CheckColor * 6;
			int checkedOffset = Checked ? 1 : 0;
			Rectangle src = new Rectangle((colorOffset + 3) * (SPRITESIZE + SPRITEMARGIN), (5 + checkedOffset) * (SPRITESIZE + SPRITEMARGIN), SPRITESIZE, SPRITESIZE);
			Rectangle tgt = new Rectangle((int)Position.X, (int)Position.Y, SPRITESIZE, SPRITESIZE);
			tgt = Camera.ScreenRect(tgt);
			SpriteBatch.Draw(Sheet, tgt, src, Color.White);
		}
	}
}
