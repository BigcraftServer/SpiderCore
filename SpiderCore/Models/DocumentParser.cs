using SpiderCore.Enum;
using System;
using System.Collections.Generic;
using System.Text;

namespace SpiderCore.Models {
  public class DocumentParser {
    public DocumentParser() { }
    public DocumentParser(int index) {
      this.Index = index;
      this.PositionType = PositionType.Index;
      this.OutputType = OutputType.Text;
    }
    public DocumentParser(string xPath) {
      this.XPath = xPath;
      this.PositionType = PositionType.XPath;
      this.OutputType = OutputType.Text;
    }

    public DocumentParser(int index, Dictionary<string, string> converts) {
      this.Index = index;
      this.Converts = converts;
      this.PositionType = PositionType.Index;
      this.OutputType = OutputType.Text | OutputType.Convert;
    }
    public DocumentParser(string xPath, Dictionary<string, string> converts) {
      this.XPath = xPath;
      this.Converts = converts;
      this.PositionType = PositionType.XPath;
      this.OutputType = OutputType.Text | OutputType.Convert;
    }
    public string XPath { get; set; }
    public int Index { get; set; }
    public PositionType PositionType { get; set; }
    public OutputType OutputType { get; set; }
    public IDictionary<string, DocumentParser> Parser { get; set; }
    public IDictionary<string, string> Converts { get; set; }

    public object Position {
      get {
        object result = null;
        switch (this.PositionType) {
          case PositionType.XPath:
            result = XPath;
            break;
          case PositionType.Index:
            result = Index;
            break;
          case PositionType.None:
          default:
            throw new InvalidCastException("吃柠檬");
        }
        return result;
      }
    }

    public static implicit operator DocumentParser(int index) {
      return new DocumentParser(index);
    }
    public static implicit operator DocumentParser(string xPaht) {
      return new DocumentParser(xPaht);
    }
  }
}
