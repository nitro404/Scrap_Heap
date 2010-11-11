using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;

namespace SproketEngine {

	class Camera {

		private Matrix m_view;
		private Matrix m_projection;

		protected float m_rotationSpeed = 20.0f;
		protected float m_movementSpeed = 75.0f;

		private float m_fov = 75.0f;
		private float m_aspectRatio;
		private float m_nearPlane = 0.1f;
		private float m_farPlane = 10000.0f;

		protected Vector3 m_position;
		protected Vector3 m_lastPosition;
		protected Vector3 m_rotation;
		private Vector3 m_forward;
		private Vector3 m_left;

		private GameSettings m_settings;

		public Camera() {
            reset();
		}

		public void initialize(GameSettings settings) {
			m_settings = settings;

			m_aspectRatio = (float) settings.screenWidth / (float) settings.screenHeight;

			m_projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(m_fov), m_aspectRatio, m_nearPlane, m_farPlane);
		}

		public Vector3 position {
			get { return m_position; }
			set { m_position = value; }
		}

		public Vector3 lastPosition {
			get { return m_lastPosition; }
		}

		public Matrix view {
			get { return m_view; }
		}

		public Matrix projection {
			get { return m_projection; }
		}

		public virtual void reset() {
			m_position = Vector3.Zero;
			m_lastPosition = m_position;
			m_rotation = Vector3.Zero;
			m_forward = Vector3.Forward;
			m_left = Vector3.Left;
            m_view = Matrix.Identity;
		}

		public virtual void handleInput(GameTime gameTime, bool gameIsActive) {
			KeyboardState keyboard = Keyboard.GetState();
			MouseState mouse = Mouse.GetState();

			m_lastPosition = m_position;

			float timeElapsed = (float) gameTime.ElapsedGameTime.TotalSeconds;

			m_rotation.X += MathHelper.ToRadians((mouse.Y - m_settings.screenHeight / 2) * m_rotationSpeed * 0.01f);
			m_rotation.Y += MathHelper.ToRadians((mouse.X - m_settings.screenWidth / 2) * m_rotationSpeed * 0.01f);

			m_forward = Vector3.Normalize(new Vector3((float) Math.Sin(-m_rotation.Y), (float) Math.Sin(m_rotation.X), (float) Math.Cos(-m_rotation.Y)));
			m_left = Vector3.Normalize(new Vector3((float) Math.Cos(m_rotation.Y), 0f, (float) Math.Sin(m_rotation.Y)));

            Vector3 moveForward;
            Vector3 moveLeft = m_left;

            if (m_settings.m_clipping) { //Clipping On
                moveForward = new Vector3(m_forward.X, 0, m_forward.Z);
                moveLeft = new Vector3(m_left.X, 0, m_left.Z);
            }
            else {
                moveForward = m_forward;
                moveLeft = m_left;
            }

            if(keyboard.IsKeyDown(Keys.W)) {
				m_position -= m_movementSpeed * timeElapsed * moveForward;
            }

            if (keyboard.IsKeyDown(Keys.S))
            {
                m_position += m_movementSpeed * timeElapsed * moveForward;
            }

            if (keyboard.IsKeyDown(Keys.A))
            {
                m_position -= m_movementSpeed * timeElapsed * moveLeft;
            }

            if (keyboard.IsKeyDown(Keys.D))
            {
                m_position += m_movementSpeed * timeElapsed * moveLeft;
            }
			if(keyboard.IsKeyDown(Keys.Space)) {
				m_position.Y += m_movementSpeed*2 * timeElapsed;
			}

			if(keyboard.IsKeyDown(Keys.LeftControl)) {
				m_position.Y -= m_movementSpeed * timeElapsed;
			}

			m_view = Matrix.CreateTranslation(-m_position) *
					 Matrix.CreateRotationY(m_rotation.Y) *
					 Matrix.CreateRotationX(m_rotation.X);

			if(gameIsActive) {
				Mouse.SetPosition(m_settings.screenWidth / 2, m_settings.screenHeight / 2);
			}

		}

	}

}
