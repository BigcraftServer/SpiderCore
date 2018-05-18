using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using SpiderCore.Enum;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace SpiderCore.Models {
  public class ConvertParser {
    [JsonConstructor]
    public ConvertParser([JsonProperty("Value")]string value, [JsonProperty("TargetValue")]string targetValue, [JsonProperty("ConvertMatchType")]ConvertMatchType? convertMatchType = ConvertMatchType.Default) {
      this.Value = value;
      this.TargetValue = targetValue;
      if (convertMatchType.HasValue)
        this.ConvertMatchType = convertMatchType.Value;
    }
    public string Value { get; set; }
    [JsonConverter(typeof(StringEnumConverter))]
    public ConvertMatchType ConvertMatchType { get; set; }
    public string TargetValue { get; set; }
    public static implicit operator ConvertParser(KeyValuePair<string, string> item) {
      return new ConvertParser(item.Key, item.Value);
    }
    public static implicit operator ConvertParser(ValueTuple<string, string, Enum.ConvertMatchType> keyValueTypePair) {
      return new ConvertParser(keyValueTypePair.Item1, keyValueTypePair.Item2, keyValueTypePair.Item3);
    }
    public static implicit operator ConvertParser(ValueTuple<string, string> keyValuePair) {
      return new ConvertParser(keyValuePair.Item1, keyValuePair.Item2);
    }
    public static implicit operator ConvertParser(string targetValue) {
      return new ConvertParser(null, targetValue);
    }

    public string Convert(string text) {
      text = text.Trim();
      string replaceValue = TargetValue;
      if (ConvertMatchType.HasFlag(ConvertMatchType.Equal)) {
        text = replaceValue;
      } else if (ConvertMatchType.HasFlag(ConvertMatchType.Contain)) {
        if (ConvertMatchType.HasFlag(ConvertMatchType.ReplaceAll)) {
          text = text.Replace(Value, TargetValue, StringComparison.CurrentCultureIgnoreCase);
        } else {
          var regex = new Regex(Regex.Escape(Value), RegexOptions.IgnoreCase);
          text = regex.Replace(text, TargetValue, 1);
        }
      } else if (ConvertMatchType.HasFlag(ConvertMatchType.Regex)) {
        var regex = new Regex(Value);
        text = regex.Replace(text, TargetValue);
      }
      return text;
    }
  }
}
