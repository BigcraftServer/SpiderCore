using System;
using System.Collections.Generic;
using System.Text;

namespace SpiderCore.Enum {
  public enum OutputType {
    None = 0x00,
    Text = 0x01,
    Html = 0x02,
    Array = 0x04,
    Object = 0x08,
    Convert = 0x10,
  }
}
