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
		}

		public void NewGame(int level)
		{
			// https://www.researchgate.net/publication/334724493_Dr_Mario_Puzzle_Generation_Theory_Practice_History_FamicomNES
			Viruses = new int[BOARDWIDTH, BOARDHEIGHT];
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
			int cellsLeft = BOARDWIDTH * (BOARDHEIGHT - maxRow);
			int virusesLeft = (level + 1) * 4;
			int[] colorsPlaced = new int[4];
			for (int j = BOARDHEIGHT-1; j >= maxRow; j--)
			{
				for (int i = 0; i < BOARDWIDTH; i++)
				{
					float placeRoll = (float)rng.NextDouble();
					// 2-away rule
					bool[] available = { true, true, true, true };
					if (i - 2 >= 0 && Viruses[i - 2, j] != 0)
					{
						available[Viruses[i - 2, j]] = false;
					}
					if (j + 2 < BOARDHEIGHT && Viruses[i, j + 2] != 0)
					{
						available[Viruses[i, j + 2]] = false;
					}
					int numColorsAvailable = 0;
					for (int k = 1; k < 4; k++)
					{
						numColorsAvailable += available[k] ? 1 : 0;
					}
					// chance stuff
					float[] chanceSize = { cellsLeft - virusesLeft, 0, 0, 0 };
					float[] colorChanceA = { cellsLeft - virusesLeft, 0, 0, 0 };
					colorChanceA[0] /= cellsLeft;
					float virusChance = 1 - colorChanceA[0];
					int mostPlaced = Math.Max(Math.Max(colorsPlaced[0], colorsPlaced[1]), colorsPlaced[2]);
					for (int k = 1; k < 4; k++)
					{
						chanceSize[k] = available[k] ? (float)virusesLeft / numColorsAvailable : 0;
						int below = colorsPlaced[k];
						while (below < mostPlaced)
						{
							chanceSize[k] *= 3;
							below++;
						}
					}
					for (int k = 1; k < 4; k++)
					{
						colorChanceA[k] = chanceSize[k] / (chanceSize[1] + chanceSize[2] + chanceSize[3]);
						colorChanceA[k] *= virusChance;
					}
					float colorRunning = 0;

					for (int k = 0; k < 4; k++)
					{
						colorRunning += colorChanceA[k];
						float colorChance = colorRunning;
						if (placeRoll < colorChance)
						{
							Viruses[i, j] = k;
							colorsPlaced[k]++;
							if (k != 0)
							{
								virusesLeft--; 
							}
							break;
						}
					}
					cellsLeft--;
				}
			}
			// If there's a color completely missing, just make a new level
			if (colorsPlaced[1] == 0 || colorsPlaced[2] == 0 || colorsPlaced[3] == 0)
			{
				NewGame(level);
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
