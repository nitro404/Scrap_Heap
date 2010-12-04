using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace SproketEngine {

    enum WeaponType { Pistol = 0, ShrapnelCannon = 1, GaussGun = 2, SlugThrower = 3, MachineRifle = 4, Experimental, Sniper, Electricity, Explosive, Special }

	class WeaponCollection {

		private List<Weapon> m_weapons;
		private WeaponType m_currentWeapon = WeaponType.Pistol;

		private List<Model> m_models;

		public WeaponCollection() {
			m_weapons = new List<Weapon>();
			m_models = new List<Model>();
		}

		public void loadContent(ContentManager content) {
			m_models.Add(content.Load<Model>("Models\\Weapons\\Pistol"));
            m_models.Add(content.Load<Model>("Models\\Weapons\\ShrapCan"));
            m_models.Add(content.Load<Model>("Models\\Weapons\\GaussGun"));
            m_models.Add(content.Load<Model>("Models\\Weapons\\SlugThrower"));
            m_models.Add(content.Load<Model>("Models\\Weapons\\MachineRifle"));

			m_weapons.Add(new Weapon(m_models[0], new Ammunition(AmmunitionType._9mm)));
            m_weapons.Add(new Weapon(m_models[1], new Ammunition(AmmunitionType.FragCharge)));
            m_weapons.Add(new Weapon(m_models[2], new Ammunition(AmmunitionType.Bolt)));
            m_weapons.Add(new Weapon(m_models[3], new Ammunition(AmmunitionType.Slug)));
            m_weapons.Add(new Weapon(m_models[4], new Ammunition(AmmunitionType._7_62x39mm)));
		}

		public void initialize() {
			
		}

		public void setLighting(Vector3 lighting) {
			for(int i=0;i<m_weapons.Count();i++) {
				m_weapons[i].lighting = lighting;
			}
		}

		public bool selectWeapon(int weaponNumber) {
			if(weaponNumber >= 0 && weaponNumber < m_weapons.Count()) {
				m_currentWeapon = (WeaponType) weaponNumber;
				return true;
			}
			return false;
		}

		public void draw(Vector3 position, Vector3 forward, Vector3 rotation, Matrix view, Matrix projection) {
			if(m_currentWeapon >= 0 && m_weapons.Count() > 0) {
				m_weapons[(int) m_currentWeapon].draw(position, forward, rotation, view, projection);
			}
		}

	}

}
