using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SproketEngine {

	enum AmmunitionType {
		_454Casull, _357Magnum, _9mm, // pistols / sub-machine guns
		_7_62x39mm, _5_56x45mm, // assault rifles / machine guns
        Slug, // slug thrower
		Bolt, // railgun
		FragCharge, // shrapnel cannon
		VoltPack, // electrical weapons
		Rocket // rocket launcher
	}

	class AmmunitionCollection {

		private List<Ammunition> m_ammunition;

		public AmmunitionCollection() {
			m_ammunition = new List<Ammunition>();
		}

	}

}
