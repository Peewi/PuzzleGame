using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace PuzzleGame.UI
{
	class SelectBox : Button
	{
		public override string Text
		{
			get
			{
				try
				{
					return Options[Selected].DisplayText;
				}
				catch (Exception)
				{
					return "Empty";
				}
			}
		}
		public List<SelectBoxOption> Options = new List<SelectBoxOption>();
		public SelectBoxOption SelectedOption => Options[selected];
		private int selected = 0;

		public int Selected
		{
			get { return selected; }
			set { 
				selected = value;
				SelectionChanged.Invoke(null, new EventArgs());
			}
		}

		public event EventHandler SelectionChanged;

		public SelectBox(Game game) : base(game)
		{
			OnClick += (sender, e) => { OpenOptionsList(); };
			SelectionChanged += (sender, e) => { };
		}

		private void OpenOptionsList()
		{
			var optionsScreen = new UIScreen(game1)
			{
				ScreenBelowVisible = true,
				EscapeCloses = true,
				BackgroundColor = Color.Black * 0.5f
			};
			var optionsStack = new StackPanel(game1)
			{
				MainNavigation = true
			};
			var scroller = new ScrollPanel(game1)
			{
				HAnchor = HorizontalAnchor.Middle,
				VAnchor = VerticalAnchor.Middle,
				Child = optionsStack
			};
			optionsScreen.Children.Add(scroller);
			for (int i = 0; i < Options.Count; i++)
			{
				var optionPanel = new Button(game1)
				{
					Text = Options[i].DisplayText
				};
				if (i == Selected)
				{
					optionPanel.HasFocus = true;
					optionsStack.Selection = i;
				}
				optionsStack.Children.Add(optionPanel);
				int num = i;
				optionPanel.OnClick += (sender, e) =>
				{
					Selected = num;
					game1.UI.CloseTopScreen();
				};
			}

			optionsScreen.UpdateLayout(optionsScreen.Bounds);
			game1.UI.OpenScreen(optionsScreen);
		}

		public override void Draw(GameTime gameTime)
		{
			base.Draw(gameTime);
		}
	}

	struct SelectBoxOption
	{
		public string DisplayText;
		public object Value;

		public SelectBoxOption(string displayText, object value)
		{
			DisplayText = displayText;
			Value = value;
		}
	}
}
