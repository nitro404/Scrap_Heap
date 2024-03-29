﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using XNAQ3Lib.Q3BSP;

namespace SproketEngine {
	class Player : MovableEntity {

		// local variables
		private string m_name;

		private int m_health;
		private int m_maxHealth = 100;

		private WeaponCollection m_weapons;
		private AmmunitionCollection m_ammo;

		private Camera m_camera;

		private SpriteSheet m_crosshairSprites;
		private Sprite m_crosshair = null;

		// "global" variables
		protected GameSettings m_settings;

		public Player(String name, Vector3 position, Vector3 rotation) :
			base(position, rotation, null, new Vector3(4, 14, 4), 
			10.0f, 20.0f, 6.0f, -50.0f, 45, 75) {

			// initialize the player
			m_name = name;
			m_health = m_maxHealth;
			m_ammo = new AmmunitionCollection();
			m_weapons = new WeaponCollection();

			m_camera = new Camera();
		}

		public void initialize(GameSettings settings) {
			// initialize "global" variables
			m_camera.initialize(settings);
			m_weapons.initialize();
			m_settings = settings;
		}

		public new void reset() {
			base.reset();
			m_active = true;
		}

		public void loadContent(ContentManager content, SpriteSheet crosshairSprites) {
			// load the player's weapons and crosshair
			m_weapons.loadContent(content);
			m_crosshairSprites = crosshairSprites;
			if (m_crosshairSprites != null) {
				m_crosshair = crosshairSprites.getSprite("Crosshair 2");
			}
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

		public string name {
			get { return m_name; }
		}

		public int health {
			get { return m_health; }
		}

		public void setLighting(Vector3 lighting) {
			m_weapons.setLighting(lighting);
		}

		public bool selectWeapon(int weaponNumber) {
			return m_weapons.selectWeapon(weaponNumber);
		}

		// move the player forward
		public new void moveForward() {
			if (!m_settings.clipping) {
				m_velocity -= new Vector3(m_forward.X, m_forward.Y, m_forward.Z);
				m_moving = true;
			}
			else {
				base.moveForward();
			}
		}

		// move the player backward
		public new void moveBackward() {
			if (!m_settings.clipping) {
				m_velocity += new Vector3(m_forward.X, m_forward.Y, m_forward.Z);
				m_moving = true;
			}
			else {
				base.moveBackward();
			}
		}

		// move the player left
		public new void moveLeft() {
			if (!m_settings.clipping) {
				m_velocity -= new Vector3(m_left.X, m_left.Y, m_left.Z);
				m_moving = true;
			}
			else {
				base.moveLeft();
			}
		}

		// move the player right
		public new void moveRight() {
			if (!m_settings.clipping) {
				m_velocity += new Vector3(m_left.X, m_left.Y, m_left.Z);
				m_moving = true;
			}
			else {
				base.moveRight();
			}
		}

		private Vector3 slopeCollision(Q3BSPLevel level, Vector3 start, Vector3 collision, Vector3 end, Vector3 offset, float angle) {
			start += offset;
			end += offset;

			// check the slope of a collision face
			Q3BSPCollisionData slopetest = level.TraceBox(start, end, minPoint, maxPoint);

			// get the angle of the collision face, and check to see that it is not too steep for the player to climb, then return the appropriate collision point
			if (slopetest.collisionPoint != collision + offset) {
				float opp = offset.Length();
				float adj = (end - start).Length();
				float theta = MathHelper.ToDegrees((float) Math.Atan(opp/adj));
				if (theta < angle)
					return slopetest.collisionPoint;	
			}

			return collision;
		}

		public override void handleCollision(Q3BSPLevel level, GameTime gameTime) {
			// if noclipping is enabled, do not check for collisions
			if (!m_settings.clipping) {
				position = newPosition;
			}
			// otherwise, check for collisions
			else {
				Q3BSPCollisionData collision = level.TraceBox(position, newPosition, minPoint, maxPoint);
				Vector3 point = collision.collisionPoint;

				if (collision.collisionPoint != collision.endPosition) {
					Vector3 start = collision.startPosition;
					Vector3 col = collision.collisionPoint;
					Vector3 end = collision.endPosition;

					//Wall Detection: Not Working 
					//point = slopeCollision(level, start, col, end, m_left, 75);
					//Ramp Detection
					point = slopeCollision(level, start, col, end, new Vector3(0, 0.5f, 0), maxClimb);
					
				}
				position = point;

				//Gravity
				//Check if on floor
				collision = level.TraceBox(position, position - new Vector3(0, 1, 0), minPoint, maxPoint);
				if (collision.collisionPoint == collision.endPosition || isJumping) {
					//Not on floor so check gravity
					updateGravity(gameTime);
					collision = level.TraceBox(position, newPosition, minPoint, maxPoint);
					if (collision.collisionPoint != collision.endPosition) {
						resetGravity();
					}
					position = collision.collisionPoint;
				}
				else {
					//On floor don't do gravity, but reset it
					resetGravity();
				}
			}
		}

		public void handleInput(GameTime gameTime, bool gameIsActive) {
			MouseState mouse = Mouse.GetState();

			// compute the player rotation based on the change in position of the mouse cursor
			m_rotation.X += MathHelper.ToRadians((mouse.Y - m_settings.screenHeight / 2) * m_rotationSpeed * 0.01f);
			m_rotation.Y += MathHelper.ToRadians((mouse.X - m_settings.screenWidth / 2) * m_rotationSpeed * 0.01f);

			// update the forward and left vectors of the player
			m_forward = Vector3.Normalize(new Vector3((float) Math.Sin(-m_rotation.Y), (float) Math.Sin(m_rotation.X), (float) Math.Cos(-m_rotation.Y)));
			m_left = Vector3.Normalize(new Vector3((float) Math.Cos(m_rotation.Y), 0f, (float) Math.Sin(m_rotation.Y)));

			// if the game window is active, reset the mouse to the center of the game window
			if (gameIsActive) {
				Mouse.SetPosition(m_settings.screenWidth / 2, m_settings.screenHeight / 2);
			}

			// reset the moving variable
			m_moving = false;
		}

		public void draw(SpriteBatch spriteBatch) {
			// render the player's currently selected weapon
			m_weapons.draw(position, m_forward, rotation, view, projection);

			// render the player's crosshair
			if (m_crosshair != null) {
				spriteBatch.Begin(SpriteBlendMode.AlphaBlend, SpriteSortMode.BackToFront, SaveStateMode.SaveState);
				m_crosshair.draw(spriteBatch, Vector2.One, 0, new Vector2(m_settings.screenWidth / 2, m_settings.screenHeight / 2), SpriteEffects.None);
				spriteBatch.End();
			}
		}
	}
}
