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
			worldMatrix *= Matrix.CreateScale(0.005f, 0.005f, 0.005f);

            worldMatrix *= Matrix.CreateTranslation(Vector3.Right * -0.83f);

            worldMatrix *= Matrix.CreateRotationX(rotation.X);
            worldMatrix *= Matrix.CreateRotationY(-rotation.Y + MathHelper.Pi);            

			worldMatrix *= Matrix.CreateTranslation(position);
			worldMatrix *= Matrix.CreateTranslation(Vector3.Up * 11.58f);
			worldMatrix *= Matrix.CreateTranslation(forward * -0.28f);

			Matrix[] transforms = new Matrix[m_model.Bones.Count];
			m_model.CopyAbsoluteBoneTransformsTo(transforms);
			foreach(ModelMesh mesh in m_model.Meshes) {
				foreach(BasicEffect effect in mesh.Effects) {
					effect.EnableDefaultLighting();
                    //effect.DirectionalLight1.DiffuseColor = new Vector3(0.5f, 0.6f, 0.7f); //Core Light
                    effect.DirectionalLight1.DiffuseColor = new Vector3(0.9f, 0.8f, 0.3f); //Foundry Light
					effect.World = transforms[mesh.ParentBone.Index] * worldMatrix;
					effect.View = view;
					effect.Projection = projection;

				}
				mesh.Draw();
			}
		}
		
	}

}
