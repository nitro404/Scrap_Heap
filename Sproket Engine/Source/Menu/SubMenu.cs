using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SproketEngine {

	abstract class SubMenu {

		protected List<MenuItem> m_items;
		protected string m_title;
		protected int m_index = 0;
		protected Vector2 m_position;
		protected SpriteFont m_titleFont;
		protected SpriteFont m_itemFont;
		protected Color m_titleColour;
		protected Color m_selectedItemColour;
		protected Color m_unselectedItemColour;
		protected Color m_arrowColour;

		protected Menu m_parentMenu;
		protected CommandInterpreter m_interpreter;

		public SubMenu(string title, Menu parentMenu, Vector2 position, Color titleColour, Color selectedItemColour, Color unselectedItemColour, Color arrowColour) {
			m_items = new List<MenuItem>();
			m_title = title;
			m_parentMenu = parentMenu;
			m_position = position;
			m_titleColour = titleColour;
			m_selectedItemColour = selectedItemColour;
			m_unselectedItemColour = unselectedItemColour;
			m_arrowColour = arrowColour;
		}

		public virtual void initialize(CommandInterpreter interpreter) {
			m_interpreter = interpreter;
		}

		public virtual void setContent(SpriteFont titleFont, SpriteFont itemFont) {
			m_titleFont = titleFont;
			m_itemFont = itemFont;
		}

		public int size() {
			return m_items.Count();
		}

		public int index {
			get { return m_index; }
			set { if(value >= 0 && value < m_items.Count()) { m_index = value; } }
		}

		public bool addItem(MenuItem item) {
			if(item == null || m_items.Contains(item)) { return false; }
			m_items.Add(item);
			return true;
		}

		public MenuItem getItem(int index) {
			if(index < 0 || index >= m_items.Count()) { return null; }
			return m_items.ElementAt(index);
		}

		public virtual void up() {
			m_index--;
			if(m_index < 0) { m_index = m_items.Count() - 1; }
		}

		public virtual void down() {
			m_index++;
			if(m_index >= m_items.Count()) { m_index = 0; }
		}

		public virtual void left() {
			m_items.ElementAt(m_index).left();
		}

		public virtual void right() {
			m_items.ElementAt(m_index).right();
		}

		public abstract void select();

		public void reset() {
			m_index = 0;
		}

		public void update(GameTime gameTime) {
			for(int i=0;i<m_items.Count();i++) {
				m_items[i].update(gameTime);
			}
		}

		public void draw(SpriteBatch spriteBatch) {
			spriteBatch.DrawString(m_titleFont, m_title, m_position, m_titleColour);
			for(int i=0;i<m_items.Count();i++) {
				m_items[i].draw(spriteBatch, i == m_index);
			}
		}

	}

}
