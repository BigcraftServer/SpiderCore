using System;
using System.Collections.Generic;
using System.Text;

namespace SpiderCore.Models {
  public class ResponseParser {
    public IDictionary<string, DocumentParser> Documents { get; set; }
    public string Cookies { get; set; }
    public string Headers { get; set; }
  }
}
