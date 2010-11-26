using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SproketEngine {

	class Weapon {

		private Ammunition m_ammunition;

		private Model m_model;

		public Weapon(Model model, Ammunition ammunition) {
			m_model = model;
			m_ammunition = ammunition;
		}

		public void draw(Vector3 position, Vector3 forward, Vector3 rotation, Matrix view, Matrix projection) {
			Matrix worldMatrix = Matrix.Identity;
			worldMatrix *= Matrix.CreateScale(0.05f, 0.05f, 0.05f);

			worldMatrix *= Matrix.CreateRotationY(rotation.Y);
			worldMatrix *= Matrix.CreateRotationX(MathHelper.Pi - rotation.X);

			worldMatrix *= Matrix.CreateTranslation(position);
			worldMatrix *= Matrix.CreateTranslation(Vector3.Up * 8.0f);
			worldMatrix *= Matrix.CreateTranslation(forward * 4.0f);

			Matrix[] transforms = new Matrix[m_model.Bones.Count];
			m_model.CopyAbsoluteBoneTransformsTo(transforms);
			foreach(ModelMesh mesh in m_model.Meshes) {
				foreach(BasicEffect effect in mesh.Effects) {
					effect.EnableDefaultLighting();
					effect.World = transforms[mesh.ParentBone.Index] * worldMatrix;
					effect.View = view;
					effect.Projection = projection;

				}
				mesh.Draw();
			}
		}
		
	}

}
