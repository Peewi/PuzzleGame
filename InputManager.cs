using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;

namespace PuzzleGame
{
	enum Actions
	{
		Undefined,
		Up,
		Down,
		Left,
		Right,
		RotateLeft,
		RotateRight
	}
	enum MenuActions
	{
		Undefined,
		Up,
		Down,
		Left,
		Right,
		Confirm,
		Back,
		Pause,
		Debug1
	}
	enum InputTypes
	{
		None,
		Keyboard,
		Mouse,
		Controller
	}
	enum Sticks
	{
		Left,
		Right
	}
	enum MouseInputs
	{
		None,
		LeftMouse,
		RightMouse,
		MiddleMouse,
		Mouse4,
		Mouse5,
		ScrollUp,
		ScrollDown
	}
	/// <summary>
	/// Combines mouse buttons and keyboard keys into one thing.
	/// Does not handle mouse movement.
	/// </summary>
	struct Input
	{
		public readonly InputTypes Type;
		public readonly Keys Key;
		public readonly MouseInputs MouseInput;
		public readonly Buttons? ControllerButton;

		public Input(Keys key)
		{
			Type = InputTypes.Keyboard;
			Key = key;
			MouseInput = MouseInputs.None;
			ControllerButton = null;
		}

		public Input(MouseInputs mouseIn)
		{
			Type = InputTypes.Mouse;
			Key = Keys.None;
			MouseInput = mouseIn;
			ControllerButton = null;
		}

		public Input(Buttons button)
		{
			Type = InputTypes.Controller;
			Key = Keys.None;
			MouseInput = MouseInputs.None;
			ControllerButton = button;
		}

		public Input(InputTypes t)
		{
			Type = t;
			Key = Keys.None;
			MouseInput = MouseInputs.None;
			ControllerButton = null;
		}

		public override string ToString()
		{
			switch (Type)
			{
				case InputTypes.None:
					return Type.ToString();
				case InputTypes.Keyboard:
					return Key.ToString();
				case InputTypes.Mouse:
					return MouseInput.ToString();
				case InputTypes.Controller:
					return ControllerButton.ToString();
				default:
					break;
			}
			return base.ToString();
		}
	}
	class InputManager : GameComponent
	{
		public bool UnsavedChanges = false;

		/// <summary>
		/// The actions that should appear in the keybindings menu
		/// </summary>
		public Actions[] BindableActions = { Actions.Down, Actions.Left, Actions.Right, Actions.RotateLeft, Actions.RotateRight };
		Buttons[] BindableButtons = new Buttons[]
			{
				Buttons.A,
				Buttons.B,
				Buttons.X,
				Buttons.Y,
				Buttons.LeftShoulder,
				Buttons.RightShoulder,
				Buttons.LeftTrigger,
				Buttons.RightTrigger,
				Buttons.DPadUp,
				Buttons.DPadDown,
				Buttons.DPadLeft,
				Buttons.DPadRight,
				Buttons.LeftThumbstickUp,
				Buttons.LeftThumbstickDown,
				Buttons.LeftThumbstickLeft,
				Buttons.LeftThumbstickRight,
				Buttons.RightThumbstickUp,
				Buttons.RightThumbstickDown,
				Buttons.RightThumbstickRight,
				Buttons.RightThumbstickLeft,
				Buttons.Start,
				Buttons.Back,
				Buttons.LeftStick,
				Buttons.RightStick,
				//Buttons.BigButton
			};
		Dictionary<Actions, Input[]> Keybinds = new Dictionary<Actions, Input[]>();
		Dictionary<MenuActions, Input[]> MenuBinds = new Dictionary<MenuActions, Input[]>();
		Dictionary<Actions, Input[]> previousKeybinds;
		public Sticks MovementStick = Sticks.Left;
		public Sticks AimStick = Sticks.Right;
		public bool MenuBack => JustPressed(MenuActions.Back);
		public bool Pause => JustPressed(MenuActions.Pause);
		public bool Click =>
			mouse.LeftButton == ButtonState.Pressed
			&& previousMouse.LeftButton == ButtonState.Released;
		public bool RightClick =>
			mouse.RightButton == ButtonState.Pressed
			&& previousMouse.RightButton == ButtonState.Released;
		public bool MiddleClick =>
			mouse.MiddleButton == ButtonState.Pressed
			&& previousMouse.MiddleButton == ButtonState.Released;
		public bool Mouse4Click =>
			mouse.XButton1 == ButtonState.Pressed
			&& previousMouse.XButton1 == ButtonState.Released;
		public bool Mouse5Click =>
			mouse.XButton2 == ButtonState.Pressed
			&& previousMouse.XButton2 == ButtonState.Released;
		public bool MouseIsDown => mouse.LeftButton == ButtonState.Pressed;
		public bool MouseIsMoving => mouse.Position != previousMouse.Position;
		#region MenuInputs
		public bool MenuLeft => JustPressed(MenuActions.Left);
		public bool MenuRight => JustPressed(MenuActions.Right);
		public bool MenuUp => JustPressed(MenuActions.Up);
		public bool MenuDown => JustPressed(MenuActions.Down);
		public bool MenuConfirm => JustPressed(MenuActions.Confirm);
		public bool Debug1 => JustPressed(MenuActions.Debug1);
		#endregion
		#region PlayerInputs
		public bool PlayerLeft => Pressed(Actions.Left);
		public bool PlayerRight => Pressed(Actions.Right);
		public bool PlayerUp => Pressed(Actions.Up);
		public bool PlayerDown => Pressed(Actions.Down);
		/// <summary>
		/// Normalized move direction
		/// </summary>
		public Vector2 MoveDirection { get
			{
				Vector2 dir = Vector2.Zero;
				if (MovementStick == Sticks.Left)
				{
					dir = controller.ThumbSticks.Left;
				}
				else if (MovementStick == Sticks.Right)
				{
					dir = controller.ThumbSticks.Right;
				}
					
				if (dir != Vector2.Zero)
				{
					dir.Y = -dir.Y;
					dir.Normalize();
					return dir;
				}
				if (PlayerUp)
				{
					dir.Y -= 1;
				}
				if (PlayerDown)
				{
					dir.Y += 1;
				}
				if (PlayerLeft)
				{
					dir.X -= 1;
				}
				if (PlayerRight)
				{
					dir.X += 1;
				}
				if (dir != Vector2.Zero)
				{
					dir.Normalize();
				}
				return dir;
			} }
		/// <summary>
		/// Normalized aim direction.
		/// </summary>
		/// <param name="playerPos">Where on the screen the player is aiming from. Only used for mouse aiming.</param>
		/// <returns>A normalized <c>Vector2</c>. Points to the right if mouse and player positions are the same.</returns>
		public Vector2 AimDirection(Vector2 playerPos)
		{
			Vector2 dir = Vector2.Zero;
			if (AimStick == Sticks.Left)
			{
				dir = controller.ThumbSticks.Left;
			}
			else if (AimStick == Sticks.Right)
			{
				dir = controller.ThumbSticks.Right;
			}
			if (dir != Vector2.Zero)
			{
				dir.Y = -dir.Y;
				dir.Normalize();
				return dir;
			}
			dir = MousePos - playerPos;
			if (dir != Vector2.Zero)
			{
				dir.Normalize();
			}
			else
			{
				dir = new Vector2(1, 0);
			}
			return dir;
		}
		#endregion
		public Vector2 MousePos => mouse.Position.ToVector2();
		/// <summary>
		/// How much the mouse wheel was scrolled since the last frame;
		/// </summary>
		public int Scrolled => (mouse.ScrollWheelValue - previousMouse.ScrollWheelValue) / 120;
		public int RawScroll => mouse.ScrollWheelValue;
		KeyboardState keyboard, previousKeyboard;
		MouseState mouse, previousMouse;
		GamePadState controller, previousController;
		public InputManager(Game game) : base(game)
		{
			UpdateOrder = 0;
			SetDefaultKeys();
		}

		public void SetDefaultKeys()
		{
			Keybinds.Clear();
			SetKeybind(new Input(Keys.Up), MenuActions.Up);
			SetKeybind(new Input(Keys.Down), MenuActions.Down);
			SetKeybind(new Input(Keys.Left), MenuActions.Left);
			SetKeybind(new Input(Keys.Right), MenuActions.Right);
			SetKeybind(new Input(Keys.Enter), MenuActions.Confirm);
			SetKeybind(new Input(Keys.Escape), MenuActions.Back);
			SetKeybind(new Input(Keys.Escape), MenuActions.Pause);
			SetKeybind(new Input(Keys.F1), MenuActions.Debug1);

			SetKeybind(new Input(Buttons.LeftThumbstickUp), MenuActions.Up);
			SetKeybind(new Input(Buttons.LeftThumbstickDown), MenuActions.Down);
			SetKeybind(new Input(Buttons.LeftThumbstickLeft), MenuActions.Left);
			SetKeybind(new Input(Buttons.LeftThumbstickRight), MenuActions.Right);
			SetKeybind(new Input(Buttons.DPadUp), MenuActions.Up);
			SetKeybind(new Input(Buttons.DPadDown), MenuActions.Down);
			SetKeybind(new Input(Buttons.DPadLeft), MenuActions.Left);
			SetKeybind(new Input(Buttons.DPadRight), MenuActions.Right);
			SetKeybind(new Input(Buttons.A), MenuActions.Confirm);
			SetKeybind(new Input(Buttons.B), MenuActions.Back);
			SetKeybind(new Input(Buttons.Start), MenuActions.Pause);

			//SetKeybind(new Input(Keys.W), Actions.Up);
			//SetKeybind(new Input(Keys.S), Actions.Down);
			//SetKeybind(new Input(Keys.A), Actions.Left);
			//SetKeybind(new Input(Keys.D), Actions.Right);
			SetKeybind(new Input(Keys.Up), Actions.RotateRight);
			SetKeybind(new Input(Keys.Down), Actions.Down);
			SetKeybind(new Input(Keys.Left), Actions.Left);
			SetKeybind(new Input(Keys.Right), Actions.Right);
			SetKeybind(new Input(Keys.Z), Actions.RotateLeft);
			SetKeybind(new Input(Keys.X), Actions.RotateRight);

			//SetKeybind(new Input(Buttons.DPadUp), Actions.Up);
			SetKeybind(new Input(Buttons.DPadDown), Actions.Down);
			SetKeybind(new Input(Buttons.DPadLeft), Actions.Left);
			SetKeybind(new Input(Buttons.DPadRight), Actions.Right);
			SetKeybind(new Input(Buttons.LeftThumbstickDown), Actions.Down);
			SetKeybind(new Input(Buttons.LeftThumbstickLeft), Actions.Left);
			SetKeybind(new Input(Buttons.LeftThumbstickRight), Actions.Right);
			SetKeybind(new Input(Buttons.A), Actions.RotateLeft);
			SetKeybind(new Input(Buttons.X), Actions.RotateRight);
			SetKeybind(new Input(Buttons.B), Actions.RotateRight);
			SetKeybind(new Input(Buttons.Y), Actions.RotateLeft);
		}
		/// <summary>
		/// Get the inputs bound to an action
		/// </summary>
		/// <param name="action">What you wanna know about</param>
		/// <returns>An array of <c>Input</c>s.</returns>
		public Input[] GetKeybinds(Actions action)
		{
			if (Keybinds.TryGetValue(action, out Input[] outVal))
			{
				return outVal;
			}
			return new[] { new Input(InputTypes.None) };
		}
		/// <summary>
		/// Get the inputs of certain kinds bound to an action
		/// </summary>
		/// <param name="action">What you wanna know about</param>
		/// <param name="filter">Array of <c>InputTypes</c>.</param>
		/// <returns>An array of <c>Input</c>s.</returns>
		public Input[] GetKeybinds(Actions action, InputTypes[] filter)
		{
			if (Keybinds.TryGetValue(action, out Input[] outVal))
			{
				return outVal.Where(item => filter.Contains(item.Type)).ToArray();
			}
			return new[] { new Input(InputTypes.None) };
		}
		/// <summary>
		/// Set a keybind.
		/// </summary>
		/// <param name="input">input</param>
		/// <param name="action">action</param>
		public void SetKeybind(Input input, Actions action)
		{
			UnsavedChanges = true;
			Actions removed = RemoveKeybind(input);
			if (removed == action)
			{
				return;
			}
			if (Keybinds.TryGetValue(action, out Input[] existing))
			{
				Array.Resize(ref existing, existing.Length + 1);
				existing[^1] = input;
				Keybinds[action] = existing;
			}
			else
			{
				Keybinds[action] = new[] { input };
			}
		}
		/// <summary>
		/// Set a keybind for a menu action
		/// </summary>
		/// <param name="input">input</param>
		/// <param name="action">menu action</param>
		public void SetKeybind(Input input, MenuActions action)
		{
			if (MenuBinds.TryGetValue(action, out Input[] existing))
			{
				Array.Resize(ref existing, existing.Length + 1);
				existing[^1] = input;
				MenuBinds[action] = existing;
			}
			else
			{
				MenuBinds[action] = new[] { input };
			}
		}
		/// <summary>
		/// Remove the keybind for a specific input.
		/// </summary>
		/// <param name="input">The input you want gone</param>
		/// <returns>The action of the removed keybind. <c>Actions.Undefined</c> if nothing was removed.</returns>
		Actions RemoveKeybind(Input input)
		{
			Actions removeAct = Actions.Undefined;
			Input[] newThing = null;
			bool inputFound = false;
			foreach (var kvPair in Keybinds)
			{
				for (int i = 0; i < kvPair.Value.Length; i++)
				{
					inputFound = inputFound || kvPair.Value[i].Equals(input);
				}
				if (inputFound)
				{
					removeAct = kvPair.Key;
					var foo = kvPair.Value.ToList();
					foo.Remove(input);
					newThing = foo.ToArray();
					break;
				}
			}
			if (inputFound)
			{
				Keybinds[removeAct] = newThing;
				return removeAct;
			}
			return Actions.Undefined;
		}

		/// <summary>
		/// Returns a keyboard key that was pressed in the last frame.
		/// Used for setting keybindings, not gameplay.
		/// </summary>
		/// <returns><c>Keys</c> with the pressed key. <c>Keys.None</c> if nothing was pressed</returns>
		public Keys RecentKey()
		{
			foreach (var key in keyboard.GetPressedKeys())
			{
				if (JustPressed(key))
				{
					return key;
				}
			}
			return Keys.None;
		}
		/// <summary>
		/// Returns a mouse thing that was input in the last frame.
		/// Used for setting keybindings, not gameplay.
		/// </summary>
		/// <returns><c>MouseInputs</c> with the input. <c>MouseInputs.None</c> if nothing was input</returns>
		public MouseInputs RecentMouseInput()
		{
			if (Click)
			{
				return MouseInputs.LeftMouse;
			}
			if (RightClick)
			{
				return MouseInputs.RightMouse;
			}
			if (MiddleClick)
			{
				return MouseInputs.MiddleMouse;
			}
			if (Mouse4Click)
			{
				return MouseInputs.Mouse4;
			}
			if (Mouse5Click)
			{
				return MouseInputs.Mouse5;
			}
			if (Scrolled > 0)
			{
				return MouseInputs.ScrollUp;
			}
			if (Scrolled < 0)
			{
				return MouseInputs.ScrollDown;
			}
			return MouseInputs.None;
		}
		public Buttons? RecentButton()
		{
			foreach (var item in BindableButtons)
			{
				if (JustPressed(item))
				{
					return item;
				}
			}
			return null;
		}
		/// <summary>
		/// Input received in the last frame. 
		/// InputTypes.None indicates nothing was pressed.
		/// Used for setting keybindings, not gameplay.
		/// </summary>
		/// <returns>An <c>Input</c> struct. The field not indicated by Input.Type should be None.</returns>
		public Input RecentInput()
		{
			var k = RecentKey();
			if (k != Keys.None)
			{
				return new Input(k);
			}
			var m = RecentMouseInput();
			if (m != MouseInputs.None)
			{
				return new Input(m);
			}
			var b = RecentButton();
			if (b is Buttons button)
			{
				return new Input(button);
			}
			return new Input(InputTypes.None);
		}
		/// <summary>
		/// Input received in the last frame, but limited to specific kinds. 
		/// InputTypes.None indicates nothing was pressed.
		/// Used for setting keybindings, not gameplay.
		/// </summary>
		/// <param name="filter"></param>
		/// <returns>An <c>Input</c> struct.</returns>
		public Input RecentInput(InputTypes[] filter)
		{
			if (filter.Contains(InputTypes.Keyboard))
			{
				var k = RecentKey();
				if (k != Keys.None)
				{
					return new Input(k);
				} 
			}
			if (filter.Contains(InputTypes.Mouse))
			{
				var m = RecentMouseInput();
				if (m != MouseInputs.None)
				{
					return new Input(m);
				} 
			}
			if (filter.Contains(InputTypes.Controller))
			{
				var b = RecentButton();
				if (b is Buttons button)
				{
					return new Input(button);
				} 
			}
			return new Input(InputTypes.None);
		}
		/// <summary>
		/// Whether a given input is currently being pressed.
		/// </summary>
		/// <param name="input">Input you're checking</param>
		/// <returns><c>true</c> = it's being pressed, <c>false</c> = it's not</returns>
		bool Pressed(Input input)
		{
			switch (input.Type)
			{
				case InputTypes.Keyboard:
					return keyboard.IsKeyDown(input.Key);
				case InputTypes.Mouse:
					switch (input.MouseInput)
					{
						case MouseInputs.None:
							return false;
						case MouseInputs.LeftMouse:
							return mouse.LeftButton == ButtonState.Pressed;
						case MouseInputs.RightMouse:
							return mouse.RightButton == ButtonState.Pressed;
						case MouseInputs.MiddleMouse:
							return mouse.MiddleButton == ButtonState.Pressed;
						case MouseInputs.Mouse4:
							return mouse.XButton1 == ButtonState.Pressed;
						case MouseInputs.Mouse5:
							return mouse.XButton2 == ButtonState.Pressed;
						case MouseInputs.ScrollUp:
							return Scrolled > 0;
						case MouseInputs.ScrollDown:
							return Scrolled < 0;
					}
					break;
				case InputTypes.Controller:
					if (input.ControllerButton is Buttons b)
					{
						return controller.IsButtonDown(b);
					}
					break;
			}
			return false;
		}
		/// <summary>
		/// Whether any of the inputs for an action is being pressed
		/// </summary>
		/// <param name="action">action</param>
		/// <returns><c>true</c> = pressed, <c>false</c> = not pressed</returns>
		public bool Pressed(Actions action)
		{
			if (Keybinds.TryGetValue(action, out Input[] inputs))
			{
				foreach (var input in inputs)
				{
					if (Pressed(input))
					{
						return true;
					}
				}
			}
			return false;
		}
		/// <summary>
		/// Whether any of the inputs for a menu action is being pressed
		/// </summary>
		/// <param name="action">actions</param>
		/// <returns><c>true</c> = pressed, <c>false</c> = not pressed</returns>
		public bool Pressed(MenuActions action)
		{
			if (MenuBinds.TryGetValue(action, out Input[] inputs))
			{
				foreach (var input in inputs)
				{
					if (Pressed(input))
					{
						return true;
					}
				}
			}
			return false;
		}
		/// <summary>
		/// Whether a given input was pressed in the last frame.
		/// </summary>
		/// <param name="input">Input you're checking</param>
		/// <returns><c>true</c> = pressed, <c>false</c> = not pressed</returns>
		bool JustPressed(Input input)
		{
			switch (input.Type)
			{
				case InputTypes.Keyboard:
					return JustPressed(input.Key);
				case InputTypes.Mouse:
					switch (input.MouseInput)
					{
						case MouseInputs.LeftMouse:
							return Click;
						case MouseInputs.RightMouse:
							return RightClick;
						case MouseInputs.MiddleMouse:
							return MiddleClick;
						case MouseInputs.Mouse4:
							return Mouse4Click;
						case MouseInputs.Mouse5:
							return Mouse5Click;
						case MouseInputs.ScrollUp:
							return Scrolled > 0;
						case MouseInputs.ScrollDown:
							return Scrolled < 0;
					}
					break;
				case InputTypes.Controller:
					if (input.ControllerButton is Buttons b)
					{
						return JustPressed(b);
					}
					break;
			}
			return false;
		}
		/// <summary>
		/// Whether a given controller button was pressed in the last frame.
		/// </summary>
		/// <param name="button">Button you're checking</param>
		/// <returns><c>true</c> = pressed, <c>false</c> = not pressed</returns>
		bool JustPressed(Buttons button)
		{
			return previousController.IsButtonUp(button) && controller.IsButtonDown(button);
		}
		/// <summary>
		/// Whether a given key was pressed in the last frame.
		/// </summary>
		/// <param name="k">Key you're checking</param>
		/// <returns><c>true</c> = pressed, <c>false</c> = not pressed</returns>
		bool JustPressed(Keys k)
		{
			return previousKeyboard.IsKeyUp(k) && keyboard.IsKeyDown(k);
		}
		/// <summary>
		/// Whether any of the inputs for an action was pressed in the last frame.
		/// </summary>
		/// <param name="action">action</param>
		/// <returns><c>true</c> = pressed, <c>false</c> = not pressed</returns>
		public bool JustPressed(Actions action)
		{
			if (Keybinds.TryGetValue(action, out Input[] inputs))
			{
				foreach (var input in inputs)
				{
					if (JustPressed(input))
					{
						return true;
					}
				}
			}
			return false;
		}
		/// <summary>
		/// Whether any of the inputs for a menu action was pressed in the last frame.
		/// </summary>
		/// <param name="action">action</param>
		/// <returns><c>true</c> = pressed, <c>false</c> = not pressed</returns>
		bool JustPressed(MenuActions action)
		{
			if (MenuBinds.TryGetValue(action, out Input[] inputs))
			{
				foreach (var input in inputs)
				{
					if (JustPressed(input))
					{
						return true;
					}
				}
				//return JustPressed(inp);
			}
			return false;
		}

		public bool KeyWithCtrlPressed(Keys k)
		{
			return keyboard.IsKeyDown(Keys.LeftControl)
				|| keyboard.IsKeyDown(Keys.RightControl)
				&& JustPressed(k);
		}

		public override void Update(GameTime gameTime)
		{
			base.Update(gameTime);
			previousKeyboard = keyboard;
			keyboard = Keyboard.GetState();
			previousMouse = mouse;
			mouse = Mouse.GetState();
			previousController = controller;
			controller = GamePad.GetState(0, GamePadDeadZone.Circular);
		}
		/// <summary>
		/// Allows reverting to the current state if you change your mind about keybind changes.
		/// </summary>
		public void SetRevertState()
		{
			UnsavedChanges = false;
			previousKeybinds = new Dictionary<Actions, Input[]>();
			foreach (var item in Keybinds)
			{
				previousKeybinds[item.Key] = item.Value;
			}
		}
		/// <summary>
		/// Go back to the keybinds from the time <c>SetRevertState</c> was called.
		/// </summary>
		public void RevertChanges()
		{
			UnsavedChanges = false;
			Keybinds = previousKeybinds;
			previousKeybinds = null;
		}
		/// <summary>
		/// Save keybinds as json file
		/// </summary>
		/// <param name="file">Full path for file to save to</param>
		public void SaveKeybinds(string file)
		{
			var sb = new StringBuilder();
			var sw = new StringWriter(sb);
			using (var writer = new JsonTextWriter(sw))
			{
				writer.Formatting = Formatting.Indented;
				writer.WriteStartObject();
				writer.WritePropertyName("keybinds");
				writer.WriteStartArray();
				foreach (var kvPair in Keybinds)
				{
					foreach (var singleInput in kvPair.Value)
					{
						writer.WriteStartObject();
						writer.WritePropertyName("action");
						writer.WriteValue(kvPair.Key.ToString());
						writer.WritePropertyName("inputType");
						writer.WriteValue(singleInput.Type.ToString());
						writer.WritePropertyName("input");
						switch (singleInput.Type)
						{
							case InputTypes.Keyboard:
								writer.WriteValue(singleInput.Key.ToString());
								break;
							case InputTypes.Mouse:
								writer.WriteValue(singleInput.MouseInput.ToString());
								break;
							case InputTypes.Controller:
								writer.WriteValue(singleInput.ControllerButton.ToString());
								break;
							default:
								writer.WriteValue("none");
								break;
						}
						writer.WriteEndObject();
					}
				}
				writer.WriteEndArray();
				writer.WriteEndObject();
			}
			File.WriteAllText(file, sb.ToString());
			UnsavedChanges = false;
		}
		/// <summary>
		/// Load keybinds from a json file.
		/// </summary>
		/// <param name="file">Full path for a json file</param>
		public void LoadKeybinds(string file)
		{
			if (File.Exists(file))
			{
				Keybinds.Clear();
				string keysText = File.ReadAllText(file);
				JObject o;
				try
				{
					o = JObject.Parse(keysText);
				}
				catch (Exception e)
				{
					Debug.WriteLine($"Couldn't parse {file}");
					Debug.WriteLine(e.Message);
					return;
				}
				var foo = o.SelectToken("keybinds");
				if (foo == null)
				{
					Debug.WriteLine($"Bad keybinds file {file}");
					return;
				}
				foreach (var item in foo.Children())
				{
					try
					{
						Actions action = (Actions)Enum.Parse(typeof(Actions), item.SelectToken("action").ToString());
						InputTypes inputType = (InputTypes)Enum.Parse(typeof(InputTypes), item.SelectToken("inputType").ToString());
						Input theInput;
						switch (inputType)
						{
							case InputTypes.Keyboard:
								Keys key = (Keys)Enum.Parse(typeof(Keys), item.SelectToken("input").ToString());
								theInput = new Input(key);
								break;
							case InputTypes.Mouse:
								MouseInputs mInput = (MouseInputs)Enum.Parse(typeof(MouseInputs), item.SelectToken("input").ToString());
								theInput = new Input(mInput);
								break;
							case InputTypes.Controller:
								Buttons b = (Buttons)Enum.Parse(typeof(Buttons), item.SelectToken("input").ToString());
								theInput = new Input(b);
								break;
							default:
								theInput = new Input(InputTypes.None);
								break;
						}
						SetKeybind(theInput, action);
					}
					catch (Exception e)
					{
						Debug.WriteLine("Error reading keybinds: ");
						Debug.WriteLine(e.Message);
					}
				}
			}
			else
			{
				Debug.WriteLine("Keybinds file not found");
			}
		}
	}
}
