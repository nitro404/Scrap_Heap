using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using XNAQ3Lib.Q3BSP;

namespace SproketEngine {

	class MovableEntity : Entity {

		protected bool m_moving;
		protected bool m_jumping;
		protected Vector3 m_newPosition;
		protected Vector3 m_velocity;
		protected Vector3 m_gravity;
		protected Vector3 m_forward;
		protected Vector3 m_left;

		protected float m_maxClimb; // maximum angle entity can climb
		protected float m_jumpStrength;

		protected float m_rotationSpeed;
		protected float m_movementSpeed;
		protected float m_acceleration;
		protected float m_deceleration;


		public MovableEntity(Vector3 position, Vector3 rotation, Model model, Vector3 dimensions,
							 float maxSpeed, float turnSpeed, float acceleration, float deceleration,
							 float maxClimbAngle, float jumpStrength) :
			base(position, rotation, model, dimensions) {
		
			m_newPosition = position;
			m_movementSpeed = maxSpeed;
			m_rotationSpeed = turnSpeed;
			m_acceleration = acceleration;
			m_deceleration = deceleration;

			m_maxClimb = maxClimbAngle;
			m_jumpStrength = jumpStrength;

			m_forward = Vector3.Forward;
			m_left = Vector3.Left;
			m_velocity = Vector3.Zero;
			m_gravity = Vector3.Zero;
			m_moving = false;
			m_jumping = false;

			
		}

		public void reset() {
			m_moving = false;
			m_jumping = false;
			position = Vector3.Zero;
			m_newPosition = m_position;
			m_rotation = Vector3.Zero;
			m_forward = Vector3.Forward;
			m_left = Vector3.Left;
			m_gravity = Vector3.Zero;
			m_velocity = Vector3.Zero;
		}


		public Vector3 velocity {
			get { return m_velocity; }
			set { m_velocity = value; }
		}

		public Vector3 gravity {
			get { return m_gravity; }
		}

		public Vector3 newPosition {
			get { return m_newPosition; }
		}

		public Vector3 dimensions {
			get { return m_dimensions; }
		}

		public Vector3 minPoint {
			get { return m_minPoint; }
		}

		public Vector3 maxPoint {
			get { return m_maxPoint; }
		}

		public int id {
			get { return m_id; }
		}

		public float maxClimb {
			get { return m_maxClimb; }
		}

		public bool isJumping {
			get { return m_jumping; }
		}

		public void resetGravity() {
			m_gravity = Vector3.Zero;
			m_jumping = false;
		}

		public void jump() {
			if (!m_jumping) {
				m_gravity += Vector3.Up * m_jumpStrength;
			}
			m_jumping = true;
		}


		public virtual void update(GameTime gameTime) {
			if (!active)
				return;

			// compute acceleration vector
			float acceleration = (m_moving) ? m_acceleration : m_deceleration;
			Vector3 tempVelocity = new Vector3(m_velocity.X, m_velocity.Y, m_velocity.Z);
			if(tempVelocity.Length() != 0) { tempVelocity.Normalize(); }
			Vector3 accelerationVector = tempVelocity * acceleration;

			// apply acceleration to the velocity
			m_velocity += accelerationVector * (float) gameTime.ElapsedGameTime.TotalSeconds;

			// clamp speed
			if(m_velocity.Length() > m_movementSpeed) {
				m_velocity.Normalize();
				m_velocity *= m_movementSpeed;
			}
			else if(m_velocity.Length() < 1) {
				m_velocity = Vector3.Zero;
			}

			// set new position (for use in collision system)
			m_newPosition = m_position + (m_velocity * m_acceleration * (float) gameTime.ElapsedGameTime.TotalSeconds);
		}

		public void updateGravity(GameTime gameTime) {
			// compute gravity
			m_gravity.Y -= GameConstants.GRAVITY * (float) gameTime.ElapsedGameTime.TotalSeconds;
			m_newPosition = m_position + (m_gravity * (float) gameTime.ElapsedGameTime.TotalSeconds);
		}

		public override void handleCollision(Q3BSPLevel level, GameTime gameTime){
			if (!active)
				return;

			Q3BSPCollisionData collision = level.TraceBox(position, newPosition, minPoint, maxPoint);
			position = collision.collisionPoint;
		}

		public void moveForward() {
            Vector3 straight = new Vector3(m_forward.X, 0, m_forward.Z);
            straight.Normalize();
            straight *= m_forward.Length();
            m_velocity -= straight;
			m_moving = true;
		}

		public void moveBackward() {
            Vector3 straight = new Vector3(m_forward.X, 0, m_forward.Z);
            straight.Normalize();
            straight *= m_forward.Length();
            m_velocity += straight;
			m_moving = true;
		}

		public void moveLeft() {
            Vector3 straight = new Vector3(m_left.X, 0, m_left.Z);
            straight.Normalize();
            straight *= m_left.Length();
            m_velocity -= straight;
			m_moving = true;
		}

		public void moveRight() {
            Vector3 straight = new Vector3(m_left.X, 0, m_left.Z);
            straight.Normalize();
            straight *= m_left.Length();
            m_velocity += straight;
			m_moving = true;
		}

	}

}
