using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace SproketEngine {
	class Ricket : Enemy {
		static Model s_model;

		public Ricket(Vector3 position, Vector3 rotation) :
			base(position, rotation, s_model, new Vector3(4, 14, 4), 0.025f, 
				 10.0f, 20.0f, 6.0f, -50.0f, 45, 75, 100) {
			
		}

		public static void loadContent(Model model) {
			s_model = model;
		}

		//AI
		public override void update(GameTime gameTime) {
			if (!m_active)
				return;

			rotateTo(s_player.position, gameTime);
			moveForward();
			base.update(gameTime);
		}

		//Custom Collision Detection 
		public override void handleCollision(XNAQ3Lib.Q3BSP.Q3BSPLevel level, GameTime gameTime) {
			base.handleCollision(level, gameTime);
			//Gravity
			//Check if on floor
			Q3BSPCollisionData collision = level.TraceBox(position, position - new Vector3(0, 1, 0), minPoint, maxPoint);
			if (collision.collisionPoint == collision.endPosition || isJumping) {
				//Not on floor so check gravity
				updateGravity(gameTime);
				collision = level.TraceBox(position, newPosition, minPoint, maxPoint);
				if (collision.collisionPoint != collision.endPosition) {
					resetGravity();
				}
				position = collision.collisionPoint;
			}
			else {
				//On floor don't do gravity, but reset it
				resetGravity();
			}
		}
	}
}
