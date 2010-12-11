using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace SproketEngine {

	enum QuadrotorSpinState { Idle=0, SpinUp=1, Spinning=2, SpinDown=3, }

	enum QuadrotorMovementState { Idle=0, Moving=1, }

	class Quadrotor : Enemy {

		private float m_spin = 0.0f;
		private float m_spinSpeed = 0.0f;
		private float m_actualSpinSpeed;
		private int m_spinDirection = 1;
		private static float m_minSpinSpeed = 3.5f;
		private static float m_maxSpinSpeed = 14.0f;

		private QuadrotorSpinState m_spinState = QuadrotorSpinState.Idle;
		//private QuadrotorMovementState m_movementState = QuadrotorMovementState.Idle;

		private float m_lastSpinStateChange = 0.0f;
		private float m_nextSpinStateChange = 0.0f;

		private float m_lastMovementStateChange = 0.0f;
		//private float m_nextMovementStateChange = 0.0f;

		private static Random m_random = new Random();

		static Model s_model;

		public Quadrotor(Vector3 position, Vector3 rotation) :
			base(position, rotation, s_model, new Vector3(4, 14, 4), 0.025f, 
				 10.0f, 20.0f, 6.0f, -50.0f, 45, 75, 100) {

			randomizeSpin();
		}

		public static void loadContent(Model model) {
			s_model = model;
		}

		private void randomizeSpin() {
			// randomize both the spin speed and direction of the quadrotor
			randomizeSpinSpeed();
			randomizeSpinDirection();
		}

		private void randomizeSpinSpeed() {
			// randomize the spinning speed of the quadrotor
			m_actualSpinSpeed = (float) ((m_random.NextDouble() * (m_maxSpinSpeed - m_minSpinSpeed)) + m_minSpinSpeed);
		}

		private void randomizeSpinDirection() {
			// randomize the spin direction of the quadrotor
			m_spinDirection = (((m_random.Next(0, 2) % 2) == 0) ? 1 : -1);
		}

		private void updateSpinState() {
			// if the state is idle, check to see if the quadrotor should start spinning up
			if(m_spinState == QuadrotorSpinState.Idle) {
				if(m_lastSpinStateChange >= m_nextSpinStateChange) {
					m_spinState = QuadrotorSpinState.SpinUp;
					randomizeSpinSpeed();
					m_lastSpinStateChange = 0.0f;
					m_nextSpinStateChange = 0.0f;
				}
			}
			// if the state is spinup, check to see if the quadrotor is at it's desired spinning speed, then set the state to spinning
			// if not, increment the current spin speed
			else if(m_spinState == QuadrotorSpinState.SpinUp) {
				if(m_spinSpeed >= m_actualSpinSpeed) {
					m_spinState = QuadrotorSpinState.Spinning;
					m_lastSpinStateChange = 0.0f;
					m_nextSpinStateChange = (float) (m_random.NextDouble() * 11.0f) + 6.0f;
				}
				else {
					m_spinSpeed += m_actualSpinSpeed / (float) ((m_random.NextDouble() * 75.0f) + 125.0f);
					if(m_spinSpeed > m_actualSpinSpeed) {
						m_spinSpeed = m_actualSpinSpeed;
					}
				}
			}
			// if the state is set to spinning, check to see if the spin time has elapsed, then set state to spindown
			else if(m_spinState == QuadrotorSpinState.Spinning) {
				if(m_lastSpinStateChange >= m_nextSpinStateChange) {
					m_lastSpinStateChange = 0.0f;
					m_nextSpinStateChange = 0.0f;
					m_spinState = QuadrotorSpinState.SpinDown;
				}
			}
			// if the state is spindown, check to see if the quadrotor is at it's minimum spinning speed, then set the state to idle
			// if not, decrement the current spin speed
			else if(m_spinState == QuadrotorSpinState.SpinDown) {
				if(m_spinSpeed <= 2.75f) {
					m_spinState = QuadrotorSpinState.Idle;
					m_lastSpinStateChange = 0.0f;
					m_nextSpinStateChange = (float) (m_random.NextDouble() * 2.0f) + 3.0f;
				}
				else {
					m_spinSpeed -= m_actualSpinSpeed / (float) ((m_random.NextDouble() * 75.0f) + 125.0f);
					if(m_spinSpeed <= 2.75f) {
						m_spinSpeed = 2.75f;
					}
				}
			}
		}

		private void updateMovementState() {
			//m_movementState = (QuadrotorMovementState) m_random.Next(0, 2);
		}

		public override void update(GameTime gameTime) {
			base.update(gameTime);

			// spin the quadrotor
			m_spin += (float) m_spinDirection * m_spinSpeed * (float) gameTime.ElapsedGameTime.TotalSeconds;

			if(m_spin > 360.0f) { m_spin -= 360.0f; }
			if(m_spin < 0.0f) { m_spin += 360.0f; }

			// increment the last state change timers
			m_lastSpinStateChange += (float) gameTime.ElapsedGameTime.TotalSeconds;
			m_lastMovementStateChange += (float) gameTime.ElapsedGameTime.TotalSeconds;

			// check for state updates (and handle state updating)
			updateSpinState();
			updateMovementState();
		}

		//Custom Collision Detection 
		public override void handleCollision(XNAQ3Lib.Q3BSP.Q3BSPLevel level, GameTime gameTime) {
			base.handleCollision(level, gameTime);
		}

		public override void draw(Matrix view, Matrix projection) {
			Matrix world = Matrix.CreateScale(m_scale, m_scale, m_scale) * 
                Matrix.CreateRotationY(m_spin) *
                Matrix.CreateTranslation(m_position);
			
			if(m_model == null)
				return;

			// render the quadrotor based on it's current spin position
			Matrix[] transforms = new Matrix[m_model.Bones.Count];
			m_model.CopyAbsoluteBoneTransformsTo(transforms);
			foreach(ModelMesh mesh in m_model.Meshes) {
				foreach(BasicEffect effect in mesh.Effects) {
					effect.EnableDefaultLighting();
					effect.DirectionalLight1.DiffuseColor = m_lighting;
					effect.World = transforms[mesh.ParentBone.Index] * world;
					effect.View = view;
					effect.Projection = projection;

				}
				mesh.Draw();
			}
		}

	}

}
