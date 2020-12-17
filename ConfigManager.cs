using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace PuzzleGame
{
	enum WindowMode
	{
		Fullscreen,
		Windowed,
		BorderlessWindow
	}
	struct Config
	{
		public int Volume;
		public DisplayMode Resolution;
		public WindowMode WindowMode;
	}
	class ConfigManager
	{
		public ConfigManager()
		{
			SetToDefaults();
			Load();
		}
		const string FOLDERNAME = "PuzzleGame";
		const string CONFIGFILENAME = "conf.json";
		const string KEYSFILENAME = "keys.json";
		const string SCOREFILENAME = "highscore.json";
		public string ConfigFolderPath => Path.Combine(
			Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
			FOLDERNAME);
		public string ConfigFilePath => Path.Combine(
			Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
			FOLDERNAME,
			CONFIGFILENAME);
		public string KeysFilePath => Path.Combine(
			Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
			FOLDERNAME,
			KEYSFILENAME);
		public string ScoreFilePath => Path.Combine(
			Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
			FOLDERNAME,
			SCOREFILENAME);
		public bool UnappliedChanges => !Config.Equals(NewConfig);
		public Config Config = new Config();
		public Config NewConfig = new Config();

		public void Save()
		{
			Config = NewConfig;
			SoundEffect.MasterVolume = Config.Volume / 100f;

			var sb = new StringBuilder();
			var sw = new StringWriter(sb);
			using (var writer = new JsonTextWriter(sw))
			{
				writer.Formatting = Formatting.Indented;
				writer.WriteStartObject();
				writer.WritePropertyName("volume");
				writer.WriteValue(Config.Volume);
				writer.WritePropertyName("mode");
				writer.WriteValue(Config.WindowMode.ToString());
				writer.WritePropertyName("width");
				writer.WriteValue(Config.Resolution.Width);
				writer.WritePropertyName("height");
				writer.WriteValue(Config.Resolution.Height);
				writer.WriteEndObject();
			}
			File.WriteAllText(ConfigFilePath, sb.ToString());
			
		}

		public void RevertChanges()
		{
			NewConfig = Config;
		}

		public void CreateConfigFolder()
		{
			Directory.CreateDirectory(ConfigFolderPath);
		}

		public void Load()
		{
			if (File.Exists(ConfigFilePath))
			{
				var confText = File.ReadAllText(ConfigFilePath);
				JObject o;
				try
				{
					o = JObject.Parse(confText);
				}
				catch (Exception e)
				{
					Debug.WriteLine($"Couldn't parse {ConfigFilePath}");
					Debug.WriteLine(e.Message);
					return;
				}
				try
				{
					Config.Volume = (int)o.SelectToken("volume");
				}
				catch (Exception)
				{
					Debug.WriteLine("Unable to read volume from config");
				}
				try
				{
					Config.WindowMode = (WindowMode)Enum.Parse(typeof(WindowMode), o.SelectToken("mode").ToString());
				}
				catch (Exception)
				{
					Debug.WriteLine("Unable to read window mode from config");
				}
				int w = 0;
				int h = 0;
				try
				{
					w = (int)o.SelectToken("width");
				}
				catch (Exception)
				{
					Debug.WriteLine("Unable to read width from config");
				}
				try
				{
					h = (int)o.SelectToken("height");
				}
				catch (Exception)
				{
					Debug.WriteLine("Unable to read height from config");
				}
				bool resolutionFound = false;
				foreach (var item in GraphicsAdapter.DefaultAdapter.SupportedDisplayModes)
				{
					if (item.Width == w && item.Height == h)
					{
						Config.Resolution = item;
						resolutionFound = true;
						break;
					}
				}
				if (!resolutionFound)
				{
					Debug.WriteLine($"Resolution {w}x{h} not found in supported display modes");
				}
				NewConfig = Config;
			}
			else
			{
				Debug.WriteLine("config file not found");
			}
		}

		void SetToDefaults()
		{
			Config.Resolution = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode;
			Config.WindowMode = WindowMode.Windowed;
			Config.Volume = 50;
			NewConfig = Config;
		}

		public void ApplyGraphics(GraphicsDeviceManager graphics, GameWindow window)
		{
			graphics.PreferredBackBufferWidth = Config.Resolution.Width;
			graphics.PreferredBackBufferHeight = Config.Resolution.Height;
			graphics.ApplyChanges();
			graphics.IsFullScreen = Config.WindowMode == WindowMode.Fullscreen;
			graphics.ApplyChanges();
			window.IsBorderless = Config.WindowMode == WindowMode.BorderlessWindow;
			if (Config.WindowMode != WindowMode.Fullscreen)
			{
				int sw = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
				int sh = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
				int ww = window.ClientBounds.Width;
				int wh = window.ClientBounds.Height;
				window.Position = new Point(sw / 2 - ww / 2, sh / 2 - wh / 2);
			}
		}
	}
}
