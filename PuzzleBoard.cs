﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PuzzleGame.UI;
using System;
using System.Collections.Generic;
using System.Linq;
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
		PillLower,
		Popping
	}

	enum PuzzleGameState
	{
		InControl,
		Popping,
		Cascading,
		GameOver,
		Victory
	}

	class PuzzleBoard : DrawableGameComponent
	{
		readonly Game1 Game1;
		InputManager Input => Game1.Input;
		SpriteBatch SpriteBatch => Game1.SpriteBatch;
		Camera Camera => Game1.Camera;
		Vector2 TopLeft;
		Random RNG;
		public int Points { get; private set; } = 0;
		public int VirusCount { get; private set; } = 0;
		public int CurrentLevel { get; private set; } = 0;
		public int Speed { get; private set; } = 0; //TODO: Make Low, Mid, Hi
		int SpeedMulti = 1;
		int CurrentMulti = 1;
		int InputDir = 0;
		float InputTimeHeld = 0;
		float InputRepeatDelay = 0.2f;
		float InputRepeatInterval = 0.1f;
		float DropInterval => DropSpeeds[StartSpeedIndex[Speed]];
		float DropTime = 0;
		Point CursorPos = new Point(3, 0);
		Point CursorPosPart2 => CursorPos + (CursorUpright ? new Point(0, -1) : new Point(1, 0));
		bool CursorActive = false;
		bool CursorUpright = false;
		int CursorColor1 = 0;
		int CursorColor2 = 1;
		(int, int)[] UpcomingPieces;
		int NextPieceIndex = 0;
		PuzzleGameState State = PuzzleGameState.InControl;
		float StateTime = 0;
		float CascadeRowTime = 0.25f;
		const int BOARDWIDTH = 8;
		const int BOARDHEIGHT = 16;
		const int TILESIZE = 8;
		int[] StartSpeedIndex = { 0, 15, 25, 31 };
		/// <summary>
		/// Definitions for speed levels (how long it takes a block to drop one row).
		/// Taken from NTSC NES Dr. Mario (https://tetris.wiki/Dr._Mario). Expressed here in time in seconds, rather than number of frames.
		/// </summary>
		float[] DropSpeeds = {
			1f / 60f * 70f,
			1f / 60f * 68f,
			1f / 60f * 66f,
			1f / 60f * 64f,
			1f / 60f * 62f,
			1f / 60f * 60f,
			1f / 60f * 58f,
			1f / 60f * 56f,
			1f / 60f * 54f,
			1f / 60f * 52f,
			1f / 60f * 50f,
			1f / 60f * 48f,
			1f / 60f * 46f,
			1f / 60f * 44f,
			1f / 60f * 42f,
			1f / 60f * 40f,
			1f / 60f * 38f,
			1f / 60f * 36f,
			1f / 60f * 34f,
			1f / 60f * 32f,
			1f / 60f * 30f,
			1f / 60f * 28f,
			1f / 60f * 26f,
			1f / 60f * 24f,
			1f / 60f * 22f,
			1f / 60f * 20f,
			1f / 60f * 19f,
			1f / 60f * 18f,
			1f / 60f * 17f,
			1f / 60f * 16f,
			1f / 60f * 15f,
			1f / 60f * 14f,
			1f / 60f * 13f,
			1f / 60f * 12f,
			1f / 60f * 11f,
			1f / 60f * 10f,
			1f / 60f * 10f,
			1f / 60f * 9f,
			1f / 60f * 9f,
			1f / 60f * 8f,
			1f / 60f * 8f,
			1f / 60f * 7f,
			1f / 60f * 7f,
			1f / 60f * 6f,
			1f / 60f * 6f,
			1f / 60f * 6f,
			1f / 60f * 6f,
			1f / 60f * 6f,
			1f / 60f * 6f,
			1f / 60f * 6f,
			1f / 60f * 6f,
			1f / 60f * 6f,
			1f / 60f * 6f,
			1f / 60f * 6f,
			1f / 60f * 6f,
			1f / 60f * 5f,
			1f / 60f * 5f,
			1f / 60f * 5f,
			1f / 60f * 5f,
			1f / 60f * 5f,
			1f / 60f * 4f,
			1f / 60f * 4f,
			1f / 60f * 4f,
			1f / 60f * 4f,
			1f / 60f * 4f,
			1f / 60f * 3f,
			1f / 60f * 3f,
			1f / 60f * 3f,
			1f / 60f * 3f,
			1f / 60f * 3f,
			1f / 60f * 2f,
			1f / 60f * 2f,
			1f / 60f * 2f,
			1f / 60f * 2f,
			1f / 60f * 2f,
			1f / 60f * 2f,
			1f / 60f * 2f,
			1f / 60f * 2f,
			1f / 60f * 2f,
			1f / 60f * 2f,
			1f / 60f * 1f
		};

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
			RNG = new Random();
			NextPieceIndex = 0;
			UpcomingPieces = new (int, int)[18];
			SetUpcoming();
			SetUpcoming();
			CursorActive = false;
			VirusCount = 0;
			CurrentLevel = level;
			State = PuzzleGameState.InControl;
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
			for (int j = BOARDHEIGHT - 1; j >= maxRow; j--)
			{
				for (int i = 0; i < BOARDWIDTH; i++)
				{
					float placeRoll = (float)RNG.NextDouble();
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
								VirusCount++;
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

		public void SetSpeed(int speed)
		{
			Speed = MathHelper.Clamp(speed, 1, 3);
			SpeedMulti = Speed;
		}

		void SetUpcoming()
		{
			(int, int)[] things = {
				(0, 0),
				(0, 1),
				(0, 2),
				(1, 0),
				(1, 1),
				(1, 2),
				(2, 0),
				(2, 1),
				(2, 2),
			};
			things = things.OrderBy(x => RNG.Next()).ToArray();
			for (int i = 0; i < 9; i++)
			{
				UpcomingPieces[i] = UpcomingPieces[i + 9];
				UpcomingPieces[i + 9] = things[i];
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
			// Here comes George, in control
			if (State == PuzzleGameState.InControl)
			{
				if (Input.JustPressed(Actions.RotateRight) || Input.JustPressed(Actions.RotateLeft))
				{
					CursorUpright = !CursorUpright;
					bool rotated = true;
					if (!IsEmpty(CursorPosPart2))
					{
						Point kickLeft = new Point(-1, 0);
						Point kickRight = new Point(1, 0);
						if (IsEmpty(CursorPos + kickLeft)
							&& IsEmpty(CursorPosPart2 + kickLeft))
						{
							CursorPos.X--;
						}
						else if (IsEmpty(CursorPos + kickRight)
							&& IsEmpty(CursorPosPart2 + kickRight))
						{
							CursorPos.X++;
						}
						else
						{
							CursorUpright = !CursorUpright;
							rotated = false;
						}
					}
					if (rotated
						&& ((!CursorUpright && Input.JustPressed(Actions.RotateRight))
						|| (CursorUpright && Input.JustPressed(Actions.RotateLeft))))
					{
						int tmp = CursorColor1;
						CursorColor1 = CursorColor2;
						CursorColor2 = tmp;
					}
				}
				if (!CursorActive)
				{
					NewPill();
				}
				if (doInput)
				{
					Point inputMove = new Point(InputDir, 0);
					if (IsEmpty(CursorPos + inputMove)
						&& IsEmpty(CursorPosPart2 + inputMove))
					{
						CursorPos.X += InputDir;
					}
				}
				// drop timer
				DropTime += dt;
				if (Input.PlayerDown)
				{
					DropTime += dt * 3;
				}
				if (DropTime >= DropInterval)
				{
					DropTime -= DropInterval;
					Point pos2 = CursorPosPart2;
					Point downMove = new Point(0, 1);
					if (!IsEmpty(CursorPos + downMove)
						|| !IsEmpty(pos2 + downMove))
					{
						Board[CursorPos.X, CursorPos.Y].Item1 = CursorUpright ? BoardSpace.PillLower : BoardSpace.PillLeft;
						Board[CursorPos.X, CursorPos.Y].Item2 = CursorColor1;
						if (pos2.Y >= 0)
						{
							Board[pos2.X, pos2.Y].Item1 = CursorUpright ? BoardSpace.PillUpper : BoardSpace.PillRight;
							Board[pos2.X, pos2.Y].Item2 = CursorColor2;
						}
						CursorActive = false;
						Pop4InARow();
						State = PuzzleGameState.Popping;
						StateTime = 0;
					}
					else
					{
						CursorPos.Y++;
					}
					CursorPos.Y %= BOARDHEIGHT;
				}
			}
			else if (State == PuzzleGameState.Popping)
			{
				StateTime += dt;
				if (StateTime >= CascadeRowTime)
				{
					ClearPopped();
					State = PuzzleGameState.Cascading;
					StateTime = 0;
					if (VirusCount == 0)
					{
						State = PuzzleGameState.Victory;
					}
				}
			}
			else if (State == PuzzleGameState.Cascading)
			{
				StateTime += dt;
				if (StateTime >= CascadeRowTime)
				{
					StateTime -= CascadeRowTime;
					if (!CascadeOneRow())
					{
						StateTime = 0;
						if (Pop4InARow())
						{
							State = PuzzleGameState.Popping;
						}
						else
						{
							State = PuzzleGameState.InControl;
						}
					}
				}
			}
			else if (State == PuzzleGameState.GameOver)
			{
				Game1.UI.OpenScreen(UIScreen.GameOverScreen(Game1));
			}
			else if (State == PuzzleGameState.Victory)
			{
				Game1.UI.OpenScreen(UIScreen.LevelCompleteScreen(Game1));
			}
			// pause button
			if (Game1.Input.Pause)
			{
				Game1.UI.TogglePause();
			}
		}

		void NewPill()
		{
			CurrentMulti = 1;
			CursorActive = true;
			CursorPos = new Point(3, 0);
			CursorUpright = false;
			// TODO: grab bag
			CursorColor1 = UpcomingPieces[NextPieceIndex].Item1;
			CursorColor2 = UpcomingPieces[NextPieceIndex].Item2;
			NextPieceIndex++;
			if (NextPieceIndex == 9)
			{
				NextPieceIndex = 0;
				SetUpcoming();
			}

			if (Board[3, 0].Item1 != BoardSpace.Blank || Board[4, 0].Item1 != BoardSpace.Blank)
			{
				State = PuzzleGameState.GameOver;
			}
		}
		/// <summary>
		/// Make freefloating pills fall 1 row.
		/// </summary>
		/// <returns>Whether any changes were made to the board</returns>
		bool CascadeOneRow()
		{
			bool changed = false;
			// starting 1 row from the bottom
			// bottom row can never drop
			for (int j = BOARDHEIGHT - 2; j >= 0; j--)
			{
				for (int i = 0; i < BOARDWIDTH; i++)
				{
					// Since we're sweeping from the lower left, we only
					// need to care about round, lower and left pills
					if (Board[i, j].Item1 == BoardSpace.PillRound)
					{
						if (Board[i, j + 1].Item1 == BoardSpace.Blank)
						{
							Board[i, j + 1] = Board[i, j];
							Board[i, j] = (BoardSpace.Blank, 0);
							changed = true;
						}
					}
					else if (Board[i, j].Item1 == BoardSpace.PillLower)
					{
						if (Board[i, j + 1].Item1 == BoardSpace.Blank)
						{
							Board[i, j + 1] = Board[i, j];
							Board[i, j] = Board[i, j - 1];
							Board[i, j - 1] = (BoardSpace.Blank, 0);
							changed = true;
						}
					}
					else if (Board[i, j].Item1 == BoardSpace.PillLeft)
					{
						if (Board[i, j + 1].Item1 == BoardSpace.Blank
							&& Board[i + 1, j + 1].Item1 == BoardSpace.Blank)
						{
							Board[i, j + 1] = Board[i, j];
							Board[i, j] = (BoardSpace.Blank, 0);
							Board[i + 1, j + 1] = Board[i + 1, j];
							Board[i + 1, j] = (BoardSpace.Blank, 0);
							changed = true;
						}
					}
				}
			}
			return changed;
		}
		/// <summary>
		/// Pop any occurrences of 4 in a row.
		/// Tiles are not removed immediately, but changed to an intermediate state.
		/// </summary>
		/// <returns>Whether any changes were made to the board</returns>
		bool Pop4InARow()
		{
			bool changed = false;
			const int ROWLENGTH = 4;
			// find rows and columns of 4 and replace them with popping
			for (int j = 0; j < BOARDHEIGHT; j++)
			{
				for (int i = 0; i < BOARDWIDTH; i++)
				{
					if (Board[i, j].Item1 != BoardSpace.Blank)
					{
						int rowColor = Board[i, j].Item2;
						int row = 1;
						int col = 1;
						for (int k = 1; k < ROWLENGTH; k++)
						{
							if (i + k < BOARDWIDTH)
							{
								row += (Board[i + k, j].Item2 == rowColor && Board[i + k, j].Item1 != BoardSpace.Blank) ? 1 : 0;
							}
							if (j + k < BOARDHEIGHT)
							{
								col += (Board[i, j + k].Item2 == rowColor && Board[i, j + k].Item1 != BoardSpace.Blank) ? 1 : 0;
							}
						}
						// convert tiles, award points for viruses
						if (row >= ROWLENGTH)
						{
							for (int k = 0; k < ROWLENGTH; k++)
							{
								if (Board[i + k, j].Item1 == BoardSpace.Virus)
								{
									Points += 100 * CurrentMulti * SpeedMulti;
									CurrentMulti++;
									VirusCount--;
								}
								Board[i + k, j].Item1 = BoardSpace.Popping;
								changed = true;
							}
						}
						if (col >= ROWLENGTH)
						{
							for (int k = 0; k < ROWLENGTH; k++)
							{
								if (Board[i, j + k].Item1 == BoardSpace.Virus)
								{
									Points += 100 * CurrentMulti * SpeedMulti;
									CurrentMulti++;
									VirusCount--;
								}
								Board[i, j + k].Item1 = BoardSpace.Popping;
								changed = true;
							}
						}
					}
				}
			}
			// Change orphaned half pills to round pills
			for (int j = 0; j < BOARDHEIGHT; j++)
			{
				for (int i = 0; i < BOARDWIDTH; i++)
				{
					Point checkOffset = Point.Zero;
					switch (Board[i, j].Item1)
					{
						case BoardSpace.PillLeft:
							checkOffset.X = 1;
							break;
						case BoardSpace.PillRight:
							checkOffset.X = -1;
							break;
						case BoardSpace.PillUpper:
							checkOffset.Y = 1;
							break;
						case BoardSpace.PillLower:
							checkOffset.Y = -1;
							break;
					}
					if (checkOffset != Point.Zero
						&& (j + checkOffset.Y < 0
						|| Board[i + checkOffset.X, j + checkOffset.Y].Item1 == BoardSpace.Popping))
					{
						Board[i, j].Item1 = BoardSpace.PillRound;
					}
				}
			}
			return changed;
		}
		/// <summary>
		/// Remove any popping tiles from the board.
		/// </summary>
		/// <returns>Whether any changes were made to the board</returns>
		bool ClearPopped()
		{
			bool changed = false;
			for (int j = 0; j < BOARDHEIGHT; j++)
			{
				for (int i = 0; i < BOARDWIDTH; i++)
				{
					if (Board[i, j].Item1 == BoardSpace.Popping)
					{
						Board[i, j] = (BoardSpace.Blank, 0);
						changed = true;
					}
				}
			}
			return changed;
		}

		bool IsEmpty(Point pos)
		{
			if (pos.Y < 0)
			{
				return true;
			}
			else if (pos.X < 0
				|| pos.X >= BOARDWIDTH
				|| pos.Y >= BOARDHEIGHT)
			{
				return false;
			}
			return Board[pos.X, pos.Y].Item1 == BoardSpace.Blank;
		}

		public override void Draw(GameTime gameTime)
		{
			Rectangle bg = new Rectangle((int)TopLeft.X, (int)TopLeft.Y, BOARDWIDTH * TILESIZE, BOARDHEIGHT * TILESIZE);
			Rectangle cursor = new Rectangle((int)TopLeft.X + CursorPos.X * TILESIZE, (int)TopLeft.Y + CursorPos.Y * TILESIZE, TILESIZE, TILESIZE);
			bg = Camera.ScreenRect(bg);
			cursor = Camera.ScreenRect(cursor);
			SpriteBatch.Draw(Game1.Pixel, bg, Color.Black);
			SpriteBatch.Draw(Game1.Pixel, cursor, Color.White);

			Rectangle blockSrc = new Rectangle(0, 0, 16, 16);
			Rectangle pillSrc = new Rectangle(0, 16, 16, 16);
			Rectangle roundSrc = new Rectangle(16, 16, 16, 16);
			Rectangle popSrc = new Rectangle(48, 16, 16, 16);

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
							SpriteBatch.Draw(Game1.Blocks, drawRect, blockSrc, Colors[Board[i, j].Item2], 0, spaceOrigin, SpriteEffects.None, 0.5f);
							break;
						case BoardSpace.PillRound:
							SpriteBatch.Draw(Game1.Blocks, drawRect, roundSrc, Colors[Board[i, j].Item2], 0, spaceOrigin, SpriteEffects.None, 0.5f);
							break;
						case BoardSpace.PillLeft:
							SpriteBatch.Draw(Game1.Blocks, drawRect, pillSrc, Colors[Board[i, j].Item2], 0, spaceOrigin, SpriteEffects.None, 0.5f);
							break;
						case BoardSpace.PillRight:
							SpriteBatch.Draw(Game1.Blocks, drawRect, pillSrc, Colors[Board[i, j].Item2], MathHelper.Pi, spaceOrigin, SpriteEffects.None, 0.5f);
							break;
						case BoardSpace.PillUpper:
							SpriteBatch.Draw(Game1.Blocks, drawRect, pillSrc, Colors[Board[i, j].Item2], MathHelper.PiOver2, spaceOrigin, SpriteEffects.None, 0.5f);
							break;
						case BoardSpace.PillLower:
							SpriteBatch.Draw(Game1.Blocks, drawRect, pillSrc, Colors[Board[i, j].Item2], MathHelper.PiOver2 * 3, spaceOrigin, SpriteEffects.None, 0.5f);
							break;
						case BoardSpace.Popping:
							SpriteBatch.Draw(Game1.Blocks, drawRect, popSrc, Colors[Board[i, j].Item2], 0, spaceOrigin, SpriteEffects.None, 0.5f);
							break;
						default:
							break;
					}
				}
			}
			if (CursorActive)
			{
				Rectangle currentPill1 = new Rectangle((int)TopLeft.X + 4 + CursorPos.X * TILESIZE, (int)TopLeft.Y + 5 + CursorPos.Y * TILESIZE, TILESIZE, TILESIZE);
				Rectangle currentPill2 = new Rectangle((int)TopLeft.X + 4 + CursorPos.X * TILESIZE, (int)TopLeft.Y + 5 + CursorPos.Y * TILESIZE, TILESIZE, TILESIZE);
				float rot1, rot2;
				if (CursorUpright)
				{
					currentPill2.Y -= TILESIZE;
					rot1 = MathHelper.PiOver2 * 3;
					rot2 = MathHelper.PiOver2;
				}
				else
				{
					currentPill2.X += TILESIZE;
					rot1 = 0;
					rot2 = MathHelper.Pi;
				}
				currentPill1 = Camera.ScreenRect(currentPill1);
				currentPill2 = Camera.ScreenRect(currentPill2);
				SpriteBatch.Draw(Game1.Blocks, currentPill1, pillSrc, Colors[CursorColor1], rot1, spaceOrigin, SpriteEffects.None, 0.5f);
				SpriteBatch.Draw(Game1.Blocks, currentPill2, pillSrc, Colors[CursorColor2], rot2, spaceOrigin, SpriteEffects.None, 0.5f);
			}
			Vector2 upcomingPos = TopLeft;
			upcomingPos.X += TILESIZE * BOARDWIDTH + 1;

			if (true)
			{
				Rectangle upcomingLeft = new Rectangle((int)upcomingPos.X + 4, (int)upcomingPos.Y + 4 * TILESIZE, TILESIZE, TILESIZE);
				Rectangle upcomingRight = new Rectangle((int)upcomingPos.X + 4 + TILESIZE, (int)upcomingPos.Y + 4 * TILESIZE, TILESIZE, TILESIZE);
				upcomingLeft = Camera.ScreenRect(upcomingLeft);
				upcomingRight = Camera.ScreenRect(upcomingRight);
				SpriteBatch.Draw(Game1.Blocks, upcomingLeft, pillSrc, Colors[UpcomingPieces[NextPieceIndex].Item1], 0, spaceOrigin, SpriteEffects.None, 0.5f);
				SpriteBatch.Draw(Game1.Blocks, upcomingRight, pillSrc, Colors[UpcomingPieces[NextPieceIndex].Item2], MathHelper.Pi, spaceOrigin, SpriteEffects.None, 0.5f);
			}
			else
			{
				// draw full queue
				Rectangle currentIndicator = new Rectangle((int)upcomingPos.X, (int)upcomingPos.Y + NextPieceIndex * TILESIZE, TILESIZE * 3, TILESIZE);
				currentIndicator = Camera.ScreenRect(currentIndicator);
				SpriteBatch.Draw(Game1.Pixel, currentIndicator, null, Color.White, 0, Vector2.Zero, SpriteEffects.None, 0.5f);
				for (int i = 0; i < UpcomingPieces.Length; i++)
				{
					Rectangle upcomingLeft = new Rectangle((int)upcomingPos.X + 4, (int)upcomingPos.Y + 4 + i * TILESIZE, TILESIZE, TILESIZE);
					Rectangle upcomingRight = new Rectangle((int)upcomingPos.X + 4 + TILESIZE, (int)upcomingPos.Y + 4 + i * TILESIZE, TILESIZE, TILESIZE);
					upcomingLeft = Camera.ScreenRect(upcomingLeft);
					upcomingRight = Camera.ScreenRect(upcomingRight);
					SpriteBatch.Draw(Game1.Blocks, upcomingLeft, pillSrc, Colors[UpcomingPieces[i].Item1], 0, spaceOrigin, SpriteEffects.None, 0.5f);
					SpriteBatch.Draw(Game1.Blocks, upcomingRight, pillSrc, Colors[UpcomingPieces[i].Item2], MathHelper.Pi, spaceOrigin, SpriteEffects.None, 0.5f);
				}
			}
		}
	}
}
