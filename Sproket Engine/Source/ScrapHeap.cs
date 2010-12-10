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
using KiloWatt.Base.Graphics;
using KiloWatt.Base.Animation;

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
		Player player;
		Q3BSPLevel level;
		SpriteSheetCollection spriteSheets;
		EntitySystem entitySystem;
		CollisionSystem collisionSystem;
		RenderTarget2D buffer;
		Effect blur;
        Effect post;

        AudioEngine audioEngine;
        WaveBank waveBank;
        SoundBank soundBank;

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
			player = new Player("Player", Vector3.Zero, Vector3.Zero);
			collisionSystem = new CollisionSystem();
			entitySystem = new EntitySystem();
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

			buffer = new RenderTarget2D(graphics.GraphicsDevice, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height, 1, GraphicsDevice.DisplayMode.Format);

			player.initialize(settings);

			screenManager.initialize(this, settings, interpreter, controlSystem, menu, console);

			interpreter.initialize(this, player, screenManager, controlSystem, settings, console);

			controlSystem.initialize(settings, interpreter);

			menu.initialize(settings, interpreter);

			console.initialize(settings, interpreter);

			base.Initialize();
            graphics.GraphicsDevice.RenderState.CullMode = CullMode.None; // no backface culling
		}

		/// <summary>
		/// LoadContent will be called once per game and is the place to load
		/// all of your content.
		/// </summary>
		protected override void LoadContent() {
			spriteBatch = new SpriteBatch(GraphicsDevice);

			spriteSheets = SpriteSheetCollection.parseFrom(Content.RootDirectory + "/" + settings.spriteSheetFileName, Content);

			menu.loadContent(Content);

			console.loadContent(Content);

			player.loadContent(Content, spriteSheets.getSpriteSheet("Crosshairs"));
			
			entitySystem.loadContent(Content);

			// load shaders
			blur = Content.Load<Effect>("Shaders\\Blur");
			post = Content.Load<Effect>("Shaders\\PostFx");

            // load sound information
            audioEngine = new AudioEngine("Content\\Sounds\\ScrapHeap.xgs");
            waveBank = new WaveBank(audioEngine, "Content\\Sounds\\waveBank.xwb");
            soundBank = new SoundBank(audioEngine, "Content\\Sounds\\soundBank.xsb");
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
					if(!newLevel.InitializeLevel(GraphicsDevice, Content, @"Shaders\")) {
						return false;
					}
				}
			}
			catch(Exception) {
				return false;
			}

			level = newLevel;
			collisionSystem.level = level;
			entitySystem.initialize(level);

			player.reset();

			List<Entity> entities = new List<Entity>();
			entities.Clear();
			entities.Add((Entity) player);
			entities.AddRange(entitySystem.entities);

			collisionSystem.initialize(this, settings, entities, level);

			Q3BSPEntity spawn = level.GetEntity("info_player_start");

			if(spawn != null) {
				Vector3 position = Q3BSPLevel.GetXNAPosition(spawn);

				// offset spawn position to bottom-center of entity
				position.Y -= 6.0f;

				player.position = position;
			}

			// add 0.001 to prevent falling through level
			player.position += new Vector3(0, 0.001f, 0);
			
			// set model lighting based on map lighting (temporarily hard-coded)
			Vector3 lighting = Vector3.One;
			if(levelName.Equals("core", StringComparison.OrdinalIgnoreCase) ||
			   levelName.Equals("core.bsp", StringComparison.OrdinalIgnoreCase)) {
				lighting = new Vector3(0.5f, 0.6f, 0.7f);
			}
			else if(levelName.Equals("foundry", StringComparison.OrdinalIgnoreCase) ||
					levelName.Equals("foundry.bsp", StringComparison.OrdinalIgnoreCase)) {
				lighting = new Vector3(0.9f, 0.8f, 0.3f);
			}
			player.setLighting(lighting);
			entitySystem.setLighting(lighting);

			return true;
		}

        public bool levelLoaded() {
            return level != null;
        }

		/// <summary>
		/// Handles any user input for game interaction.
		/// </summary>
		public void handleInput(GameTime gameTime) {
			player.handleInput(gameTime, IsActive);

			controlSystem.handleInput(gameTime);

			player.update(gameTime);

			collisionSystem.update(gameTime);
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

			entitySystem.update(gameTime);
			screenManager.update(gameTime);
            audioEngine.Update();

			base.Update(gameTime);
		}

		/// <summary>
		/// This is called when the game should draw itself.
		/// </summary>
		/// <param name="gameTime">Provides a snapshot of timing values.</param>
		protected override void Draw(GameTime gameTime) {
			graphics.GraphicsDevice.SetRenderTarget(0, buffer);
			GraphicsDevice.Clear(Color.Black);

			if(level != null) {
				level.RenderLevel(player.position, player.view, player.projection, gameTime, graphics.GraphicsDevice);

				entitySystem.draw(player.view, player.projection);

				player.draw(spriteBatch);
			}

			graphics.GraphicsDevice.SetRenderTarget(0, null);

            // blur game screen if menu is open
            if(menu.active) {
                blur.Begin();
                spriteBatch.Begin(SpriteBlendMode.None, SpriteSortMode.Immediate, SaveStateMode.SaveState);
                foreach(EffectTechnique t in blur.Techniques) {
                    foreach(EffectPass p in t.Passes) {
                        p.Begin();
                        spriteBatch.Draw(buffer.GetTexture(), Vector2.Zero, Color.White);
                        p.End();
                    }
                }
                spriteBatch.End();
                blur.End();
            }
            else {
                post.Begin();
                spriteBatch.Begin(SpriteBlendMode.None, SpriteSortMode.Immediate, SaveStateMode.SaveState);
                foreach(EffectTechnique t in post.Techniques) {
                    foreach(EffectPass p in t.Passes) {
                        p.Begin();
                        spriteBatch.Draw(buffer.GetTexture(), Vector2.Zero, Color.White);
                        p.End();
                    }
                }
                spriteBatch.End();
                post.End();
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
