using System;
using System.Collections.Generic;
using System.Data;
using System.Threading;
using System.Xml;
using Noobot.Core.MessagingPipeline.Middleware;
using Noobot.Core.MessagingPipeline.Request;
using Noobot.Core.MessagingPipeline.Response;
using Noobot.Core.Plugins.StandardPlugins;
using System.ServiceModel.Syndication;
using System.Linq;
using System.Net;
using System.Web;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Noobot.Toolbox.Pipeline.Middleware
{
    public class WebQueryMiddleware : MiddlewareBase
    {
        private readonly StatsPlugin _statsPlugin;

        public WebQueryMiddleware(IMiddleware next, StatsPlugin statsPlugin) : base(next)
        {
            _statsPlugin = statsPlugin;
            HandlerMappings = new[]
            {
                new HandlerMapping
                {
                    ValidHandles = new []{"MSDN"},
                    Description = "Return a list of MSDN articles",
                    EvaluatorFunc = MSDN
                },
            new HandlerMapping
                {
                    ValidHandles = new []{"Urban"},
                    Description = "Return Urban Definition",
                    EvaluatorFunc = Urban
                }
            };
        }

        private IEnumerable<ResponseMessage> MSDN(IncomingMessage message, string matchedHandle)
        {
            _statsPlugin.IncrementState("MSDN");

            var searchText = message.TargetedText.ToLower().ReplaceFirst("msdn ", "").Trim();

            var reader = XmlReader.Create(@"https://social.msdn.microsoft.com/search/en-US/feed?query=" + HttpUtility.UrlEncode(searchText) + "&format=RSS");
            var data = SyndicationFeed.Load(reader);
            reader.Close();

            var response = "";

            if (data == null)
            {
                yield return message.ReplyToChannel("Something went on fire!");
            }
            else
            {
                data.Items.Take(5).ToList().ForEach(x =>
                {
                    response +=  "<" + x.Links.First().GetAbsoluteUri() + "|" + x.Title.Text.Replace("<", "&lt;").Replace(">","&gt;").Replace(" - msdn.microsoft.com", "") + ">\r\n";
                });

                yield return message.ReplyToChannel(response);
            }
        }

        private IEnumerable<ResponseMessage> Urban(IncomingMessage message, string matchedHandle)
        {
            _statsPlugin.IncrementState("Urban");

            var searchText = message.TargetedText.ToLower().ReplaceFirst("urban ", "").Trim();
            var result = "";

            try
            {
                var jsonStr = new WebClient().DownloadString(@"http://api.urbandictionary.com/v0/define?term=" + HttpUtility.UrlEncode(searchText));
                var definition = JObject.Parse(jsonStr)["list"][0]["definition"];
                result = definition.Value<string>();
            }
            catch (Exception ex)
            {
            }

            if (!string.IsNullOrEmpty(result))
            {
                yield return message.ReplyToChannel($"`{searchText}` - {result}");
            }
            else
            {
                yield return message.ReplyToChannel("Something went on fire!");
            }
        }
    }

    public static class StringExtensionMethods
    {
        public static string ReplaceFirst(this string text, string search, string replace)
        {
            int pos = text.IndexOf(search);
            if (pos < 0)
            {
                return text;
            }
            return text.Substring(0, pos) + replace + text.Substring(pos + search.Length);
        }
    }
}