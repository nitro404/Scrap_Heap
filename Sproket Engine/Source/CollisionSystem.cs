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
			if(!m_settings.clipping) {
				m_player.position = m_player.newPosition;
				return;
			}

			Q3BSPCollisionData collision = m_level.TraceBox(m_player.position, m_player.newPosition, m_player.minPoint, m_player.maxPoint);
			Vector3 point = collision.collisionPoint;

			if (collision.collisionPoint != collision.endPosition) {
				Vector3 offset = new Vector3(0, 0.5f, 0);
				Vector3 start = collision.startPosition + offset;
				Vector3 end = collision.endPosition + offset;

				Q3BSPCollisionData slopetest = m_level.TraceBox(start, end, m_player.minPoint, m_player.maxPoint);

				if (slopetest.collisionPoint != collision.collisionPoint + offset) {
					float opp = offset.Length();
					float adj = (end - start).Length();
					float theta = MathHelper.ToDegrees((float)Math.Atan(opp/adj));
					if(theta < m_player.maxClimb)
						point = slopetest.collisionPoint;
				}
			}
			m_player.position = point;

			//Gravity
			//Check if on floor
			collision = m_level.TraceBox(m_player.position, m_player.position - new Vector3 (0, 1, 0), m_player.minPoint, m_player.maxPoint);
			if (collision.collisionPoint == collision.endPosition || m_player.isJumping) {
				//Not on floor so check gravity
				m_player.updateGravity(gameTime);
				collision = m_level.TraceBox(m_player.position, m_player.newPosition, m_player.minPoint, m_player.maxPoint);
				if (collision.collisionPoint != collision.endPosition) {
					m_player.resetGravity();
				}
				m_player.position = collision.collisionPoint;
			}
			else {
				//On floor don't do gravity, but reset it
				m_player.resetGravity();
			}
		}

	}

}
