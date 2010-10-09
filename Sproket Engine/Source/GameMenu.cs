using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace SproketEngine {

	class GameMenu {

		private bool m_active = false;

		private GameSettings m_settings;
		private CommandInterpreter m_interpreter;

		public GameMenu() { }

		public bool active {
			get { return m_active; }
			set { m_active = value; }
		}

		public void toggle() { m_active = !m_active; }

		public void open() { m_active = true; }

		public void close() { m_active = false; }

		public void initialize(GameSettings settings, CommandInterpreter interpreter) {
			m_settings = settings;
			m_interpreter = interpreter;
		}

		public void loadContent(ContentManager content) {
			
		}

		public void handleInput() {

		}

		public void update(GameTime gameTime) {

		}

		public void draw(SpriteBatch spriteBatch) {

		}

	}

}
