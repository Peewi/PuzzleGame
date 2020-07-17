using PuzzleGame.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.IO;
namespace PuzzleGame
{
	/// <summary>
	/// This is the main type for your game.
	/// </summary>
	class Game1 : Game
	{
		GraphicsDeviceManager graphics;
		public SpriteBatch SpriteBatch;
		Texture2D circle;
		public Texture2D Circle8px;
		public Texture2D Happy;
		public Texture2D Mad;
		public Texture2D Splosion;
		public Texture2D UISheet;
		public SpriteFont Font;
		public Texture2D Pixel;
		public Texture2D Blocks;
		public Camera Camera = new Camera();
		public Camera UICamera = new Camera();
		public UIManager UI;
		public InputManager Input;
		public PuzzleBoard Board;
		public ConfigManager Config;
		public SoundManager Sound;
		public int Score = 0;
		bool CurrentlyPlaying = false;

		public Game1()
		{
			graphics = new GraphicsDeviceManager(this);
			Content.RootDirectory = "Content";
			IsMouseVisible = true;
			Window.AllowUserResizing = true;
			Window.ClientSizeChanged += (sender, e) =>
			{
				Camera.SetWindowSize(Window.ClientBounds.Width, Window.ClientBounds.Height);
				UICamera.SetWindowSize(Window.ClientBounds.Width, Window.ClientBounds.Height);
			};
			graphics.PreparingDeviceSettings += (sender, e) =>
			{
				Camera.SetWindowSize(Window.ClientBounds.Width, Window.ClientBounds.Height);
				UICamera.SetWindowSize(Window.ClientBounds.Width, Window.ClientBounds.Height);
			};
		}

		/// <summary>
		/// Allows the game to perform any initialization it needs to before starting to run.
		/// This is where it can query for any required services and load any non-graphic
		/// related content.  Calling base.Initialize will enumerate through any components
		/// and initialize them as well.
		/// </summary>
		protected override void Initialize()
		{
			// TODO: Add your initialization logic here
			Input = new InputManager(this);
			Components.Add(Input);
			UI = new UIManager(this);
			Components.Add(UI);
			Config = new ConfigManager();
			Config.CreateConfigFolder();
			ApplyGraphicsConfig();
			Input.LoadKeybinds(Config.KeysFilePath);
			Sound = new SoundManager(this);
			Components.Add(Sound);

			base.Initialize();
		}

		/// <summary>
		/// LoadContent will be called once per game and is the place to load
		/// all of your content.
		/// </summary>
		protected override void LoadContent()
		{
			// Create a new SpriteBatch, which can be used to draw textures.
			SpriteBatch = new SpriteBatch(GraphicsDevice);

			// TODO: use this.Content to load your game content here
			circle = Content.Load<Texture2D>("circle");
			Circle8px = Content.Load<Texture2D>("circle8px");
			Blocks = Content.Load<Texture2D>("blocks");
			Happy = Content.Load<Texture2D>("Happy");
			Mad = Content.Load<Texture2D>("Mad");
			Splosion = Content.Load<Texture2D>("splosion");
			UISheet = Content.Load<Texture2D>("UIpackSheet_transparent");
			Pixel = new Texture2D(GraphicsDevice, 1, 1, false, SurfaceFormat.Color);
			Pixel.SetData(new Color[] { Color.White });
			Font = Content.Load<SpriteFont>("MainFont");
			Sound.LoadSounds();
		}

		/// <summary>
		/// UnloadContent will be called once per game and is the place to unload
		/// game-specific content.
		/// </summary>
		protected override void UnloadContent()
		{
			// TODO: Unload any non ContentManager content here
		}

		/// <summary>
		/// Allows the game to run logic such as updating the world,
		/// checking for collisions, gathering input, and playing audio.
		/// </summary>
		/// <param name="gameTime">Provides a snapshot of timing values.</param>
		protected override void Update(GameTime gameTime)
		{
			// Doing all my game logic through GameComponents.

			base.Update(gameTime);
		}

		public void ApplyGraphicsConfig()
		{
			Config.ApplyGraphics(graphics, Window);
		}

		public void NewGame()
		{
			UI.OpenScreen(UIScreen.GameHUDScreen(this));
			Components.Remove(Board);
			Board = new PuzzleBoard(this);
			Components.Add(Board);
			Camera.Center = new Vector2(160, 90);
			CurrentlyPlaying = true;
			Score = 0;
		}

		public void MainMenu()
		{
			Components.Remove(UI);
			UI = new UIManager(this);
			Components.Add(UI);
			Components.Remove(Board);
			CurrentlyPlaying = false;
		}

		public void GameOver()
		{
			CurrentlyPlaying = false;
			UI.OpenScreen(UIScreen.GameOverScreen(this));
		}

		/// <summary>
		/// This is called when the game should draw itself.
		/// </summary>
		/// <param name="gameTime">Provides a snapshot of timing values.</param>
		protected override void Draw(GameTime gameTime)
		{
			GraphicsDevice.Clear(Color.CornflowerBlue);

			// TODO: Add your drawing code here
			SpriteBatch.Begin(samplerState: SamplerState.PointClamp);
			//Rectangle topLeft = Camera.ScreenRect(new Rectangle(0, 0, 25, 25));
			//Rectangle botRight = Camera.ScreenRect(new Rectangle(295, 155, 25, 25));
			//SpriteBatch.Draw(Pixel, topLeft, Color.White);
			//SpriteBatch.Draw(circle, botRight, Color.White);
			//SpriteBatch.DrawString(Font, "helloooooooooo", Vector2.Zero, Color.Black);
			base.Draw(gameTime);
			SpriteBatch.End();
		}
	}
}
