///////////////////////////////////////////////////////////////////////
// Project: XNA Quake3 Lib - BSP
// Author: Aanand Narayanan and Craig Sniffen
// Copyright (c) 2006-2009 All rights reserved
///////////////////////////////////////////////////////////////////////
using System;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Storage;
using Microsoft.Xna.Framework.Content;

namespace XNAQ3Lib.Q3BSP
{
    public enum Q3BSPRenderType
    {
        BSPCulling,
        StaticBuffer
    }

    public sealed partial class Q3BSPLevel
    {
        #region Variables
        const float EPSILON = 0.03125f;
        readonly Q3BSPRenderType renderType;

        Q3BSPTextureData[] textureData;
        Plane[] planes;
        Q3BSPNode[] nodes;
        Q3BSPLeaf[] leafs;
        int[] leafFaces;
        int[] leafBrushes;
        Q3BSPModel[] models;
        Q3BSPBrush[] brushes;
        Q3BSPBrushSide[] brushSides;
        Q3BSPVertex[] vertices;
        int[] meshVertices;
        Q3BSPEffect[] effects;
        Q3BSPFace[] faces;
        Q3BSPLightMapData[] lightMapData;
        Q3BSPLightVolume[] lightVolumes;
        Q3BSPVisData visData;
        Q3BSPPatch[] patches;
        Q3BSPLightMapManager lightMapManager;
        Q3BSPShaderManager shaderManager;
        Q3BSPEntityManager entityManager;
        Q3BSPSkybox skybox;

        VertexDeclaration vertexDeclaration;

        string levelBasePath = "Maps/";
        Q3BSPLogger bspLogger;
        bool levelInitialized = false;
        bool[] facesToDraw;
        int lastGoodCluster = 0;
        int maximumNumberOfIndicesToDraw;

        public int CurrentLeaf = 0;
        public int CurrentCluster = 0;
        #endregion

        public Q3BSPLevel(Q3BSPRenderType renderType)
        {
            this.renderType = renderType;

            bspLogger = new Q3BSPLogger("log.txt");
        }
		
		public Q3BSPEntity GetEntity(string entityName) {
			return entityManager.GetEntity(entityName);
		}

		public static Vector3 GetXNAPosition(Q3BSPEntity entity) {
			if(entity == null) { return Vector3.Zero; }

			string origin = (string) entity.Entries["origin"];

			string[] xzy = null;
			if(origin != null) { xzy = origin.Split(' '); }

			if(xzy == null || xzy.Length != 3) { return Vector3.Zero; }

			Vector3 position = Vector3.Zero;
			try {
				position = new Vector3(float.Parse(xzy[0]), float.Parse(xzy[2]), -float.Parse(xzy[1])) * 0.25f;
			}
			catch(Exception) { }

			return position;
		}

		public static Vector3 BSPToXNAPosition(Vector3 position) {
			return new Vector3(position.X, position.Z, -position.Y) * 0.25f;
		}

        private int GetCameraLeaf(Vector3 cameraPosition)
        {
            int currentNode = 0;

            while (0 <= currentNode)
            {
                Plane currentPlane = planes[nodes[currentNode].Plane];
                if (PlaneIntersectionType.Front == ClassifyPoint(currentPlane, cameraPosition))
                {
                    currentNode = nodes[currentNode].Left;
                }
                else
                {
                    currentNode = nodes[currentNode].Right;
                }
            }

            return (~currentNode);
        }

        private PlaneIntersectionType ClassifyPoint(Plane plane, Vector3 pos)
        {
            float e = Vector3.Dot(plane.Normal, pos) - plane.D;

            if (e > Q3BSPConstants.Epsilon)
            {
                return PlaneIntersectionType.Front;
            }

            if (e < -Q3BSPConstants.Epsilon)
            {
                return PlaneIntersectionType.Back;
            }

            return PlaneIntersectionType.Intersecting;
        }

        private void ResetFacesToDraw()
        {
            for (int i = 0; i < facesToDraw.Length; i++)
            {
                facesToDraw[i] = false;
            }
        }

        #region Properties
        public string BasePath
        {
            get
            {
                return levelBasePath;
            }
            set
            {
                levelBasePath = value;
            }
        }
        #endregion
    }
}
