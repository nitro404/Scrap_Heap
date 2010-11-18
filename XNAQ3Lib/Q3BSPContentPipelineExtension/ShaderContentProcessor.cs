using System;
using System.IO;
using System.Text;
using System.ComponentModel;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Graphics;
using Microsoft.Xna.Framework.Content.Pipeline.Processors;

// TODO: replace these with the processor input and output types.
using TInput = System.Collections.Generic.List<Q3BSPContentPipelineExtension.Q3BSPMaterialContent>;
using TOutput = System.Collections.Generic.Dictionary<string, Q3BSPContentPipelineExtension.Q3BSPMaterialContent>;


namespace Q3BSPContentPipelineExtension
{
    /// <summary>
    /// This class will be instantiated by the XNA Framework Content Pipeline
    /// to apply custom processing to content data, converting an object of
    /// type TInput to TOutput. The input and output types may be the same if
    /// the processor wishes to alter data without changing its type.
    ///
    /// This should be part of a Content Pipeline Extension Library project.
    ///
    /// TODO: change the ContentProcessor attribute to specify the correct
    /// display name for this processor.
    /// </summary>
    [ContentProcessor(DisplayName = "XNA Q3 Library Shader Processor")]
    public class ShaderContentProcessor : ContentProcessor<TInput, TOutput>
    {
        ShaderModel shaderModel = ShaderModel.ShaderModel1;

        [DisplayName("Shader Model")]
        [Description("What Shader Model should these shaders be compiled for?")]
        [DefaultValue(ShaderModel.ShaderModel1)]
        public ShaderModel ShaderModel
        {
            get;
            set;
        }

        public override TOutput Process(TInput input, ContentProcessorContext context)
        {
            Dictionary<string, Q3BSPMaterialContent> processedDictionary = new Dictionary<string,Q3BSPMaterialContent>();

            #region Initialize StreamWriter, if necessary
#if DEBUG
            StreamWriter sw = new StreamWriter(context.OutputFilename.Substring(0, context.OutputFilename.LastIndexOf('.')) + ".compiled.txt");
#else
            StreamWriter sw = null;
#endif 
            #endregion

            for (int i = 0; i < input.Count; i++)
            {
                Q3BSPMaterialContent shader = input[i];

                #region Throw any errors in the parsed shader
                if (shader.stages.Count > 8)
                {
                    throw new InvalidContentException(shader.shaderName + " has " + shader.stages.Count + " stages, but the maximum supported is 8.");
                }
                if (processedDictionary.ContainsKey(shader.shaderName))
                {
                    throw new InvalidContentException("Material " + shader.shaderName + " is defined more than once.");
                }
                #endregion

                #region Log any needed warnings
                if (shader.stages.Count > 4 && shaderModel == ShaderModel.ShaderModel1)
                {
                    context.Logger.LogWarning("", new ContentIdentity(), shader.shaderName + " has more than 4 stages, Shader Model 2.0 is required.");
                }
                #endregion

                #region Compile the Effect
#if DEBUG
                CompiledEffect compiledEffect = Effect.CompileEffectFromSource(GenerateEffectFromShader(shader, sw), null, null, CompilerOptions.Debug, TargetPlatform.Windows);
#else
                CompiledEffect compiledEffect = Effect.CompileEffectFromSource(GenerateEffectFromShader(shader, sw), null, null, CompilerOptions.None, TargetPlatform.Windows);
#endif
                #endregion

                if (compiledEffect.ErrorsAndWarnings.Contains("error"))
                {
                    throw new InvalidContentException(shader.shaderName + ": " + compiledEffect.ErrorsAndWarnings);
                }

                shader.compiledEffect = compiledEffect;
                shader.ShaderCode = compiledEffect.GetEffectCode();

                processedDictionary.Add(shader.shaderName, shader);

            }
#if DEBUG
            sw.Flush();
            sw.Dispose();
#endif

            return processedDictionary;
        }

        string GenerateEffectFromShader(Q3BSPMaterialContent shader, StreamWriter sw)
        {
            StringBuilder sb = new StringBuilder();

            int stageNumber = 0;
            int texCoordNumber = 0;

            string externs = "";
            string samplers = "";
            string vertexShaderInput = "";
            string vertexShaderOutput = "";
            string stageInputStructs = "";
            string vertexShaderProgram = "";
            string pixelShaderProgram = "";
            string technique = "";

            #region Generate Generic External Variables
            sb.AppendLine("// Generic Externs");
            sb.AppendLine("uniform float4x4 worldViewProj;");
            sb.AppendLine("uniform float gameTime;");
            externs += sb.ToString();
            sb.Remove(0, sb.ToString().Length);
            #endregion

            foreach (Q3BSPMaterialStageContent stage in shader.stages)
            {
                stage.InitializeEffectNames(stageNumber);

                #region Generate External Variables
                sb.AppendLine("// " + stage.StageName + "'s externs");
                sb.AppendLine("uniform texture " + stage.TextureEffectParameterName + ";");
                sb.AppendLine("uniform float3x2 " + stage.TcModName + ";");
                externs += sb.ToString();

                sb.Remove(0, sb.ToString().Length);
                #endregion

                #region Generate Samplers
                sb.AppendLine("sampler " + stage.SamplerName + " = sampler_state");
                sb.AppendLine("{");
                sb.AppendLine("\tTexture = <" + stage.TextureEffectParameterName + ">;");
                sb.AppendLine("\tminfilter = LINEAR;");
                sb.AppendLine("\tmagfilter = LINEAR;");
                sb.AppendLine("\tmipfilter = NONE;");
                sb.AppendLine("\tAddressU = Wrap;");
                sb.AppendLine("\tAddressV = Wrap;");
                sb.AppendLine("};");
                samplers += sb.ToString();

                sb.Remove(0, sb.ToString().Length);
                #endregion

                #region Generate Input Structures
                sb.AppendLine("struct " + stage.InputName);
                sb.AppendLine("{");
                sb.AppendLine("\tfloat2 uv : TEXCOORD" + texCoordNumber + ";");
                sb.AppendLine("};");
                stageInputStructs += sb.ToString();

                sb.Remove(0, sb.ToString().Length);

                texCoordNumber++;
                #endregion

                stageNumber++;
            }

            #region Generate VertexShaderInput
            sb.AppendLine("struct VertexShaderInput");
            sb.AppendLine("{");
            if (shader.IsSky)
            {
                sb.AppendLine("\tfloat4 position : POSITION0;");
            }
            else
            {
                sb.AppendLine("\tfloat4 position : POSITION0;");
                sb.AppendLine("\tfloat3 normal : NORMAL0;");
                sb.AppendLine("\tfloat2 texcoords : TEXCOORD0;");
                sb.AppendLine("\tfloat2 lightmapcoords : TEXCOORD1;");
                sb.AppendLine("\tfloat4 diffuse : COLOR0;");
            }
            sb.AppendLine("};");
            vertexShaderInput = sb.ToString();

            sb.Remove(0, sb.ToString().Length);
            #endregion

            #region Generate VertexShaderOutput
            sb.AppendLine("struct VertexShaderOutput");
            sb.AppendLine("{");
            sb.AppendLine("\tfloat4 position : POSITION0;");
            foreach (Q3BSPMaterialStageContent stage in shader.stages)
            {
                sb.AppendLine("\t" + stage.InputName + " " + stage.StageName + ";");
            }
            sb.AppendLine("};");
            vertexShaderOutput = sb.ToString();

            sb.Remove(0, sb.ToString().Length);
            #endregion

            #region Generate VertexShaderProgram
            sb.AppendLine("VertexShaderOutput VertexShaderProgram(VertexShaderInput input)");
            sb.AppendLine("{");
            sb.AppendLine("\tVertexShaderOutput output;");
            sb.AppendLine("\toutput.position = mul(input.position, worldViewProj);");

            if (shader.IsSky)
            {
                float zScale = shader.SkyHeight / 128.0f;
                float zOffset = 0.707f;

                sb.AppendLine("\tfloat3 S = normalize(float3(input.position.x, input.position.z, input.position.y));");
                sb.AppendLine("\tS.z = " + zScale + " * (S.z + " + zOffset + ");");
                sb.AppendLine("\tS = normalize(S);");
                sb.AppendLine();

                foreach (Q3BSPMaterialStageContent stage in shader.stages)
                {
                    sb.AppendLine("\toutput." + stage.StageName + ".uv = float2(S.x, S.y);");
                    sb.Append(WriteTcModEffectCode(stage));
                }
            }
            else
            {

                foreach (Q3BSPMaterialStageContent stage in shader.stages)
                {
                    if (stage.IsLightmapStage)
                    {
                        sb.AppendLine("\toutput." + stage.StageName + ".uv = input.lightmapcoords;");
                    }
                    else
                    {
                        sb.AppendLine("\toutput." + stage.StageName + ".uv = input.texcoords;");
                    }

                    sb.Append(WriteTcModEffectCode(stage));
                }
            }
            sb.AppendLine();
            sb.AppendLine("\treturn output;");
            sb.AppendLine("};");

            vertexShaderProgram = sb.ToString();
            sb.Remove(0, sb.ToString().Length);
            #endregion

            #region Generate PixelShaderProgram
            sb.AppendLine("float4 PixelShaderProgram(VertexShaderOutput input) : COLOR0");
            sb.AppendLine("{");
            sb.AppendLine("\tfloat4 destination = (float4)0;");
            sb.AppendLine("\tfloat4 source = (float4)0;");
            sb.AppendLine();

            sb.Append(WriteBlendFuncEffectCode(shader));

            sb.AppendLine();
            sb.AppendLine("\treturn destination;");
            sb.AppendLine("}");

            pixelShaderProgram = sb.ToString();
            sb.Remove(0, sb.ToString().Length);
            #endregion

            #region Generate Technique
            string shaderModel = "1_1";

            if (this.shaderModel == ShaderModel.ShaderModel2 || shader.stages.Count > 4)
            {
                shaderModel = "2_0";
            }

            sb.AppendLine("technique Technique1");
            sb.AppendLine("{");
            sb.AppendLine("\tpass Pass1");
            sb.AppendLine("\t{");
            sb.AppendLine("\t\tVertexShader = compile vs_" + shaderModel +" VertexShaderProgram();");
            sb.AppendLine("\t\tPixelShader = compile ps_" + shaderModel + " PixelShaderProgram();");
            sb.AppendLine("\t}");
            sb.AppendLine("}");

            technique = sb.ToString();
            sb.Remove(0, sb.ToString().Length);
            #endregion

            if (sw != null)
            {
                sw.WriteLine("Shader: " + shader.shaderName);
                sw.Write(externs);
                sw.WriteLine();
                sw.Write(samplers);
                sw.WriteLine();
                sw.Write(stageInputStructs);
                sw.WriteLine();
                sw.Write(vertexShaderInput);
                sw.WriteLine();
                sw.Write(vertexShaderOutput);
                sw.WriteLine();
                sw.Write(vertexShaderProgram);
                sw.WriteLine();
                sw.Write(pixelShaderProgram);
                sw.WriteLine();
                sw.Write(technique);
                sw.WriteLine();
                sw.Flush();
            }

            return externs + samplers + stageInputStructs + vertexShaderInput + vertexShaderOutput + vertexShaderProgram + pixelShaderProgram + technique;
        }

        static string WriteBlendFuncEffectCode(Q3BSPMaterialContent mat)
        {
            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < mat.stages.Count; i++)
            {
                #region Test for Bad BlendFuncs
                if (mat.stages[i].SourceBlendFactor == Q3BSPBlendFuncFactor.Invalid || mat.stages[i].DestinationBlendFactor == Q3BSPBlendFuncFactor.Invalid)
                {
                    sb.AppendLine("\t// blendfunc invalid");
                    continue;
                }

                if (mat.stages[i].SourceBlendFactor == Q3BSPBlendFuncFactor.Zero && mat.stages[i].DestinationBlendFactor == Q3BSPBlendFuncFactor.Zero)
                {
                    continue;
                }
                #endregion

                string sourceColor = "tex2D(" + mat.stages[i].SamplerName + ", input." + mat.stages[i].StageName + ".uv)";
                string destinationColor = "destination";
                string sourceSide;
                string destinationSide;

                sb.AppendLine("\t// " + mat.stages[i].TextureEffectParameterName);

                #region Avoid calculating the source color  extra times
                if (mat.stages[i].SourceBlendFactor == Q3BSPBlendFuncFactor.Source || mat.stages[i].DestinationBlendFactor == Q3BSPBlendFuncFactor.Source ||
                    mat.stages[i].SourceBlendFactor == Q3BSPBlendFuncFactor.SourceAlpha || mat.stages[i].SourceBlendFactor == Q3BSPBlendFuncFactor.SourceAlpha ||
                    mat.stages[i].SourceBlendFactor == Q3BSPBlendFuncFactor.InverseSource || mat.stages[i].DestinationBlendFactor == Q3BSPBlendFuncFactor.InverseSource ||
                    mat.stages[i].SourceBlendFactor == Q3BSPBlendFuncFactor.InverseSourceAlpha || mat.stages[i].DestinationBlendFactor == Q3BSPBlendFuncFactor.InverseSourceAlpha)
                {
                    sb.AppendLine("\tfloat4 " + mat.stages[i].StageName + "_source = " + sourceColor + ";");
                    sourceColor = mat.stages[i].StageName + "_source";
                }
                #endregion

                sb.Append("\tdestination = ");

                sourceSide = WriteBlendFuncEquationSide(sourceColor, mat.stages[i].SourceBlendFactor, sourceColor, destinationColor);
                destinationSide = WriteBlendFuncEquationSide(destinationColor, mat.stages[i].DestinationBlendFactor, sourceColor, destinationColor);

                // Write equation code. At least one side has to have a value.
                if (sourceSide != "" && destinationSide != "")
                {
                    sb.Append("(" + sourceSide + ") + (" + destinationSide + ")");
                }
                else
                {
                    sb.Append(sourceSide + destinationSide);
                }

                sb.AppendLine(";");
                sb.AppendLine();
            }
            sb.AppendLine("\tdestination.a = 1.0;");
            return sb.ToString();
        }

        static string WriteBlendFuncEquationSide(string side, Q3BSPBlendFuncFactor blendFactor, string source, string destination)
        {
            switch (blendFactor)
            {
                case Q3BSPBlendFuncFactor.One:
                    return side;
                case Q3BSPBlendFuncFactor.Zero:
                    return "";
                case Q3BSPBlendFuncFactor.Destination:
                    return side + " * " + destination;
                case Q3BSPBlendFuncFactor.InverseDestination:
                    return side + " * float4(1.0 - " + destination + ".r, 1.0 - " + destination + ".g, 1.0 - " + destination + ".b, 1.0 - " + destination + ".a)";
                case Q3BSPBlendFuncFactor.DestinationAlpha:
                    return side + " * " + destination + ".a";
                case Q3BSPBlendFuncFactor.InverseDestinationAlpha:
                    return side + " * (1.0 -" + destination + ".a)";
                case Q3BSPBlendFuncFactor.Source:
                    return side + " * " + source;
                case Q3BSPBlendFuncFactor.InverseSource:
                    return side + " * float4(1.0 - " + source + ".r, 1.0 - " + source + ".g, 1.0 - " + source + ".b, 1.0 - " + source + ".a)";
                case Q3BSPBlendFuncFactor.SourceAlpha:
                    return side + " * " + source + ".a";
                case Q3BSPBlendFuncFactor.InverseSourceAlpha:
                    return side + " * (1.0 -" + source + ".a)";
                default:
                    return side;
            }
        }

        string WriteTcModEffectCode(Q3BSPMaterialStageContent stage)
        {
            if (stage.TcModStatements == null || stage.TcModStatements.Length == 0)
            {
                return "";
            }

            StringBuilder sb = new StringBuilder();
            foreach (Q3BSPMaterialStageTcMod tcMod in stage.TcModStatements)
            {

                string s = "output." + stage.StageName + ".uv.x";
                string t = "output." + stage.StageName + ".uv.y";

                switch (tcMod.Function)
                {
                    case Q3BSPTcModFunction.Scale:
                        sb.AppendLine("\t" + s + " *= " + tcMod.Data[0] + ";");
                        sb.AppendLine("\t" + t + " *= " + tcMod.Data[1] + ";");
                        break;
                    case Q3BSPTcModFunction.Scroll:
                        sb.AppendLine("\t" + s + " += " + tcMod.Data[0] + " * gameTime;");
                        sb.AppendLine("\t" + t + " += " + tcMod.Data[1] + " * gameTime;");
                        break;
                    case Q3BSPTcModFunction.Turbulence:
                        sb.AppendLine("\t" + s + " += 1 - sin(((((input.position.x + input.position.z) * 0.001 + (" + tcMod.Data[2] + " * gameTime))) % 1025) * " + MathHelper.TwoPi + ") * " + tcMod.Data[0] + ";");
                        sb.AppendLine("\t" + t + " += 1 - sin(((((input.position.y) * 0.001 + (" + tcMod.Data[2] + " * gameTime))) % 1025) * " + MathHelper.TwoPi + ") * " + tcMod.Data[0] + ";");
                        break;
                    case Q3BSPTcModFunction.Rotate:
                        string cosValue = "cos(" + tcMod.Data[0] + " * gameTime)";
                        string sinValue = "sin(" + tcMod.Data[0] + " * gameTime)";

                        sb.AppendLine("\t float s_" + stage.StageName + " = " + s + ";");
                        sb.AppendLine("\t float t_" + stage.StageName + " = " + t + ";");

                        sb.AppendLine("\t" + s + " = s_" + stage.StageName + "*" + cosValue + " + t_" + stage.StageName + " * -" + sinValue + " + (0.5 - 0.5 * " + cosValue + "+ 0.5 *" + sinValue + ");");
                        sb.AppendLine("\t" + t + " = s_" + stage.StageName + "*" + sinValue + " + t_" + stage.StageName + " * " + cosValue + " + (0.5 - 0.5 * " + sinValue + "- 0.5 *" + cosValue + ");");
                        break;
                    default:
                        sb.AppendLine("\t// Invalid or unparsed tcMod statement.");
                        break;
                }
            }
            return sb.ToString();
        }
    }

    public enum ShaderModel
    {
        ShaderModel1,
        ShaderModel2
    }
}