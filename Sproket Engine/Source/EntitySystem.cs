using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XNAQ3Lib.Q3BSP;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace SproketEngine {

	class EntitySystem {

		// local variables
		private List<Entity> m_entities;
		private Q3BSPLevel m_level;

		private List<Model> m_models;

		public EntitySystem() {
			m_models = new List<Model>();
			m_entities = new List<Entity>();
		}

		// load the enemy models
		public void loadContent(ContentManager content) {
			Ricket.loadContent(content.Load<Model>("Models\\Enemies\\Robo1"));
			Quadrotor.loadContent(content.Load<Model>("Models\\Enemies\\Robo2"));
			Destrotron.loadContent(content.Load<Model>("Models\\Enemies\\Robo3"));
		}
		
		// initialize the entity system with a new level
		public void initialize(Q3BSPLevel level) {
			m_level = level;

			if(m_level == null) {
				m_entities = null;
				return;
			}
			
			loadEntities();
		}

		public void reset() {
			m_entities.Clear();
		}

		public List<Entity> entities {
			get { return m_entities; }
		}

		// set the lighting model
		public void setLighting(Vector3 lighting) {
			foreach (Entity entity in m_entities) {
				entity.lighting = lighting;
			}
		}

		private void loadEntities() {
			if(m_level == null) { return; }

			// parse through all of the entities in the level
			for(int i=0;i<m_level.NumberOfEntities();i++) {
				Q3BSPEntity entity = m_level.GetEntity(i);
                float rotation;
                try {
                    rotation = float.Parse((string) entity.Entries["angle"]);
                } 
                catch {
                    rotation = 0;
                }

				// initialize the entities based on their class name at their appropriate location in XNA co-ordinates
				// also, offset the position based on the center (6) of the entity placeholder from the level
				// then offset it by an additional -0.001 so that the entities do not fall through the map
				if(entity.GetClassName().Equals("enemy_robot1", StringComparison.OrdinalIgnoreCase)) {
					m_entities.Add((Entity) new Ricket(Q3BSPLevel.GetXNAPosition(entity) - new Vector3(0, 5.999f, 0), new Vector3(0, rotation, 0)));
				}
				else if(entity.GetClassName().Equals("enemy_robot2", StringComparison.OrdinalIgnoreCase)) {
					m_entities.Add((Entity) new Quadrotor(Q3BSPLevel.GetXNAPosition(entity) - new Vector3(0, 5.999f, 0), new Vector3(0, rotation, 0)));
				}
				else if (entity.GetClassName().Equals("enemy_robot3", StringComparison.OrdinalIgnoreCase)) {
					m_entities.Add((Entity) new Destrotron(Q3BSPLevel.GetXNAPosition(entity) - new Vector3(0, 5.999f, 0), new Vector3(0, rotation, 0)));
				}
			}
		}

		// update the entities
		public void update(GameTime gameTime) {
			foreach (Enemy e in m_entities) {
				e.update(gameTime);
			}
		}

		// draw the entities
		public void draw(Matrix view, Matrix projection) {
			if(m_level == null) { return; }

			foreach(Entity entity in m_entities) {
				entity.draw(view, projection);
			}
		}

	}

}
