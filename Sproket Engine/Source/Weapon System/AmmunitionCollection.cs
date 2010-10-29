using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SproketEngine {

	enum AmmunitionType {
		_454Casull, _357Magnum, _9mm, // pistols / sub-machine guns
		_12Gauge, // shotguns
		_7_62x39mm, _5_56x45mm, // assault rifles / machine guns
		Rail, // railgun
		Shrapnel, // shrapnel cannon
		_50BMG, // sniper rifles
		ElectricityModule, // electrical weapons
		Rocket // rocket launcher
	}

	class AmmunitionCollection {

		private List<Ammunition> m_ammunition;

		public AmmunitionCollection() {
			m_ammunition = new List<Ammunition>();
		}

	}

}
