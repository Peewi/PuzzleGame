using Microsoft.Xna.Framework;
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
		Blank = 0,
		Virus = 1,
		PillLeft = 2,
		PillRight = 3,
		PillUpper = 4,
		PillLower = 5,
		PillRound = 6,
		Popping = 7
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
		int SpeedIncrease = 0;
		const int MAXSPEEDINCREASE = 49;
		int SpeedMulti = 1;
		int PiecesDropped = 0;
		int CurrentMulti = 1;
		int InputDir = 0;
		float InputTimeHeld = 0;
		float InputRepeatDelay = 0.2f;
		float InputRepeatInterval = 0.1f;
		float DropInterval => DropSpeeds[StartSpeedIndex[Speed] + SpeedIncrease];
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
		readonly float[] DropSpeeds = {
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
		const int SPRITESIZE = 16;
		/// <summary>
		/// Spritesheet source rectangles. Matches <c>(int)Boardspace - 1</c>
		/// </summary>
		readonly Rectangle[] SpriteSrc =
		{
			new Rectangle(0 * SPRITESIZE, 0, SPRITESIZE, SPRITESIZE),
			new Rectangle(1 * SPRITESIZE, 0, SPRITESIZE, SPRITESIZE),
			new Rectangle(2 * SPRITESIZE, 0, SPRITESIZE, SPRITESIZE),
			new Rectangle(3 * SPRITESIZE, 0, SPRITESIZE, SPRITESIZE),
			new Rectangle(4 * SPRITESIZE, 0, SPRITESIZE, SPRITESIZE),
			new Rectangle(5 * SPRITESIZE, 0, SPRITESIZE, SPRITESIZE),
			new Rectangle(6 * SPRITESIZE, 0, SPRITESIZE, SPRITESIZE),
			new Rectangle(7 * SPRITESIZE, 0, SPRITESIZE, SPRITESIZE),
			new Rectangle(0 * SPRITESIZE, SPRITESIZE, SPRITESIZE, SPRITESIZE),
			new Rectangle(1 * SPRITESIZE, SPRITESIZE, SPRITESIZE, SPRITESIZE),
			new Rectangle(2 * SPRITESIZE, SPRITESIZE, SPRITESIZE, SPRITESIZE),
			new Rectangle(3 * SPRITESIZE, SPRITESIZE, SPRITESIZE, SPRITESIZE),
			new Rectangle(4 * SPRITESIZE, SPRITESIZE, SPRITESIZE, SPRITESIZE),
			new Rectangle(5 * SPRITESIZE, SPRITESIZE, SPRITESIZE, SPRITESIZE),
			new Rectangle(6 * SPRITESIZE, SPRITESIZE, SPRITESIZE, SPRITESIZE),
			new Rectangle(7 * SPRITESIZE, SPRITESIZE, SPRITESIZE, SPRITESIZE)
		};
		readonly Color[] Colors = { Color.Red, Color.Blue, Color.Yellow };
		const int NUMBEROFCOLORS = 3;
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
			UpcomingPieces = new (int, int)[NUMBEROFCOLORS * NUMBEROFCOLORS * 2];
			SetUpcoming();
			SetUpcoming();
			CursorActive = false;
			VirusCount = 0;
			CurrentLevel = level;
			State = PuzzleGameState.InControl;
			PiecesDropped = 0;
			SpeedIncrease = 0;
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
			int[] colorsPlaced = new int[NUMBEROFCOLORS + 1];
			for (int j = BOARDHEIGHT - 1; j >= maxRow; j--)
			{
				for (int i = 0; i < BOARDWIDTH; i++)
				{
					float placeRoll = (float)RNG.NextDouble();
					// 2-away rule
					bool[] available = new bool[NUMBEROFCOLORS + 1];
					for (int k = 0; k < NUMBEROFCOLORS + 1; k++)
					{
						available[k] = true;
					}
					if (i - 2 >= 0 && Board[i - 2, j].Item1 == BoardSpace.Virus)
					{
						available[Board[i - 2, j].Item2 + 1] = false;
					}
					if (j + 2 < BOARDHEIGHT && Board[i, j + 2].Item1 == BoardSpace.Virus)
					{
						available[Board[i, j + 2].Item2 + 1] = false;
					}
					int numColorsAvailable = 0;
					for (int k = 1; k < NUMBEROFCOLORS + 1; k++)
					{
						numColorsAvailable += available[k] ? 1 : 0;
					}
					// chance stuff
					float[] chanceSize = new float[NUMBEROFCOLORS + 1];
					chanceSize[0] = cellsLeft - virusesLeft;
					float[] colorChances = new float[NUMBEROFCOLORS + 1];
					colorChances[0] = (cellsLeft - virusesLeft) / (float)cellsLeft;
					float virusChance = 1 - colorChances[0];
					int mostPlaced = colorsPlaced[0];
					for (int k = 0; k < NUMBEROFCOLORS; k++)
					{
						mostPlaced = Math.Max(mostPlaced, colorsPlaced[k + 1]);
					}
					float combinedChanceSize = 0;
					for (int k = 1; k < NUMBEROFCOLORS + 1; k++)
					{
						chanceSize[k] = available[k] ? (float)virusesLeft / numColorsAvailable : 0;
						for (int l = colorsPlaced[k]; l < mostPlaced; l++)
						{
							chanceSize[k] *= 3;
							combinedChanceSize += chanceSize[k];
						}
					}
					for (int k = 1; k < NUMBEROFCOLORS + 1; k++)
					{
						colorChances[k] = chanceSize[k] / combinedChanceSize;
						colorChances[k] *= virusChance;
					}
					float colorRunning = 0;

					for (int k = 0; k < NUMBEROFCOLORS + 1; k++)
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
			for (int i = 0; i < NUMBEROFCOLORS; i++)
			{
				if (colorsPlaced[i+1] == 0)
				{
					NewGame(level);
					break;
				}
			}
		}

		public void SetSpeed(int speed)
		{
			Speed = MathHelper.Clamp(speed, 1, 3);
			SpeedMulti = Speed;
		}

		void SetUpcoming()
		{
			int possiblePieces = NUMBEROFCOLORS * NUMBEROFCOLORS;
			(int, int)[] things = new (int, int)[possiblePieces];
			for (int i = 0; i < NUMBEROFCOLORS; i++)
			{
				for (int j = 0; j < NUMBEROFCOLORS; j++)
				{
					things[i * NUMBEROFCOLORS + j] = (i, j);
				}
			}
			things = things.OrderBy(x => RNG.Next()).ToArray();
			for (int i = 0; i < possiblePieces; i++)
			{
				UpcomingPieces[i] = UpcomingPieces[i + possiblePieces];
				UpcomingPieces[i + possiblePieces] = things[i];
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
			PiecesDropped++;
			if (PiecesDropped % 10 == 0)
			{
				SpeedIncrease = Math.Min(SpeedIncrease + 1, MAXSPEEDINCREASE);
			}
			CursorActive = true;
			CursorPos = new Point(3, 0);
			CursorUpright = false;
			CursorColor1 = UpcomingPieces[NextPieceIndex].Item1;
			CursorColor2 = UpcomingPieces[NextPieceIndex].Item2;
			NextPieceIndex++;
			if (NextPieceIndex == NUMBEROFCOLORS * NUMBEROFCOLORS)
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
			bg = Camera.ScreenRect(bg);
			SpriteBatch.Draw(Game1.Pixel, bg, Color.Black);

			Vector2 spaceOrigin = new Vector2(8, 8);

			for (int i = 0; i < BOARDWIDTH; i++)
			{
				for (int j = 0; j < BOARDHEIGHT; j++)
				{
					if (Board[i, j].Item1 != BoardSpace.Blank)
					{
						Rectangle drawRect = new Rectangle((int)TopLeft.X + 4 + i * TILESIZE, (int)TopLeft.Y + 4 + j * TILESIZE, TILESIZE, TILESIZE);
						drawRect = Camera.ScreenRect(drawRect);
						SpriteBatch.Draw(Game1.Blocks, drawRect, SpriteSrc[(int)Board[i, j].Item1 - 1], Colors[Board[i, j].Item2], 0, spaceOrigin, SpriteEffects.None, 0.5f);
						SpriteBatch.Draw(Game1.Blocks, drawRect, SpriteSrc[(int)Board[i, j].Item1 - 1 + 8], Color.White, 0, spaceOrigin, SpriteEffects.None, 0.5f);
					}
				}
			}
			if (CursorActive)
			{
				Rectangle currentPill1 = new Rectangle((int)TopLeft.X + 4 + CursorPos.X * TILESIZE, (int)TopLeft.Y + 5 + CursorPos.Y * TILESIZE, TILESIZE, TILESIZE);
				Rectangle currentPill2 = new Rectangle((int)TopLeft.X + 4 + CursorPos.X * TILESIZE, (int)TopLeft.Y + 5 + CursorPos.Y * TILESIZE, TILESIZE, TILESIZE);
				int src1, src2;
				if (CursorUpright)
				{
					currentPill2.Y -= TILESIZE;
					src1 = 4;
					src2 = 3;
				}
				else
				{
					currentPill2.X += TILESIZE;
					src1 = 1;
					src2 = 2;
				}
				int hilight1 = src1;
				hilight1 += 8;
				int hilight2 = src2;
				hilight2 += 8;
				currentPill1 = Camera.ScreenRect(currentPill1);
				currentPill2 = Camera.ScreenRect(currentPill2);
				SpriteBatch.Draw(Game1.Blocks, currentPill1, SpriteSrc[src1], Colors[CursorColor1], 0, spaceOrigin, SpriteEffects.None, 0.5f);
				SpriteBatch.Draw(Game1.Blocks, currentPill2, SpriteSrc[src2], Colors[CursorColor2], 0, spaceOrigin, SpriteEffects.None, 0.5f);
				SpriteBatch.Draw(Game1.Blocks, currentPill1, SpriteSrc[hilight1], Color.White, 0, spaceOrigin, SpriteEffects.None, 0.5f);
				SpriteBatch.Draw(Game1.Blocks, currentPill2, SpriteSrc[hilight2], Color.White, 0, spaceOrigin, SpriteEffects.None, 0.5f);
			}
			Vector2 upcomingPos = TopLeft;
			upcomingPos.X += TILESIZE * BOARDWIDTH + 1;

			Rectangle upcomingLeft = new Rectangle((int)upcomingPos.X + 4, (int)upcomingPos.Y + 4 * TILESIZE, TILESIZE, TILESIZE);
			Rectangle upcomingRight = new Rectangle((int)upcomingPos.X + 4 + TILESIZE, (int)upcomingPos.Y + 4 * TILESIZE, TILESIZE, TILESIZE);
			upcomingLeft = Camera.ScreenRect(upcomingLeft);
			upcomingRight = Camera.ScreenRect(upcomingRight);
			SpriteBatch.Draw(Game1.Blocks, upcomingLeft, SpriteSrc[1], Colors[UpcomingPieces[NextPieceIndex].Item1], 0, spaceOrigin, SpriteEffects.None, 0.5f);
			SpriteBatch.Draw(Game1.Blocks, upcomingLeft, SpriteSrc[1+8], Color.White, 0, spaceOrigin, SpriteEffects.None, 0.5f);
			SpriteBatch.Draw(Game1.Blocks, upcomingRight, SpriteSrc[2], Colors[UpcomingPieces[NextPieceIndex].Item2], 0, spaceOrigin, SpriteEffects.None, 0.5f);
			SpriteBatch.Draw(Game1.Blocks, upcomingRight, SpriteSrc[2 + 8], Color.White, 0, spaceOrigin, SpriteEffects.None, 0.5f);

		}
	}
}
