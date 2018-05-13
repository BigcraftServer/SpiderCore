using System;
using System.Collections.Generic;
using System.Text;

namespace SpiderCore.Models {
  public class DocumentParser {
    public string XPath { get; set; }
    public string Type { get; set; }
    public IDictionary<string, DocumentNodeParser> Parser { get; set; }
  }
}
