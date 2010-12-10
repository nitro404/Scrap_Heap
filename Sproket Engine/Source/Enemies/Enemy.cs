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

        private float m_scale;

		private int m_health;
		private int m_maxHealth;

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

        public bool modelLoaded() {
            return m_model != null;
        }

		public int health {
			get { return m_health; }
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
