///////////////////////////////////////////////////////////////////////
// Project: XNA Quake3 Lib - MD5 
// Author: Craig Sniffen
// Copyright (c) 2008-2009 All rights reserved
///////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Graphics;
using Microsoft.Xna.Framework.Content.Pipeline.Processors;
using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Compiler;
using XNAQ3Lib.MD5;

// TODO: replace this with the type you want to write out.
using TWrite = XNAQ3Lib.MD5.MD5Submesh;

namespace MD5ContentPipelineExtension
{
    /// <summary>
    /// This class will be instantiated by the XNA Framework Content Pipeline
    /// to write the specified data type into binary .xnb format.
    ///
    /// This should be part of a Content Pipeline Extension Library project.
    /// </summary>
    [ContentTypeWriter]
    public class MD5SubmeshContentTypeWriter : ContentTypeWriter<TWrite>
    {
        protected override void Write(ContentWriter output, TWrite value)
        {
            output.Write(value.Shader);
            output.Write(value.NumberOfVertices);
            output.Write(value.NumberOfTriangles);
            output.Write(value.NumberOfWeights);

            output.WriteObject<MD5Vertex[]>(value.Vertices);
            output.WriteObject<MD5Triangle[]>(value.Triangles);
            output.WriteObject<MD5Weight[]>(value.Weights);
        }

        public override string GetRuntimeReader(TargetPlatform targetPlatform)
        {
            // TODO: change this to the name of your ContentTypeReader
            // class which will be used to load this data.
            return typeof(XNAQ3Lib.MD5.ContentReaders.MD5SubmeshContentReader).AssemblyQualifiedName;
        }
    }
}