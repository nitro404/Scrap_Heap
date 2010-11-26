using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace SproketEngine {

	enum WeaponCategory { Pistol, ShotgunSMG, AssaultHeavy, Railgun, ShrapnelCannon, Experimental, Sniper, Electricity, Explosive, Special }

	class WeaponCollection {

		private List<Weapon> m_weapons;
		private int m_currentWeapon = -1;

		private List<Model> m_models;

		public WeaponCollection() {
			m_weapons = new List<Weapon>();
			m_models = new List<Model>();
		}

		public void loadContent(ContentManager content) {
			m_models.Add(content.Load<Model>("Models\\Gun1"));

			m_weapons.Add(new Weapon(m_models[0], new Ammunition(AmmunitionType._9mm)));

			m_currentWeapon = 0;
		}

		public void initialize() {
			
		}

		public void draw(Vector3 position, Vector3 forward, Vector3 rotation, Matrix view, Matrix projection) {
			if(m_currentWeapon >= 0 && m_weapons.Count() > 0) {
				m_weapons[m_currentWeapon].draw(position, forward, rotation, view, projection);
			}
		}

	}

}
