using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace SproketEngine {

    class Enemy {

		private int m_id;
		private static int m_idCounter = 0;

		private Vector3 m_position;
		private Vector3 m_rotation;

        private float m_scale;

		private int m_health;
		private int m_maxHealth = 100;

		private Vector3 m_lighting = Vector3.One;
		private Model m_model;

		public Enemy(Vector3 position, Vector3 rotation, Model model, float scale) {
			m_id = m_idCounter++;
			m_health = m_maxHealth;

			m_position = position;
			m_rotation = rotation;
			m_model = model;
            m_scale = scale;
		}

		public Vector3 position {
			get { return m_position; }
		}

		public Vector3 rotation {
			get { return m_rotation; }
		}
		
		public Vector3 lighting {
			get { return m_lighting; }
			set { m_lighting = value; }
		}

        public bool modelLoaded() {
            return m_model != null;
        }

		public int health {
			get { return m_health; }
		}

        public void draw(Matrix view, Matrix projection)
        {
            //TODO: Rotation
            Matrix worldMatrix = Matrix.CreateScale(m_scale, m_scale, m_scale) * 
                Matrix.CreateRotationY(m_rotation.Y) *
                Matrix.CreateTranslation(m_position);

            Matrix[] transforms = new Matrix[m_model.Bones.Count];
            m_model.CopyAbsoluteBoneTransformsTo(transforms);
            foreach(ModelMesh mesh in m_model.Meshes) {
                foreach(BasicEffect effect in mesh.Effects) {
                    effect.EnableDefaultLighting();
					effect.DirectionalLight1.DiffuseColor = m_lighting;
                    effect.World = transforms[mesh.ParentBone.Index] * worldMatrix;
                    effect.View = view;
                    effect.Projection = projection;

                }
                mesh.Draw();
            }
        }

    }

}
