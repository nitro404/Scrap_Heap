using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SproketEngine {

	class GameSinglePlayerMenu : SubMenu {

		public GameSinglePlayerMenu(Menu parentMenu, Vector2 position, Color titleColour, Color selectedItemColour, Color unselectedItemColour, Color arrowColour)
			: base("Single Player", parentMenu, position, titleColour, selectedItemColour, unselectedItemColour, arrowColour) {
			m_parentMenu = parentMenu;
		}

		public override void setContent(SpriteFont titleFont, SpriteFont itemFont) {
			base.setContent(titleFont, itemFont);
			createMenu();
		}

		public void createMenu() {
			float x = m_position.X;
			float y = m_position.Y + m_titleFont.LineSpacing;
			addItem(new SimpleMenuItem("Test Map", x, y, m_itemFont, m_selectedItemColour, m_unselectedItemColour, m_arrowColour));
			y += m_itemFont.LineSpacing;
			addItem(new SimpleMenuItem("Back", x, y, m_itemFont, m_selectedItemColour, m_unselectedItemColour, m_arrowColour));
		}

		public override void up() {
			base.up();
		}

		public override void down() {
			base.down();
		}
		public override void left() {
			base.left();
		}

		public override void right() {
			base.right();
		}

		public override void select() {
			if(m_index == 0) {
				m_interpreter.execute("menu off");
				m_interpreter.execute("map test.bsp");
			}
			else if(m_index == 1) {
				m_parentMenu.back();
			}
		}

	}

}
