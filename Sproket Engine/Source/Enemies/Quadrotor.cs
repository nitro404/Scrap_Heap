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
		private static float m_minSpinSpeed = 2.5f;
		private static float m_maxSpinSpeed = 8.0f;

		private QuadrotorSpinState m_spinState = QuadrotorSpinState.Idle;
		private QuadrotorMovementState m_movementState = QuadrotorMovementState.Idle;

		private float m_lastSpinStateChange = 0.0f;
		private float m_nextSpinStateChange = 0.0f;

		private float m_lastMovementStateChange = 0.0f;
		private float m_nextMovementStateChange = 0.0f;

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
			randomizeSpinSpeed();
			randomizeSpinDirection();
		}

		private void randomizeSpinSpeed() {
			m_actualSpinSpeed = (float) ((m_random.NextDouble() * (m_maxSpinSpeed - m_minSpinSpeed)) + m_minSpinSpeed);
		}

		private void randomizeSpinDirection() {
			m_spinDirection = (((m_random.Next(0, 2) % 2) == 0) ? 1 : -1);
		}

		private void updateSpinState() {
			if(m_spinState == QuadrotorSpinState.Idle) {
				if(m_lastSpinStateChange >= m_nextSpinStateChange) {
					m_spinState = QuadrotorSpinState.SpinUp;
					randomizeSpinSpeed();
					m_lastSpinStateChange = 0.0f;
					m_nextSpinStateChange = 0.0f;
				}
			}
			else if(m_spinState == QuadrotorSpinState.SpinUp) {
				if(m_spinSpeed >= m_actualSpinSpeed) {
					m_spinState = QuadrotorSpinState.Spinning;
					m_lastSpinStateChange = 0.0f;
					m_nextSpinStateChange = (float) (m_random.NextDouble() * 11.0f) + 6.0f;
				}
				else {
					m_spinSpeed += m_actualSpinSpeed / (float) ((m_random.NextDouble() * 100.0f) + 150.0f);
					if(m_spinSpeed > m_actualSpinSpeed) {
						m_spinSpeed = m_actualSpinSpeed;
					}
				}
			}
			else if(m_spinState == QuadrotorSpinState.Spinning) {
				if(m_lastSpinStateChange >= m_nextSpinStateChange) {
					m_lastSpinStateChange = 0.0f;
					m_nextSpinStateChange = 0.0f;
					m_spinState = QuadrotorSpinState.SpinDown;
				}
			}
			else if(m_spinState == QuadrotorSpinState.SpinDown) {
				if(m_spinSpeed <= m_minSpinSpeed) {
					m_spinState = QuadrotorSpinState.Idle;
					m_lastSpinStateChange = 0.0f;
					m_nextSpinStateChange = (float) (m_random.NextDouble() * 2.0f) + 3.0f;
				}
				else {
					m_spinSpeed -= m_actualSpinSpeed / (float) ((m_random.NextDouble() * 100.0f) + 150.0f);
					if(m_spinSpeed <= m_minSpinSpeed) {
						m_spinSpeed = m_minSpinSpeed;
					}
				}
			}
		}

		private void updateMovementState() {
			//m_movementState = (QuadrotorMovementState) m_random.Next(0, 2);
		}

		public override void update(GameTime gameTime) {
			base.update(gameTime);

			m_spin += (float) m_spinDirection * m_spinSpeed * (float) gameTime.ElapsedGameTime.TotalSeconds;

			if(m_spin > 360.0f) { m_spin -= 360.0f; }
			if(m_spin < 0.0f) { m_spin += 360.0f; }

			m_lastSpinStateChange += (float) gameTime.ElapsedGameTime.TotalSeconds;
			m_lastMovementStateChange += (float) gameTime.ElapsedGameTime.TotalSeconds;

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
