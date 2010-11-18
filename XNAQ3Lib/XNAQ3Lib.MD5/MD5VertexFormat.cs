///////////////////////////////////////////////////////////////////////
// Project: XNA Quake3 Lib - MD5 
// Author: Craig Sniffen
// Copyright (c) 2008-2009 All rights reserved
///////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;


namespace XNAQ3Lib.MD5
{
    /// <summary>
    /// Vertex format that contains a V4 Position, V3 Normal, V2 TexCoord, V4 Indices, and a V4 Weights
    /// </summary>
    public struct MD5VertexFormat
    {
        public Vector4 Position;
        public Vector3 Normal;
        public Vector2 TexCoord;
        public Vector4 BoneIndices;
        public Vector4 BoneWeights;

        static public VertexElement[] VertexElements
        {
            get
            {
                VertexElement[] elements = new VertexElement[5];

                elements[0] = new VertexElement(0, 0, VertexElementFormat.Vector4, VertexElementMethod.Default, VertexElementUsage.Position, 0);
                elements[1] = new VertexElement(0, 16, VertexElementFormat.Vector3, VertexElementMethod.Default, VertexElementUsage.Normal, 0);
                elements[2] = new VertexElement(0, 28, VertexElementFormat.Vector2, VertexElementMethod.Default, VertexElementUsage.TextureCoordinate, 0);
                elements[3] = new VertexElement(0, 36, VertexElementFormat.Vector4, VertexElementMethod.Default, VertexElementUsage.BlendIndices, 0);
                elements[4] = new VertexElement(0, 52, VertexElementFormat.Vector4, VertexElementMethod.Default, VertexElementUsage.BlendWeight, 0);

                return elements;
            }
        }
    }

}
