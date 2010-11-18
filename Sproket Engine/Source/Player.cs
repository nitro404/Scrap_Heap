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

    class Player {

		private int m_id;
		private static int m_idCounter = 0;
		private string m_name;

		private Vector3 m_position;
		private Vector3 m_lastPosition;
		private Vector3 m_rotation;
		private Vector3 m_forward;
		private Vector3 m_left;

		private Vector3 m_dimensions = new Vector3(4, 14, 4);
		private Vector3 m_minPoint;
		private Vector3 m_maxPoint;

		private float m_rotationSpeed = 20.0f;
		private float m_movementSpeed = 75.0f;

		private Model m_model;

        private int m_health;
		private int m_maxHealth = 100;

        private WeaponCollection m_weapons;
        private AmmunitionCollection m_ammo;

		private Camera m_camera;

		private GameSettings m_settings;

		public Player(string playerName, Vector3 position, Vector3 rotation, Model model) {
			m_id = m_idCounter++;
			m_name = playerName;
			m_health = m_maxHealth;

			m_position = position;
			m_rotation = rotation;
			
			float xSize = (m_dimensions.X / 2);
			float ySize = m_dimensions.Y;
			float zSize = (m_dimensions.Z / 2);
			m_minPoint = new Vector3(-xSize, 0, -zSize);
			m_maxPoint = new Vector3(xSize, ySize, zSize);

			m_model = model;

			m_ammo = new AmmunitionCollection();
			m_weapons = new WeaponCollection();

			m_camera = new Camera();
		}

		public void initialize(GameSettings settings) {
			m_settings = settings;

			m_camera.initialize(settings);
		}

		public Vector3 position {
			get { return m_position; }
			set { m_position = value; }
		}

		public Vector3 lastPosition {
			get { return m_lastPosition; }
		}

		public Vector3 rotation {
			get { return m_rotation; }
		}

		public Vector3 dimensions {
			get { return m_dimensions; }
		}

		public Vector3 minPoint {
			get { return m_minPoint; }
		}

		public Vector3 maxPoint {
			get { return m_maxPoint ; }
		}

		public Matrix view {
			get { return m_camera.view; }
		}

		public Matrix projection {
			get { return m_camera.projection; }
		}

		public int id {
			get { return m_id; }
		}

		public string name {
			get { return m_name; }
		}

		public int health {
			get { return m_health; }
		}

		public void reset() {
			m_camera.reset();
			position = Vector3.Zero;
			m_lastPosition = m_position;
			m_rotation = Vector3.Zero;
			m_forward = Vector3.Forward;
			m_left = Vector3.Left;
		}

		public void update(GameTime gameTime) {

		}

		public void handleInput(GameTime gameTime, bool gameIsActive) {
			KeyboardState keyboard = Keyboard.GetState();
			MouseState mouse = Mouse.GetState();

			m_lastPosition = m_position;

			float timeElapsed = (float) gameTime.ElapsedGameTime.TotalSeconds;

			m_rotation.X += MathHelper.ToRadians((mouse.Y - m_settings.screenHeight / 2) * m_rotationSpeed * 0.01f);
			m_rotation.Y += MathHelper.ToRadians((mouse.X - m_settings.screenWidth / 2) * m_rotationSpeed * 0.01f);

			m_forward = Vector3.Normalize(new Vector3((float) Math.Sin(-m_rotation.Y), (float) Math.Sin(m_rotation.X), (float) Math.Cos(-m_rotation.Y)));
			m_left = Vector3.Normalize(new Vector3((float) Math.Cos(m_rotation.Y), 0f, (float) Math.Sin(m_rotation.Y)));

            Vector3 moveForward;
            Vector3 moveLeft;

            if (m_settings.m_clipping) {
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

            if (keyboard.IsKeyDown(Keys.S)) {
                m_position += m_movementSpeed * timeElapsed * moveForward;
            }

            if (keyboard.IsKeyDown(Keys.A)) {
                m_position -= m_movementSpeed * timeElapsed * moveLeft;
            }

            if (keyboard.IsKeyDown(Keys.D)) {
                m_position += m_movementSpeed * timeElapsed * moveLeft;
            }

			if(keyboard.IsKeyDown(Keys.Space)) {
				m_position.Y += m_movementSpeed*2 * timeElapsed;
			}

			if(keyboard.IsKeyDown(Keys.LeftControl)) {
				m_position.Y -= m_movementSpeed * timeElapsed;
			}

			m_camera.update(gameTime, m_position, m_rotation);

			if(gameIsActive) {
				Mouse.SetPosition(m_settings.screenWidth / 2, m_settings.screenHeight / 2);
			}
		}

    }

}
