using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace SproketEngine {

	enum ScreenType { Game, Menu, Console }

	class ScreenManager {

		private ScreenType activeScreen = ScreenType.Game;
		private bool m_consoleKeyPressed = false;

		private ScrapHeap m_game;
		private GameSettings m_settings;
		private CommandInterpreter m_interpreter;
		private GameMenu m_menu;
		private GameConsole m_console;

		public ScreenManager() { }

		public void initialize(ScrapHeap game, GameSettings settings, CommandInterpreter interpreter, GameMenu menu, GameConsole console) {
			m_game = game;
			m_settings = settings;
			m_interpreter = interpreter;
			m_menu = menu;
			m_console = console;
		}

		public void toggle(ScreenType screen) {
			if(screen == ScreenType.Menu) {
				m_menu.toggle();
			}
			if(screen == ScreenType.Console) {
				m_console.toggle();
			}

			updateActiveScreen();
		}

		public void show(ScreenType screen, bool visible) {
			if(visible) { show(screen); }
			else { hide(screen); }
		}

		public void show(ScreenType screen) {
			if(screen == ScreenType.Game) {
				m_menu.close();
				m_console.close();
			}
			else if(screen == ScreenType.Menu) {
				m_menu.open();
			}
			else if(screen == ScreenType.Console) {
				m_console.open();
			}

			activeScreen = screen;
		}

		public void hide(ScreenType screen) {
			if(screen == ScreenType.Menu) {
				m_menu.close();
			}
			else if(screen == ScreenType.Console) {
				m_console.close();
			}

			updateActiveScreen();
		}

		private void updateActiveScreen() {
			activeScreen = ScreenType.Game;
			if(m_menu.active) { activeScreen = ScreenType.Menu; }
			if(m_console.active) { activeScreen = ScreenType.Console; }
		}

		public void handleInput() {
			KeyboardState keyboard = Keyboard.GetState();

			if(keyboard.IsKeyDown(Keys.OemTilde)) {
				if(!m_consoleKeyPressed) {
					toggle(ScreenType.Console);
					m_consoleKeyPressed = true;
				}
			}
			else { m_consoleKeyPressed = false; }

				 if(activeScreen == ScreenType.Game) { m_game.handleInput(); }
			else if(activeScreen == ScreenType.Menu) { m_menu.handleInput(); }
			else if(activeScreen == ScreenType.Console) { m_console.handleInput(); }
		}

		public void update(GameTime gameTime) {
			if(m_menu.active) { m_menu.update(gameTime); }
			if(m_console.active) { m_console.update(gameTime); }
		}

		public void draw(SpriteBatch spriteBatch, GraphicsDevice graphics) {
			spriteBatch.Begin();

			if(m_menu.active) { m_menu.draw(spriteBatch); }
			if(m_console.active) { m_console.draw(spriteBatch); }

			spriteBatch.End();
		}

	}

}
