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

		private bool m_moving;
		private Vector3 m_position;
		private Vector3 m_newPosition;
		private Vector3 m_velocity;
		private Vector3 m_gravity;
		private Vector3 m_rotation;
		private Vector3 m_forward;
		private Vector3 m_left;

		private Vector3 m_dimensions = new Vector3(4, 14, 4);
		private Vector3 m_minPoint;
		private Vector3 m_maxPoint;

		private float m_rotationSpeed = 20.0f;
		private float m_movementSpeed = 10.0f;
		private float m_acceleration = 6.0f;
		private float m_deceleration = -50.0f;

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
			m_newPosition = position;
			m_rotation = rotation;
			m_velocity = Vector3.Zero;
			m_gravity = Vector3.Zero;
			m_moving = false;

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

		public void loadContent(ContentManager content) {
			m_weapons.loadContent(content);
		}

		public void initialize(GameSettings settings) {
			m_settings = settings;

			m_camera.initialize(settings);

			m_weapons.initialize();
		}

		public Vector3 position {
			get { return m_position; }
			set { m_position = value; }
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
			get { return m_maxPoint; }
		}

		public Matrix view {
			get { return m_camera.getView(m_position, m_rotation); }
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

		public void resetGravity() {
			m_gravity = Vector3.Zero;
		}

		public void jump() {

		}

		public bool selectWeapon(int weaponNumber) {
			return m_weapons.selectWeapon(weaponNumber);
		}

		public void reset() {
			m_moving = false;
			position = Vector3.Zero;
			m_newPosition = m_position;
			m_rotation = Vector3.Zero;
			m_forward = Vector3.Forward;
			m_left = Vector3.Left;
			m_gravity = Vector3.Zero;
			m_velocity = Vector3.Zero;
		}

		public void update(GameTime gameTime) {

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
			m_newPosition = m_position + (m_velocity * 7.0f * (float) gameTime.ElapsedGameTime.TotalSeconds);

			// compute gravity
			m_gravity.Y += GameConstants.GRAVITY * m_acceleration * (float) gameTime.ElapsedGameTime.TotalSeconds;

		}

		public void moveForward() {
			m_velocity -= new Vector3(m_forward.X, 0, m_forward.Z);
			m_moving = true;
		}

		public void moveBackward() {
			m_velocity += new Vector3(m_forward.X, 0, m_forward.Z);
			m_moving = true;
		}

		public void moveLeft() {
			m_velocity -= new Vector3(m_left.X, 0, m_left.Z);
			m_moving = true;
		}

		public void moveRight() {
			m_velocity += new Vector3(m_left.X, 0, m_left.Z);
			m_moving = true;
		}

		public void handleInput(GameTime gameTime, bool gameIsActive) {
			MouseState mouse = Mouse.GetState();

			m_rotation.X += MathHelper.ToRadians((mouse.Y - m_settings.screenHeight / 2) * m_rotationSpeed * 0.01f);
			m_rotation.Y += MathHelper.ToRadians((mouse.X - m_settings.screenWidth / 2) * m_rotationSpeed * 0.01f);

			m_forward = Vector3.Normalize(new Vector3((float) Math.Sin(-m_rotation.Y), (float) Math.Sin(m_rotation.X), (float) Math.Cos(-m_rotation.Y)));
			m_left = Vector3.Normalize(new Vector3((float) Math.Cos(m_rotation.Y), 0f, (float) Math.Sin(m_rotation.Y)));

			if(gameIsActive) {
				Mouse.SetPosition(m_settings.screenWidth / 2, m_settings.screenHeight / 2);
			}

			m_moving = false;
		}

		public void draw() {
			m_weapons.draw(position, m_forward, rotation, view, projection);
		}

	}

}
