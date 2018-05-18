using System;
using System.Collections.Generic;
using System.Text;

namespace SpiderCore.Enum {
  public enum ConvertMatchType {
    None = 0x00,
    Equal = 0x01,
    Contain = 0x02,
    IgnoreCase = 0x04,
    ReplaceAll = 0x08,
    Replace = 0x10,
    Regex = 0x20,
    Default = IgnoreCase | ReplaceAll | Equal,
  }
}
