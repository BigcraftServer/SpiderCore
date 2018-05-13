using System;
using System.Collections.Generic;
using System.Text;

namespace SpiderCore.Models {
  public class DocumentNodeParser {
    public string Position { get; set; }
    public IDictionary<string, string> Convert { get; set; } = null;
    public DocumentNodeParser() { }
    public DocumentNodeParser(int position) {
      this.Position = position.ToString();
    }
    public static implicit operator DocumentNodeParser(Int32 position) {
      return new DocumentNodeParser(position);
    }
  }
}
