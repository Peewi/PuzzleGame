using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace PuzzleGame
{
	class PuzzleBoard : DrawableGameComponent
	{
		readonly Game1 Game1;
		InputManager Input => Game1.Input;
		SpriteBatch SpriteBatch => Game1.SpriteBatch;
		Camera Camera => Game1.Camera;
		Vector2 TopLeft;
		int InputDir = 0;
		float InputTimeHeld = 0;
		float InputRepeatDelay = 0.4f;
		float InputRepeatInterval = 0.1f;
		int CursorX = 0;
		const int BOARDWIDTH = 8;
		const int BOARDHEIGHT = 16;
		const int TILESIZE = 8;

		Color[] Colors = { Color.Red, Color.Blue, Color.Yellow };
		int[,] Viruses = new int[BOARDWIDTH, BOARDHEIGHT];

		public PuzzleBoard(Game game) : base(game)
		{
			if (game is Game1 g1)
			{
				Game1 = g1;
			}
			else
			{
				throw new Exception();
			}
			TopLeft = Camera.Center;
			TopLeft.X -= BOARDWIDTH * TILESIZE / 2;
			TopLeft.Y -= BOARDHEIGHT * TILESIZE / 2;
			NewGame(10);
		}

		void NewGame(int level)
		{
			Viruses = new int[BOARDWIDTH, BOARDHEIGHT];
			int virusesLeft = (level + 1) * 4;
			Random rng = new Random();
			int maxRow = BOARDHEIGHT - 10;
			if (level >= 15)
			{
				maxRow--;
			}
			if (level >= 17)
			{
				maxRow--;
			}
			if (level >= 19)
			{
				maxRow--;
			}
			int currentColor = rng.Next(0, 3);
			while (virusesLeft > 0)
			{
				int x = rng.Next(BOARDWIDTH);
				int y = rng.Next(maxRow, BOARDHEIGHT);
				while (Viruses[x,y] != 0)
				{
					x++;
					if (x >= BOARDWIDTH)
					{
						x = 0;
						y--;
						if (y < maxRow)
						{
							y++;
							x = BOARDWIDTH - 1;
							break;
						}
					}
				}
				if (Viruses[x, y] == 0)
				{
					Viruses[x, y] = currentColor + 1;
					currentColor++;
					currentColor %= 3;
					virusesLeft--;
				}
			}
		}

		public override void Update(GameTime gameTime)
		{
			float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;
			if (Game1.UI.CurrentScreenPurpose == UI.ScreenPurpose.PauseMenu)
			{
				return;
			}
			bool doInput = false;
			if ((Input.PlayerLeft && InputDir == -1) || (Input.PlayerRight && InputDir == 1))
			{
				InputTimeHeld += dt;
			}
			else
			{
				InputTimeHeld = 0;
			}
			if (InputTimeHeld >= InputRepeatDelay + InputRepeatInterval)
			{
				InputTimeHeld -= InputRepeatInterval;
				doInput = true;
			}
			if (Input.JustPressed(Actions.Left))
			{
				doInput = true;
				InputDir = -1;
			}
			if (Input.JustPressed(Actions.Right))
			{
				doInput = true;
				InputDir = 1;
			}
			if (doInput)
			{
				CursorX += InputDir;
				CursorX = MathHelper.Clamp(CursorX, 0, BOARDWIDTH - 1);
			}
			if (Game1.Input.Pause)
			{
				Game1.UI.TogglePause();
			}
		}

		public override void Draw(GameTime gameTime)
		{
			Rectangle bg = new Rectangle((int)TopLeft.X, (int)TopLeft.Y, BOARDWIDTH * TILESIZE, BOARDHEIGHT * TILESIZE);
			Rectangle cursor = new Rectangle((int)TopLeft.X + CursorX * TILESIZE, (int)TopLeft.Y, TILESIZE, TILESIZE);
			bg = Camera.ScreenRect(bg);
			cursor = Camera.ScreenRect(cursor);
			SpriteBatch.Draw(Game1.Pixel, bg, Color.Black);
			SpriteBatch.Draw(Game1.Pixel, cursor, Color.White);

			Rectangle blockSrc = new Rectangle(0, 0, 16, 16);

			for (int i = 0; i < BOARDWIDTH; i++)
			{
				for (int j = 0; j < BOARDHEIGHT; j++)
				{
					if (Viruses[i, j] > 0)
					{
						Rectangle block = new Rectangle((int)TopLeft.X + i * TILESIZE, (int)TopLeft.Y + j * TILESIZE, TILESIZE, TILESIZE);
						block = Camera.ScreenRect(block);
						SpriteBatch.Draw(Game1.Blocks, block, blockSrc, Colors[Viruses[i, j] - 1]);
					}
				}
			}
		}
	}
}
