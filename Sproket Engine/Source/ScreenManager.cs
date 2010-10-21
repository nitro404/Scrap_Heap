using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace SproketEngine {

	enum ScreenType { Game, Menu, Console }

	enum ScreenVisibilityChange { None, Show, Hide, Toggle }

	class ScreenManager {

		private ScreenType m_activeScreen = ScreenType.Game;

		private ScrapHeap m_game;
		private GameSettings m_settings;
		private CommandInterpreter m_interpreter;
		private Menu m_menu;
		private GameConsole m_console;

		public ScreenManager() { }

		public void initialize(ScrapHeap game, GameSettings settings, CommandInterpreter interpreter, Menu menu, GameConsole console) {
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

		public void set(ScreenType screen, ScreenVisibilityChange visibility) {
				 if(visibility == ScreenVisibilityChange.Toggle) { toggle(screen); }
			else if(visibility == ScreenVisibilityChange.Show) { show(screen); }
			else if(visibility == ScreenVisibilityChange.Hide) { hide(screen); }
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

			m_activeScreen = screen;
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
			m_activeScreen = ScreenType.Game;
			if(m_menu.active) { m_activeScreen = ScreenType.Menu; }
			if(m_console.active) { m_activeScreen = ScreenType.Console; }
		}

		public void handleInput(GameTime gameTime) {
			KeyboardState keyboard = Keyboard.GetState();

			if(m_activeScreen == ScreenType.Game) { m_game.handleInput(gameTime); }

			if(m_activeScreen != ScreenType.Console) {
				m_menu.handleInput();
			}

			m_console.handleInput();
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
