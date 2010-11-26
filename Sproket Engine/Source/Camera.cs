using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;

namespace SproketEngine {

	class Camera {

		private Matrix m_projection;

		private float m_fov = 75.0f;
		private float m_aspectRatio;
		private float m_nearPlane = 0.1f;
		private float m_farPlane = 10000.0f;

		private static Vector3 m_cameraOffset = new Vector3(0, 12, 0);

		private GameSettings m_settings;

		public void initialize(GameSettings settings) {
			m_settings = settings;

			m_aspectRatio = (float) settings.screenWidth / (float) settings.screenHeight;

			m_projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(m_fov), m_aspectRatio, m_nearPlane, m_farPlane);
		}

		public Matrix getView(Vector3 position, Vector3 rotation) {
			return Matrix.CreateTranslation(-(position + m_cameraOffset)) *
				   Matrix.CreateRotationY(rotation.Y) *
				   Matrix.CreateRotationX(rotation.X);
		}

		public Matrix projection {
			get { return m_projection; }
		}

	}

}
