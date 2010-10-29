using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SproketEngine {

	enum WeaponCategory { Pistol, ShotgunSMG, AssaultHeavy, Railgun, ShrapnelCannon, Experimental, Sniper, Electricity, Explosive, Special }

	class WeaponCollection {

		private List<Weapon> m_weapons;

		public WeaponCollection() {
			m_weapons = new List<Weapon>();
		}

	}

}
