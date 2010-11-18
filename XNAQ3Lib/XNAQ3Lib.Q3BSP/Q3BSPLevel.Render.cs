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
    public partial class Q3BSPLevel
    {
        VertexBuffer vertexBuffer;
        IndexBuffer[] indexBuffers;
        int[] indexBufferLengths;
        Vector2[] textureAndLightMapIndices;

        public void RenderLevel(Vector3 cameraPosition, Matrix viewMatrix, Matrix projMatrix, GameTime gameTime, GraphicsDevice graphics)
        {
            graphics.RenderState.FillMode = FillMode.Solid;

            // Render Skybox before everything else
            if (skybox != null)
            {
                graphics.RenderState.DepthBufferEnable = false;
                Matrix skyboxView = viewMatrix;
                skyboxView.Translation = Vector3.Zero;
                skybox.Render(graphics, skyboxView, projMatrix, gameTime);
            }

            if (renderType == Q3BSPRenderType.StaticBuffer)
            {
                RenderLevelStatic(viewMatrix, projMatrix, graphics, gameTime);
            }

            else
            {
                RenderLevelBSP(cameraPosition, viewMatrix, projMatrix, gameTime, graphics);
            }
        }

        public void RenderLevelStatic(Matrix viewMatrix, Matrix projMatrix, GraphicsDevice graphics, GameTime gameTime)
        {
            Effect effect;
            Matrix matrixWorldViewProjection = viewMatrix * projMatrix;

            graphics.VertexDeclaration = vertexDeclaration;
            graphics.RenderState.DepthBufferEnable = true;
            //graphics.RenderState.CullMode = CullMode.None;
            //graphics.RenderState.FillMode = FillMode.WireFrame;
            graphics.Vertices[0].SetSource(vertexBuffer, 0, Q3BSPVertex.SizeInBytes);

            for (int i = 0; i < indexBuffers.Length; ++i)
            {
                if (!shaderManager.IsMaterialDrawable((int)textureAndLightMapIndices[i].X))
                {
                    continue;
                }

                graphics.Indices = indexBuffers[i];
                effect = shaderManager.GetEffect((int)textureAndLightMapIndices[i].X, (int)textureAndLightMapIndices[i].Y, viewMatrix, matrixWorldViewProjection, gameTime);

                effect.Begin();
                foreach (EffectPass pass in effect.CurrentTechnique.Passes)
                {
                    pass.Begin();
                    graphics.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, vertices.Length, 0, indexBufferLengths[i] / 3);
                    pass.End();
                }
                effect.End();
            }
        }

        public void RenderLevelBSP(Vector3 cameraPosition, Matrix viewMatrix, Matrix projMatrix, GameTime gameTime, GraphicsDevice graphics)
        {
            int cameraLeaf = GetCameraLeaf(cameraPosition);
            int cameraCluster = leafs[cameraLeaf].Cluster;
            CurrentCluster = cameraCluster;
            CurrentLeaf = cameraLeaf;

            if (0 > cameraCluster)
            {
                cameraCluster = lastGoodCluster;
            }
            lastGoodCluster = cameraCluster;

            ResetFacesToDraw();

            BoundingFrustum frustum = new BoundingFrustum(viewMatrix * projMatrix);
            ArrayList visibleFaces = new ArrayList();
            foreach (Q3BSPLeaf leaf in leafs)
            {
                if (!visData.FastIsClusterVisible(cameraCluster, leaf.Cluster))
                {
                    continue;
                }

                //// Culls visible leafs. Unsure as to why.
                //if (!frustum.Intersects(leaf.Bounds))
                //{
                //    continue;
                //}

                for (int i = 0; i < leaf.LeafFaceCount; i++)
                {
                    int faceIndex = leafFaces[leaf.StartLeafFace + i];
                    Q3BSPFace face = faces[faceIndex];
                    if (face.FaceType != Q3BSPFaceType.Billboard && !facesToDraw[faceIndex])
                    {
                        
                        facesToDraw[faceIndex] = true;
                        visibleFaces.Add(face);
                    }
                }
            }

            if (0 >= visibleFaces.Count)
            {
                return;
            }

            Q3BSPFaceComparer fc = new Q3BSPFaceComparer();
            visibleFaces.Sort(fc);

            graphics.VertexDeclaration = vertexDeclaration;
            Matrix matrixWorldViewProjection = viewMatrix * projMatrix;
            Effect effect;

            int[] indexArray = new int[maximumNumberOfIndicesToDraw];
            
            int lastTextureIndex = 0;
            int lastLightMapIndex = 0;
            int accumulatedIndexCount = 0;

            graphics.RenderState.DepthBufferEnable = true;

            foreach (Q3BSPFace face in visibleFaces)
            {

                if (face.FaceType == Q3BSPFaceType.Patch)
                {
                    effect = shaderManager.GetEffect(face.TextureIndex, face.LightMapIndex, viewMatrix, matrixWorldViewProjection, gameTime);

                    effect.Begin();
                    foreach (EffectPass pass in effect.CurrentTechnique.Passes)
                    {
                        pass.Begin();
                        patches[face.PatchIndex].Draw(graphics);
                        pass.End();
                    }
                    effect.End();

                    continue;
                }

                if ((face.TextureIndex != lastTextureIndex || face.LightMapIndex != lastLightMapIndex) && accumulatedIndexCount > 0)
                {
                    if (shaderManager.IsMaterialDrawable(lastTextureIndex))
                    {
                        effect = shaderManager.GetEffect(lastTextureIndex, lastLightMapIndex, viewMatrix, matrixWorldViewProjection, gameTime);

                        effect.Begin();
                        foreach (EffectPass pass in effect.CurrentTechnique.Passes)
                        {
                            pass.Begin();
                            graphics.DrawUserIndexedPrimitives<Q3BSPVertex>(PrimitiveType.TriangleList, vertices, 0, vertices.Length, indexArray, 0, accumulatedIndexCount / 3);
                            pass.End();
                        }
                        effect.End();
                    }

                    //indexArray = new int[maximumNumberOfIndicesToDraw];
                    accumulatedIndexCount = 0;
                }

                lastTextureIndex = face.TextureIndex;
                lastLightMapIndex = face.LightMapIndex;

                for (int i = 0; i < face.MeshVertexCount; ++i)
                {
                    indexArray[accumulatedIndexCount] = (face.StartVertex + meshVertices[face.StartMeshVertex + i]);
                    accumulatedIndexCount++;
                }
            }

            // Draw the final batch of faces
            if (indexArray.Length != 0 && shaderManager.IsMaterialDrawable(lastTextureIndex))
            {
                effect = shaderManager.GetEffect(lastTextureIndex, lastLightMapIndex, viewMatrix, matrixWorldViewProjection, gameTime);

                effect.Begin();
                foreach (EffectPass pass in effect.CurrentTechnique.Passes)
                {
                    pass.Begin();
                    graphics.DrawUserIndexedPrimitives<Q3BSPVertex>(PrimitiveType.TriangleList, vertices, 0, vertices.Length, indexArray, 0, accumulatedIndexCount / 3);
                    pass.End();
                }
                effect.End();
            }
        }
    }
}
