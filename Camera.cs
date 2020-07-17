using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PuzzleGame
{
	class Camera
	{
		Vector2 Position = Vector2.Zero;
		public Vector2 TopLeft => Position;
		public Vector2 TopRight => Position + new Vector2(Width, 0);
		public Vector2 BotLeft => Position + new Vector2(0, Height);
		public Vector2 BotRight => Position + new Vector2(Width, Height);
		int Width = 320;
		int Height = 180;
		/// <summary>
		/// Where to point the camera.
		/// </summary>
		public Vector2 Center
		{
			get
			{
				return new Vector2(Position.X + Width / 2, Position.Y + Height / 2);
			}
			set
			{
				value.X -= Width / 2;
				value.Y -= Height / 2;
				Position = value;
			}
		}

		Vector2 AspectRatioCorrection = Vector2.Zero;
		public float Scale { get; private set; } = 1f;

		/// <summary>
		/// Update Scale and AspectRatioCorrection when the size of the viewport changes.
		/// </summary>
		/// <param name="w">New viewport width</param>
		/// <param name="h">New viewport height</param>
		public void SetWindowSize(int w, int h)
		{
			float wScale = (float)w / Width;
			float hScale = (float)h / Height;
			Scale = Math.Min(wScale, hScale);
			Vector2 squareWindow = new Vector2(w / Scale, h / Scale);
			Vector2 camSize = new Vector2(Width, Height);
			AspectRatioCorrection = (camSize - squareWindow) / 2;
		}
		/// <summary>
		/// Convert world coordinates to screen coordinates
		/// </summary>
		/// <param name="worldPos">World coordinates</param>
		/// <returns>Screen coordinates</returns>
		public Vector2 ScreenPos(Vector2 worldPos)
		{
			Vector2 FloorPos = Position;
			FloorPos.X = MathF.Floor(FloorPos.X);
			FloorPos.Y = MathF.Floor(FloorPos.Y);
			return (worldPos - FloorPos - AspectRatioCorrection) * Scale;
		}
		/// <summary>
		/// Convert a rectangle from world coordinates to screen coordinates
		/// </summary>
		/// <param name="worldRect">Rectangle with world coordinates</param>
		/// <returns>Rectangle with screen coordinates</returns>
		public Rectangle ScreenRect(Rectangle worldRect)
		{
			Vector2 FloorPos = Position;
			FloorPos.X = MathF.Floor(FloorPos.X);
			FloorPos.Y = MathF.Floor(FloorPos.Y);
			int x = (int)((worldRect.X - FloorPos.X - AspectRatioCorrection.X) * Scale);
			int y = (int)((worldRect.Y - FloorPos.Y - AspectRatioCorrection.Y) * Scale);
			int w = (int)Math.Ceiling(worldRect.Width * Scale);
			int h = (int)Math.Ceiling(worldRect.Height * Scale);
			return new Rectangle(x, y, w, h);
		}
		/// <summary>
		/// Convert screen coordinates to world coordinates
		/// </summary>
		/// <param name="screenPos">Screen coordinates</param>
		/// <returns>World coordinates</returns>
		public Vector2 WorldPos(Vector2 screenPos)
		{
			Vector2 FloorPos = Position;
			FloorPos.X = MathF.Floor(FloorPos.X);
			FloorPos.Y = MathF.Floor(FloorPos.Y);
			return FloorPos + AspectRatioCorrection + screenPos / Scale;
		}
		/// <summary>
		/// Convert a rectangle from screen coordinates to world coordinates
		/// </summary>
		/// <param name="screenRect">Rectangle with screen coordinates</param>
		/// <returns>Rectangle with world coordinates</returns>
		public Rectangle WorldRect(Rectangle screenRect)
		{
			Vector2 FloorPos = Position;
			FloorPos.X = MathF.Floor(FloorPos.X);
			FloorPos.Y = MathF.Floor(FloorPos.Y);
			int x = (int)(FloorPos.X + AspectRatioCorrection.X + screenRect.X / Scale);
			int y = (int)(FloorPos.Y + AspectRatioCorrection.Y + screenRect.Y / Scale);
			int w = (int)(screenRect.Width / Scale);
			int h = (int)(screenRect.Height / Scale);
			return new Rectangle(x, y, w, h);
		}

	}
}
