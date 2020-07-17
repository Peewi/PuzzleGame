using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PuzzleGame
{
	class SoundManager : GameComponent
	{
		Game1 Game1;
		public SoundEffect wilhelm;
		public SoundEffect noise;
		public SoundEffect Explosion;
		public SoundEffect Hit;
		public SoundEffect Jump;
		List<SoundEffectInstance> PausableSounds = new List<SoundEffectInstance>();
		List<SoundEffectInstance> UnPausableSounds = new List<SoundEffectInstance>();
		bool Paused = false;
		public SoundManager(Game game) : base(game)
		{
			if (game is Game1 g1)
			{
				Game1 = g1;
			}
			else
			{
				throw new Exception();
			}
			UpdateOrder = 4;
		}

		public override void Update(GameTime gameTime)
		{
			base.Update(gameTime);
			if (Game1.UI.CurrentScreenPurpose == UI.ScreenPurpose.PauseMenu)
			{
				if (!Paused)
				{
					Paused = true;
					foreach (var snd in PausableSounds)
					{
						snd.Pause();
					}
				}
			}
			else if (Paused)
			{
				Paused = false;
				foreach (var snd in PausableSounds)
				{
					if (snd.State == SoundState.Paused)
					{
						snd.Resume();
					}
				}
			}
			PausableSounds = PausableSounds.Where(item => item.State != SoundState.Stopped).ToList();
			UnPausableSounds = UnPausableSounds.Where(item => item.State != SoundState.Stopped).ToList();
		}

		public void StopAllSounds()
		{
			foreach (var item in PausableSounds)
			{
				item.Stop();
			}
			PausableSounds.Clear();
			foreach (var item in UnPausableSounds)
			{
				item.Stop();
			}
			UnPausableSounds.Clear();
		}

		public void LoadSounds()
		{
			wilhelm = Game1.Content.Load<SoundEffect>("Wilhelm_Scream");
			noise = Game1.Content.Load<SoundEffect>("minimize_004");
			Explosion = Game1.Content.Load<SoundEffect>("Explosion");
			Hit = Game1.Content.Load<SoundEffect>("Hit_Hurt");
			Jump = Game1.Content.Load<SoundEffect>("Jump");
		}

		public void PlaySound(SoundEffect sound)
		{
			PlaySound(sound, true);
		}

		public void PlaySound(SoundEffect sound, bool pauseable)
		{
			var bar = sound.CreateInstance();
			if (pauseable)
			{
				PausableSounds.Add(bar);
			}
			else
			{
				UnPausableSounds.Add(bar);
			}
			bar.Play();
		}
	}
}
