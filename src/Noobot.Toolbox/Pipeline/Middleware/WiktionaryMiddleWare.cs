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
using System.Web;

namespace Noobot.Toolbox.Pipeline.Middleware
{
    public class WiktionaryMiddleWare : MiddlewareBase
    {
        private readonly StatsPlugin _statsPlugin;

        public WiktionaryMiddleWare(IMiddleware next, StatsPlugin statsPlugin) : base(next)
        {
            _statsPlugin = statsPlugin;
            HandlerMappings = new[]
            {
                new HandlerMapping
                {
                    ValidHandles = new []{"Define"},
                    Description = "Return defition from Wiktionary",
                    EvaluatorFunc = Define
                }
            };
        }

        private IEnumerable<ResponseMessage> Define(IncomingMessage message, string matchedHandle)
        {
            _statsPlugin.IncrementState("Wiktionary");

            var searchText = message.TargetedText.ToLower().Replace("define ", "").Trim();

            var reader = XmlReader.Create(@"https://en.wiktionary.org/w/index.php?title=" + HttpUtility.UrlEncode(searchText));
            var data = SyndicationFeed.Load(reader);
            reader.Close();

            var response = "";

            if (data == null)
            {
                yield return message.ReplyToChannel("Uh oh, something went on fire");
            }
            else
            {
                data.Items.Take(5).ToList().ForEach(x =>
                {
                    response += "<" + x.Links.First().GetAbsoluteUri() + "|" + x.Title.Text.Replace("<", "&lt;").Replace(">", "&gt;").Replace(" - msdn.microsoft.com", "") + ">\r\n";
                });

                yield return message.ReplyToChannel(response);
            }
        }
    }
}