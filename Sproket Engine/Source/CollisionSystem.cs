using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XNAQ3Lib.Q3BSP;
using Microsoft.Xna.Framework;

namespace SproketEngine {

	class CollisionSystem {

		private ScrapHeap m_game;
		private Player m_player;
		private Q3BSPLevel m_level;
		private GameSettings m_settings;

		public CollisionSystem() {

		}

		public void initialize(ScrapHeap game, GameSettings settings, Player player, Q3BSPLevel level) {
			m_game = game;
			m_settings = settings;
			m_player = player;
			m_level = level;
		}

		public Q3BSPLevel level {
			get { return m_level; }
			set { m_level = value; }
		}

		public void update(GameTime gameTime) {
			if(!m_settings.clipping) { return; }

			Q3BSPCollisionData collision = m_level.TraceRay(m_player.lastPosition, m_player.position);
			m_player.position = collision.collisionPoint;
		}

	}

}
