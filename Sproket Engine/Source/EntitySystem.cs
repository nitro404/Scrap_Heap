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

		private List<Enemy> m_enemies;
		private Q3BSPLevel m_level;

		private List<Model> m_models;

		public EntitySystem() {
			m_models = new List<Model>();
		}

		public void loadContent(ContentManager content) {
			m_models.Add(content.Load<Model>("Models\\Enemies\\Robo1"));
            m_models.Add(content.Load<Model>("Models\\Enemies\\Robo2"));
            m_models.Add(content.Load<Model>("Models\\Enemies\\Robo3"));
		}
		
		public void initialize(Q3BSPLevel level) {
			m_level = level;

			if(m_level == null) {
				m_enemies = null;
				return;
			}

			m_enemies = new List<Enemy>();

			loadEntities();
		}

		private void loadEntities() {
			if(m_level == null) { return; }

			for(int i=0;i<m_level.NumberOfEntities();i++) {
				Q3BSPEntity entity = m_level.GetEntity(i);
				if(entity.GetClassName().Equals("enemy_robot1", StringComparison.OrdinalIgnoreCase)) {
					m_enemies.Add(new Enemy(Q3BSPLevel.GetXNAPosition(entity) + new Vector3(0, 6, 0), Vector3.Zero, m_models[0]));
				}
				else if(entity.GetClassName().Equals("enemy_robot2", StringComparison.OrdinalIgnoreCase)) {
					m_enemies.Add(new Enemy(Q3BSPLevel.GetXNAPosition(entity) + new Vector3(0, 6, 0), Vector3.Zero, m_models[1]));
				}
				else if(entity.GetClassName().Equals("enemy_robot3", StringComparison.OrdinalIgnoreCase)) {
					m_enemies.Add(new Enemy(Q3BSPLevel.GetXNAPosition(entity) + new Vector3(0, 6, 0), Vector3.Zero, m_models[2]));
				}
			}
		}

		public void draw(Matrix view, Matrix projection) {
			if(m_level == null) { return; }

			for(int i=0;i<m_enemies.Count();i++) {
				m_enemies[i].draw(view, projection);
			}
		}

	}

}
