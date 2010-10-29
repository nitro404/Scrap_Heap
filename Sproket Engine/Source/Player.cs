using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace SproketEngine {

    class Player {

		private int m_id;
		private static int m_idCounter = 0;
		private string m_name;

		private Vector3 m_position;
		private Vector3 m_rotation;
		private Model m_model;


        private int m_health;
		private int m_maxHealth = 100;

        private WeaponCollection m_weapons;
        private AmmunitionCollection m_ammo;

		public Player(int playerID, string playerName, Vector3 position, Vector3 rotation, Model model) {
			m_id = m_idCounter++;
			m_name = playerName;
			m_health = m_maxHealth;

			m_position = position;
			m_rotation = rotation;
			m_model = model;

			m_ammo = new AmmunitionCollection();
			m_weapons = new WeaponCollection();
		}

		public Vector3 position {
			get { return m_position; }
		}

		public Vector3 rotation {
			get { return m_rotation; }
		}

		public int id {
			get { return m_id; }
		}

		public string name {
			get { return m_name; }
		}

		public int health {
			get { return m_health; }
		}

    }

}
