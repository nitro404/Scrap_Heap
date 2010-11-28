using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KiloWatt.Base.Animation
{
  public interface ISource<T>
  {
    void Get(out T val);
  }
}
