using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace SproketEngine {

    class Entity {

		private int m_id;
		private static int m_idCounter = 0;

        private Vector3 m_position;
        private Vector3 m_rotation;
        private Model m_model;

        public Entity(Vector3 position, Vector3 rotation, Model model) {
			m_id = m_idCounter++;

			m_position = position;
			m_rotation = rotation;
			m_model = model;
        }

		public Vector3 position {
			get { return m_position; }
		}

		public Vector3 rotation {
			get { return m_rotation; }
		}

    }
}
