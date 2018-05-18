using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
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
    public DocumentParser(int index, ConvertParserList converts) : this(index) {
      this.Converts = converts;
      this.OutputType = OutputType.Convert;
    }
    public DocumentParser(string xPath, ConvertParserList converts) : this(xPath) {
      this.Converts = converts;
      this.OutputType = OutputType.Convert;
    }
    [JsonConstructor]
    public DocumentParser([JsonProperty("XPath")]string xPath, [JsonProperty("Index")]int? index, [JsonProperty("Converts")]ConvertParserList converts) {
      if (!string.IsNullOrWhiteSpace(xPath) || index.HasValue) {
        if (index.HasValue) {
          this.Index = index;
          this.PositionType = PositionType.Index;
          this.OutputType = OutputType.Text;
        } else {
          this.XPath = xPath;
          this.PositionType = PositionType.XPath;
          this.OutputType = OutputType.Text;
        }
      } else {
        throw new ArgumentNullException($"{nameof(xPath)} and {nameof(index)}", "not be null");
      }
      if (converts != null) {
        this.Converts = converts;
        this.OutputType = OutputType.Convert;
      }
    }
    public string XPath { get; set; }
    public int? Index { get; set; } = null;
    [JsonConverter(typeof(StringEnumConverter))]
    public PositionType PositionType { get; set; }
    [JsonConverter(typeof(StringEnumConverter))]
    public OutputType OutputType { get; set; }
    public IDictionary<string, DocumentParser> Parser { get; set; }
    public ConvertParserList Converts { get; set; }

    public object Position {
      get {
        object result = null;
        switch (this.PositionType) {
          case PositionType.XPath:
            result = XPath;
            break;
          case PositionType.Index:
            result = Index.Value;
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
