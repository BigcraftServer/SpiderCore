using Grpc.Core;
using HtmlAgilityPack;
using ProxyPool.Grpc;
using RestSharp;
using SpiderCore.Enum;
using SpiderCore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace SpiderCore {
  class Program {
    public static void Main(string[] args) {
      MainAsync().Wait();
    }
    static async Task MainAsync() {
      string ParserRule = @"
{
	""Document"": [{
    ""Proxies"": {
      ""XPath"": ""//table[@id='proxylisttable']/tbody/tr"",
			""Type"": ""Array"",
			""Parser"": {
        ""IpAddress"": 0,
				""Port"": 1,
				""CountryCode"": 2,
				""AnonymousType"": {
            ""Position"": 4
        },
				""Type"": {
            ""Position"": 5,
				""Convert"": [{
						""yes"": ""HTTPS""
          },
					{
						""no"": ""HTTP""
					}
				]
				}
			}
		}
	}]
}
";
      var requestPP = new ResponseParser() {
        Document = new Dictionary<string, DocumentParser>() {
          {
            "Proxies",
            new DocumentParser("//table[@id='proxylisttable']/tbody/tr"){
              OutputType = Enum.OutputType.Array,
              Parser = new Dictionary<string,DocumentParser>(){
                { "IpAddress",0 },
                { "Port",1 },
                { "CountryCode",2 },
                { "AnonymousType",4},
                { "Type",
                  new DocumentParser(6,new Dictionary<string,string>(){
                      { "yes","HTTPS"},{"no","HTTP" }
                    })
                }
              }
            }
          },
          {
            "FAQs",
            new DocumentParser("//div[@id='accordion']/div[@class='panel panel-default']"){
              OutputType = Enum.OutputType.Array,
              Parser = new Dictionary<string,DocumentParser>(){
                {"Question",".//div[1]/h3/a" },
                { "Answer",".//div[2]/div/p" }
              }
            }
          }
        }
      };
      var url = "https://free-proxy-list.net/";
      var a = Newtonsoft.Json.JsonConvert.SerializeObject(requestPP);
      var client = new RestClient(url);
      client.Timeout = 15000;
      client.Proxy = new WebProxy("127.0.0.1", 1080);
      client.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/60.0.3112.113 Safari/537.36"; ;
      var request = new RestRequest(Method.GET);
      IRestResponse restResponse = await client.ExecuteTaskAsync(request);
      //restResponse.Content
      var doc = new HtmlDocument();
      doc.LoadHtml(restResponse.Content);

      dynamic resultObject = new System.Dynamic.ExpandoObject();

      var resultDic = resultObject as IDictionary<String, object>;

      object Parser(HtmlNode node, DocumentParser parser) {
        switch (parser.OutputType) {
          case Enum.OutputType.Text | Enum.OutputType.Convert:
            return parser.Converts[GetNode(node, parser.PositionType, parser.Position).InnerText];
          case Enum.OutputType.Object:
            dynamic objectResult = new System.Dynamic.ExpandoObject();
            var objectResultMap = objectResult as IDictionary<string, object>;
            foreach (var _parser in parser.Parser) {
              objectResultMap[_parser.Key] = Parser(node, _parser.Value);
            }
            return objectResult;
          case Enum.OutputType.Text:
            return GetNode(node, parser.PositionType, parser.Position).InnerText;
          case Enum.OutputType.Html:
            return GetNode(node, parser.PositionType, parser.Position).InnerHtml;
          case Enum.OutputType.Array:
            List<object> results = new List<object>();
            foreach (var _node in GetNodes(node, parser.PositionType, parser.Position)) {
              dynamic result = new System.Dynamic.ExpandoObject();
              var resultMap = result as IDictionary<string, object>;
              foreach (var _parser in parser.Parser) {
                resultMap[_parser.Key] = Parser(_node, _parser.Value);
              }
              results.Add(result);
            }
            return results;
          case Enum.OutputType.None:
          default:
            throw new Exception("吃柠檬");
        }
      }
      HtmlNode GetNode(HtmlNode node, PositionType positionType, object position) {
        switch (positionType) {
          case PositionType.XPath:
            return node.SelectSingleNode(position.ToString());
          case PositionType.Index:
            return node.ChildNodes[Convert.ToInt32(position)];
          case PositionType.None:
          default:
            throw new Exception("吃柠檬");
        }
      }
      HtmlNodeCollection GetNodes(HtmlNode documentNode, PositionType positionType, object position) {
        switch (positionType) {
          case Enum.PositionType.XPath:
            return documentNode.SelectNodes(position.ToString());
          case Enum.PositionType.Index:
            return documentNode.ChildNodes[Convert.ToInt32(position)].ChildNodes;
          case Enum.PositionType.None:
          default:
            throw new Exception("吃柠檬");
        }
      }

      foreach (var item in requestPP.Document) {
        resultDic[item.Key] = Parser(doc.DocumentNode, item.Value);
      }
      List<dynamic> proxies = new List<dynamic>();
      int i = 0;
      var tasks = (resultDic["Proxies"] as IList<dynamic>).Select(async proxy => {
        var tempClient = new RestClient("http://source.gbihealth.com");
        tempClient.Timeout = 15000;
        tempClient.Proxy = new WebProxy((proxy as IDictionary<string, object>)["IpAddress"].ToString(), Convert.ToInt32((proxy as IDictionary<string, object>)["Port"]));
        tempClient.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/60.0.3112.113 Safari/537.36"; ;
        var tempRequest = new RestRequest(Method.GET);
        IRestResponse tempRestResponse = await client.ExecuteTaskAsync(tempRequest);
        if (tempRestResponse.StatusCode == HttpStatusCode.OK) {
          i++;
          Console.WriteLine($"{i}/300");
          proxies.Add(proxy);
        }
      });
      await Task.WhenAll(tasks);

      Console.ReadKey();
    }
  }
}
