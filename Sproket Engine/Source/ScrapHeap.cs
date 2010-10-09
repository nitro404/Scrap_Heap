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

namespace SproketEngine {

	/// <summary>
	/// This is the main type for your game
	/// </summary>
	public class ScrapHeap : Microsoft.Xna.Framework.Game {

		GameSettings settings;
		GameConsole console;
		GraphicsDeviceManager graphics;
		ContentManager content;
		SpriteBatch spriteBatch;

		bool fullScreenKeyPressed = false;

		public ScrapHeap() {
			settings = new GameSettings();
			graphics = new GraphicsDeviceManager(this);
			content = new ContentManager(Services, "Content");
			console = new GameConsole();
		}

		/// <summary>
		/// Allows the game to perform any initialization it needs to before starting to run.
		/// This is where it can query for any required services and load any non-graphic
		/// related content.  Calling base.Initialize will enumerate through any components
		/// and initialize them as well.
		/// </summary>
		protected override void Initialize() {
			// load the game settings from file
			settings.loadFrom(content.RootDirectory + "/" + GameSettings.defaultFileName);

			// set the screen resolution
			graphics.PreferredBackBufferWidth = settings.screenWidth;
			graphics.PreferredBackBufferHeight = settings.screenHeight;
			graphics.ApplyChanges();

			// set the screen attributes / full screen mode
			Window.AllowUserResizing = false;
			if(settings.fullScreen) {
				graphics.ToggleFullScreen();
			}

			// initialize the game console
			console.initialize(settings);

			base.Initialize();
		}

		/// <summary>
		/// LoadContent will be called once per game and is the place to load
		/// all of your content.
		/// </summary>
		protected override void LoadContent() {
			spriteBatch = new SpriteBatch(GraphicsDevice);

			// load console related content
			console.loadContent(content);
		}

		/// <summary>
		/// UnloadContent will be called once per game and is the place to unload
		/// all content.
		/// </summary>
		protected override void UnloadContent() { }

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
				if(keyboard.IsKeyDown(Keys.Escape) || gamePad.IsButtonDown(Buttons.Back)) {
					Exit();
				}

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
					// let the console handle input
					console.handleInput();
				}
			}

			// update the console
			console.update(gameTime);

			base.Update(gameTime);
		}

		/// <summary>
		/// This is called when the game should draw itself.
		/// </summary>
		/// <param name="gameTime">Provides a snapshot of timing values.</param>
		protected override void Draw(GameTime gameTime) {
			GraphicsDevice.Clear(Color.Black);

			// draw the console
			spriteBatch.Begin();
			console.draw(spriteBatch);
			spriteBatch.End();

			base.Draw(gameTime);
		}

		protected override void OnExiting(object sender, EventArgs args) {
			// update the game settings file with changes
			settings.saveTo(content.RootDirectory + "/" + GameSettings.defaultFileName);

			base.OnExiting(sender, args);
		}
	}
}
