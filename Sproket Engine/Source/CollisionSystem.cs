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

        private Vector3 m_mins = new Vector3(-2, -8, -2);
        private Vector3 m_maxs = new Vector3(2, 2, 2);
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
			Q3BSPCollisionData collision = m_level.TraceBox(m_player.lastPosition, m_player.position, m_mins, m_maxs);
			Vector3 point = collision.collisionPoint;

            Vector3 newposition = point - new Vector3(0, 1, 0);
            collision = m_level.TraceBox(point, newposition, m_mins, m_maxs);
            m_player.position = collision.collisionPoint;
		}

	}

}
