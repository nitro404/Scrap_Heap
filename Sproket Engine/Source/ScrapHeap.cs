using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;
using XNAQ3Lib.MD5;
using XNAQ3Lib.Q3BSP;

namespace SproketEngine {

	/// <summary>
	/// This is the main type for your game
	/// </summary>
	public class ScrapHeap : Microsoft.Xna.Framework.Game {

		GameSettings settings;
		ScreenManager screenManager;
		CommandInterpreter interpreter;
		ControlSystem controlSystem;
		Menu menu;
		GameConsole console;
		GraphicsDeviceManager graphics;
		SpriteBatch spriteBatch;

		Q3BSPLevel level;
		Camera camera;

		bool fullScreenKeyPressed = false;

		public ScrapHeap() {
			settings = new GameSettings();
			screenManager = new ScreenManager();
			graphics = new GraphicsDeviceManager(this);
			interpreter = new CommandInterpreter();
			controlSystem = new ControlSystem();
			menu = new Menu();
			console = new GameConsole();
			Content.RootDirectory = "Content";
			camera = new Camera(this);
		}

		/// <summary>
		/// Allows the game to perform any initialization it needs to before starting to run.
		/// This is where it can query for any required services and load any non-graphic
		/// related content.  Calling base.Initialize will enumerate through any components
		/// and initialize them as well.
		/// </summary>
		protected override void Initialize() {
			// load the game settings from file
			settings.loadFrom(Content.RootDirectory + "/" + GameSettings.defaultFileName);

			// set the screen resolution
			graphics.PreferredBackBufferWidth = settings.screenWidth;
			graphics.PreferredBackBufferHeight = settings.screenHeight;
			graphics.ApplyChanges();

			// set the screen attributes / full screen mode
			Window.AllowUserResizing = false;
			if(settings.fullScreen) {
				graphics.ToggleFullScreen();
			}

			camera.initialize(graphics.GraphicsDevice);

			screenManager.initialize(this, settings, interpreter, controlSystem, menu, console);

			interpreter.initialize(this, screenManager, controlSystem, settings, console);

			controlSystem.initialize(settings, interpreter);

			menu.initialize(settings, interpreter);

			console.initialize(settings, interpreter);

			base.Initialize();
		}

		/// <summary>
		/// LoadContent will be called once per game and is the place to load
		/// all of your content.
		/// </summary>
		protected override void LoadContent() {
			spriteBatch = new SpriteBatch(GraphicsDevice);

			menu.loadContent(Content);

			console.loadContent(Content);
		}

		/// <summary>
		/// UnloadContent will be called once per game and is the place to unload
		/// all content.
		/// </summary>
		protected override void UnloadContent() { }

		public bool loadLevel(string levelName) {
			if(levelName == null) {
				return false;
			}

			levelName = levelName.Trim();

			Q3BSPLevel newLevel = new Q3BSPLevel(Q3BSPRenderType.StaticBuffer);
			newLevel.BasePath = levelName;

			bool addBsp = !levelName.ToLower().EndsWith(".bsp");

			try {
				if(newLevel.LoadFromFile(Content.RootDirectory + "/maps/" + newLevel.BasePath + (addBsp ? ".bsp" : ""))) {
					if(!newLevel.InitializeLevel(GraphicsDevice, Content, @"shaders\")) {
						return false;
					}
				}
			}
			catch(Exception) {
				return false;
			}

			camera.reset();

			level = newLevel;			

			return true;
		}

        public bool levelLoaded() {
            return level != null;
        }

		/// <summary>
		/// Handles any user input for game interaction.
		/// </summary>
		public void handleInput(GameTime gameTime) {
			camera.handleInput(gameTime);

			controlSystem.handleInput(gameTime);
		}

		/// <summary>
		/// Allows the game to run logic such as updating the world,
		/// checking for collisions, gathering input, and playing audio.
		/// </summary>
		/// <param name="gameTime">Provides a snapshot of timing values.</param>
		protected override void Update(GameTime gameTime) {
			KeyboardState keyboard = Keyboard.GetState();
			GamePadState gamePad = GamePad.GetState(PlayerIndex.One);
			bool alternateInput = false;

			if(IsActive) {
				// toggle between windowed mode and fullscreen
				if((keyboard.IsKeyDown(Keys.LeftAlt) || keyboard.IsKeyDown(Keys.RightAlt)) &&
					keyboard.IsKeyDown(Keys.Enter)) {
					if(!fullScreenKeyPressed) {
						graphics.ToggleFullScreen();
						settings.fullScreen = graphics.IsFullScreen;
						alternateInput = true;
						fullScreenKeyPressed = true;
					}
				}
				else { fullScreenKeyPressed = false; }

				if(!alternateInput) {
					screenManager.handleInput(gameTime);
				}
			}

			screenManager.update(gameTime);

			base.Update(gameTime);
		}

		/// <summary>
		/// This is called when the game should draw itself.
		/// </summary>
		/// <param name="gameTime">Provides a snapshot of timing values.</param>
		protected override void Draw(GameTime gameTime) {
			GraphicsDevice.Clear(Color.Black);

			if(level != null) {
				level.RenderLevel(camera.position, camera.view, camera.projection, gameTime, graphics.GraphicsDevice);
			}

			screenManager.draw(spriteBatch, graphics.GraphicsDevice);

			base.Draw(gameTime);
		}

		protected override void OnExiting(object sender, EventArgs args) {
			// update the game settings file with changes
			settings.saveTo(Content.RootDirectory + "/" + GameSettings.defaultFileName);

			base.OnExiting(sender, args);
		}
	}
}
