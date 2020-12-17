using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace PuzzleGame
{
	class HighScore
	{
		const int MAXSCORES = 10;
		public List<(string, int)> Scores = new List<(string, int)>();
		Game1 Game;
		
		public HighScore(Game1 game)
		{
			Game = game;
			for (int i = 0; i < 10; i++)
			{
				Scores.Add(("NAME", 100));
			}
			Load();
		}

		public void Add(string name, int score)
		{
			Scores.Add((name, score));
			Scores.Sort((x, y) => y.Item2.CompareTo(x.Item2));
			if (Scores.Count > MAXSCORES)
			{
				Scores.RemoveAt(Scores.Count - 1);
			}
		}

		public bool HighScoreAchieved(int score)
		{
			return score > Scores[^1].Item2;
		}

		public void Save()
		{
			var sb = new StringBuilder();
			var sw = new StringWriter(sb);
			using (var writer = new JsonTextWriter(sw))
			{
				writer.Formatting = Formatting.Indented;
				writer.WriteStartObject();
				writer.WritePropertyName("scores");
				writer.WriteStartArray();
				foreach (var item in Scores)
				{
					writer.WriteStartObject();
					writer.WritePropertyName("name");
					writer.WriteValue(item.Item1);
					writer.WritePropertyName("score");
					writer.WriteValue(item.Item2);
					writer.WriteEndObject();
				}
				writer.WriteEndArray();
				writer.WriteEndObject();
			}
			File.WriteAllText(Game.Config.ScoreFilePath, sb.ToString());
		}

		public void Load()
		{
			if (File.Exists(Game.Config.ScoreFilePath))
			{
				var scoreText = File.ReadAllText(Game.Config.ScoreFilePath);
				JObject o;
				try
				{
					o = JObject.Parse(scoreText);
				}
				catch (Exception e)
				{
					Debug.WriteLine($"Couldn't parse {Game.Config.ScoreFilePath}");
					Debug.WriteLine(e.Message);
					return;
				}
				try
				{
					foreach (var item in o.SelectToken("scores"))
					{
						try
						{
							string foo = (string)item.SelectToken("name");
							int bar = (int)item.SelectToken("score");
							Add(foo, bar);
						}
						catch
						{
							Debug.WriteLine("Bad score?");
						}
					}
				}
				catch
				{
					return;
				}
			}
			else
			{
				Debug.WriteLine("High score file not found");
			}
		}
	}
}
