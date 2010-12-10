﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XNAQ3Lib.Q3BSP;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace SproketEngine {

	class EntitySystem {

		private List<Entity> m_entities;
		private Q3BSPLevel m_level;

		private List<Model> m_models;

		public EntitySystem() {
			m_models = new List<Model>();
			m_entities = new List<Entity>();
		}

		public void loadContent(ContentManager content) {
			Ricket.loadContent(content.Load<Model>("Models\\Enemies\\Robo1"));
			Quadrotor.loadContent(content.Load<Model>("Models\\Enemies\\Robo2"));
			Destrotron.loadContent(content.Load<Model>("Models\\Enemies\\Robo3"));
		}
		
		public void initialize(Q3BSPLevel level) {
			m_level = level;

			if(m_level == null) {
				m_entities = null;
				return;
			}
			
			loadEntities();
		}

		public List<Entity> entities {
			get { return m_entities; }
		}
		public void setLighting(Vector3 lighting) {
			foreach (Entity entity in m_entities) {
				entity.lighting = lighting;
			}
		}

		private void loadEntities() {
			if(m_level == null) { return; }

			for(int i=0;i<m_level.NumberOfEntities();i++) {
				Q3BSPEntity entity = m_level.GetEntity(i);
                float rotation;
                try {
                    rotation = float.Parse((string) entity.Entries["angle"]);
                } 
                catch {
                    rotation = 0;
                }
				Model model;
				float scale;
				Vector3 size;
				if(entity.GetClassName().Equals("enemy_robot1", StringComparison.OrdinalIgnoreCase)) {
					m_entities.Add((Entity) new Ricket(Q3BSPLevel.GetXNAPosition(entity) - new Vector3(0, 6, 0), new Vector3(0, rotation, 0)));
				}
				else if(entity.GetClassName().Equals("enemy_robot2", StringComparison.OrdinalIgnoreCase)) {
					model = m_models[1];
					size = new Vector3(4, 14, 4);
					scale = 0.05f;
				}
				else if (entity.GetClassName().Equals("enemy_robot3", StringComparison.OrdinalIgnoreCase)) {
					model = m_models[2];
					size = new Vector3(4, 14, 4);
					scale = 0.03f;
				}
				else {
					model = null;
					size = Vector3.Zero;
					scale = 0.0f;
				}
				//m_entities.Add((Entity)new Enemy(Q3BSPLevel.GetXNAPosition(entity) - new Vector3(0, 6, 0), new Vector3(0, rotation, 0), model, size, scale, 10.0f, 20.0f, 6.0f, -50.0f, 45, 75, 100));
			}
		}

		public void update(GameTime gameTime) {
			foreach (Enemy e in m_entities) {
				e.update(gameTime);
			}
		}

		public void draw(Matrix view, Matrix projection) {
			if(m_level == null) { return; }

			foreach(Entity entity in m_entities) {
				entity.draw(view, projection);
			}
		}

	}

}
