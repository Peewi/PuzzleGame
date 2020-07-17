using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace PuzzleGame
{
	enum BoardSpace
	{
		Blank,
		Virus,
		PillRound,
		PillLeft,
		PillRight,
		PillUpper,
		PillLower
	}

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
		float DropInterval = 0.75f;
		float DropTime = 0;
		int CursorX = 0;
		int CursorY = 0;
		const int BOARDWIDTH = 8;
		const int BOARDHEIGHT = 16;
		const int TILESIZE = 8;

		Color[] Colors = { Color.Red, Color.Blue, Color.Yellow };
		(BoardSpace, int)[,] Board = new (BoardSpace, int)[BOARDWIDTH, BOARDHEIGHT];

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
			Board = new (BoardSpace, int)[BOARDWIDTH, BOARDHEIGHT];
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
					if (i - 2 >= 0 && Board[i - 2, j].Item1 == BoardSpace.Virus)
					{
						available[Board[i - 2, j].Item2 + 1] = false;
					}
					if (j + 2 < BOARDHEIGHT && Board[i, j + 2].Item1 == BoardSpace.Virus)
					{
						available[Board[i, j + 2].Item2 + 1] = false;
					}
					int numColorsAvailable = 0;
					for (int k = 1; k < 4; k++)
					{
						numColorsAvailable += available[k] ? 1 : 0;
					}
					// chance stuff
					float[] chanceSize = { cellsLeft - virusesLeft, 0, 0, 0 };
					float[] colorChances = { cellsLeft - virusesLeft, 0, 0, 0 };
					colorChances[0] /= cellsLeft;
					float virusChance = 1 - colorChances[0];
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
						colorChances[k] = chanceSize[k] / (chanceSize[1] + chanceSize[2] + chanceSize[3]);
						colorChances[k] *= virusChance;
					}
					float colorRunning = 0;

					for (int k = 0; k < 4; k++)
					{
						colorRunning += colorChances[k];
						if (placeRoll < colorRunning)
						{
							if (k == 0)
							{
								Board[i, j] = (BoardSpace.Blank, 0);
							}
							else
							{
								Board[i, j] = (BoardSpace.Virus, k - 1);
							}
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
			// look at all this input shit.
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
			// drop timer
			DropTime += dt;
			if (DropTime >= DropInterval)
			{
				DropTime -= DropInterval;
				CursorY++;
				CursorY %= BOARDHEIGHT;
			}
			// pause button
			if (Game1.Input.Pause)
			{
				Game1.UI.TogglePause();
			}
		}

		public override void Draw(GameTime gameTime)
		{
			Rectangle bg = new Rectangle((int)TopLeft.X, (int)TopLeft.Y, BOARDWIDTH * TILESIZE, BOARDHEIGHT * TILESIZE);
			Rectangle cursor = new Rectangle((int)TopLeft.X + CursorX * TILESIZE, (int)TopLeft.Y + CursorY * TILESIZE, TILESIZE, TILESIZE);
			bg = Camera.ScreenRect(bg);
			cursor = Camera.ScreenRect(cursor);
			SpriteBatch.Draw(Game1.Pixel, bg, Color.Black);
			SpriteBatch.Draw(Game1.Pixel, cursor, Color.White);

			Rectangle blockSrc = new Rectangle(0, 0, 16, 16);
			Rectangle pillSrc = new Rectangle(0, 16, 16, 16);
			Rectangle roundSrc = new Rectangle(16, 16, 16, 16);

			Vector2 spaceOrigin = new Vector2(8, 8);

			for (int i = 0; i < BOARDWIDTH; i++)
			{
				for (int j = 0; j < BOARDHEIGHT; j++)
				{
					Rectangle drawRect = new Rectangle((int)TopLeft.X + 4 + i * TILESIZE, (int)TopLeft.Y + 4 + j * TILESIZE, TILESIZE, TILESIZE);
					drawRect = Camera.ScreenRect(drawRect);
					switch (Board[i, j].Item1)
					{
						case BoardSpace.Blank:
							break;
						case BoardSpace.Virus:
							SpriteBatch.Draw(Game1.Blocks, drawRect, blockSrc, Colors[Board[i, j].Item2], 0,spaceOrigin, SpriteEffects.None, 0.5f);
							break;
						case BoardSpace.PillRound:
							SpriteBatch.Draw(Game1.Blocks, drawRect, roundSrc, Colors[Board[i, j].Item2], 0, spaceOrigin, SpriteEffects.None, 0.5f);
							break;
						case BoardSpace.PillLeft:
							SpriteBatch.Draw(Game1.Blocks, drawRect, pillSrc, Colors[Board[i, j].Item2], MathF.PI, spaceOrigin, SpriteEffects.None, 0.5f);
							break;
						case BoardSpace.PillRight:
							SpriteBatch.Draw(Game1.Blocks, drawRect, pillSrc, Colors[Board[i, j].Item2], 0, spaceOrigin, SpriteEffects.None, 0.5f);
							break;
						case BoardSpace.PillUpper:
							SpriteBatch.Draw(Game1.Blocks, drawRect, pillSrc, Colors[Board[i, j].Item2], MathHelper.PiOver2, spaceOrigin, SpriteEffects.None, 0.5f);
							break;
						case BoardSpace.PillLower:
							SpriteBatch.Draw(Game1.Blocks, drawRect, pillSrc, Colors[Board[i, j].Item2], MathHelper.PiOver2 * 3, spaceOrigin, SpriteEffects.None, 0.5f);
							break;
						default:
							break;
					}
				}
			}
		}
	}
}
