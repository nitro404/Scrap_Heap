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

    class Entity {
		protected int m_id;
		protected static int m_idCounter = 0;

        protected Vector3 m_position;
        protected Vector3 m_rotation;
        protected Model m_model;

		protected Vector3 m_dimensions;
		protected Vector3 m_minPoint;
		protected Vector3 m_maxPoint;

		protected Vector3 m_lighting = Vector3.One;

		protected bool m_active;

        public Entity(Vector3 position, Vector3 rotation, Model model, Vector3 dimensions) {
			m_id = m_idCounter++;

			m_position = position;
			m_rotation = rotation;
			m_model = model;

			m_dimensions = dimensions;
			float xSize = (m_dimensions.X / 2);
			float ySize = m_dimensions.Y;
			float zSize = (m_dimensions.Z / 2);
			m_minPoint = new Vector3(-xSize, 0, -zSize);
			m_maxPoint = new Vector3(xSize, ySize, zSize);
        }

		public Vector3 position {
			get { return m_position; }
			set { m_position = value; }
		}

		public Vector3 rotation {
			get { return m_rotation; }
		}

		public Vector3 lighting {
			get { return m_lighting; }
			set { m_lighting = value; }
		}

		public bool active {
			get { return m_active; }
			set { m_active = value; }
		}

		public virtual void handleCollision(Q3BSPLevel level, GameTime gameTime) {
			//TODO: Collision Handling
			return;
		}

		public void draw(Matrix world, Matrix view, Matrix projection) {
			if (m_model == null)
				return;

			Matrix[] transforms = new Matrix[m_model.Bones.Count];
			m_model.CopyAbsoluteBoneTransformsTo(transforms);
			foreach (ModelMesh mesh in m_model.Meshes) {
				foreach (BasicEffect effect in mesh.Effects) {
					effect.EnableDefaultLighting();
					effect.DirectionalLight1.DiffuseColor = m_lighting;
					effect.World = transforms[mesh.ParentBone.Index] * world;
					effect.View = view;
					effect.Projection = projection;

				}
				mesh.Draw();
			}
		}

		public virtual void draw(Matrix view, Matrix projection) {
			Matrix world = Matrix.Identity *
						   Matrix.CreateRotationX(rotation.X) *
						   Matrix.CreateRotationY(rotation.Y) *
						   Matrix.CreateRotationZ(rotation.Z) *
						   Matrix.CreateTranslation(position);
			draw(world, view, projection);
		}
    }
}
