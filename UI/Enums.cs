using System;
using System.Collections.Generic;
using System.Text;

namespace PuzzleGame.UI
{
	
	enum HorizontalAnchor
	{
		Unanchored,
		Left,
		Middle,
		Right
	}
	enum VerticalAnchor
	{
		Unanchored,
		Top,
		Middle,
		Bottom
	}
	enum LayoutDirection
	{
		Horizontal,
		Vertical,
		Undefined
	}
	enum ScreenPurpose
	{
		MainMenu,
		PauseMenu,
		GameOverMenu,
		Options,
		HUD,
		Tutorial,
		Unspecified
	}
}
