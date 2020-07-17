using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Text;

namespace PuzzleGame.UI
{
	class KeybindPanel : Button
	{
		public Actions Action = Actions.Undefined;
		public bool Active { get; private set; } = false;
		int DoubleActivationProtectionTimer = 0;
		int ActiveTimer = 0;
		public InputTypes[] Filter = new InputTypes[] { InputTypes.Keyboard, InputTypes.Mouse, InputTypes.Controller };
		public event EventHandler KeyAssigned;

		public KeybindPanel(Game game) : base(game)
		{
			KeyAssigned += (sender, e) => { };
			OnClick += (sender, e) =>
			{
				if (Active)
				{
					return;
				}
				if (DoubleActivationProtectionTimer < 5)
				{
					return;
				}
				Active = true;
				Text = "Press new key";
				ActiveTimer = 0;
			};
		}

		public override void Update(GameTime gameTime)
		{
			if (!Active)
			{
				base.Update(gameTime);
			}
			DoubleActivationProtectionTimer++;
			ActiveTimer++;
			if (!Active || ActiveTimer < 3)
			{
				return;
			}
			var i = game1.Input.RecentInput(Filter);
			if (i.Type != InputTypes.None)
			{
				if (i.Key == Keys.Escape)
				{
					//Input = Input;
				}
				else
				{
					//Input = i;
					game1.Input.SetKeybind(i, Action);
					KeyAssigned.Invoke(null, new EventArgs());
				}
				SetInactive();
			}
		}

		public void SetInactive()
		{
			SetText();
			Active = false;
			DoubleActivationProtectionTimer = 0;
		}

		public void SetText()
		{
			Input[] inputs = game1.Input.GetKeybinds(Action, Filter);
			if (inputs.Length >= 1)
			{
				Text = inputs[0].ToString();
				for (int i = 1; i < inputs.Length; i++)
				{
					Text += ", ";
					Text += inputs[i].ToString();
				}
			}
			else
			{
				Text = "None";
			}
		}

	}
}
