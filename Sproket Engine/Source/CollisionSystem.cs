﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XNAQ3Lib.Q3BSP;
using Microsoft.Xna.Framework;

namespace SproketEngine {

	class CollisionSystem {

		private ScrapHeap m_game;
		private List<Entity> m_entities;
		private Q3BSPLevel m_level;

		private GameSettings m_settings;

		public CollisionSystem() {

		}

		public void initialize(ScrapHeap game, GameSettings settings, List<Entity> entities, Q3BSPLevel level) {
			m_game = game;
			m_settings = settings;
			m_entities = entities;
			m_level = level;
		}

		public Q3BSPLevel level {
			get { return m_level; }
			set { m_level = value; }
		}

		public void update(GameTime gameTime) {

			Entity player = m_entities.Find(entity => entity is Player);
			//TODO: Handle all entity collisions
			//Currently running handleCollision for every entity on the map kills the game
			//which is obviously expected.
			//Gotta figure something out here.
			foreach (Entity entity in m_entities) {

				if (Vector3.Distance(player.position, entity.position) < 75.0f) {
					entity.active = true;
					entity.handleCollision(m_level, gameTime);
				}
				else {
					entity.active = false;
				}
			}
		}

	}

}
