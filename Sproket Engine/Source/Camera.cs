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

		private float m_rotationSpeed = 20.0f;
		private float m_movementSpeed = 75.0f;

		private float m_fov = 90.0f;
		private float m_aspectRatio;
		private float m_nearPlane = 0.1f;
		private float m_farPlane = 10000.0f;

		private Vector3 m_position;
		private Vector3 m_rotation;
		private Vector3 m_forward;
		private Vector3 m_left;

		private ScrapHeap m_game;

		public Camera(ScrapHeap game) {
			m_game = game;
            reset();
		}

		public void initialize(GraphicsDevice graphics) {
			m_aspectRatio = graphics.Viewport.Width / graphics.Viewport.Height;

			m_projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(m_fov), m_aspectRatio, m_nearPlane, m_farPlane);
		}

		public Vector3 position {
			get { return m_position; }
		}

		public Matrix view {
			get { return m_view; }
		}

		public Matrix projection {
			get { return m_projection; }
		}

		public void reset() {
			m_position = Vector3.Zero;
			m_rotation = Vector3.Zero;
			m_forward = Vector3.Forward;
			m_left = Vector3.Left;
            m_view = Matrix.Identity;
		}

		public void handleInput(GameTime gameTime) {
			KeyboardState keyboard = Keyboard.GetState();
			MouseState mouse = Mouse.GetState();

			m_rotation.X += MathHelper.ToRadians((mouse.Y - m_game.Window.ClientBounds.Y / 2) * m_rotationSpeed * 0.01f); // pitch
			m_rotation.Y += MathHelper.ToRadians((mouse.X - m_game.Window.ClientBounds.X / 2) * m_rotationSpeed * 0.01f); // yaw

			m_forward = Vector3.Normalize(new Vector3((float) Math.Sin(-m_rotation.Y), (float)Math.Sin(m_rotation.X), (float)Math.Cos(-m_rotation.Y)));
			m_left = Vector3.Normalize(new Vector3((float) Math.Cos(m_rotation.Y), 0f, (float)Math.Sin(m_rotation.Y)));

			if(keyboard.IsKeyDown(Keys.W)) {
				m_position -= m_movementSpeed * (float) gameTime.ElapsedGameTime.TotalSeconds * m_forward;
			}

			if(keyboard.IsKeyDown(Keys.S)) {
				m_position += m_movementSpeed * (float) gameTime.ElapsedGameTime.TotalSeconds * m_forward;
			}

			if(keyboard.IsKeyDown(Keys.A)) {
				m_position -= m_movementSpeed * (float) gameTime.ElapsedGameTime.TotalSeconds * m_left;
			}

			if(keyboard.IsKeyDown(Keys.D)) {
				m_position += m_movementSpeed * (float) gameTime.ElapsedGameTime.TotalSeconds * m_left;
			}

			if(keyboard.IsKeyDown(Keys.LeftShift)) {
				m_position.Y += m_movementSpeed * (float) gameTime.ElapsedGameTime.TotalSeconds;
			}

			if(keyboard.IsKeyDown(Keys.LeftControl)) {
				m_position.Y -= m_movementSpeed * (float) gameTime.ElapsedGameTime.TotalSeconds;
			}

			m_view = Matrix.Identity;
			m_view *= Matrix.CreateTranslation(-m_position);
			m_view *= Matrix.CreateRotationY(m_rotation.Y);
			m_view *= Matrix.CreateRotationX(m_rotation.X);

			if(m_game.IsActive) {
				Mouse.SetPosition(m_game.Window.ClientBounds.X / 2, m_game.Window.ClientBounds.Y / 2);
			}

		}

	}

}
