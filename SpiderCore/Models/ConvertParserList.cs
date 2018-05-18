using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpiderCore.Models {
  public class ConvertParserList : IDictionary<string, ConvertParser> {
    public ConvertParser this[string key] { get => parsers.SingleOrDefault(c => c.Value == key); set => this.Add(key, value); }

    public IList<ConvertParser> parsers { get; set; } = new List<ConvertParser>();

    public ICollection<string> Keys => parsers.Select(c => c.Value).ToList();

    public ICollection<ConvertParser> Values => parsers.Select(c => c).ToList();

    public int Count => parsers.Count();

    ICollection<string> IDictionary<string, ConvertParser>.Keys => throw new NotImplementedException();

    ICollection<ConvertParser> IDictionary<string, ConvertParser>.Values => throw new NotImplementedException();

    int ICollection<KeyValuePair<string, ConvertParser>>.Count => throw new NotImplementedException();

    bool ICollection<KeyValuePair<string, ConvertParser>>.IsReadOnly => throw new NotImplementedException();

    //public void Add(string key, string value) {
    //  parsers.Add((key, value));
    //}

    //public void Add(KeyValuePair<string, string> item) {
    //  parsers.Add(item);
    //}

    public void Add(string key, ConvertParser value) {
      if (string.IsNullOrEmpty(value.Value)) {
        value.Value = key;
      }
      parsers.Add(value);
    }

    public void Add(KeyValuePair<string, ConvertParser> item) {
      parsers.Add(item.Value);
    }

    public void Clear() {
      parsers.Clear();
    }

    public bool Contains(KeyValuePair<string, ConvertParser> item) {
      try {
        return this[item.Key] == item.Value;
      } catch {
        return false;
      }
    }

    public bool ContainsKey(string key) {
      try {
        return this[key] != null;
      } catch {
        return false;
      }
    }

    public void CopyTo(KeyValuePair<string, ConvertParser>[] array, int arrayIndex) {
      throw new NotImplementedException();
    }

    public IEnumerator<KeyValuePair<string, ConvertParser>> GetEnumerator() {
      return parsers.Select(c => new KeyValuePair<string, ConvertParser>(c.Value, c)).GetEnumerator();
    }

    public bool Remove(string key) {
      if (ContainsKey(key)) {
        return parsers.Remove(this[key]);
      }
      return true;
    }

    public bool Remove(KeyValuePair<string, ConvertParser> item) {
      if (this.Contains(item)) {
        return parsers.Remove(item.Value);
      }
      return true;
    }

    public bool TryGetValue(string key, out ConvertParser value) {
      throw new NotImplementedException();
    }

    IEnumerator IEnumerable.GetEnumerator() {
      throw new NotImplementedException();
    }
  }
}
