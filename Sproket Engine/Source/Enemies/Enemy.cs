using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace SproketEngine {

    class Enemy : MovableEntity{

        protected float m_scale;

		protected int m_health;
		protected int m_maxHealth;

		protected static Player s_player;

		public Enemy(Vector3 position, Vector3 rotation, Model model, Vector3 dimensions, float scale,
					 float maxSpeed, float turnSpeed, float acceleration, float deceleration,
					 float maxClimbAngle, float jumpStrength, int maxHealth) :
			base(position, rotation, model, dimensions,
				 maxSpeed, turnSpeed, acceleration, deceleration,
				 maxClimbAngle, jumpStrength) {

			m_maxHealth = maxHealth;
			m_health = m_maxHealth;

			m_position = position;
			m_rotation = rotation;
			m_model = model;
            m_scale = scale;
		}

		public static void setPlayer(Player player) {
			s_player = player;
		}

        public bool modelLoaded() {
            return m_model != null;
        }

		public int health {
			get { return m_health; }
		}

		public void rotateTo(Vector3 position, GameTime gameTime) {
			Vector3 forward = Vector3.Forward;
			forward.Normalize();

			Vector3 toward = m_position - position;
			toward.Normalize();

			//float angle = (float) Math.Acos(Vector3.Dot(forward, toward));
			//angle of v2 relative to v1 = atan2(v2.y,v2.x) - atan2(v1.y,v1.x)
			float angle = (float) Math.Atan2(forward.Z, forward.X) - (float) Math.Atan2(toward.Z, toward.X);

			m_rotation.Y = angle;

			//if (Math.Abs(angle) < m_rotationSpeed * gameTime.ElapsedGameTime.TotalSeconds) {
			//    m_rotation.Y += angle;
			//}
			//else {
			//    m_rotation.Y += m_rotationSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds * Math.Sign(angle);
			//}


		}
		public override void update(GameTime gameTime) {
			//m_forward = Vector3.Normalize(new Vector3((float) Math.Sin(-m_rotation.Y), (float) Math.Sin(m_rotation.X), (float) Math.Cos(-m_rotation.Y)));
			//m_left = Vector3.Normalize(new Vector3((float) Math.Cos(m_rotation.Y), 0f, (float) Math.Sin(m_rotation.Y)));
			m_forward = Vector3.Transform(Vector3.Forward, Matrix.CreateRotationY(m_rotation.Y));
			m_left = Vector3.Transform(Vector3.Left, Matrix.CreateRotationY(m_rotation.Y));
			base.update(gameTime);
		}
		public override void draw(Matrix view, Matrix projection) {
			//TODO: Rotation
			Matrix world = Matrix.CreateScale(m_scale, m_scale, m_scale) * 
                Matrix.CreateRotationY(m_rotation.Y) *
                Matrix.CreateTranslation(m_position);
			base.draw(world, view, projection);
		}
    }

}
