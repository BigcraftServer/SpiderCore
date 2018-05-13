using Grpc.Core;
using HtmlAgilityPack;
using RestSharp;
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
          { "Proxies",new DocumentParser(){
            XPath=  "//table[@id='proxylisttable']/tbody/tr",
            Type = "Array",
            Parser = new Dictionary<string,DocumentNodeParser>(){
              { "IpAddress",0 },
              { "Port",1 },
              { "CountryCode",2 },
              { "AnonymousType",4},
              { "Type",new DocumentNodeParser(){
                Position = "6",
                Convert = new Dictionary<string,string>(){
                  { "yes","HTTPS"},{"no","HTTP" }
                }
              } }
            }
          } }
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


      foreach (var parser in requestPP.Document) {
        switch (parser.Value.Type) {
          case "Single":
            throw new NotImplementedException();
            break;
          case "Array":
            IList<dynamic> result = new List<dynamic>();
            //获取这个节点
            var currentNode = doc.DocumentNode.SelectNodes(parser.Value.XPath);
            foreach (var node in currentNode) {

              dynamic nodeResultObject = new System.Dynamic.ExpandoObject();

              var nodeResultDic = nodeResultObject as IDictionary<String, object>;
              //从这个Node里拿Parser的值
              foreach (var nodeParser in parser.Value.Parser) {
                if (nodeParser.Value.Convert != null && nodeParser.Value.Convert.Any()) {
                  nodeResultDic[nodeParser.Key] = nodeParser.Value.Convert[node.ChildNodes[int.Parse(nodeParser.Value.Position)].InnerText];
                } else {
                  nodeResultDic[nodeParser.Key] = node.ChildNodes[int.Parse(nodeParser.Value.Position)].InnerText;
                }
              }
              result.Add(nodeResultDic);
            }
            resultDic[parser.Key] = result;
            break;
          default:
            throw new InvalidOperationException($"{parser.Value.Type} is invalid");
        }
      }

      //     IRestResponse response = client.Execute(request);
      //     var content = response.Content; // raw content as string
      //
      //     // or automatically deserialize result
      //     // return content type is sniffed but can be explicitly set via RestClient.AddHandler();
      //     RestResponse<Person> response2 = client.Execute<Person>(request);
      //     var name = response2.Data.Name;
      //
      //     // easy async support
      //     client.ExecuteAsync(request, response => {
      //       Console.WriteLine(response.Content);
      //     });
      //
      //     // async with deserialization
      //     var asyncHandle = client.ExecuteAsync<Person>(request, response => {
      //       Console.WriteLine(response.Data.Name);
      //     });
      //
      //     // abort the request on demand
      //     asyncHandle.Abort();
      //var url = "https://free-proxy-list.net/";
      //var web = new HtmlWeb();
      //var doc = web.Load(url, "GET", new WebProxy("127.0.0.1", 1080), new NetworkCredential());
      //var allNode = doc.DocumentNode.SelectSingleNode("//table[@id='proxylisttable']").SelectNodes("//tbody/tr");
      //IList<ProxyPool.Grpc.Proxy> proxies = new List<ProxyPool.Grpc.Proxy>();
      //foreach (var item in allNode) {
      //  ProxyPool.Grpc.Proxy proxy = new ProxyPool.Grpc.Proxy() {
      //    IpAddress = item.ChildNodes[0].InnerText.Trim(),
      //    Port = int.Parse(item.ChildNodes[1].InnerText.Trim()),
      //    CountryCode = item.ChildNodes[2].InnerText.Trim(),
      //    AnonymousType = item.ChildNodes[4].InnerText.Trim(),
      //    Type = item.ChildNodes[6].InnerText.Trim()
      //  };
      //  proxies.Add(proxy);
      //}
      ////Test
      //var testUrl = url;
      //IList<ProxyPool.Grpc.Proxy> proxies1 = new List<ProxyPool.Grpc.Proxy>();
      //int i = 1;
      //var tasks = proxies.Select(async proxy => {
      //  HttpWebRequest request = WebRequest.CreateHttp("http://source.gbihealth.com/");
      //  request.Proxy = new WebProxy(proxy.IpAddress, proxy.Port);
      //  request.Method = "GET";
      //  request.Timeout = 50000;
      //  request.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/60.0.3112.113 Safari/537.36";
      //  try {
      //    using (HttpWebResponse response = await request.GetResponseAsync() as HttpWebResponse) {
      //      if (response.StatusCode != HttpStatusCode.RequestTimeout || response.StatusCode != HttpStatusCode.GatewayTimeout) {
      //        i++;
      //        Console.WriteLine($"{i}/{proxies.Count}");
      //        proxies1.Add(proxy);
      //      }
      //    }
      //  } catch {

      //  }
      //  wc.Encoding = Encoding.UTF8;
      //  wc.Proxy = new WebProxy(proxy.IpAddress, proxy.Port);
      //  string html = await wc.DownloadStringTaskAsync(new Uri("http://www.lagou.com/"));
      //  proxies1.Add(proxy);
      //}
      //});
      //await Task.WhenAll(tasks);
      //for (int i = 0; i < proxies.Count; i++) {
      //  try {
      //    var testWeb = new HtmlWeb();
      //    var testDoc = testWeb.Load(testUrl, "GET", new WebProxy(proxies[i].IpAddress, proxies[i].Port), new NetworkCredential());
      //  } catch {
      //    proxies.RemoveAt(i);
      //    i--;
      //    continue;
      //  }
      //}
      Console.ReadKey();
    }
  }
}
