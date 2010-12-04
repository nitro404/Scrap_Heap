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
		private bool m_jumping;
		private Vector3 m_position;
		private Vector3 m_newPosition;
		private Vector3 m_velocity;
		private Vector3 m_gravity;
		private Vector3 m_rotation;
		private Vector3 m_forward;
		private Vector3 m_left;

		private float m_maxClimb = 45;	//Maximum angle player can climb
		private float m_jumpStrength = 75;

		private Vector3 m_dimensions = new Vector3(4, 10, 4);
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

		private SpriteSheet m_crosshairSprites;
		private Sprite m_crosshair = null;

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

		public void loadContent(ContentManager content, SpriteSheet crosshairSprites) {
			m_weapons.loadContent(content);
			m_crosshairSprites = crosshairSprites;
			if(m_crosshairSprites != null) {
				m_crosshair = crosshairSprites.getSprite("Crosshair 2");
			}
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

		public Sprite crosshair {
			get { return m_crosshair; }
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
			m_newPosition = m_position + (m_velocity * m_acceleration * (float) gameTime.ElapsedGameTime.TotalSeconds);


		}

		public void updateGravity(GameTime gameTime) {
			// compute gravity
			m_gravity.Y -= GameConstants.GRAVITY * (float) gameTime.ElapsedGameTime.TotalSeconds;
			m_newPosition = m_position + (m_gravity * (float) gameTime.ElapsedGameTime.TotalSeconds);
		}

		public void moveForward() {
            if (!m_settings.clipping) {
                m_velocity -= new Vector3(m_forward.X, m_forward.Y, m_forward.Z);
            }
            else {
                Vector3 straight = new Vector3(m_forward.X, 0, m_forward.Z);
                straight.Normalize();
                straight *= m_forward.Length();
                m_velocity -= straight;
            }
			m_moving = true;
		}

		public void moveBackward() {
            if (!m_settings.clipping) {
                m_velocity += new Vector3(m_forward.X, m_forward.Y, m_forward.Z);
            }
            else {
                Vector3 straight = new Vector3(m_forward.X, 0, m_forward.Z);
                straight.Normalize();
                straight *= m_forward.Length();
                m_velocity += straight;
            }
			m_moving = true;
		}

		public void moveLeft() {
            if (!m_settings.clipping) {
                m_velocity -= new Vector3(m_left.X, m_left.Y, m_left.Z);
            }
            else {
                Vector3 straight = new Vector3(m_left.X, 0, m_left.Z);
                straight.Normalize();
                straight *= m_left.Length();
                m_velocity -= straight;
            }
			m_moving = true;
		}

		public void moveRight() {
            if (!m_settings.clipping) {
                m_velocity += new Vector3(m_left.X, m_left.Y, m_left.Z);
            }
            else {
                Vector3 straight = new Vector3(m_left.X, 0, m_left.Z);
                straight.Normalize();
                straight *= m_left.Length();
                m_velocity += straight;
            }
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

		public void draw(SpriteBatch spriteBatch) {
			m_weapons.draw(position, m_forward, rotation, view, projection);

			if(m_crosshair != null) {
				spriteBatch.Begin(SpriteBlendMode.AlphaBlend, SpriteSortMode.BackToFront, SaveStateMode.SaveState);
				m_crosshair.draw(spriteBatch, Vector2.One, 0, new Vector2(m_settings.screenWidth / 2, m_settings.screenHeight / 2), SpriteEffects.None);
				spriteBatch.End();
			}
		}

	}

}
