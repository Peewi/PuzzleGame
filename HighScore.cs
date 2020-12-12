using System;
using System.Collections.Generic;
using System.Text;

namespace PuzzleGame
{
	class HighScore
	{
		const int MAXSCORES = 10;
		public List<(string, int)> Scores = new List<(string, int)>();
		
		public HighScore()
		{
			for (int i = 0; i < 10; i++)
			{
				Scores.Add(("NAME", 100));
			}
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
	}
}
