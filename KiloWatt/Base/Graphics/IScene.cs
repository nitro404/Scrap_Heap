using System;

using Microsoft.Xna.Framework.Graphics;

namespace KiloWatt.Base.Graphics
{
  public interface IScene
  {
    int TechniqueIndex(string technique);
    void AddRenderable(ISceneRenderable sr);
    void RemoveRenderable(ISceneRenderable sr);
    ISceneTexture GetSceneTexture();
    void Clear();
  }

  public interface ISceneTexture : IDisposable
  {
    Texture2D Texture { get; }
  }
}
