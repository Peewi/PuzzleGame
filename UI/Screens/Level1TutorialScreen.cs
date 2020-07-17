using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace PuzzleGame.UI
{
	partial class UIScreen
	{
		/// <summary>
		/// The game HUD, in screen form
		/// </summary>
		/// <param name="game"></param>
		/// <returns></returns>
		public static UIScreen Level1TutorialScreen(Game game)
		{
			Game1 g1 = (Game1)game;
			//TODO: Put any HUD stuff here
			var retVal = new UIScreen(game)
			{
				Purpose = ScreenPurpose.Tutorial
			};
			retVal.Escape += (sender, e) =>
			{
				//g1.UI.TogglePause();
			};
			Input[] activateInputs = g1.Input.GetKeybinds(Actions.ActivateRobots, new InputTypes[] { InputTypes.Keyboard });
			Input[] jumpInputs = g1.Input.GetKeybinds(Actions.Jump, new InputTypes[] { InputTypes.Keyboard });
			Input[] attackInputs = g1.Input.GetKeybinds(Actions.Attack, new InputTypes[] { InputTypes.Keyboard });
			string actKeys;
			string jumpKeys;
			string atkKeys;
			if (activateInputs.Length >= 1)
			{
				actKeys = activateInputs[0].ToString();
				for (int i = 1; i < activateInputs.Length; i++)
				{
					actKeys += ", ";
					actKeys += activateInputs[i].ToString();
				}
			}
			else
			{
				actKeys = "None";
			}
			if (jumpInputs.Length >= 1)
			{
				jumpKeys = jumpInputs[0].ToString();
				for (int i = 1; i < jumpInputs.Length; i++)
				{
					jumpKeys += ", ";
					jumpKeys += jumpInputs[i].ToString();
				}
			}
			else
			{
				jumpKeys = "None";
			}
			if (attackInputs.Length >= 1)
			{
				atkKeys = attackInputs[0].ToString();
				for (int i = 1; i < attackInputs.Length; i++)
				{
					atkKeys += ", ";
					atkKeys += attackInputs[i].ToString();
				}
			}
			else
			{
				atkKeys = "None";
			}
			var tutor = new TextPanel(game)
			{
				Text = $"Gather robots and destroy the target." +
				$"\nThe target is quite sturdy" +
				$"\nPress {actKeys} to activate nearby robots" +
				$"\nPress {jumpKeys} to jump and order all your robots to jump" +
				$"\nPress {atkKeys} to order your robots to attack",
				Width = 128+64,
				Height = 48,
				HAnchor = HorizontalAnchor.Middle,
				VAnchor = VerticalAnchor.Top
			};
			retVal.Children.Add(tutor);
			retVal.UpdateLayout(retVal.Bounds);
			return retVal;
		}
	}
}
